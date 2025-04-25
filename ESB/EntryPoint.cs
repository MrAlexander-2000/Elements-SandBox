using Raylib_cs;

namespace ESB
{
    public class EntryPoint
    {
        static string startupMessage = "EntryPoint: Starting up ESB - Version:";
        public static string versionNum = "1.0.0";
        public static void Main(string[] args)
        {
            Console.WriteLine(startupMessage + " " + versionNum);
            SetupWindow setupWindow = new SetupWindow();
            setupWindow.CreateWindow();
        }
    }
}
