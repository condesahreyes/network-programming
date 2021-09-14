using LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
