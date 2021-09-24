using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BatchAddingParameters
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;
            if (path == null)
                return null;
            var name = WindowMain.GetFileFolderName(path);
            var fileImage = @"\res\file-icon-48.png"; 
            if (string.IsNullOrEmpty(name))
                 fileImage = @"\res\drive-icon-48.png"; 
            else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
                fileImage = @"\res\folder-closed.png";
            else if (name.Contains(".rfa"))
                fileImage = @"\res\revitfile-icon.png";

            return new BitmapImage(new Uri(Main.DllFolderLocation + fileImage)); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
