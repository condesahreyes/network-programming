using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cliente.Constantes
{
    public static class Metodos
    {
        public static int ObtenerOpcion(string mensaje, int opcionMinima, int opcionMaxima)
        {
            while (true)
            {
                Console.WriteLine(mensaje);
                var opcion = Console.ReadLine();

                if (!Regex.IsMatch(opcion, "^[" + opcionMinima + "-" + opcionMaxima + "]$"))
                    Console.WriteLine(" \n Ingrese una opción valida. Entre " + opcionMinima + " y "
                        + opcionMaxima + "\n");
                else
                    return Convert.ToInt32(opcion);
            }
        }

    }
}
