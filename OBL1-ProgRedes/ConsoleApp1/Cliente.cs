using System;

namespace Cliente
{
    public class Cliente
    {
        private static FuncionalidadesCliente funcionalidades;

        static void Main(string[] args)
        {
            funcionalidades = new FuncionalidadesCliente();
            funcionalidades.MenuPrincipal();
        }
    }
}
