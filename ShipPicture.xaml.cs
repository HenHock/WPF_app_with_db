using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppBD
{
    /// <summary>
    /// Логика взаимодействия для ShipPicture.xaml
    /// </summary>
    public partial class ShipPicture : UserControl
    {
        DataRow ship;

        public ShipPicture(DataRow Ship)
        {
            InitializeComponent();

            ship = Ship;

            textBlockTitle.Text = ship[1].ToString();

            if (!ship[4].ToString().Equals(""))
                imgShip.Source = DataManager.imageToBitmap(DataManager.byteArrayToImage(ship[4] as Byte[]));
            else
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(@"pack://application:,,,/images/default.png");
                bitmapImage.EndInit();
                imgShip.Source = bitmapImage;
            }
        }

        private void chooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.isAdmin)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Выберите картинку";
                openFileDialog.Filter = "jpg and png files (*.jpg;*.png)|*.jpg;*.png";

                if (openFileDialog.ShowDialog() == true)
                {
                    using (Bitmap img = new Bitmap(openFileDialog.FileName))
                    {
                        string FileName = openFileDialog.FileName;
                        imgShip.Source = new BitmapImage(new Uri(FileName));
                        Bitmap bi = new Bitmap(FileName);
                        ship[4] = DataManager.imageToByteArray(bi);
                        DatabaseConnector.UpdateBD("Ship",DataManager.tableShip);
                        imgShip.Height = 100;
                        imgShip.Width = 200;
                    }
                }
            }
            else
            {
                MessageBox.Show("Вы не можете редактировать картинки кораблей. Обратись за правами к администратору!");
            }
        }
    }
}
