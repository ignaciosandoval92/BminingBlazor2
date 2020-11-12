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
                "insert into Proyecto (Proyecto.Cod_Proyecto,Proyecto.Nombre_Proyecto,Proyecto.Cod_TipoProyecto,Proyecto.Fecha_Inicio,Proyecto.Fecha_Fin,Proyecto.Id_Creador,Proyecto.Id_JefeProyecto)" +
                " Values (@Cod_Proyecto,@Nombre_Proyecto,@Cod_TipoProyecto,@Fecha_Inicio,@Fecha_Fin,@Id_Creador,@Id_JefeProyecto)";
            await _dataAccess.SaveData(sql, proyecto, _configuration.GetConnectionString("default"));

            sql =
                "select Proyecto.Id_Proyecto " +
                $"from {TableConstants.TablaProyecto} " +
                $"  where Proyecto.Cod_Proyecto = '{proyecto.Cod_Proyecto}';";

            var items = await _dataAccess.LoadData<ProyectoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));

            return items.First().Id_proyecto;

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
                "select (Proyecto.Id_Proyecto,Proyecto.Cod_Proyecto,Proyecto.Nombre_Proyecto,Tipo_Proyecto.Tipo_Proyecto,EstadoPago.Estado_Pago as Tipo_Pago,Tipo_EstadoPago.TipoEstadoPago " +
                $" from {TableConstants.TablaProyecto},{TableConstants.TablaTipoEstadoPago},{TableConstants.TablaTipoProyecto},{TableConstants.TablaTipoEstadoPago} " +
                $" where Proyecto.Id_Proyecto=EstadoPago.Id_Proyecto,EstadoPago.Cod_TipoEstadoPago=Tipo_EstadoPago.Cod_TipoEstadoPago,Proyecto.Cod_TipoProyecto=Tipo_Proyecto.Cod_TipoProyecto";
            var proyectos =
                await _dataAccess.LoadData<ViewProyectoModel, dynamic>(sql, new { },
                    _configuration.GetConnectionString("default"));
            return proyectos;
        }
    }
}
