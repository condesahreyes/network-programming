using System.Text.RegularExpressions;
using System;

namespace Cliente.Constantes
{
    public static class Metodo
    {
        public static int ObtenerOpcion(string mensaje, int opcionMinima, int opcionMaxima)
        {
            
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(mensaje);
                Console.ForegroundColor = ConsoleColor.White;

                var opcion = Console.ReadLine();

                if (!Regex.IsMatch(opcion, "^[" + opcionMinima + "-" + opcionMaxima + "]$"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" \nIngrese una opción valida. Entre " + opcionMinima + " y "
                        + opcionMaxima + "\n");
                }
                else
                {
                    Console.Clear();
                    return Convert.ToInt32(opcion);
                }
            }
            
        }

    }
}
