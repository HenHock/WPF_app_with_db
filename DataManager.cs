using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;

namespace AppBD
{
    public static class DataManager
    {
        public static List<string> nameTables = DatabaseConnector.GetTables(); // Список названий таблиц
        public static List<string> Views; // Список представлений
        public static DataTable currentTable = new DataTable(); // Текущая открытая таблица
        public static DataTable tableShip = new DataTable(); // Таблица для отображение картинок кораблей в галереи.
        public static DataTable Users = new DataTable(); // Таблица, которая хранит информацию о пользователях.
        public static int indexUser;

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();

            imageIn.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null) return null;
            MemoryStream ms = new MemoryStream(byteArrayIn);

            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;

        }

        public static BitmapImage imageToBitmap(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();

            imageIn.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bm = new BitmapImage();
            bm.BeginInit();
            bm.CacheOption = BitmapCacheOption.OnLoad;
            bm.StreamSource = ms;
            bm.EndInit();

            return bm;
        }
    }
}
