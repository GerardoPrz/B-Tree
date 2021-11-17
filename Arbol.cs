using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bplustree2
{
    class Arbol : List<Pagina>
    {
        private int orden;
        public int Orden
        {
            get { return orden; }
            set { orden = value; }
        }

        public Arbol(int orden)
        {
            this.orden = orden;
        }
        public void AddPagina(Pagina p)
        {
            Add(p);
        }
    }
}
