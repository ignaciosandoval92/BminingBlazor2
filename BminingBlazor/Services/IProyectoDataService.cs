using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace BminingBlazor.Services
{
    public interface IProyectoDataService
    {
        Task<int> CreateProyecto(ProyectoModel proyecto);
        Task AddIntegrante(IntegranteModel integrante);
        Task<List<TipoProyectoModel>> ReadTipoProyecto();
        Task<List<TipoEstadoPagoModel>> ReadTipoEstadoPago();
        Task AddEstadoPago(EstadoPagoModel estadopago);
        Task<int> AddCreadorProyecto(ProyectoModel proyecto);
        Task<int> AddJefeProyecto(ProyectoModel proyecto);
        Task<List<ViewProyectoModel>> ReadProyectos();
        Task<int> ReadJefeProyecto(int id_proyecto);
        Task<List<UsuarioEditModel>> ReadIntegrantes(int id_proyecto);
        Task<int> EditEstadoPago(EstadoPagoModel estadopago);
    }
}