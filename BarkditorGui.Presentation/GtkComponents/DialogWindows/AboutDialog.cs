using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace BarkditorGui.Presentation.GtkComponents.DialogWindows;

public class AboutDialog : Dialog
{
    public AboutDialog(Window parent) : this(new Builder("AboutDialog.glade"))
    {
        Parent = parent;
    }

    private AboutDialog(Builder builder) : base(builder.GetRawOwnedObject("AboutDialog"))
    {
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath("../../../Css/style.css");
        builder.Autoconnect(this);

        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);

        DeleteEvent += CloseDialog;
    }

    private void CloseDialog(object sender, EventArgs a)
    {
        Application.Quit();
    }
}
