using Cliente.Constantes;
using LogicaNegocio;
using System.Threading.Tasks;

namespace Cliente
{
    public class Menu
    {
        private Funcionalidad funcionalidadesCliente;
        private Usuario usuario;

        public Menu() { }

        public async Task MenuPrincipal()
        {
            this.funcionalidadesCliente = await Funcionalidad.ObtenerInstancia();
            int opcion = Metodo.ObtenerOpcion(Mensaje.menuPrincipal, 0, 1);

            switch (opcion)
            {
                case 0:
                    Mensaje.Desconectar();
                    funcionalidadesCliente.DesconectarUsuario(usuario);
                    break;
                case 1:
                    usuario = await funcionalidadesCliente.InicioSesion();
                    await MenuFuncionalidades();
                    break;
            }
        }

        public async Task MenuFuncionalidades()
        {
            int opcion = -1;
            while (opcion != 0)
            {
                opcion = Metodo.ObtenerOpcion(Mensaje.menuFuncionalidades, 0, 7);
                switch (opcion)
                {
                    case 0:
                        Mensaje.Desconectar();
                        funcionalidadesCliente.DesconectarUsuario(this.usuario);
                        break;
                    case 1:
                        await funcionalidadesCliente.PublicarJuego();
                        break;
                    case 2:
                        await funcionalidadesCliente.BajaModificacionJuego();
                        break;
                    case 3:
                        await funcionalidadesCliente.BuscarJuego();
                        break;
                    case 4:
                        await funcionalidadesCliente.CalificarUnJuego(this.usuario);
                        break;
                    case 5:
                        await funcionalidadesCliente.DetalleDeUnJuego();
                        break;
                    case 6:
                        await funcionalidadesCliente.AdquirirJuego(this.usuario);
                        break;
                    case 7:
                        await funcionalidadesCliente.ListaJuegosAdquiridos(this.usuario);
                        break;
                }
            }
        }

    }
}
