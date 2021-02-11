using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.Report;


namespace BminingBlazor.ViewModels.Projects
{
    public class CNode
    {
        private ProjectViewModel data;        
        private CNode son;
        private CNode brother;
     

        public ProjectViewModel Data { get => data; set => data=value; }
        public CNode Son { get => son; set => son = value; }
        public CNode Brother { get => brother; set => brother = value; }
        
        public CNode()
        {
            data = new ProjectViewModel();
            son = null;
            brother = null;
          
        }
    }
}
