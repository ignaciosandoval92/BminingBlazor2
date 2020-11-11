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
    }
}