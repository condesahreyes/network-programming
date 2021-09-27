﻿using Microsoft.Extensions.Configuration;
using Protocolo.Transferencia_de_datos;
using System.Collections.Generic;
using System.Net.Sockets;
using Cliente.Constantes;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System.IO;

namespace Cliente
{
    public class Conexion
    {
        Transferencia transferencia;

        public Conexion()
        {
            IConfiguration configuracion = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false).Build();

            int puertoCliente = int.Parse(configuracion["puertoCliente"]);
            int puertoServidor = int.Parse(configuracion["puertoServidor"]);
            string ipServidor = configuracion["ipServidor"];
            string ipCliente = configuracion["ipCliente"];

            IPEndPoint endPointCliente = new IPEndPoint(IPAddress.Parse(ipCliente), puertoCliente);
            IPEndPoint endPointServidor = new IPEndPoint(IPAddress.Parse(ipServidor), puertoServidor);

            Socket sender = new Socket(endPointCliente.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            sender.Bind(endPointCliente);

            transferencia = new Transferencia(sender);

            // Conectarse desde un socket client
            sender.Connect(endPointServidor);
            Mensaje.Conectado(sender.RemoteEndPoint.ToString());
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            Controlador.Desconectar(transferencia);
        }

        public string EsperarPorRespuesta()
        {
            string respuesta = Controlador.RecibirMensajeGenerico
                (transferencia, Constante.largoEncabezado);
            string[] respuestas = respuesta.Split("#");

            return respuestas[0];
        }

        public void EnvioEncabezado(int largoMensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(largoMensaje, accion);
            Controlador.EnviarEncabezado(transferencia, encabezado);
        }

        public Encabezado RecibirEncabezado()
        {
            return Controlador.RecibirEncabezado(transferencia);
        }

        public void EnvioDeMensaje(string mensaje)
        {
            Controlador.EnviarDatos(transferencia, mensaje);
        }

        public string RecibirMensaje(int largoMensaje)
        {
            return Controlador.RecibirMensajeGenerico(transferencia, largoMensaje);
        }

        public Juego RecibirUnJuegoPorTitulo(string titulo)
        {
            Juego juego = Controlador.RecibirUnJuegoPorTitulo(transferencia, titulo);
            return juego;
        }

        public List<string> RecibirListaDeJuegos()
        {
            EnvioEncabezado(0, Accion.ListaJuegos);

            string juegos = Controlador.RecibirEncabezadoYMensaje
                (transferencia, Accion.ListaJuegos);

            return Mapper.StringAListaJuegosString(juegos);
        }

        public void EnvioDeArchivo(string archivo)
        {
            ControladorDeArchivos.EnviarArchivo(archivo, transferencia);
        }

        public string RecibirArchivos(int largoMensaje, string caratula)
        {
            string nombreArchivo = Controlador.RecibirMensajeGenerico(transferencia, largoMensaje);
            ControladorDeArchivos.RecibirArchivos(transferencia, nombreArchivo);

            return nombreArchivo;
        }
    }
}