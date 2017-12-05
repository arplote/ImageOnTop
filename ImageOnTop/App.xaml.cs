using System;
using System.Windows;

namespace ImageOnTop
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                MainWindow window = new MainWindow();
                window.Title = "ImageOnTop";
                window.SetImage(e.Args[0]);
                window.Show();
            }
            else
            {
                Console.WriteLine("Please enter the URI of an image as an argument.");
                Application.Current.Shutdown();
            }

        }
    }
}
