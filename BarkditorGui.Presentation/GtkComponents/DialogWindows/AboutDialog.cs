using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace BarkditorGui.Presentation.GtkComponents.DialogWindows;

public class AboutDialog : Dialog
{
    [UI] private readonly Button _cancelButton;

    public AboutDialog() : this(new Builder("AboutDialog.glade")) { }

    private AboutDialog(Builder builder) : base(builder.GetRawOwnedObject("AboutDialog"))
    {
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath("../../../CSS/style.css");
        builder.Autoconnect(this);

        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);
        
        DeleteEvent += CloseApp;
        _cancelButton.Clicked += CloseApp;
    }

    private void CloseApp(object sender, EventArgs a)
    {
        Application.Quit();
    }
}