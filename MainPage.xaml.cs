namespace PictureApp
{
  using System;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using Windows.ApplicationModel;
  using Windows.UI.Core;
  using Windows.UI.Xaml;
  using Windows.UI.Xaml.Controls;

  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
      this.Loaded += this.OnLoaded;
    }
    public ObservableCollection<PictureSource> PictureSources
    {
      get
      {
        return (this.pictureSources);
      }
      private set
      {
        this.pictureSources = value;
      }
    }
    async void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.PictureSources = new ObservableCollection<PictureSource>();
      this.DataContext = this;
      await this.AddPictureSourceAsync("Film Posters", localPictureFolder);
      await this.AddStreamedPictureSourcesAsync();   
     
    }
    async Task AddPictureSourceAsync(string name, string folderName)
    {
      try
      {
        var folder =
          await Package.Current.InstalledLocation.GetFolderAsync(folderName);

        PictureSource source = new PictureSource(name, folder);
        await source.PopulateAsync();
        this.PictureSources.Add(source);
      }
      catch (FileNotFoundException)
      {
      }
    }
    void OnItemClick(object sender, ItemClickEventArgs e)
    {
      this.photoGrid.Visibility = Visibility.Visible;

      var item = (Picture)e.ClickedItem;

      this.photoImage.Source = item.Source;
    }
    void OnDismissPhoto(object sender, RoutedEventArgs e)
    {
      this.photoGrid.Visibility = Visibility.Collapsed;
    }
    async Task AddStreamedPictureSourcesAsync()
    {
      // Handle any streamed packages that are already installed.
      var groups = await Package.Current.GetContentGroupsAsync();

      // TBD - unsure exactly of the state to check for here in order
      // to be sure that the content group is present.
      foreach (var group in groups.Where(
        g => !g.IsRequired && g.State == PackageContentGroupState.Staged))
      {
        await this.AddPictureSourceAsync(group.Name, group.Name);
      }

      // Now set up handlers to wait for any others to arrive
      this.catalog = PackageCatalog.OpenForCurrentPackage();
      this.catalog.PackageContentGroupStaging += OnContentGroupStaging;
    }

    async void OnContentGroupStaging(
      PackageCatalog sender, PackageContentGroupStagingEventArgs args)
    {
      if (args.IsComplete)
      {
          await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
              await this.AddPictureSourceAsync(
                args.ContentGroupName,
                args.ContentGroupName);
            }
          );
      }
    }
    PackageCatalog catalog;
    ObservableCollection<PictureSource> pictureSources;
    const string localPictureFolder = "Posters";
  }
}
