using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bplustree2
{
    class Elemento
    {
        private String cadena;
        private int direccion;

       public Elemento(String cadena, int direccion)
       {
            this.cadena = cadena;
            this.direccion = direccion;
       }

        public int Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }
        
        public String Cadena
        {
            get { return cadena; }
            set { cadena = value; }
        }
         

    }
}
