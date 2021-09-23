using Cliente.Constantes;
using LogicaNegocio;

namespace Cliente
{
    public class Menu
    {
        private Funcionalidad funcionalidadesCliente;
        private Usuario usuario;

        public Menu()
        {
            this.funcionalidadesCliente = Funcionalidad.ObtenerInstancia();
            MenuPrincipal();
        }

        public void MenuPrincipal()
        {
            int opcion = Metodo.ObtenerOpcion(Mensaje.menuPrincipal, 0, 1);

            switch (opcion)
            {
                case 0:
                    Mensaje.Desconectar();
                    funcionalidadesCliente.DesconectarUsuario(usuario);
                    break;
                case 1:
                    usuario = funcionalidadesCliente.InicioSesion();
                    MenuFuncionalidades();
                    break;
            }

        }

        public void MenuFuncionalidades()
        {
            int opcion = -1;
            while (opcion != 0)
            {
                opcion = Metodo.ObtenerOpcion(Mensaje.menuFuncionalidades, 0, 5);
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
                        funcionalidadesCliente.BuscarJuego();
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
