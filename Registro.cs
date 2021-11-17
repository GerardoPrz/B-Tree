using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bplustree2
{
    class Registro
    {
        private int direccion;
        //private int direccionSiguienteRegistro
        private int clave;
        private int direccionIZQ;
        private int direccionDER;

        public int Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }

        public int Clave
        {
            get { return clave; }
            set { clave = value; }
        }

        public int DireccionIZQ
        {
            get { return direccionIZQ; }
            set { direccionIZQ = value; }
        }

        public int DireccionDER
        {
            get { return direccionDER; }
            set { direccionDER = value; }
        }
        public Registro(int direccion, int clave, int direccionIZQ, int direccionDER)
        {
            this.direccion = direccion;
            this.clave = clave;
            this.direccionIZQ = direccionIZQ;
            this.direccionDER = direccionDER;
        }
    }
}
