using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.Projects
{
    public class CProject
    {
        private CNode raiz;
        private CNode trabajo;
        private int i = 0;
        
        public CProject()
        {
            raiz = new CNode();
        }

        public CNode Insertar(ProjectViewModel pDato,CNode pNodo)
        {
            if (pNodo==null)
            {
                raiz = new CNode();
                raiz.Dato = pDato;

                raiz.Hijo = null;
                raiz.Hermano = null;

                return raiz;
            }

            if(pNodo.Hijo==null)
            {
                CNode temp = new CNode();
                temp.Dato = pDato;
                pNodo.Hijo = temp;
                return temp;
            }
            else
            {
                trabajo = pNodo.Hijo;

                while(trabajo.Hermano!=null)
                {
                    trabajo = trabajo.Hermano;
                }

                CNode temp = new CNode();
                temp.Dato = pDato;
                trabajo.Hermano = temp;
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
            Console.WriteLine(pNodo.Dato);

            if(pNodo.Hijo!=null)
            {
                i++;
                TransversaPreO(pNodo.Hijo);
                i--;
            }

            if(pNodo.Hermano!=null)
            {
                TransversaPreO(pNodo.Hermano);
            }


        }

    }
}
