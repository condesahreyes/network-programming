using System.Collections.Generic;
using LogicaNegocio;
using System;

namespace Cliente.Constantes
{
    public static class Mensaje
    {
        public static string menuPrincipal =
            "********************** Menú Principal **********************" +
            "\n 0. Salir" +
            "\n 1. Iniciar Sesión" +
            "\n Seleccione una opción \n";

        public static string menuFuncionalidades =
            "********************** Menú Usuario **********************" +
            "\n 0. Desconectarse" +
            "\n 1. Publicar un juego" +
            "\n 2. Baja y modificación de juego" +
            "\n 3. Buscar Juegos " +
            "\n 4. Publicar una calificación del Juego" +
            "\n 5. Detalle de un Juego" +
            "\n Seleccione una opción \n";

        public static string seleccioneJuego = "Seleccione el juego que desea visualizar.";

        public static void Conectado(string conectadoA)
        {
            Console.WriteLine("Socket conectado a " + conectadoA);
        }

        public static void Desconectar()
        {
            Console.WriteLine("Gracias por utilizar nuestro sistema.");
        }

        public static void JuegoCreado()
        {
            Console.WriteLine("Su juego se ha dado de alta con exito");
        }


        public static void JuegoExistente()
        {
            Console.WriteLine("El juego ya existe en el sistema");
        }

        public static void NoExistenJuegos()
        {
            Console.WriteLine("Aún no existen juegos en el sistema.");
        }
        
        public static void MostrarJuego(Juego juego)
        {
            Console.WriteLine(juego.ToString());
        }

        public static void MostrarJuegos(List<string> juegos)
        {
                for (int i = 0; i < juegos.Count; i++)
                    Console.WriteLine(i + ". " + juegos[i]);
        }

        public static void CalificacionCreada()
        {
            Console.WriteLine("Su calificacion se a publicado con exito");
        }
    }
}
