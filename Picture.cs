using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureApp
{
  public class Picture
  {
    public Picture(StorageFile file)
    {
      this.File = file;
      this.Name = Path.GetFileNameWithoutExtension(file.Name);
    }
    internal async Task PopulateAsync()
    {
      var source = new BitmapImage();
      var stream = await this.File.OpenReadAsync();
      source.SetSource(stream);
      this.Source = source;
    }
    public string Name { get; private set; }
    public StorageFile File { get; private set; }
    public BitmapSource Source
    {
      get;
      private set;
    }
  }
}
