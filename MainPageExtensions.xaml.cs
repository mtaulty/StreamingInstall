namespace PictureApp
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using Windows.ApplicationModel;
  using Windows.UI.Core;
  using Windows.UI.Xaml;
  using Windows.UI.Xaml.Controls;


  public sealed partial class MainPage : Page
  {
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

      foreach (var group in groups.Where(
        g => !g.IsRequired && g.State == PackageContentGroupState.Staged))
      {
        await this.AddPictureSourceAsync(group.Name, group.Name);
      }

      // Now set up handlers to wait for any others to arrive
      this.catalog = PackageCatalog.OpenForCurrentPackage();
      this.catalog.PackageInstalling += OnPackageInstalling;
    }
    async void OnPackageInstalling(
      PackageCatalog sender,
      PackageInstallingEventArgs args)
    {
      if (args.IsComplete)
      {
        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
          async () =>
          {
            await this.AddPictureSourceAsync(args.Package.Id.FullName,
              args.Package.Id.FullName);
          }
        );
      }
    }

    PackageCatalog catalog;
  }
}
