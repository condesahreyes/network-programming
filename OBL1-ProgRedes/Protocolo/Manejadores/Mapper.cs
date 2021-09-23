using System.Collections.Generic;
using LogicaNegocio;
using System;

namespace Protocolo
{
    public class Mapper
    {

        public static string EncabezadoAString(Encabezado encabezado)
        {
            string encabezadoString = encabezado.accion + "#" + encabezado.largoMensaje;

            while (encabezadoString.Length < Constante.largoEncabezado)
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
            string juegoEnString = "";
            string calificaciones = "";
            if(juego != null)
            {
                foreach (Calificacion unaCalificacion in juego.calificaciones)
                    calificaciones += unaCalificacion.Comentario + "@" + unaCalificacion.Nota + "@" + unaCalificacion.Usuario + "/";

                    juegoEnString = juego.Titulo + "#" + juego.Genero + "#" + juego.Sinopsis + "#" + calificaciones;
            }

            return juegoEnString;
        }

        public static Juego StringAJuego(string juegoEnString)
        {
            Juego juego = null;
            if (juegoEnString != "")
            {
                string[] datosDelJuego = juegoEnString.Split("#");

                string titulo = datosDelJuego[0];
                string genero = datosDelJuego[1];
                string sinopsis = datosDelJuego[2];
                string caliicaciones = datosDelJuego[3];

                string[] datosDeCalificacion = caliicaciones.Split("/");

                juego = new Juego(titulo, genero, sinopsis, null);

                if (datosDeCalificacion[0] != "")
                    for (int i = 0; i < datosDeCalificacion.Length - 1; i++)
                    {
                        string[] detalleCalificacion = datosDeCalificacion[i].Split("@");
                        string comentario = detalleCalificacion[0];
                        int nota = Convert.ToInt32(detalleCalificacion[1]);
                        juego.calificaciones.Add(new Calificacion(titulo, nota, comentario, detalleCalificacion[2]));
                    }

            }

            return juego;
        }

        public static string CalificacionAString(Calificacion calificacion)
        {
            string calificacionEnString = calificacion.TituloJuego + "#" + calificacion.Nota + "#" 
                + calificacion.Comentario + "#" + calificacion.Usuario;

            return calificacionEnString;
        }

        public static Calificacion StringACalificacion(string calificacionEnString)
        {
            string[] datosDeCalificacion = calificacionEnString.Split("#");

            string tituloJuego = datosDeCalificacion[0];
            int nota = Convert.ToInt32(datosDeCalificacion[1]);
            string comentario = datosDeCalificacion[2];
            string nombreUsuario = datosDeCalificacion[3];

            Calificacion calificacion = new Calificacion(tituloJuego, nota, comentario, nombreUsuario);

            return calificacion;
        }

        public static string ListaDeJuegosAString(List<Juego> juegos)
        {
            string juegosString = "";

            foreach (Juego juego in juegos)
                juegosString += juego.Titulo + "#";

            return juegosString;
        }

        public static List<string> StringAListaJuegosString(string juegosString)
        {
            List<string> juegos = new List<string>();
            string[] cadaJuego = juegosString.Split("#");

            for (int i = 0; i < cadaJuego.Length-1; i++)
                juegos.Add(cadaJuego[i]);

            return juegos;
        }

        public static string ListaJuegosAString(List<Juego> juegos)
        {
            string juegosString = "";

            foreach(Juego juego in juegos)
            { 
                juegosString += JuegoAString(juego) +";";
            }
            return juegosString;
        }
        public static List<Juego> PasarStringAListaDeJuegos(string juegosString)
        {
            if(juegosString == "")
            {
                return null;
            }
            List<Juego> juegos = new List<Juego>();
            string[] juego = juegosString.Split(";");

            for(int i=0; i< juego.Length; i++)
            {
                juegos.Add(StringAJuego(juego[i]));
            }

            return juegos;
        }
    }
}
