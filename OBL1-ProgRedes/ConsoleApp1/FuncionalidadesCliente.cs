using System;
using System.Text.RegularExpressions;

namespace Cliente
{
    public class FuncionalidadesCliente
    {
        string mensajeMenuPrincipal =
            "********************** Menú Principal **********************" +
            "\n 0. Salir" +
            "\n 1. Iniciar Sesión" +
            "\n 2. Registrarse" +
            "\n Seleccione una opción \n";

        string mensajeMenuFuncionalidades =
            "********************** Menú Usuario **********************" +
            "\n 0. Desconectarse" +
            "\n 1. Ver catalogo de juegos" +
            "\n 2. Adquirir un juego" + 
            "\n 3. Publicar un juego" +
            "\n 4. Publicar una calificación del Juego" +
            "\n 5. Buscar Juegos " +
            "\n Seleccione una opción \n";

        public void MenuPrincipal()
        {
            int opcion = ObtenerOpcionSeleccionada(mensajeMenuPrincipal, 0, 2);
        }

        public void MenuFuncionalidades()
        {
            int opcion = ObtenerOpcionSeleccionada(mensajeMenuFuncionalidades, 0, 5);
        }

        private int ObtenerOpcionSeleccionada(string mensajeMenu, int opcionMinima, int opcionMaxima)
        {

            while (true)
            {
                Console.WriteLine(mensajeMenu);
                var opcion = Console.ReadLine();

                if (!Regex.IsMatch(opcion, "^[opcionMinima-opcionMaxima]$"))
                    Console.WriteLine(" \n Ingrese una opción valida. Entre opcionMinima y opcionMaxima \n");
                else
                    return Convert.ToInt32(opcion);
            }

        } 
    }
}
