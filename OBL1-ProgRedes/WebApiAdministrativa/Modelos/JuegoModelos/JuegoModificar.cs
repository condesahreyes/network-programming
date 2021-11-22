using LogicaNegocio;

namespace WebApiAdministrativa.Modelos.JuegoModelos
{
    public class JuegoModificar
    {
        public string Sinopsis { get; set; }
        public string Caratula { get; set; }
        public string Genero { get; set; }

        public JuegoModificar(){ }

        public static Juego ModeloADominio(JuegoModificar modelo, string tituloJuego)
        {
            return new Juego(tituloJuego, modelo.Genero, modelo.Sinopsis,
                modelo.Caratula);
        }
    }
}
