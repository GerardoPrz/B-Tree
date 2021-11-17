using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bplustree2
{
    class Pagina
    {
        private string tipodePagina;
        private int direccion;
        private List<Registro> registros;///

        public Pagina(string tipodePagina, int direccion)
        {
            this.tipodePagina = tipodePagina;
            this.direccion = direccion;
            this.registros = new List<Registro>();
        }

        public string TipodePagina
        {
            get { return tipodePagina; }
            set { tipodePagina = value; }
        }

        public int Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }

        public List<Registro> Registros
        {
            get { return registros; }
            set { registros = value; }
        }
    }
}
