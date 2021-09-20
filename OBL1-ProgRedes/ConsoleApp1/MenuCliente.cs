using Cliente.Constantes;
using LogicaNegocio;

namespace Cliente
{
    public class MenuCliente
    {
        private FuncionalidadesCliente funcionalidadesCliente;
        private Usuario usuario;

        public MenuCliente()
        {
            this.funcionalidadesCliente = FuncionalidadesCliente.ObtenerInstancia();
            MenuPrincipal();
        }

        public void MenuPrincipal()
        {
            int opcion = Metodos.ObtenerOpcion(Mensaje.menuPrincipal, 0, 1);

            switch (opcion)
            {
                case 0:
                    Mensaje.Desconectar();
                    funcionalidadesCliente.DesconectarUsuario(usuario);
                    break;
                case 1:
                    usuario = funcionalidadesCliente.InicioSesion();
                    break;
            }

            MenuFuncionalidades();
        }

        public void MenuFuncionalidades()
        {
            int opcion = -1;
            while (opcion != 0)
            {
                opcion = Metodos.ObtenerOpcion(Mensaje.menuFuncionalidades, 0, 5);
                switch (opcion)
                {
                    case 0:
                        Mensaje.Desconectar();
                        funcionalidadesCliente.DesconectarUsuario(this.usuario);
                        break;
                    case 1:
                        funcionalidadesCliente.PublicarJuego();
                        break;
                    case 2:

                        break;
                    case 3:

                        break;
                    case 4:
                        funcionalidadesCliente.CalificarUnJuego(this.usuario);
                        break;
                    case 5:
                        funcionalidadesCliente.DetalleDeUnJuego();
                        break;
                }
            }
        }

    }
}
