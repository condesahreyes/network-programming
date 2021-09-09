using LogicaNegocio;
using Servidor.FuncionalidadesEntidades;
using Servidor.FuncionalidadesPorEntidad;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        internal void InicioSesionCliente(string nombreUsuario)
        {
            Usuario miUsuario = funcionesUsuario.ObtenerUsuario(nombreUsuario);
        }

        public void CrearJuego(byte[] obj)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(obj, 0, obj.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Juego juego = (Juego)binForm.Deserialize(memStream);

            funcionesJuego.AgregarJuego(juego);
            Console.ReadLine(); // Borrar
        }

    }
}
