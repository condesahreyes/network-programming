using Microsoft.Extensions.Configuration;
using Servidor.FuncionalidadesPorEntidad;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System.IO;
using System;

namespace Servidor
{
    public class Conexion
    {
        private List<Socket> clientesConectados = new List<Socket>();
        private Funcionalidad funcionalidadesServidor;
        private Socket handler;

        private int cantConexionesEnEspera;
        private int puerto;

        private bool salir = false;

        private string ipServidor;

        public Conexion() 
        {
            IConfiguration configuracion = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false).Build();

            puerto = int.Parse(configuracion["port"]);
            cantConexionesEnEspera = int.Parse(configuracion["backLog"]);
            ipServidor = configuracion["ip"];
            Escuchar();
        }

        public void Escuchar()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);

            Socket listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(endPoint);

            listener.Listen(cantConexionesEnEspera);

            Thread hiloDeEscucha = new Thread(() => EscucharPorUsuario(listener));
            hiloDeEscucha.Start();

            MenuServidor(listener);
        }

        private void MenuServidor(Socket listener)
        {
            LogicaJuego _logicaJuegos = new LogicaJuego();
            
            Console.WriteLine("Bienvenido al server \n0- Terminar la conexion. \n1- Ver catalogo de juegos");
   
            string accion = Console.ReadLine();
            if (!Regex.IsMatch(accion, "^[" + 0 + "-" + 1 + "]$"))
            {
                Console.Clear();
                Console.WriteLine("Ingrese una opcion valida entre 0 y 1");
                MenuServidor(listener);
            }
            switch (accion)
            {
                case "0":
                    salir = true;

                    listener.Close(0);

                    foreach (var socketCliente in clientesConectados)
                    {
                        socketCliente.Shutdown(SocketShutdown.Both);
                        socketCliente.Close(1);
                    }
                    break;
                case "1":
                    _logicaJuegos.VerCatalogoJuegos();
                    MenuServidor(listener);
                    break;
            }
        }

        private void EscucharPorUsuario(Socket listener)
        {
            Usuario usuario = null;
            try
            {
                while (!salir)
                {
                    handler = listener.Accept();
                    Thread hiloPorUsuario = new Thread(() => ConexionUsuario(usuario, handler));
                    hiloPorUsuario.Start();
                    clientesConectados.Add(handler);
                }
            }
            catch (SocketException)
            {
                if(usuario != null)
                    Console.WriteLine("Conexion finalizada por usuario: " + usuario.NombreUsuario);
                else
                    Console.WriteLine("Conexion finalizada por una maquina sin usuario");

                Console.WriteLine("Presione enter si desea finalizar la conexion del servidor....");
            }
        }

        private void ConexionUsuario(Usuario usuario, Socket socket)
        {
            while (!salir)
            {
                try
                {
                    EjecutarAccion(ref usuario, socket);
                }
                catch (SocketException)
                {
                    return;
                }
            }

        }

        private void EjecutarAccion(ref Usuario usuario, Socket handler)
        {
                Encabezado encabezado = Controlador.RecibirEncabezado(new Transferencia(handler));
                Transferencia transferencia = new Transferencia(handler);

                funcionalidadesServidor = new Funcionalidad(transferencia);

                string accion = encabezado.accion;
                int largoMensajeARecibir = encabezado.largoMensaje;

                switch (accion)
                {
                    case Accion.Login:
                        usuario = funcionalidadesServidor.InicioSesionCliente(usuario, largoMensajeARecibir);
                        break;
                    case Accion.AdquirirJuego:
                        funcionalidadesServidor.AdquirirJuego(largoMensajeARecibir, usuario);
                        break;
                    case Accion.VerJuegosAdquiridos:
                        funcionalidadesServidor.VerJuegosAdquiridos(largoMensajeARecibir, usuario);
                        break;
                    case Accion.PublicarJuego:
                        funcionalidadesServidor.CrearJuego(largoMensajeARecibir);
                        break;
                    case Accion.ListaJuegos:
                        funcionalidadesServidor.EnviarListaJuegos();
                        break;
                    case Accion.PedirDetalleJuego:
                        funcionalidadesServidor.EnviarDetalleDeUnJuego(largoMensajeARecibir);
                        break;
                    case Accion.PublicarCalificacion:
                        funcionalidadesServidor.CrearCalificacion(largoMensajeARecibir);
                        break;
                    case Accion.BuscarTitulo:
                        funcionalidadesServidor.BuscarJuegoPorTitulo(largoMensajeARecibir);
                        break;
                    case Accion.BuscarGenero:
                        funcionalidadesServidor.BuscarJuegoPorGenero(largoMensajeARecibir);
                        break;
                    case Accion.BuscarCalificacion:
                        funcionalidadesServidor.BuscarJuegoPorCalificacion(largoMensajeARecibir);
                        break;
                    case Accion.EliminarJuego:
                        funcionalidadesServidor.EliminarJuego(largoMensajeARecibir);
                        break;
                    case Accion.ModificarJuego:
                        funcionalidadesServidor.ModificarJuego(largoMensajeARecibir);
                        break;
                }
        }

    }
}
