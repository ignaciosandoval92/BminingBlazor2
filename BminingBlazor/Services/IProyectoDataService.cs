using System.Threading.Tasks;
using Models;

namespace BminingBlazor.Services
{
    public interface IProyectoDataService
    {
        Task<int> CreateProyecto(ProyectoModel proyecto);
    }
}