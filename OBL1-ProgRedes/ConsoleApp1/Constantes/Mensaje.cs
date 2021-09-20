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

        public static void ErrorGenerico()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ocurrio un error inesperado");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Conectado(string conectadoA)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Socket conectado a " + conectadoA);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Desconectar()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Gracias por utilizar nuestro sistema.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void InicioSesion()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Se ha iniciado sesión \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void JuegoCreado()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Su juego se ha dado de alta con exito \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void JuegoExistente()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("El juego ya existe en el sistema \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void NoExistenJuegos()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Aún no existen juegos en el sistema. \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void MostrarJuego(Juego juego)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(juego.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void MostrarJuegos(List<string> juegos)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("******************** Listado de juegos *********************\n");

            for (int i = 0; i < juegos.Count; i++)
                Console.WriteLine(i + ". " + juegos[i]);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void CalificacionCreada()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Su calificacion se a publicado con exito \n");
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
