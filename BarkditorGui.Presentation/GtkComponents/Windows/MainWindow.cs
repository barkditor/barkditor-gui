using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using AboutDialog = BarkditorGui.Presentation.GtkComponents.DialogWindows.AboutDialog;

namespace BarkditorGui.Presentation.GtkComponents.Windows;
class MainWindow : Window
{
    [UI] private readonly MenuItem _aboutButton;

    public MainWindow() : this(new Builder("MainWindow.glade")) { }
    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath("../../../CSS/style.css");
        builder.Autoconnect(this);
        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);
        
        DeleteEvent += Window_DeleteEvent;
        _aboutButton.ActivateItem += AboutButton_Clicked;
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }

    private void AboutButton_Clicked(object sender, EventArgs a)
    {
        // todo: implement dialog calling
    }
}