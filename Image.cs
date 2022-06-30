using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;

namespace Project4
{
    public static class Image
    {

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        public static BitmapImage initializeImage(int width, int height)
        {
            Bitmap img = new Bitmap(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    img.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            return ToBitmapImage(img);
        }

        public static void draw(Bitmap img, float x, float y, Color c)
        {
            // Bitmap img = BitmapImage2Bitmap(image);
            img.SetPixel((int)x, (int)y, c);
            //return img;
        }

        public static void clearImage(BitmapImage img, int rows, int columns)
        {
            Bitmap bp = BitmapImage2Bitmap(img);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                    bp.SetPixel(j, i, Color.FromArgb(255, 255, 255));
            }
        }

    }
}
