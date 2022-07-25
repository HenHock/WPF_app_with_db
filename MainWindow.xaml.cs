//using LiveCharts.Wpf;
//using LiveCharts;
using DevExpress.Xpf.Reports.UserDesigner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool BDChange = false; // Переменная для проверки были ли сохранены изменения или нет.
        public static bool isAdmin;
        private int indexOfShip;

        public MainWindow()
        {
            InitializeComponent();

            MinHeight = 720;
            MinWidth = 1400;

            tableListBox.ItemsSource = DataManager.nameTables;
            ViewListBox.ItemsSource = DataManager.Views;

            // Проходим все строки в таблице Users для проверки является ли входящий человек админом или нет
            foreach (DataRow row in DataManager.Users.Rows)
            {
                if (Convert.ToInt32(row[0]) == DataManager.indexUser)
                    isAdmin = Convert.ToBoolean(Convert.ToInt32(row[4]));
            }

            setRule(isAdmin);

            this.Closed += MainWindow_Closed;

            listBoxReport.Items.Add("Корабли и битвы");
            listBoxReport.Items.Add("Корабли и классы");
        }
        /// <summary>
        /// Ограничивает или снимает ограничения для взаимодействия с некоторыми объектами.
        /// </summary>
        /// <param name="flag">True - снять ограничения. Fasle - установить ограничения</param>
        private void setRule(bool flag)
        {
            if (!flag)
            {
                tableListBox.ItemsSource = DataManager.Views;
                int deleteIdexRow = -1;

                foreach (string str in tableListBox.Items)
                {
                    if (str.Equals("BattleOutcomeView"))
                    {
                        deleteIdexRow = tableListBox.Items.IndexOf(str);
                    }
                }

                if(deleteIdexRow >= 0)
                   DataManager.Views.RemoveAt(deleteIdexRow);

                toolsStackPanel.Visibility = Visibility.Collapsed;
                tableDataGrid.IsReadOnly = true;
                reportTabItem.Visibility = Visibility.Collapsed;
            }
            else
            {
                viewStackPanel.Visibility = Visibility.Visible;
                tableDataGrid.IsReadOnly = false;
            }
        }
        /// <summary>
        /// Сохраняет изменения в БД.
        /// </summary>
        private void SaveBD()
        {
            if(BDChange)
            {
                MessageBoxResult result = MessageBox.Show("Сохранить изменения в базе данных?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (tableListBox.SelectedIndex != -1)
                        DatabaseConnector.UpdateBD(nameTableTextBlock.Text);
                    BDChange = false;
                }
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            SaveBD();
            this.Close();
        }

        //Основная страница
        #region mainInfo
        /// <summary>
        /// Событие на кнопке "Таблицы"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableButton_Click(object sender, RoutedEventArgs e)
        {
            if (tableListBox.Visibility == Visibility.Collapsed)
                tableListBox.Visibility = Visibility.Visible;
            else {
                tableListBox.Visibility = Visibility.Collapsed;
                toolsStackPanel.Visibility = Visibility.Collapsed;
                if(tableListBox.SelectedIndex != -1)
                    infoStackPanel.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Событие на кнопке "Представления"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewListBox.Visibility == Visibility.Collapsed)
                ViewListBox.Visibility = Visibility.Visible;
            else
            {
                ViewListBox.Visibility = Visibility.Collapsed;
                adviceStackPanel.Visibility = Visibility.Collapsed;
                if(ViewListBox.SelectedIndex != -1)
                    infoStackPanel.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Событие, которые вызыватеся при переходе с влкдаки "Таблицы" на "Представления"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox.Name.Equals("tableListBox"))
            {
                if (isAdmin)
                {
                    toolsStackPanel.Visibility = Visibility.Visible;
                    adviceStackPanel.Visibility = Visibility.Collapsed;
                }
            }
            else if (listBox.Name.Equals("ViewListBox"))
            {
                if (isAdmin)
                {
                    toolsStackPanel.Visibility = Visibility.Collapsed;
                    adviceStackPanel.Visibility = Visibility.Visible;
                    adviceTextBlock.Text = "Для редактирования представлений вам следует изменять изначальные таблицы.";
                }
            }

            tableDataGrid.Visibility = Visibility.Visible;

            if (listBox.SelectedIndex != -1)
            {
                if(BDChange)
                {
                    int index = -1, i = 0;
                    foreach (string nameTable in listBox.Items)
                    {
                        if (nameTable.Equals(nameTableTextBlock.Text))
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    if (index != listBox.SelectedIndex)
                    {
                        MessageBoxResult result = MessageBox.Show("Изменения не будут сохранены!\nВы действительно хотите продолжить?",
                        "Вопрос",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                        if (result == MessageBoxResult.No)
                        {
                            listBox.SelectedIndex = index;
                            e.Handled = true;
                            return;
                        }
                        else if(result == MessageBoxResult.Yes)
                        {
                            BDChange = false;
                        }
                    }
                    else if (index == listBox.SelectedIndex)
                        return;
                }

                addStackPanel.Visibility = Visibility.Collapsed;
                editStackPanel.Visibility = Visibility.Collapsed;

                infoScrollViewer.Visibility = Visibility.Visible;
                infoStackPanel.Visibility = Visibility.Visible;

                if (tableDataGrid.Items.Count > 0)
                    DataManager.currentTable = new DataTable();

                tableDataGrid.SelectionChanged += tableDataGrid_SelectionChange;
                tableDataGrid.IsReadOnly = true;
                tableDataGrid.Margin = new Thickness(10);
                tableDataGrid.AutoGenerateColumns = true;
                tableDataGrid.PreviewKeyDown += tableDataGrid_PreviewKeyDown;

                nameTableTextBlock.Text = listBox.SelectedItem.ToString();
                DatabaseConnector.getInfoFromTable(nameTableTextBlock.Text);
                tableDataGrid.ItemsSource = DataManager.currentTable.DefaultView;
            }
        }

        private void tableDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (tableDataGrid.SelectedIndex != -1)
                if (e.Key == Key.Delete)
                    BDChange = true;
        }
        /// <summary>
        /// Событие, которое привязанно к кнопке "Удалить" и удаляет выделенные строки из таблицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(tableDataGrid.SelectedIndex != -1)
            {
                if (tableDataGrid.SelectedItems != null || DataManager.currentTable != null || tableDataGrid != null)
                {
                    while (tableDataGrid.SelectedItems.Count > 0)
                    {
                        if ((tableDataGrid.Items[tableDataGrid.SelectedIndex] as DataRowView) == null) return;
                        (tableDataGrid.Items[tableDataGrid.SelectedIndex] as DataRowView).Row.Delete();
                    }
                }
                BDChange = true;
            }
            else
                MessageBox.Show("Выделите строку, которую хотите удалить",
                        "Выделите строку",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
        }
        //Поиск
        #region findBlock

        private void findTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(tableDataGrid != null)
            {
                var filteredTable = DataManager.currentTable.Clone();

                foreach (DataRow row in DataManager.currentTable.Rows)
                {
                    if (findTextBox.Text.Equals(""))
                    {
                        countRow.Visibility = Visibility.Collapsed;
                        filteredTable.Rows.Add(row.ItemArray);
                    }
                    else
                    {
                        countRow.Visibility = Visibility.Visible;
                        for (int i = 0; i < DataManager.currentTable.Columns.Count; i++)
                        {
                            if (row[i].ToString().StartsWith(findTextBox.Text))
                            {
                                filteredTable.Rows.Add(row.ItemArray);
                            }
                        }
                    }
                }

                tableDataGrid.ItemsSource = filteredTable.DefaultView;
                countRow.Text = $"Количество найденных записей: {filteredTable.Rows.Count}";
            }
        }
        #endregion
        //Кнопка добавить
        #region addBlock
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (addStackPanel.Visibility == Visibility.Collapsed)
                addStackPanel.Visibility = Visibility.Visible;
            else addStackPanel.Visibility = Visibility.Collapsed;

            if (addStackPanel.Children.Count > 0)
                addStackPanel.Children.Clear();

            StackPanel stackPanel;

            foreach (DataColumn column in DataManager.currentTable.Columns)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = column.ColumnName;

                TextBox textBox = new TextBox();
                textBox.HorizontalAlignment = HorizontalAlignment.Left;

                stackPanel = new StackPanel();
                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(textBox);
                addStackPanel.Children.Add(stackPanel);
            }

            Button confirmButton = new Button();
            confirmButton.Content = "Ок";
            confirmButton.Width = 80;
            confirmButton.Click += addConfirmButton_Click;
            confirmButton.MouseEnter += Button_MouseEnter;
            confirmButton.MouseLeave += Button_MouseLeave;

            Button cancelButton = new Button();
            cancelButton.Content = "Отмена";
            cancelButton.Margin = new Thickness(5, 0, 0, 0);
            cancelButton.Click += addCancelButton_Click;
            cancelButton.MouseEnter += Button_MouseEnter;
            cancelButton.MouseLeave += Button_MouseLeave;

            stackPanel = new StackPanel();
            stackPanel.Margin = new Thickness(0, 0, 0, 5);
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(confirmButton);
            stackPanel.Children.Add(cancelButton);

            addStackPanel.Children.Add(stackPanel);
        }

        private void addConfirmButton_Click(object sender, EventArgs e)
        {
            DataRow newRow = DataManager.currentTable.NewRow();
            int i = 0;

            foreach(TabPanel tabPanel in addStackPanel.Children)
            {
                foreach(UIElement el in tabPanel.Children)
                {
                    if(el.GetType().ToString().Equals("System.Windows.Controls.TextBox"))
                    {
                        TextBox textBox = el as TextBox;
                        newRow[i] = textBox.Text;
                        i++;
                    }
                }
            }

            DataManager.currentTable.Rows.Add(newRow);
            BDChange = true;

            foreach (TabPanel tabPanel in addStackPanel.Children)
            {
                foreach (UIElement el in tabPanel.Children)
                {
                    if (el.GetType().ToString().Equals("System.Windows.Controls.TextBox"))
                    {
                        TextBox textBox = el as TextBox;
                        textBox.Text = "";
                    }
                }
            }
        }

        private void addCancelButton_Click(object sender, EventArgs e)
        {
            addStackPanel.Children.Clear();
            addStackPanel.Visibility = Visibility.Collapsed;
        }
        #endregion
        //Кнопка редактирования
        #region editBlock
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (tableDataGrid.SelectedIndex != -1)
            {
                if (editStackPanel.Visibility == Visibility.Collapsed)
                    editStackPanel.Visibility = Visibility.Visible;
                else editStackPanel.Visibility = Visibility.Collapsed;

                if (editStackPanel.Children.Count > 0)
                    editStackPanel.Children.Clear();

                StackPanel stackPanel;

                foreach (DataColumn column in DataManager.currentTable.Columns)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = column.ColumnName;
                    var bc = new BrushConverter();
                    textBlock.Foreground = (Brush)bc.ConvertFromString("#FF64B4BF");

                    if (textBlock.Text.IndexOf("img") >= 0)
                        break;

                    TextBox textBox = new TextBox();
                    foreach(DataRow row in DataManager.currentTable.Rows)
                    {
                        if(tableDataGrid.SelectedIndex == DataManager.currentTable.Rows.IndexOf(row))
                        {
                            textBox.Text = row[column.ColumnName].ToString();
                            break;
                        }
                    }

                    stackPanel = new StackPanel();
                    stackPanel.Children.Add(textBlock);
                    stackPanel.Children.Add(textBox);
                    editStackPanel.Children.Add(stackPanel);
                }

                Button confirmButton = new Button();
                confirmButton.Content = "Ок";
                confirmButton.Width = 80;
                confirmButton.Click += confirmButton_Click;
                confirmButton.MouseEnter += Button_MouseEnter;
                confirmButton.MouseLeave += Button_MouseLeave;

                Button cancelButton = new Button();
                cancelButton.Content = "Отмена";
                cancelButton.Margin = new Thickness(5, 0, 0, 0);
                cancelButton.Click += cancelButton_Click;
                cancelButton.MouseEnter += Button_MouseEnter;
                cancelButton.MouseLeave += Button_MouseLeave;


                stackPanel = new StackPanel();
                stackPanel.Margin = new Thickness(0, 0, 0, 5);
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Children.Add(confirmButton);
                stackPanel.Children.Add(cancelButton);

                editStackPanel.Children.Add(stackPanel);
            }
            else MessageBox.Show("Выберите строку, которую хотите отредактировать",
                "Выберите строку", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (editStackPanel.Visibility == Visibility.Visible)
            {
                editStackPanel.Children.Clear();
                editStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            DataRowView updateRow = (tableDataGrid.SelectedItem as DataRowView);

            int i = 0;
            foreach(StackPanel stackPanel in editStackPanel.Children)
            {
                foreach(UIElement uI in stackPanel.Children)
                {
                    if(uI.GetType().ToString().Equals("System.Windows.Controls.TextBox"))
                    {
                        TextBox textBox = uI as TextBox;
                        updateRow[i] = textBox.Text;
                        i++;
                    }
                }
            }

            BDChange = true;
        }
        #endregion
        /// <summary>
        /// Событие, которые вызвается при изменение строки в текущей открытой таблице и обновляет данные о строке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableDataGrid_SelectionChange(object sender, EventArgs e)
        {
            if (editStackPanel.Visibility == Visibility.Visible)
            {
                if (editStackPanel.Children.Count > 0)
                    editStackPanel.Children.Clear();

                StackPanel stackPanel;

                foreach (DataColumn column in DataManager.currentTable.Columns)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = column.ColumnName;

                    if (textBlock.Text.IndexOf("img") >= 0)
                        break;

                    TextBox textBox = new TextBox();

                    foreach (DataRow row in DataManager.currentTable.Rows)
                    {
                        if (tableDataGrid.SelectedIndex == DataManager.currentTable.Rows.IndexOf(row))
                        {
                            textBox.Text = row[column.ColumnName].ToString();
                            break;
                        }
                    }

                    stackPanel = new StackPanel();
                    stackPanel.Children.Add(textBlock);
                    stackPanel.Children.Add(textBox);
                    editStackPanel.Children.Add(stackPanel);
                }

                Button confirmButton = new Button();
                confirmButton.Content = "Ок";
                confirmButton.Width = 80;
                confirmButton.Click += confirmButton_Click;
                confirmButton.MouseEnter += Button_MouseEnter;
                confirmButton.MouseLeave += Button_MouseLeave;

                Button cancelButton = new Button();
                cancelButton.Content = "Отмена";
                cancelButton.Margin = new Thickness(5, 0, 0, 0);
                cancelButton.MouseEnter += Button_MouseEnter;
                cancelButton.MouseLeave += Button_MouseLeave;
                cancelButton.Click += cancelButton_Click;

                stackPanel = new StackPanel();
                stackPanel.Margin = new Thickness(0, 0, 0, 5);
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Children.Add(confirmButton);
                stackPanel.Children.Add(cancelButton);

                editStackPanel.Children.Add(stackPanel);
            }
        }
        /// <summary>
        /// Событие, которые вызввается при нажатие на кнопку "Сохранить" и сохраняет измененые данные в строке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement uI in detailsStackPanel.Children)
            {
                if (uI.GetType().ToString().Equals("System.Windows.Controls.ListBox"))
                {
                    ListBox listBox = uI as ListBox;
                    
                    if(listBox.SelectedIndex != -1){
                        DatabaseConnector.UpdateBD(nameTableTextBlock.Text);
                        BDChange = false;
                        break;
                    }
                }
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            var bc = new BrushConverter();
            button.Background = Brushes.AliceBlue;
            button.Foreground = Brushes.Black;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            button.Background = Brushes.Transparent;
            button.Foreground = Brushes.White;
        }

        private void ExitImage_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Name.Equals("exitButton"))
            {
                this.Close();
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

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void squareImage_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }

        private void restoreImage_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// Создает бекапп базы данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CreateBackupButton_Click(object sender, RoutedEventArgs e)
        {
            string FilePath = "";
            bool SaveFileDialog()
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Filter = "Database backups files (*.bak)|*.bak";
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FilePath = saveFileDialog.FileName;
                    System.Console.WriteLine(FilePath);
                    return true;
                }
                return false;
            }

            if (SaveFileDialog())
            {
                SqlConnection connection = null;

                string сonnectionString = @"Data Source=LAB07_PC01\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

                try
                {
                    connection = new SqlConnection(сonnectionString);
                    connection.Open();
                    string query = "USE master BACKUP DATABASE SeaBattle TO DISK = N'" + FilePath + "' WITH NOFORMAT, NOINIT, NAME = N'SeaBattle-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                        MessageBox.Show("Успешное создание резервной копиии!\nПуть к файлу резервной копии: " + FilePath, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Востанавливает БД по бекаппу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RestoreBackupButton_Click(object sender, RoutedEventArgs e)
        {
            string FilePath = "";
            bool OpenFileDialog()
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Filter = "Database backups files (*.bak)|*.bak";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FilePath = openFileDialog.FileName;
                    System.Console.WriteLine(FilePath);
                    return true;
                }
                return false;
            }

            if (OpenFileDialog())
            {
                SqlConnection connection = null;

                string сonnectionString = @"Data Source=LAB07_PC01\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

                try
                {
                    connection = new SqlConnection(сonnectionString);
                    connection.Open();
                    string query = "USE master RESTORE DATABASE SeaBattle FROM DISK = N'" + FilePath + "' WITH  FILE = 1,  NOUNLOAD,  STATS = 5";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                        MessageBox.Show("Успешное восстановление резервной копиии", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        //RefreshDatas(); очищает и заполняет новые данные
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1 вкладка отвечат за базу данных, там если вы обладаете достаточными правами, вы сможете редактировать, удалять и добавлять информацию.\n" +
                "2 вкладка отвечат за отчёты. Если вы не обладаете достаточными правами, то это иконка будет скрыта.\n" +
                "3 вкладка отвечает за картики для кораблей, елси вы обладаете достаточными правами сможете их редактировать.","Помощь");
        }

        #endregion

        //Reports
        private void ListBoxReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        //    if (listBoxReport.SelectedItem.Equals("Корабли и битвы"))
        //    {
        //        XtraReport2 report = new XtraReport2();
        //        var window = new Report();
        //        window.reportDesigner.DocumentSource = report;
        //        report.CreateDocument();
        //        window.Show();
        //    }
        //    else if (listBoxReport.SelectedItem.Equals("Корабли и классы"))
        //    {
        //        XtraReport1 report = new XtraReport1();
        //        Report window = new Report();
        //        window.reportDesigner.DocumentSource = report;
        //        report.CreateDocument();
        //        window.Show();
        //    }
        }

        private void ReportTabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gridReport.Children.Count > 1)
                gridReport.Children.Clear();
            ReportDesigner reportDesigner = new ReportDesigner();
            Grid.SetColumnSpan(reportDesigner, 2);
            gridReport.Children.Add(reportDesigner);
        }

        //Галлерея
        #region Gallery

        private void buttonClickMe_Click(object sender, EventArgs e)
        {
            buttonClickMe.Visibility = Visibility.Collapsed;

            DatabaseConnector.getInfoFromTable("Ship", DataManager.tableShip);
            indexOfShip = 0;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 5; j++)
                {
                    if (indexOfShip < DataManager.tableShip.Rows.Count)
                    {
                        ShipPicture shipPicture = new ShipPicture(DataManager.tableShip.Rows[indexOfShip]);
                        Grid.SetRow(shipPicture, i);
                        Grid.SetColumn(shipPicture, j);
                        gridGallery.Children.Add(shipPicture);
                        indexOfShip++;
                    }
                    else return;
                }

            if (indexOfShip < DataManager.tableShip.Rows.Count)
                buttonNextRight.IsEnabled = true;
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Name.IndexOf("Right") >= 0)
            {
                buttonNextLeft.IsEnabled = true;

                if (indexOfShip == DataManager.tableShip.Rows.Count)
                    buttonNextRight.IsEnabled = false;

                if (indexOfShip != DataManager.tableShip.Rows.Count)
                {
                    gridGallery.Children.Clear();

                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < 5; j++)
                        {
                            if (indexOfShip < DataManager.tableShip.Rows.Count)
                            {
                                ShipPicture shipPicture = new ShipPicture(DataManager.tableShip.Rows[indexOfShip]);
                                Grid.SetRow(shipPicture, i);
                                Grid.SetColumn(shipPicture, j);
                                gridGallery.Children.Add(shipPicture);
                                indexOfShip++;
                            }
                            else break;
                        }
                }
            }
            else
            {
                if (indexOfShip != 0)
                {
                    indexOfShip = indexOfShip - 20 - gridGallery.Children.Count+1;

                    buttonNextRight.IsEnabled = true;

                    if (indexOfShip <= 0)
                        buttonNextLeft.IsEnabled = false;

                    gridGallery.Children.Clear();

                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < 5; j++)
                        {
                            if (indexOfShip < DataManager.tableShip.Rows.Count)
                            {
                                ShipPicture shipPicture = new ShipPicture(DataManager.tableShip.Rows[indexOfShip]);
                                Grid.SetRow(shipPicture, i);
                                Grid.SetColumn(shipPicture, j);
                                gridGallery.Children.Add(shipPicture);
                                indexOfShip++;
                            }
                            else break;
                        }
                }
            }
        }

        #endregion
    }
}
