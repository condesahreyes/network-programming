using Servidor.FuncionalidadesPorEntidad;
using Servidor.FuncionalidadesEntidades;
using System.Collections.Generic;
using Servidor.Exception;
using LogicaNegocio;
using Protocolo;
using System;

namespace Servidor
{
    public class FuncionalidadesServidor
    {
        FuncionalidadesUsuario funcionesUsuario;
        FuncionalidadesJuego funcionesJuego;
        Transferencia transferencia;

        public FuncionalidadesServidor(Transferencia transferencia)
        {
            this.transferencia = transferencia;
            this.funcionesUsuario = new FuncionalidadesUsuario();
            this.funcionesJuego = new FuncionalidadesJuego();
        }

        internal Usuario InicioSesionCliente(Usuario usuario, int largoMensajeARecibir)
        {
            usuario = ControladorDeTransferencia.RecibirUsuario(transferencia, largoMensajeARecibir);
            return funcionesUsuario.ObtenerUsuario(usuario);
        }

        public void CrearJuego(int largoMensajeARecibir)
        {
            Juego juego = ControladorDeTransferencia.PublicarJuego(transferencia, largoMensajeARecibir);
            try
            {
                bool creo = funcionesJuego.AgregarJuego(juego);

                if (creo)
                    ControladorDeTransferencia.EnviarMensajeClienteOk(transferencia);
                else
                    ControladorDeTransferencia.EnviarMensajeClienteError(transferencia);

            }
            catch (JuegoExistente)
            {
                ControladorDeTransferencia.EnviarMensajeClienteError(transferencia);
            }

        }

        internal void EnviarListaJuegos()
        {
            List<Juego> juegos = funcionesJuego.ObtenerJuegos();
            string juegosString = Mapper.ListaDeJuegosAString(juegos);
            Encabezado encabezado = new Encabezado(juegosString.Length, Accion.ListaJuegos);
            ControladorDeTransferencia.EnviarEncabezado(transferencia, encabezado);
            ControladorDeTransferencia.EnviarDatos(transferencia, juegosString);
        }

        internal void EnviarDetalleDeUnJuego(int largoMensaje)
        {
            string tituloJuego = ControladorDeTransferencia.RecibirMensajeGenerico(transferencia, largoMensaje);
            Juego juego = funcionesJuego.ObtenerJuegoPorTitulo(tituloJuego);
            string juegoEnString = Mapper.JuegoAString(juego);

            Encabezado encabezado = new Encabezado(juegoEnString.Length, Accion.EnviarDetalleJuego);
            ControladorDeTransferencia.EnviarEncabezado(transferencia, encabezado);
            ControladorDeTransferencia.EnviarDatos(transferencia, juegoEnString);

        }
    }
}
