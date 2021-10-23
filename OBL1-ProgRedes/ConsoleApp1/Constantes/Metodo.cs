using System.Text.RegularExpressions;
using System;

namespace Cliente.Constantes
{
    public static class Metodo
    {
        public static int ObtenerOpcion(string mensaje, int opcionMinima, int opcionMaxima)
        {
            int opcionInt = 0;
            var opcion = "";
            bool opcionObtenida = false;
            while (!opcionObtenida)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(mensaje);
                Console.ForegroundColor = ConsoleColor.White;

                opcion = Console.ReadLine();

                if (!Regex.IsMatch(opcion, "^[" + opcionMinima + "-" + opcionMaxima + "]$"))
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingrese una opción valida. Entre " + opcionMinima + " y "
                        + opcionMaxima + "\n");
                }
                else
                {
                    Console.Clear();
                    opcionInt  = Convert.ToInt32(opcion);
                    opcionObtenida = true;
                }
            }
            return opcionInt;

        }

    }
}
