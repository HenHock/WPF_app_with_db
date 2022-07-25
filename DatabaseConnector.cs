using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;

namespace AppBD
{
    public class DatabaseConnector
    {
        public static string сonnectionString = @"Data Source=LAB07_PC01\SQLEXPRESS;Initial Catalog=Seabattle;Integrated Security=True";
        //public static string сonnectionString = @"Data Source=DESKTOP-UVLISFT;Initial Catalog=Seabattle1;Integrated Security=True";
        //public static string сonnectionString = @"Data Source=DESKTOP-GLHF6V3;Initial Catalog=Seabattle;Integrated Security=True";
        
        /// <summary>
        /// Метод, который заполняет глобальные переменные DataManager.Views всеми предствалениями в БД и возвращает все название таблиц в БД.
        /// </summary>
        /// <returns>Список имен таблиц в БД</returns>
        public static List<string> GetTables()
        {
            using (SqlConnection connection = new SqlConnection(сonnectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");
                List<string> TableNames = new List<string>();
                DataManager.Views = new List<string>();

                foreach (DataRow row in schema.Rows)
                {
                    if(row[2].ToString().IndexOf("View") >= 0 || row[2].ToString().IndexOf("view") >= 0)
                    {
                        DataManager.Views.Add(row[2].ToString());
                    }
                    else if(row[2].ToString().IndexOf("diagram") >= 0 || row[2].ToString().IndexOf("Diagram") >= 0)
                    {

                    }
                    else
                        {
                        TableNames.Add(row[2].ToString());
                    }
                }
                return TableNames;
            }
        }
        /// <summary>
        /// Получает информацио о таблице из БД по парметру tableName и заполняет эту инофрмацию в текущую открытыю таблицу.
        /// </summary>
        /// <param name="tableName">Имя таблицы в БД</param>
        public static void getInfoFromTable(string tableName)
        {
            using (SqlConnection connection = new SqlConnection(сonnectionString))
            {
                try
                {
                    connection.Open();

                    if (DataManager.currentTable != null)
                        DataManager.currentTable = new DataTable();

                    SqlCommand command = new SqlCommand("SELECT * FROM " + tableName, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(DataManager.currentTable);
                }
                catch
                {
                    MessageBox.Show("Ошибка чтения данных из файла!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Получает всю информацио о таблице из БД по параметру tableName и заполняет этой информацией параметр dataTable;
        /// </summary>
        /// <param name="tableName">Имя таблицы в БД</param>
        /// <param name="dataTable">Контейнер типа DataTable для хранения данных их БД</param>
        public static void getInfoFromTable(string tableName, DataTable dataTable)
        {
            using (SqlConnection connection = new SqlConnection(сonnectionString))
            {
                try
                {
                    connection.Open();

                    if (DataManager.currentTable != null)
                        DataManager.currentTable = new DataTable();

                    SqlCommand command = new SqlCommand("SELECT * FROM " + tableName, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
                catch
                {
                    MessageBox.Show("Ошибка чтения данных из файла!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Обновляет таблицу в БД по параметру tableName. Обновленые данные берутся из текущей открытой в программе таблицы.
        /// </summary>
        /// <param name="tableName">Имя таблицы в БД</param>
        public static void UpdateBD(string tableName)
        {
            using (SqlConnection connection = new SqlConnection(сonnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT * FROM " + tableName, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);

                    adapter.Update(DataManager.currentTable);

                    DataManager.currentTable.AcceptChanges();
                }
                catch
                {
                    MessageBox.Show("Не удалось обновить базу данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Обновляет таблицу в БД по парметру tableName и dataTable
        /// </summary>
        /// <param name="tableName">Имя таблицы в БД</param>
        /// <param name="dataTable">Таблица из который мы берем новые данные для обновления</param>
        public static void UpdateBD(string tableName, DataTable dataTable)
        {
            using (SqlConnection connection = new SqlConnection(сonnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT * FROM " + tableName, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);

                    adapter.UpdateCommand = comandbuilder.GetUpdateCommand();
                    adapter.DeleteCommand = comandbuilder.GetDeleteCommand();

                    adapter.Update(dataTable);

                    dataTable.AcceptChanges();
                }
                    catch
                {
                    MessageBox.Show("Не удалось обновить базу данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        }
    }
}
