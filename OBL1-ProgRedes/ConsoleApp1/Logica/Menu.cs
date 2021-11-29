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

        public async Task MenuPrincipalAsync()
        {
            this.funcionalidadesCliente = await Funcionalidad.ObtenerInstanciaAsync();
            int opcion = Metodo.ObtenerOpcion(Mensaje.menuPrincipal, 0, 1);

            switch (opcion)
            {
                case 0:
                    Mensaje.Desconectar();
                    funcionalidadesCliente.DesconectarUsuario(usuario);
                    break;
                case 1:
                    usuario = await funcionalidadesCliente.InicioSesionAsync();
                    await MenuFuncionalidadesAsync();
                    break;
            }
        }

        public async Task MenuFuncionalidadesAsync()
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
                        await funcionalidadesCliente.PublicarJuegoAsync();
                        break;
                    case 2:
                        await funcionalidadesCliente.BajaModificacionJuegoAsync();
                        break;
                    case 3:
                        await funcionalidadesCliente.BuscarJuegoAsync();
                        break;
                    case 4:
                        await funcionalidadesCliente.CalificarUnJuegoAsync(this.usuario);
                        break;
                    case 5:
                        await funcionalidadesCliente.DetalleDeUnJuegoAsync();
                        break;
                    case 6:
                        await funcionalidadesCliente.AdquirirJuegoAsync(this.usuario);
                        break;
                    case 7:
                        await funcionalidadesCliente.ListaJuegosAdquiridosAsync(this.usuario);
                        break;
                }
            }
        }

    }
}
