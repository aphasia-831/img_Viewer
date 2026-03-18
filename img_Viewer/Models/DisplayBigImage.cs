using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace img_Viewer.Models
{
    public class DisplayBigImage
    {
        
            public int Id { get; set; }
            public string FilePath { get; set; }
            public List<string> Tags { get; set; } = new();
            public BitmapImage Bitmap { get; set; }
            public string ThumbnailPath => FilePath; 
        
    }
}
