namespace ElectroSim
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainWindow mw = new MainWindow(1280, 720);
            mw.Run(60);
            mw.Close();
        }
    }
}
