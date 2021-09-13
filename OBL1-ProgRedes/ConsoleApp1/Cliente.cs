using LogicaNegocio;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cliente
{
    public class Cliente
    {
        private static FuncionalidadesCliente funcionalidades;
        private static ConexionCliente conexionCliente;


        static void Main(string[] args)
        {
            conexionCliente = new ConexionCliente();
            funcionalidades = new FuncionalidadesCliente(conexionCliente);

            funcionalidades.MenuPrincipal();
        }

    }
}
