using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppBD
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            DatabaseConnector.getInfoFromTable("Users", DataManager.Users); // Получаем данные из таблицы Users в БД и заполнем ими глобальную перменную DataManager.Users
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                var bc = new BrushConverter();
                loginImage.Fill = Brushes.White;
            }
            else
            {
                var bc = new BrushConverter();
                passwordImage.Fill = Brushes.White;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                loginImage.Fill = Brushes.PaleGreen;
            }
            else
            {
                passwordImage.Fill = Brushes.PaleGoldenrod;
            }
        }

        private void ExitImage_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Name.Equals("exitButton"))
            {
                this.Close();
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            registrationStackPanel.Visibility = Visibility.Visible;
            loginStackPanel.Visibility = Visibility.Collapsed;
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            if(loginTextBox.Text.Length > 0 && passwordTextBox.Password.Length > 0)
            {
                int i = 0;
                foreach(DataRow row in DataManager.Users.Rows)
                { 
                    if(row[1].Equals(loginTextBox.Text) && row[2].Equals(passwordTextBox.Password))
                    {
                        DataManager.indexUser = i;
                        MainWindow window = new MainWindow();
                        window.Show();
                        this.Close();
                        return;
                    }
                    i++;
                }
                MessageBox.Show("Логин или пароль были введены не правильно или такого аккаунта не существует!", "Не удалось войти", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                loginTextBox.BorderBrush = Brushes.IndianRed;
                passwordTextBox.BorderBrush = Brushes.IndianRed;
            }
        }

        private void exitButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Button exitButton = sender as Button;

            exitButton.Foreground = Brushes.Red;
        }

        private void exitButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button exitButton = sender as Button;

            exitButton.Foreground = Brushes.White;
        }
        #region Валидация
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (reg_loginTextBox.Text.Length == 0)
            {
                reg_loginTextBox.BorderBrush = Brushes.IndianRed;
            }
            if (reg_passwordTextBox.Password.Length <= 4)
            {
                reg_passwordTextBox.BorderBrush = Brushes.IndianRed;
            }
            if (reg_emailTextBox.Text.Length == 0 || !IsValidEmail(reg_emailTextBox.Text))
            {
                reg_emailTextBox.BorderBrush = Brushes.IndianRed;
            }
            else
            {
                DataTable dataTable = new DataTable();
                DatabaseConnector.getInfoFromTable("Users", dataTable);
                DataRow newRow = dataTable.NewRow();

                if (dataTable.Rows.Count != 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        newRow[0] = Convert.ToInt32(row[0]) + 1;

                        if (row[1].Equals(reg_loginTextBox.Text))
                        {
                            MessageBox.Show("Пользователь с таким логином уже зарегистрирован.", "Такой логин уже существует!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (row[3].Equals(reg_emailTextBox.Text))
                        {
                            MessageBox.Show("Пользователь с такой почтой уже зарегистрирован.", "Такой Email уже существует!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
                else
                {
                    newRow[0] = 0;
                }

                newRow[1] = reg_loginTextBox.Text;
                newRow[2] = reg_passwordTextBox.Password;
                newRow[3] = reg_emailTextBox.Text;
                newRow[4] = 0;
                dataTable.Rows.Add(newRow);

                DatabaseConnector.UpdateBD("Users", dataTable);
                DataManager.Users = dataTable;

                loginTextBox.Text = reg_loginTextBox.Text;
                passwordTextBox.Password = reg_passwordTextBox.Password;

                loginStackPanel.Visibility = Visibility.Visible;
                registrationStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            registrationStackPanel.Visibility = Visibility.Collapsed;
            loginStackPanel.Visibility = Visibility.Visible;
        }

        private void reg_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Name.Equals("reg_loginTextBox"))
                {
                    reg_loginImage.Foreground = Brushes.White;
                }
                else
                {
                    reg_emailImage.Foreground = Brushes.White;
                }
            }
            else
            {
                reg_passwordImage.Foreground = Brushes.White;
            }
        }

        private void reg_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && textBox.Name.Equals("lreg_oginTextBox"))
            {
                reg_loginImage.Foreground = Brushes.PaleTurquoise;
            }
            else if (textBox != null && textBox.Name.Equals("reg_emailTextBox"))
            {
                reg_emailImage.Foreground = Brushes.PaleGoldenrod;
            }
            else
            {
                reg_passwordImage.Foreground = Brushes.PaleVioletRed;
            }
        }
        #endregion

        /// <summary>
        /// Метод позволяет пермещать окно по рабочему столу зажав левым кликом по 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void restoreImage_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
