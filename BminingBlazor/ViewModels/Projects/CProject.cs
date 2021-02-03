using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.Report;

namespace BminingBlazor.ViewModels.Projects
{
    public class CProject
    {
        private CNode root;
        private CNode work;
        private int i = 0;
        
        public CProject()
        {
            root = new CNode();
        }

        public CNode Insertar(ProjectViewModel pData,CNode pNode)
        {
            if (pNode==null)
            {
                root = new CNode();
                root.Data = pData;
                root.Son = null;
                root.Brother = null;

                return root;
            }

            if(pNode.Son==null)
            {
                CNode temp = new CNode();
                temp.Data = pData;
                pNode.Son = temp;
                return temp;
            }
            else
            {
                work = pNode.Son;

                while(work.Brother!=null)
                {
                    work = work.Brother;
                }

                CNode temp = new CNode();
                temp.Data = pData;
                work.Brother = temp;
                return temp;
            }
        }
        public void TransversaPreO(CNode pNodo)
        {
            if(pNodo==null)
            {
                return;
            }

            for (int n = 0; n < i; n++)
                Console.Write(" ");
            Console.WriteLine(pNodo.Data);

            if(pNodo.Son!=null)
            {
                i++;
                TransversaPreO(pNodo.Son);
                i--;
            }

            if(pNodo.Brother!=null)
            {
                TransversaPreO(pNodo.Brother);
            }


        }

    }
}
