using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BminingBlazor.ViewModels.Projects
{
    public class CNode
    {
        private ProjectViewModel data;
        private int level;
        private CNode son;
        private CNode brother;
        private int parentId;

        public ProjectViewModel Data { get => data; set => data=value; }
        public CNode Son { get => son; set => son = value; }
        public CNode Brother { get => brother; set => brother = value; }
        
        public CNode()
        {
            data = new ProjectViewModel();
            son = null;
            brother = null;
            level = 0;
            parentId = 0;
        }
    }
}
