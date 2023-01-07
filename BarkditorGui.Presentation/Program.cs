using System;
using Gtk;
using BarkditorGui.BusinessLogic.GtkComponents.Windows;

namespace BarkditorGui.Presentation
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.BarkditorGui.Presentation", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
