using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace qrdocs
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        

        public Window2()
        {
            InitializeComponent();
            string sqlExpression = "SELECT * FROM appdata";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\podelki\legacy\qrdocs\Database1.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlExpression, connection);
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                Submissions.ItemsSource = ds.DefaultView;
                //Submissions.AutoGenerateColumns = true;

            }

            
             
        }
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Submissions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
