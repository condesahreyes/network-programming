using LogicaNegocio;

namespace WebApiAdministrativa.Modelos.JuegoModelos
{
    public class JuegoEntrada
    {
        public string Sinopsis { get; set; }
        public string Caratula { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }

        public JuegoEntrada(){ }

        public JuegoEntrada(Juego juego)
        {
            this.Titulo = juego.Titulo;
            this.Genero = juego.Genero;
            this.Sinopsis = juego.Sinopsis;
            this.Caratula = juego.Caratula;
        }

        public static Juego ModeloADominio(JuegoEntrada modelo)
        {
            return new Juego(modelo.Titulo, modelo.Genero, modelo.Sinopsis,
                modelo.Caratula);
        }
    }
}
