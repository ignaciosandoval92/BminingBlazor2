using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Configuration;
using Models;

namespace BminingBlazor.Services
{
    public class ProyectoDataService : IProyectoDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;
        public ProyectoDataService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
        }
        public async Task<int> CreateProyecto(ProyectoModel proyecto)
        {
            var sql =
                "insert into Proyecto (Proyecto.Cod_Proyecto,Proyecto.Nombre_Proyecto,Proyecto.Cod_TipoProyecto,Proyecto.Fecha_Inicio,Proyecto.Fecha_Fin,Proyecto.Id_Creador,Proyecto.Id_JefeProyecto,Proyecto.Id_Cliente)" +
                " Values (@Cod_Proyecto,@Nombre_Proyecto,@Cod_TipoProyecto,@Fecha_Inicio,@Fecha_Fin,@Id_Creador,@Id_JefeProyecto,@Id_Cliente)";
            await _dataAccess.SaveData(sql, proyecto, _configuration.GetConnectionString("default"));

            sql =
                "select Proyecto.Id_Proyecto " +
                $"from {TableConstants.TablaProyecto} " +
                $"  where Proyecto.Cod_Proyecto = '{proyecto.Cod_Proyecto}';";

            var items = await _dataAccess.LoadData<ProyectoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));

            return items.First().Id_proyecto;

        }
        public async Task<int> AddCreadorProyecto(ProyectoModel proyecto)
        {
            var sql =
                "Update Proyecto" +
                " set Proyecto.Id_Creador=@Id_Creador" +
                " where Proyecto.Id_Proyecto=@Id_proyecto ";
            await _dataAccess.UpdateData(sql, proyecto, _configuration.GetConnectionString("default"));

           

            return 0;

        }
        public async Task<int> AddJefeProyecto(ProyectoModel proyecto)
        {
            var sql =
                "Update Proyecto" +
                " set Proyecto.Id_JefeProyecto=@Id_JefeProyecto" +
                " where Proyecto.Id_Proyecto=@Id_proyecto ";
            await _dataAccess.UpdateData(sql, proyecto, _configuration.GetConnectionString("default"));



            return 0;

        }
        public async Task<int> AddCliente(ProyectoModel proyecto)
        {
            var sql =
                "Update Proyecto" +
                " set Proyecto.Id_Cliente=@Id_Cliente" +
                " where Proyecto.Id_Proyecto=@Id_proyecto ";
            await _dataAccess.UpdateData(sql, proyecto, _configuration.GetConnectionString("default"));



            return 0;

        }
        public async Task<int> EditEstadoPago(EstadoPagoModel estadopago)
        {
            var sql =
                "Update EstadoPago" +
                " set EstadoPago.Cod_TipoEstadoPago=@Cod_TipoEstadoPago" +
                " where EstadoPago.Cod_EstadoPago=@Cod_EstadoPago ";
                ;
            await _dataAccess.UpdateData(sql, estadopago, _configuration.GetConnectionString("default"));



            return 0;

        }

        public async Task AddEstadoPago(EstadoPagoModel estadopago)
        {
            string sql =
                "insert into EstadoPago (EstadoPago.Estado_Pago,EstadoPago.Id_Proyecto,EstadoPago.Cod_TipoEstadoPago) " +
                " Values (@Estado_Pago,@Id_Proyecto,@Cod_TipoEstadoPago)";
            await _dataAccess.SaveData(sql, estadopago, _configuration.GetConnectionString("default"));
        }

        public async Task AddIntegrante(IntegranteModel integrante)
        {
            string sql = "insert into Integrantes_Proyecto (Integrantes_Proyecto.Id_Usuario,Integrantes_Proyecto.Id_Proyecto) " +
                         " Values (@Id_Usuario,@Id_Proyecto)";
            await _dataAccess.SaveData(sql, integrante, _configuration.GetConnectionString("default"));
        }
        public async Task<List<TipoProyectoModel>> ReadTipoProyecto()
        {
            string sql = $"select*from {TableConstants.TablaTipoProyecto}";
            var tipro  = await _dataAccess.LoadData<TipoProyectoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return tipro;
        }
        public async Task<List<TipoEstadoPagoModel>> ReadTipoEstadoPago()
        {
            string sql = $"select*from {TableConstants.TablaTipoEstadoPago}";
            var tipoep = await _dataAccess.LoadData<TipoEstadoPagoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return tipoep;
        }

        public async Task<List<ViewProyectoModel>> ReadProyectos()
        {
            string sql =
                "select Proyecto.Id_Proyecto,Proyecto.Cod_Proyecto,Proyecto.Nombre_Proyecto,(Usuario.Email_Bmining) as Email_JefeProyecto,Cliente.Nombre_Cliente,Tipo_Proyecto.Tipo_Proyecto,(EstadoPago.Estado_Pago) as Tipo_Pago,Tipo_EstadoPago.TipoEstadoPago,EstadoPago.Cod_EstadoPago " +
                $" from {TableConstants.TablaProyecto},{TableConstants.TablaTipoEstadoPago},{TableConstants.TablaTipoProyecto},{TableConstants.TablaEstadoPago},{TableConstants.TablaUsuario},{TableConstants.TablaClientes} " +
                $" where Proyecto.Id_Proyecto=EstadoPago.Id_Proyecto " +
                $"and EstadoPago.Cod_TipoEstadoPago=Tipo_EstadoPago.Cod_TipoEstadoPago " +
                $"and Proyecto.Cod_TipoProyecto=Tipo_Proyecto.Cod_TipoProyecto " +
                $"and Usuario.Id=Proyecto.Id_JefeProyecto " +
                $"and Proyecto.Id_Cliente=Cliente.Id_Cliente";
            var proyectos =
                await _dataAccess.LoadData<ViewProyectoModel, dynamic>(sql, new { },
                    _configuration.GetConnectionString("default"));
            return proyectos;
        }

        public async Task<int> ReadJefeProyecto(int id_proyecto)
        {
            string sql = "select Proyecto.Id_JefeProyecto " +
                         $" from {TableConstants.TablaProyecto} " +
                         $" where Proyecto.Id_Proyecto={id_proyecto}";
            var id_jefe =
                await _dataAccess.LoadData<ProyectoModel, dynamic>(sql, new { }, _configuration.GetConnectionString("default"));
            return id_jefe.First().Id_JefeProyecto;
        }
        public async Task<List<UsuarioEditModel>> ReadIntegrantes(int id_proyecto)
        {
            string sql = "select Usuario.Id, Usuario.Email_Bmining,Usuario.Nombre ,Usuario.Apellido_Paterno,Usuario.Apellido_Materno,Usuario.Rut,Usuario.Cargo,Usuario.Telefono,Usuario.Direccion,Integrantes_Proyecto.Cod_Integrantes  " +
                         $" from {TableConstants.TablaUsuario},{TableConstants.TablaIntegrantes}" +
                         $" where Integrantes_Proyecto.Id_Proyecto={id_proyecto}" +
                         $" and Usuario.Id=Integrantes_Proyecto.Id_Usuario";

            var integrantes =
                await _dataAccess.LoadData<UsuarioEditModel, dynamic>(sql, new { },
                    _configuration.GetConnectionString("default"));
            return integrantes;
        }
        public async Task DeleteIntegrante(int cod_integrantes)
        {
            string sql = "Delete " +
                         $"from {TableConstants.TablaIntegrantes} " +
                         $"where Integrantes_Proyecto.Cod_Integrantes=@cod_integrantes";
            await _dataAccess.DeleteData(sql, new { Cod_Integrantes = cod_integrantes }, _configuration.GetConnectionString("default"));

        }

        public async Task DeleteProyecto(int id_proyecto)
        {
            string sql = "Delete " +
                         $" from {TableConstants.TablaProyecto} " +
                         $" where Proyecto.Id_Proyecto=@id_proyecto";
            await _dataAccess.DeleteData(sql, new {Id_Proyecto = id_proyecto},
                _configuration.GetConnectionString("default"));
        }
        public async Task<int> CreateCliente(ClienteModel cliente)
        {
            var sql =
                "insert into Cliente (Cliente.Nombre_Cliente)" +
                " Values (@Nombre_Cliente)";
            await _dataAccess.SaveData(sql, cliente, _configuration.GetConnectionString("default"));

          
            return 0;

        }
     

        public async Task<List<ClienteModel>> ReadCliente()
        {
            string sql = $"select*from {TableConstants.TablaClientes}";
            var clientes =await _dataAccess.LoadData<ClienteModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return clientes;
        }

        public Task<List<ViewProyectoModel>> ReadProjectsByUser(int userId)
        {
            return Task.Run(() => new List<ViewProyectoModel>
            {
                new ViewProyectoModel
                {
                    Cod_EstadoPago = 1,
                    Cod_Proyecto = "BM-Project-1",
                    Id_Proyecto = 4,
                    Email_JefeProyecto = "jefe1@bmining.cl", // TODO: Cambiar a ID
                    Nombre_Cliente = "cliente 1", // TODO: Cambiar a Id Cliente
                    Nombre_Proyecto = "Proyecto 1",
                    TipoEstadoPago = "test", // TODO: Con el Cod_Estado de pago basta
                    Tipo_Proyecto = "test2", // TODO: Esto no es una ID?
                    Tipo_Pago = "2", // TODO: Esto quedo fuera creo.
                },
                new ViewProyectoModel
                {
                    Cod_EstadoPago = 1,
                    Cod_Proyecto = "BM-Project-2",
                    Id_Proyecto = 5,
                    Email_JefeProyecto = "jefe2@bmining.cl", // TODO: Cambiar a ID
                    Nombre_Cliente = "cliente 2", // TODO: Cambiar a Id Cliente
                    Nombre_Proyecto = "Proyecto 2",
                    TipoEstadoPago = "test", // TODO: Con el Cod_Estado de pago basta
                    Tipo_Proyecto = "test3", // TODO: Esto no es una ID?
                    Tipo_Pago = "2", // TODO: Esto quedo fuera creo.
                },
                new ViewProyectoModel
                {
                    Cod_EstadoPago = 1,
                    Cod_Proyecto = "BM-Project-3",
                    Id_Proyecto = 6,
                    Email_JefeProyecto = "jefe3@bmining.cl", // TODO: Cambiar a ID
                    Nombre_Cliente = "cliente 3", // TODO: Cambiar a Id Cliente
                    Nombre_Proyecto = "Proyecto 3",
                    TipoEstadoPago = "test 2", // TODO: Con el Cod_Estado de pago basta
                    Tipo_Proyecto = "test 4", // TODO: Esto no es una ID?
                    Tipo_Pago = "5", // TODO: Esto quedo fuera creo.
                }
            });
        }
    }
}
