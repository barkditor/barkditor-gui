using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using AboutDialog = BarkditorGui.BusinessLogic.GtkComponents.DialogWindows.AboutDialog;
using Grpc.Net.Client;
using ProjectFilesClient;

namespace BarkditorGui.BusinessLogic.GtkComponents.Windows;
public class MainWindow : Window
{
    [UI] private readonly MenuItem _aboutMenuItem;
    [UI] private readonly MenuItem _openFolderItem;
    [UI] private TreeView _fileTreeView;
    [UI] private readonly TextView _code;
    [UI] private readonly TextBuffer _codeTextBuffer;
    private readonly TreeStore _fileTreeStore = new TreeStore(typeof(string), typeof(Gdk.Pixbuf));
    private readonly GrpcChannel _channel = GrpcChannel.ForAddress("https://localhost:5001");
    private readonly ProjectFiles.ProjectFilesClient _projectFilesClient;

    public MainWindow() : this(new Builder("MainWindow.glade")) { }
    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath("../../../../BarkditorGui.BusinessLogic/Css/style.css");
        builder.Autoconnect(this);
        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);

        FileTreeViewInit();

        _projectFilesClient = new ProjectFiles.ProjectFilesClient(_channel);
        
        LoadSavedProject();
        
        DeleteEvent += Window_DeleteEvent;
        _aboutMenuItem.Activated += AboutButton_Clicked;
        _openFolderItem.Activated += OpenFolder_Clicked;
    }

    private void LoadSavedProject() 
    {
        var empty = new Google.Protobuf.WellKnownTypes.Empty();
        var response = _projectFilesClient.GetSavedProject(empty);
        var projectFiles = response.Files;
        
        if(projectFiles is null)
        {
            return;
        }

        ShowProjectFiles(response.Files);
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

    private void OpenFolder_Clicked(object sender, EventArgs a) 
    {
        var directoryChooser = 
            new FileChooserDialog("Choose directory to open",
            this, FileChooserAction.SelectFolder,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept);

        var directoryChooserResponse = directoryChooser.Run();
        if(directoryChooserResponse == (int) Gtk.ResponseType.Ok)
        {
            var path = directoryChooser.Filename;
            var request = new OpenFolderRequest
            {
                Path = path
            };
            var response = _projectFilesClient.OpenFolder(request);
            var projectFiles = response.Files;
            _fileTreeStore.Clear();
        
            ShowProjectFiles(projectFiles);
        }
        directoryChooser.Destroy();
    }

    private void ShowProjectFiles(FileTree fileTree, TreeIter parent) 
    {
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int) IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int) IconSize.Menu, 0);
        
        foreach(var file in fileTree.Files)
        {
            var icon = file.IsDirectory is true ? folderIcon : fileIcon;
            
            if(file.IsDirectory is false) 
            {
                _fileTreeStore.AppendValues(parent, $"  {file.Name}", icon);
                continue;
            }
            
            var treeIter = _fileTreeStore.AppendValues(parent, $"  {file.Name}", icon);
            if (file.IsDirectory is true) ShowProjectFiles(file, treeIter);
        }
    }

    private void ShowProjectFiles(FileTree fileTree) 
    {
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int) IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int) IconSize.Menu, 0);

        foreach(var file in fileTree.Files)
        {
            var icon = file.IsDirectory is true ? folderIcon : fileIcon;
            
            if(file.IsDirectory is false) 
            {
                _fileTreeStore.AppendValues($"  {file.Name}", icon);
                continue;
            }
            
            var treeIter = _fileTreeStore.AppendValues($"  {file.Name}", icon);
            if (file.IsDirectory is true) ShowProjectFiles(file, treeIter);
        }
    }
}
