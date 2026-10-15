using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace WpfTeam12
{
    /// <summary>
    /// Interaction logic for SqlWindow.xaml
    /// </summary>
    public partial class SqlWindow : Window
    {
        public SqlWindow()
        {
            InitializeComponent();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string connectionString = "Trusted_Connection=True;";
                string connectionString = "user id = PxlUser_12;";
                connectionString += "Password = YOUR_DB_PASSWORD;";
                connectionString += $@"Server={TxtServer.Text};";
                connectionString += $"Database={TxtDb.Text}";
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                conn.Close();
                MessageBox.Show("connection ok!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private SqlConnection GetConnection()
        {
            SqlConnection conn;
            try
            {
                //string connectionString = "Trusted_Connection=True;";
                string connectionString = "User ID=PxlUser_12;";
                connectionString += "Password = YOUR_DB_PASSWORD;";
                connectionString += $@"Server={TxtServer.Text};";
                connectionString += $"Database={TxtDb.Text}";
                conn = new SqlConnection(connectionString);
                conn.Open();
                conn.Close();
                return conn;
            //Server:   YOUR_SERVER
            //Database: Db2025Team_12
            //Login:    PxlUser_12
            //Password: YOUR_DB_PASSWORD
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        private void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cmd = $"Select * from {TxtTable.Text}";
                SqlCommand sql = new SqlCommand(cmd, GetConnection());
                SqlDataAdapter da = new SqlDataAdapter(sql);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DgdSql.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
