﻿using LogicaNegocio;
using System;

namespace Protocolo
{
    public class Mapper
    {

        public static string EncabezadoAString(Encabezado encabezado)
        {
            string encabezadoString = encabezado.accion + "#" + encabezado.largoMensaje;

            while (encabezadoString.Length < ConstantesDelProtocolo.largoEncabezado)
                encabezadoString += "#";

            return encabezadoString;
        }

        public static Encabezado StringAEncabezado(string encabezadoString)
        {
            string[] datosEncabezado = encabezadoString.Split("#");
            string accion = datosEncabezado[0];

            int largoMensaje = Convert.ToInt32(datosEncabezado[1]);
            Encabezado encabezado = new Encabezado(largoMensaje, accion);

            return encabezado;
        }

        public static Usuario StringAUsuario(string nombreUsuario)
        {
            return new Usuario(nombreUsuario);
        }

        public static string UsuarioAString(Usuario usuario)
        {
            return usuario.NombreUsuario;
        }

        public static string JuegoAString(Juego juego)
        {
            string juegoEnString = juego.Titulo + "#" + juego.Genero + "#" + juego.Sinopsis;

            return juegoEnString;
        }

        public static Juego StringAJuego(string juegoEnString)
        {
            string[] datosDelJuego = juegoEnString.Split("#");

            string titulo = datosDelJuego[0];
            string genero = datosDelJuego[1];
            string sinopsis = datosDelJuego[2];

            Juego juego = new Juego(titulo, genero, sinopsis, null);

            return juego;
        }
    }
}
