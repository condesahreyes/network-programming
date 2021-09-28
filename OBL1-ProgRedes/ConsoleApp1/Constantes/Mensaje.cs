using System.Collections.Generic;
using LogicaNegocio;
using System;

namespace Cliente.Constantes
{
    public static class Mensaje
    {
        public static string menuPrincipal =
            "********************* Menú Principal **********************" +
            "\n0. Salir" +
            "\n1. Iniciar Sesión\n" +
            "\nSeleccione una opción:";

        public static string menuFuncionalidades =
            "********************** Menú Usuario ***********************" +
            "\n0. Desconectarse" +
            "\n1. Publicar un juego" +
            "\n2. Baja y modificación de juego" +
            "\n3. Buscar Juegos " +
            "\n4. Publicar una calificación del Juego" +
            "\n5. Detalle de un Juego\n" +
            "\nSeleccione una opción:";

        public static string seleccioneJuego = "\nSeleccione el juego que desea visualizar:";

        public static string ranking = "Ingrese el ranking de califación por el que desea filtrar";

        public static string bajaModificacion = "0. Baja\n1. Modificacion " +
            "\n\nSeleccione la opción que desea realizar:";

        public static string buscarJuegoPorFiltro = "0. Buscar por titulo \n1. Buscar por genero " +
            "\n2. Buscar por Calificacion\n\nSeleccione una opción:";

        public static void Conectado(string conectadoA)
        {
            MostrarMensajeOk("Socket conectado a " + conectadoA + "\n");
        }

        public static void ErrorGenerico()
        {
            MostrarMensajeError("Ocurrio un error inesperado");
        }

        public static void Desconectar()
        {
            MostrarMensajeGenerico("Gracias por utilizar nuestro sistema. " +
                "\nPresione enter para cerrar la consola");
            Console.ReadLine();
        }

        public static void ConexionPerdida()
        {
            MostrarMensajeError("Se perdió la conexión con el servidor\nPresione enter para salir");
            Console.ReadLine();
        }

        public static void InicioSesion()
        {
            MostrarMensajeOk("Se ha iniciado sesión \n");
        }

        public static void JuegoCreado()
        {
            MostrarMensajeOk("Su juego se ha dado de alta con exito \n");
        }

        public static void JuegoExistente()
        {
            MostrarMensajeError("El juego ya existe en el sistema \n");
        }

        public static void BuscarJuegoPorTitulo()
        {
            MostrarMensajeGenerico("Ingrese el Titulo por el que desea filtrar");
        }

        public static void BuscarJuegoPorGenero()
        {
            MostrarMensajeGenerico("Ingrese el Genero por el que desea filtrar");
        }

        public static void NoExistenJuegos()
        {
            MostrarMensajeError("Aún no existen juegos en el sistema. \n");
        }

        public static void MostrarJuego(Juego juego)
        {
            MostrarMensajeGenerico(juego.ToString());
        }

        public static void JuegoEliminadoOk()
        {
            MostrarMensajeOk("Se ha eliminado el juego correctamente.\n");
        }

        public static void JuegoEliminadoError()
        {
            MostrarMensajeError("Ocurrio un problema, no se pudo eliminar el juego.");
        }

        public static void JuegoModificadoOk()
        {
            MostrarMensajeOk("Se ha modificado el juego correctamente.");
        }

        public static void JuegoModificadoError()
        {
            MostrarMensajeError("Error, el nuevo titulo ya existe en el sistema.");
        }

        public static void CalificacionCreada()
        {
            MostrarMensajeOk("Su calificacion se a publicado con exito \n");
        }

        public static void MostrarJuegos(List<string> juegos)
        {
            if(juegos.Count == 0) {
                MostrarMensajeError("No hay juegos registrados en el sistema");
                return;
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("******************** Listado de juegos *********************\n");

            for (int i = 0; i < juegos.Count; i++)
                Console.WriteLine(i + ". " + juegos[i]);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void MostrarObjetoJuego(List<Juego> juegos)
        {
            if (juegos == null)
            {
                MostrarMensajeError("No hay juegos registrados en el sistema\n");
                return;
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("******************** Listado de juegos *********************\n");

            for (int i = 0; i < juegos.Count; i++)
                Console.WriteLine(i + ". " + juegos[i]);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void MostrarMensajeGenerico(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(mensaje);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void MostrarMensajeError(string mensaje)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(mensaje);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void MostrarMensajeOk(string mensaje)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(mensaje);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
