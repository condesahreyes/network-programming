using LogicaNegocio;
using Servidor.FuncionalidadesEntidades;
using Servidor.FuncionalidadesPorEntidad;

namespace Servidor
{
    public class FuncionalidadesServidor
    {
        FuncionalidadesUsuario funcionesUsuario;
        FuncionalidadesJuego funcionesJuego;

        public FuncionalidadesServidor()
        {
            funcionesUsuario = new FuncionalidadesUsuario();
            funcionesJuego = new FuncionalidadesJuego();
        }

        internal Usuario InicioSesionCliente(Usuario usuario)
        {
            return funcionesUsuario.ObtenerUsuario(usuario);
        }

        public bool CrearJuego(Juego juego)
        {
            return funcionesJuego.AgregarJuego(juego);
        }
    }
}
