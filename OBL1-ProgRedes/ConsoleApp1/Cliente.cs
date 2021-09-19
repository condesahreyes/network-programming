namespace Cliente
{
    public class Cliente
    {
        private static MenuCliente menu;

        static void Main(string[] args)
        {
            menu = new MenuCliente();

            menu.MenuPrincipal();
        }
    }
}
