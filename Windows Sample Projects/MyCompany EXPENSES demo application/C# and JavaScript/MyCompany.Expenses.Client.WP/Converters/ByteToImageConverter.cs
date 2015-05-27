﻿namespace MyCompany.Expenses.Client.WP.Converters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// COnverter from byte array to image.
    /// </summary>
    public class ByteToImageConverter : IValueConverter
    {
        /// <summary>
        /// Convert byte array to image
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            MemoryStream strm = new MemoryStream((byte[])value);
            BitmapImage img = new BitmapImage();
            img.SetSource(strm);
            return img;
        }

        /// <summary>
        /// convert image to byte array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
