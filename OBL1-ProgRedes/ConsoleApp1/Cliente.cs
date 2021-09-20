namespace Cliente
{
    public class Cliente
    {
        private static Menu menu;

        static void Main(string[] args)
        {
            menu = new Menu();

            menu.MenuPrincipal();
        }
    }
}
