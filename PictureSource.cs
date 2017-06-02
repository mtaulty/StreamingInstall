namespace PictureApp
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Windows.Storage;
  using System;

  public class PictureSource
  {
    public PictureSource(string displayName, StorageFolder folder)
    {
      this.DisplayName = displayName;
      this.folder = folder;
    }

    public string DisplayName { get; set; }

    public IReadOnlyList<Picture> Pictures
    {
      get
      {
        return (this.pictures.AsReadOnly());
      }
    }
    internal async Task PopulateAsync()
    {
      this.pictures = new List<Picture>();

      var files = await this.folder.GetFilesAsync();

      foreach (var file in files)
      {
        Picture picture = new Picture(file);
        await picture.PopulateAsync();
        this.pictures.Add(picture);
      }
    }
    List<Picture> pictures;
    StorageFolder folder;
  }
}
