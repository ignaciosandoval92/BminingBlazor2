using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BminingBlazor.ViewModels.Projects
{
    public class CNode
    {
        private ProjectViewModel dato;

        private CNode hijo;
        private CNode hermano;

        public ProjectViewModel Dato { get => dato; set => dato=value; }
        public CNode Hijo { get => hijo; set => hijo = value; }
        public CNode Hermano { get => hermano; set => hermano = value; }
        
        public CNode()
        {
            dato = new ProjectViewModel();
            hijo = null;
            hermano = null;
        }
    }
}
