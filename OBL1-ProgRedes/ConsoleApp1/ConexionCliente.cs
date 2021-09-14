﻿using System.Net.Sockets;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System;

namespace Cliente
{
    public class ConexionCliente
    {
        int port = 9000;

        Socket sender;
        IPEndPoint ipEndPoint;
        Transferencia transferencia;

        public ConexionCliente()
        {
            port = 9000;

            ipEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            transferencia = new Transferencia(sender);

            // Conectarse desde un socket client
            sender.Connect(ipEndPoint);
            Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
        }

        public  void EnvioEncabezado(Encabezado encabezado)
        {
            TransferenciaDatos.EnviarEncabezado(transferencia, encabezado);
        }

        public void EnvioDeMensaje(string mensaje)
        {
            TransferenciaDatos.EnviarDatos(transferencia, mensaje);
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            sender.Shutdown(SocketShutdown.Both);
            Console.WriteLine("Se desconectoooo");
            Console.ReadLine();
            sender.Close();
            //sender.Disconnect(true);
        }

        public void MandarNuevoJuego(Juego juego)
        {
            
        }

    }
}
