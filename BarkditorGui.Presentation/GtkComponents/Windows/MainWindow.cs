using System;
using System.IO.Compression;
using System.IO;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using AboutDialog = BarkditorGui.Presentation.GtkComponents.DialogWindows.AboutDialog;
using Grpc.Net.Client;
using ProjectFilesClient;

namespace BarkditorGui.Presentation.GtkComponents.Windows;
class MainWindow : Window
{
    [UI] private readonly MenuItem _aboutMenuItem;
    [UI] private readonly MenuItem _openFolderItem;
    [UI] private TreeView _fileTreeView;
    [UI] private readonly TextView _code;
    [UI] private readonly TextBuffer _codeTextBuffer;
    private readonly TreeStore _fileTreeStore = new TreeStore(typeof(string), typeof(Gdk.Pixbuf));

    public MainWindow() : this(new Builder("MainWindow.glade")) { }
    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath("./CSS/style.css");
        builder.Autoconnect(this);
        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);

        FileTreeViewInit();
        
        DeleteEvent += Window_DeleteEvent;
        _aboutMenuItem.Activated += AboutButton_Clicked;
        _openFolderItem.Activated += OpenFolder_Clicked;
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }

    private void AboutButton_Clicked(object sender, EventArgs a)
    {
        var aboutDialog = new AboutDialog(this);
        aboutDialog.Run();
        aboutDialog.Destroy();
    }

    private void FileTreeViewInit() 
    {
        var fileColumn = new TreeViewColumn();
        fileColumn.Title = "Files";
        _fileTreeView.AppendColumn(fileColumn);

        var i = new CellRendererPixbuf();
        fileColumn.PackStart(i, false);
        fileColumn.AddAttribute(i, "pixbuf", 1);
        var c = new CellRendererText();
        fileColumn.PackStart(c, true);
        fileColumn.AddAttribute(c, "text", 0);

        _fileTreeView.Model = _fileTreeStore;
    }

    private void OpenFolder_Clicked(object sender, EventArgs a) 
    {
        var directoryChooser = 
            new FileChooserDialog("Choose directory to open",
            this, FileChooserAction.SelectFolder,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept);

        directoryChooser.Run();
        
        var path = directoryChooser.Filename;
        using var channel = GrpcChannel.ForAddress("https://localhost:7139");
        var client = new ProjectFiles.ProjectFilesClient(channel);
        var request = new GetProjectFilesRequest
        {
            Path = path
        };
        var response = client.GetProjectFiles(request);
        var projectFiles = response.ProjectFiles;
        
        ShowProjectFiles(path, projectFiles);

        directoryChooser.Destroy();
    }

    private void ShowProjectFiles(string path, GetProjectFilesResponse.Types.FileTree fileTree, TreeIter parent) 
    {
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int) IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int) IconSize.Menu, 0);
        var directoryName = System.IO.Path.GetFileName(path); 
        
        // var rootDirectory = _fileTreeStore.AppendValues($"  {directoryName}", folderIcon);
        // _codeTextBuffer.Text += "1";
        foreach(var s in fileTree.Files)
        {
            var icon = s.IsDirectory is true ? folderIcon : fileIcon;
            
            if(s.IsDirectory is false) 
            {
                _fileTreeStore.AppendValues(parent, $"  {s.Name}", icon);
                continue;
            }
            
            var a = _fileTreeStore.AppendValues(parent, $"  {s.Name}", icon);
            var directoryPath = System.IO.Path.Combine(path, fileTree.Name);
            if (s.IsDirectory is true) ShowProjectFiles(directoryPath, s, a);
        }
    }

    private void ShowProjectFiles(string path, GetProjectFilesResponse.Types.FileTree fileTree) 
    {
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int) IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int) IconSize.Menu, 0);
        var directoryName = System.IO.Path.GetFileName(path); 
        
        // var rootDirectory = _fileTreeStore.AppendValues($"  {directoryName}", folderIcon);
        // _codeTextBuffer.Text += "1";
        foreach(var s in fileTree.Files)
        {
            var icon = s.IsDirectory is true ? folderIcon : fileIcon;
            
            if(s.IsDirectory is false) 
            {
                _fileTreeStore.AppendValues($"  {s.Name}", icon);
                continue;
            }
            
            var a = _fileTreeStore.AppendValues($"  {s.Name}", icon);
            var directoryPath = System.IO.Path.Combine(path, fileTree.Name);
            if (s.IsDirectory is true) ShowProjectFiles(directoryPath, s, a);
        }
    }
}
