namespace PhysicsSim
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainWindow mw = new MainWindow(1280, 720);
            mw.VSync = OpenTK.VSyncMode.Off;
            mw.Run(60, 60);
            mw.Close();
        }
    }
}
