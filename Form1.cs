using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace bplustree2
{
    public partial class Form1 : Form
    {
        List<int> ingresados = new List<int>();
        int direccionAcumulador = 1000;
        Arbol arbol;
        bool yaHayIntermedios = false;
        public Form1()
        {
            InitializeComponent();
            this.arbol = new Arbol(2);
            Pagina raiz = new Pagina("R", direccionAcumulador);//creo la raiz que sera la inicial del arbol
            direccionAcumulador += 65;//aumento el tamaño de la raiz en la direccion

            //meto la raiz en el arbol
            arbol.Add(raiz);
        }

        private void ingresar(int clave)
        {
            Registro registro = new Registro(direccionAcumulador, clave, -1, -1);
            direccionAcumulador += 30;//aumento el tamaño del registro en la direccion


            if (arbol.Count == 1)//si el arbol solo tiene la raiz, que al inicio funcina como hoja
            {
                //voy a insertar cualquier valor en la raiz, no tengo que buscar pagina para insertar
                insertarEnRaiz(registro);
            }
            else if(arbol.Count > 1)//si el arbol tiene mas de una pagina
            {
                insertarEnNoVacio(registro);
               
                //revisar si la pagina raiz esta llena
                revisaRaizLlena();
            }
        }

        private void revisaRaizLlena()
        {
            if(arbol.ElementAt(0).Registros.Count > 2 * arbol.Orden)//si la raiz est llena entonces debo crear intermedios //los debo insertar despues de la raiz para que no hay problemas
            {
                if (yaHayIntermedios == false)//si aun no hay intermedios debo crear dos paginas intermedias nuevas
                {
                    //primero debo crear loas dos nuevas paginas intermedios
                    Pagina intermedia1 = new Pagina("I", direccionAcumulador);
                    direccionAcumulador += 65;

                    //primero debo crear loas dos nuevas paginas intermedios
                    Pagina intermedia2 = new Pagina("I", direccionAcumulador);
                    direccionAcumulador += 65;

                    //meto las paginas intermedias al arbol justo despues de la raiz
                    int indice1 = arbol.IndexOf(arbol.ElementAt(0)) + 1;
                    int indice2 = indice1 + 1;
                    arbol.Insert(indice1, intermedia1);
                    arbol.Insert(indice2, intermedia2);

                    //en la intermedia1 meto los datos 0 1 de la raiz
                    intermedia1.Registros.Add(arbol.ElementAt(0).Registros.ElementAt(0));
                    intermedia1.Registros.Add(arbol.ElementAt(0).Registros.ElementAt(1));

                    //en la intermedia2 meto los datos 3 4 de la raiz
                    intermedia2.Registros.Add(arbol.ElementAt(0).Registros.ElementAt(3));
                    intermedia2.Registros.Add(arbol.ElementAt(0).Registros.ElementAt(4));

                    //en la raiz elimino los datos 0, 1, 3 y 4 //0 1 2 3 4
                    arbol.ElementAt(0).Registros.RemoveAt(0);// 1 2 3 4
                    arbol.ElementAt(0).Registros.RemoveAt(0);// 2 3 4
                    arbol.ElementAt(0).Registros.RemoveAt(1);// 2 4
                    arbol.ElementAt(0).Registros.RemoveAt(1);// 2

                    //le doy las direcciones al que subi a la raiz
                    arbol.ElementAt(0).Registros.ElementAt(0).DireccionIZQ = intermedia1.Direccion;
                    arbol.ElementAt(0).Registros.ElementAt(0).DireccionDER = intermedia2.Direccion;

                    yaHayIntermedios = true;
                }
                else if (yaHayIntermedios == true)
                {
                    MessageBox.Show("EL arbol esta lleno");
                    //si ya se lleno la raiz y debo crear una pagina intermedia e insetarla donde corresponda
                    /*Pagina intermedia1 = new Pagina("I", direccionAcumulador);
                   direccionAcumulador += 65;

                   //busco la ultima pagina intermedia en el arbol para obtener su indice
                   int indiceParaInsertarI = 1;
                   foreach (Pagina pagina in arbol)
                   {
                       if (pagina.TipodePagina == "I")
                       {
                           indiceParaInsertarI = arbol.IndexOf(pagina) + 1;
                       }
                   }

                   //inserto la intermedia justo despues de la ultima intermedia
                   arbol.Insert(indiceParaInsertarI, intermedia1);*/
                }
            }
        }

        private void insertarEnNoVacio(Registro registro)
        {
            //primero busco la pagina donde debo insertar el dato
            Pagina paginaParaInsertar = buscaPagina(registro.Clave);

            //inserto el registro en la pagina que encontre
            paginaParaInsertar.Registros.Add(registro);

            //ordeno la pagina donde inserte el registro
            IEnumerable<Registro> paginaOrdenada = paginaParaInsertar.Registros.OrderBy(x => x.Clave);
            paginaParaInsertar.Registros = paginaOrdenada.ToList();

            //reviso si la pagina esta llena
            if (paginaParaInsertar.Registros.Count > 2 * arbol.Orden )
            {
                dividePagina(paginaParaInsertar);
            }

        }

        private Pagina buscaPagina(int clave)
        {
            //si es necesario buscar una pagina para insertar significa que el arbol ya tiene mas de una pagina
            Pagina paginaParaInsertar = null;

            bool encontrado = false;//esta bandera servira para saber si lo encontro y si no entrar a otra accion
            //primero debo recorrer todas las paginas del arbol
            for (int i = 0; i+1 < arbol.Count; i++)
            {
                Registro registroAdelantado = arbol.ElementAt(i+1).Registros.ElementAt(0);//tomo el primer registro de la pagina que sigue
                //3
                //1 2 4 5  ]]]]]]   6 7 8 9
                if (registroAdelantado != null)
                {
                    if (clave < registroAdelantado.Clave && arbol.ElementAt(i).TipodePagina != "R" && arbol.ElementAt(i).TipodePagina != "Intermedio")//si la clave del primer registro de la pagina de adelante es mayor quiere decir la pagina actual es la que debemos regresar
                    {
                        paginaParaInsertar = arbol.ElementAt(i);
                        encontrado = true;
                        break;
                    }
                }
            }

            //si durante el recorrido no encontro la pagina quiere decir que la que debemos devolver es la ultima pagina
            if (encontrado == false)
            {
                paginaParaInsertar = arbol.Last();
            }


            return paginaParaInsertar;
        }

        private void insertarEnRaiz(Registro registro)
        {
            arbol.ElementAt(0).Registros.Add(registro);//inserto en registro en la pagina 0, que es la "raiz"

            IEnumerable<Registro> paginaOrdenada = arbol.ElementAt(0).Registros.OrderBy(x => x.Clave);//ordeno la pagina donde acabo de insertar el registro
            arbol.ElementAt(0).Registros = paginaOrdenada.ToList();

            //reviso si la pagina ya esta llena
            if(arbol.ElementAt(0).Registros.Count > 2 * arbol.Orden)
            {
                dividePagina(arbol.ElementAt(0));//divido la raiz
            }
        }

        private void dividePagina(Pagina paginaParaDividir)
        {
            if(paginaParaDividir.TipodePagina == "R")//si la pagina que voy a divir es la raiz entonces creo dos paginas hojas
            {
                Pagina paginaNueva1 = new Pagina("H", direccionAcumulador);//creo la nueva pagina
                direccionAcumulador += 65;//aumento el tamaño de la pagina en la direccion
                
                Pagina paginaNueva2 = new Pagina("H", direccionAcumulador);//creo la nueva pagina
                direccionAcumulador += 65;//aumento el tamaño de la pagina en la direccion

                //ahora meto las dos paginas al arbol
                arbol.Add(paginaNueva1);
                arbol.Add(paginaNueva2);

                //ahora debo pasar los dos primeros datos que estan en la raiz a  la paginaNueva1
                paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(0));
                paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(1));

                //paso los tres ultimos elementos de la raiz a la paginaNueva2
                paginaNueva2.Registros.Add(paginaParaDividir.Registros.ElementAt(2));
                paginaNueva2.Registros.Add(paginaParaDividir.Registros.ElementAt(3));
                paginaNueva2.Registros.Add(paginaParaDividir.Registros.ElementAt(4));

                //ahora borro los elementos de la raiz que ya copie a las paginas nuevas menos el del medio
                                                                       //7 8 4 2 1
                paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(0));//  8 4 2 1
                paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(0));//    4 2 1
                paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(1));//    4   1
                paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(1));//    4   

                //ahora le doy la direccion de las dos nuevas paginas al registro que "subio" a la raiz
                paginaParaDividir.Registros.ElementAt(0).DireccionIZQ = paginaNueva1.Direccion;
                paginaParaDividir.Registros.ElementAt(0).DireccionDER = paginaNueva2.Direccion;
            }
            else if(paginaParaDividir.TipodePagina == "H")
            {
                //si voy a dividir una hoja entonces solo creo una pagina
                Pagina paginaNueva1 = new Pagina("H", direccionAcumulador);//creo la nueva pagina
                direccionAcumulador += 65;//aumento el tamaño de la pagina en la direccion

                int indice = arbol.IndexOf(paginaParaDividir) + 1;
                //meto la pagina al arbol
                arbol.Insert(indice, paginaNueva1);

                if (yaHayIntermedios == true)// si en el arbol ya hay intermedios debo subir el dato del medio de la hoja al intermedio correspondiente
                {   //ya hay paginas intermedias en el arbol entonces solo hace falta agregar una pagina intermedia y pasar datos

                    Pagina intermediaParaSubirDato = buscaIntermedia(paginaParaDividir.Registros.ElementAt(2).Clave);//le debo pasar el registro que vamos a subir es decir, el de la mitad de la pagina para dividir

                    //subo el dato de en medio en la pagina para dividir a la intermedia que encontre
                    intermediaParaSubirDato.Registros.Add(paginaParaDividir.Registros.ElementAt(2));

                    //paso lo elementos 2,3 y 4 a la pagina de la derecha (la nueva que acabamos de crear)
                    paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(2));
                    paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(3));
                    paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(4));

                    //elimino los elementos que acabo de copiar a la nueva pagina en la paginaParaDividir
                    paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(2));//0 1  3 4
                    paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(2));//0 1  4
                    paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(2));//0 1

                    //ordeno la intermedia donde subi el dato
                    IEnumerable<Registro> paginaOrdenada = intermediaParaSubirDato.Registros.OrderBy(x => x.Clave);
                    intermediaParaSubirDato.Registros = paginaOrdenada.ToList();

                    //debo revisar si la intermedia donde subimos el datos ya se lleno
                    if (intermediaParaSubirDato.Registros.Count > 2 * arbol.Orden)
                    {
                        //debo crear otro intermedio
                        Pagina nuevaIntermedia = new Pagina("I", direccionAcumulador);
                        direccionAcumulador += 65;

                        //meto la pagina al arbol justo despues de donde esta la intermedia que estoy dividiendo
                        int indiceNuevaIntermedia = arbol.IndexOf(intermediaParaSubirDato) + 1;
                        arbol.Insert(indiceNuevaIntermedia, nuevaIntermedia);

                        //el dato del medio debo subirlo a la raiz (el dato 2)
                        arbol.ElementAt(0).Registros.Add(intermediaParaSubirDato.Registros.ElementAt(2));

                        //pasarle el dato 3 y 4a la nueva intermedia que acabo de crear
                        nuevaIntermedia.Registros.Add(intermediaParaSubirDato.Registros.ElementAt(3));
                        nuevaIntermedia.Registros.Add(intermediaParaSubirDato.Registros.ElementAt(4));

                        //ahora eliminar los elementos 2, 3 y 4 de la pagina que dividí
                        intermediaParaSubirDato.Registros.RemoveAt(2);
                        intermediaParaSubirDato.Registros.RemoveAt(2);
                        intermediaParaSubirDato.Registros.RemoveAt(2);

                        //ordeno la raiz
                        IEnumerable<Registro> raizOrdenada = arbol.ElementAt(0).Registros.OrderBy(x => x.Clave);
                        arbol.ElementAt(0).Registros = raizOrdenada.ToList();
                    }
                }
                else if (yaHayIntermedios == false)
                {

                    //debo subir el dato de en medio de la pagina que voy a dividir a a la raiz 0 1 ]2] 3 4
                    arbol.ElementAt(0).Registros.Add(paginaParaDividir.Registros.ElementAt(2));

                    //paso lo elementos 2,3 y 4 a la pagina de la derecha (la nueva que acabamos de crear)
                    paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(2));
                    paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(3));
                    paginaNueva1.Registros.Add(paginaParaDividir.Registros.ElementAt(4));

                    //elimino los elementos que acabo de copiar a la nueva pagina en la paginaParaDividir
                    paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(2));//0 1  3 4
                    paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(2));//0 1  4
                    paginaParaDividir.Registros.Remove(paginaParaDividir.Registros.ElementAt(2));//0 1

                    //ordeno la raiz
                    IEnumerable<Registro> paginaOrdenada = arbol.ElementAt(0).Registros.OrderBy(x => x.Clave);
                    arbol.ElementAt(0).Registros = paginaOrdenada.ToList();
                }
            }
        }

        private Pagina buscaIntermedia(int clave)
        {
            Pagina intermediaParaDevolver = null;
            List<Pagina> auxiliarIntermedias = new List<Pagina>();
            bool encontrado = false;//esta bandera servira para saber si lo encontro y si no entrar a otra accion

            //copio todo las pagina intermedias del arbol en la lista auxiliar
            foreach (Pagina pagina in arbol)
            {
                if (pagina.TipodePagina == "I")
                {
                    auxiliarIntermedias.Add(pagina);
                }
            }

            //primero debo recorrer todas las paginas de la lista auxiliar de intermedias
            for (int i = 0; i + 1 < auxiliarIntermedias.Count; i++)
            {
                Registro registroAdelantado = auxiliarIntermedias.ElementAt(i + 1).Registros.ElementAt(0);//tomo el primer registro de la pagina que sigue
                //3
                //1 2 4 5  ]]]]]]   6 7 8 9

                if (registroAdelantado != null)
                {
                    if (clave < registroAdelantado.Clave)//si la clave del primer registro de la pagina de adelante es mayor quiere decir la pagina actual es la que debemos regresar
                    {
                        intermediaParaDevolver = auxiliarIntermedias.ElementAt(i);
                        encontrado = true;
                    }
                }
            }

            //si durante el recorrido no encontro la pagina quiere decir que la que debemos devolver es la ultima pagina
            if (encontrado == false)
            {
                intermediaParaDevolver = auxiliarIntermedias.Last();
            }

            //la pagina que encontre solo es una copia que esta en el auxiliar, entonces debo buscarla en el arbol original y devolverla
            foreach (Pagina pagina in arbol)
            {
                if (pagina == intermediaParaDevolver)
                {
                    intermediaParaDevolver = pagina;
                }
            }

            return intermediaParaDevolver;
        }

        private void enlazaRegistros()
        {
            foreach (Pagina pagina in arbol)
            {
                if (pagina.TipodePagina == "H")
                {
                    for (int i = 0; i < pagina.Registros.Count - 1; i++)//10 22 35 44
                    {
                        pagina.Registros.ElementAt(i).DireccionDER = pagina.Registros.ElementAt(i+1).Direccion;
                    }
                    pagina.Registros.Last().DireccionDER = -1;//al ultimo lo pongo en menos uno porque no esta enlazado 
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//ingresar
        {
            try
            {
                int clavePorIngresar = int.Parse(textBox1.Text);
                if (ingresados.Contains(clavePorIngresar))
                {
                    MessageBox.Show("Esta clave ya fue ingresada");
                }
                else
                {
                    ingresar(clavePorIngresar);
                    ingresados.Add(clavePorIngresar);
                }
                textBox1.Text = "";
            }
            catch (FormatException fe)
            {

                MessageBox.Show("Ingresa un dato valido");
            }
        }

        private void button1_Click(object sender, EventArgs e)//escribir en archivo
        {
            //ESCRIBIR EN U ARCHIVO
            String cadena;
            String cadena1 = "";

            StreamWriter escribir = File.AppendText("bptree.txt");

            enlazaRegistros();

            foreach (Pagina pagina in arbol)
            {
                cadena = "";
                cadena1 = "";

                cadena = "| " + pagina.Direccion + " | " + pagina.TipodePagina;
                foreach (Registro registro in pagina.Registros)
                {
                    cadena1 +=  registro.Direccion + " | " + registro.Clave + " | " + registro.DireccionDER + " |";
                    escribir.WriteLine("| " + registro.Direccion + " | " + registro.Clave + " | " + registro.DireccionDER + " | \n");
                }
                escribir.WriteLine(cadena + " | " + cadena1);
                escribir.WriteLine("\n");
            }
            escribir.WriteLine("--------------------------------------------");
            escribir.Close(); 
        }

        private void eliminar(int clave)
        {
            //primero recupero la pagina del arbol donde esta el registro que quiero eliminar
            Pagina paginaDondeElimine = buscaPaginaParaEliminaryEliminaRegistro(clave);

            //ahora tengo que evaluar si el numero de elementos sigue siendo mayor a arbol.Orden
            if(paginaDondeElimine.Registros.Count >= arbol.Orden)
            {
                //no hago nada
            }
            else
            {
                //bajar el registro padre y sustiuturlo por el dato que este mas a la derecha del subarbol izquierdo o el derecho
                bool seRealizoElPrestamo = revisarPrestamo(paginaDondeElimine);

                //si no se pudo realizar el prestamo debo fusionar paginas
                if (!seRealizoElPrestamo)
                {

                }
            }
        }

        private bool revisarPrestamo(Pagina paginaDondeElimine)
        {
            bool seRealizoElPrestamo = false;
            //primero debo saber si estoy en un extremo del arbol
            //en el extremo izquierdo ya no habiar mas a la izquierda y en el derecho ya no habria masa la derecha

            //hago una auxiliar de puras hojas
            List<Pagina> auxiliarHojas = new List<Pagina>();

            foreach (Pagina pagina in arbol)//lleno el axiliar con solo las hojas del arbol
            {
                if (pagina.TipodePagina == "H")
                {
                    auxiliarHojas.Add(pagina);
                }
            }

            //indetifico si es el extremo izquierdo o el derecho
            if(paginaDondeElimine == auxiliarHojas.First())//si es el extremo izquierdo
            {
                //reviso a la derecha
                Pagina paginaAdelante = auxiliarHojas.ElementAt(auxiliarHojas.IndexOf(auxiliarHojas.First()) + 1);
                if (paginaAdelante.Registros.Count >= 3)//reviso en la pagina siguiente del extremo izquierdo
                {
                    auxiliarHojas.First().Registros.Add(paginaAdelante.Registros.First());//bajo el primer registro de la raiz al arbol donde necesito prestrado

                    arbol.ElementAt(0).Registros.Remove(paginaAdelante.Registros.First());//eliimino el registro que baje de la raiz

                    paginaAdelante.Registros.Remove(paginaAdelante.Registros.First());//ahora lo elimino de la pagina de donde lo SAQUE


                    //si puedo pedir prestado porque al quietarle un no va a pasar nada
                    arbol.ElementAt(0).Registros.Add(paginaAdelante.Registros.First());//el primero elemento lo debo subir a la raiz

                    //elimino el 

                    //ahora la raiz
                    IEnumerable<Registro> paginaOrdenada = arbol.ElementAt(0).Registros.OrderBy(x => x.Clave);
                    arbol.ElementAt(0).Registros = paginaOrdenada.ToList();

                    //como solo estoy trabajando con copias debo encontrar las paginas que modifique en el arbol real 
                    foreach (Pagina pagina in arbol)//primero asgno el extremo izquierdo
                    {
                        if (pagina == auxiliarHojas.First())
                        {
                            pagina.Registros = auxiliarHojas.First().Registros;
                        }
                    }
                    
                    foreach (Pagina pagina in arbol)//ahora añado el siguiente
                    {
                        if (pagina == paginaAdelante)
                        {
                            pagina.Registros = paginaAdelante.Registros;
                        }
                    }

                    seRealizoElPrestamo = true;

                }
            }
            else if (paginaDondeElimine == auxiliarHojas.Last())//si es el extremo derecho
            {
                //reviso a la izquierda
                Pagina paginaAtras = auxiliarHojas.ElementAt(auxiliarHojas.IndexOf(auxiliarHojas.Last()) - 1);
                if (paginaAtras.Registros.Count >= 3)//reviso en la pagina de atras del extremo derecho
                {
                    //si puedo pedir prestado porque al quietarle un no va a pasar nada
                    auxiliarHojas.Last().Registros.Add(arbol.ElementAt(0).Registros.Last());//bajo el ultimo registro de la raiz al arbol donde necesito prestrado
                    
                    arbol.ElementAt(0).Registros.Remove(arbol.ElementAt(0).Registros.Last());//elimino el elemtento que acabo de ajar en la raiz
                    
                    arbol.ElementAt(0).Registros.Add(paginaAtras.Registros.Last());//ahora subo el ultimo de la pagina de la izquierda a la raiz

                    paginaAtras.Registros.Remove(paginaAtras.Registros.Last());//elimino el registro que acabo de subir a la raiz



                    //ahora ordeno la raiz
                    IEnumerable<Registro> paginaOrdenada = arbol.ElementAt(0).Registros.OrderBy(x => x.Clave);
                    arbol.ElementAt(0).Registros = paginaOrdenada.ToList();

                    //ahora ordeno la pagina donde estoy
                    IEnumerable<Registro> paginaOrdenada2 = auxiliarHojas.Last().Registros.OrderBy(x => x.Clave);
                    auxiliarHojas.Last().Registros = paginaOrdenada.ToList();

                    //como solo estoy trabajando con copias debo encontrar las paginas que modifique en el arbol real 
                    foreach (Pagina pagina in arbol)//primero asgno el extremo derecho
                    {
                        if (pagina == auxiliarHojas.Last())
                        {
                            pagina.Registros = auxiliarHojas.Last().Registros;
                        }
                    }

                    foreach (Pagina pagina in arbol)//ahora añado el siguiente
                    {
                        if (pagina == paginaAtras)
                        {
                            pagina.Registros = paginaAtras.Registros;
                        }
                    }

                }
                seRealizoElPrestamo = true;
            }
            else//si no estan en ninguno de los dos extremos
            {
                //voy a buscar primero en la izquierda
                Pagina paginaDondeEstoy = null;
                Pagina paginadeAtras = null;
                Pagina paginadeAdelante = null;

                //primero ubico la pagina en el auxiliar de hojas
                foreach (Pagina pagina2 in auxiliarHojas)
                {
                    if (pagina2 == paginaDondeElimine)
                    {
                        paginaDondeEstoy = pagina2;
                        break;
                    }
                }

                //ahora encuentro la pagina de atras
                paginadeAtras = auxiliarHojas.ElementAt(auxiliarHojas.IndexOf(paginaDondeEstoy) - 1);

                //ahora encuentro la pagina de adelante
                paginadeAdelante = auxiliarHojas.ElementAt(auxiliarHojas.IndexOf(paginaDondeEstoy) + 1);

                //voy a buscar primero en la izquierda
                if (paginadeAtras.Registros.Count >= 3)//reviso en la pagina de atras
                {

                }
                else if (paginadeAdelante.Registros.Count >= 3)
                {

                }
            }

            return seRealizoElPrestamo;

        }

        private Pagina buscaPaginaParaEliminaryEliminaRegistro(int clave)
        {
            Pagina paginaParaEliminar = null;

            foreach  (Pagina pagina in arbol)//recorro todas la paginas del arbol
            {
                //me aseguro que la pangina sea una hoja
                if (pagina.TipodePagina == "H")
                {
                    foreach (Registro registro in pagina.Registros)//recorro todo los registros de la pagina
                    {
                        if (registro.Clave == clave)//si clave del registro es igual a la clave que busco
                        {
                            //elimino el registro
                            pagina.Registros.Remove(registro);

                            //debo devolver esta pagina
                            paginaParaEliminar = pagina;
                            return paginaParaEliminar;
                        }
                    }
                }
            }

            return paginaParaEliminar;
        }
        private void button3_Click(object sender, EventArgs e)//eliminar
        {
            try
            {
                int clavePorEliminar = int.Parse(textBox1.Text);
                if (!ingresados.Contains(clavePorEliminar))
                {
                    MessageBox.Show("Esta clave no existe en el arbol");
                }
                else
                {
                    eliminar(clavePorEliminar);
                    ingresados.Remove(clavePorEliminar);
                }
                textBox1.Text = "";
            }
            catch (FormatException fe)
            {

                MessageBox.Show("Ingresa un dato valido");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //LECTURA DE UUN ARCHIVO
            StreamReader lectura;
            string cadena;
            richTextBox_Leer.Text = "";
            try
            {
                lectura = File.OpenText("bptree.txt");
                //Lectura Adelantada

                cadena = lectura.ReadLine();
                while (cadena != null)
                {
                    richTextBox_Leer.Text += cadena + "\n";
                    cadena = lectura.ReadLine();
                }


                lectura.Close();

            }
            catch (FileNotFoundException fe)
            {
                Console.WriteLine("Error" + fe);
            }
            catch (IOException fe)
            {
                Console.WriteLine("Error" + fe);
            }
        }


        private void button5_Click(object sender, EventArgs e)//ingresar dese archivo
        {
           StreamReader lectura2;
            string cadena;
            List<string> campos = new List<string>();
            char[] separador = { ',' };

            try
            {
                lectura2 = File.OpenText("bptreeLectura.txt");

                cadena = lectura2.ReadLine();

                campos = cadena.Split(',').ToList();

                lectura2.Close();

                foreach (string item in campos)
                {
                    ingresar(int.Parse(item));
                }

            }
            catch (FileNotFoundException fe)
            {
                Console.WriteLine("Erro" + fe);
            }
            catch (Exception fe)
            {
                Console.WriteLine("error" + fe.Message);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
