using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace qrdocs
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public int id2qr;

        public Window2()
        {
            InitializeComponent();
            LoadEntries();

            
             
        }
        private SqlDataAdapter adapter;
        private DataTable ds;
        public string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\podelki\legacy\qrdocs\Database1.mdf;Integrated Security=True;Connect Timeout=30";
        public void LoadEntries()
        {
            string sqlExpression = "SELECT * FROM appdata";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\podelki\legacy\qrdocs\Database1.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                adapter = new SqlDataAdapter(sqlExpression, connection);
                ds = new DataTable();
                adapter.Fill(ds);
                Submissions.ItemsSource = ds.DefaultView;
                
                
            }
        }
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Submissions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SqlCommandBuilder comm = new SqlCommandBuilder(adapter);
            adapter.Update(ds);
            ds.Clear();
            LoadEntries();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(IDPicker.Text, out id2qr);
            var qr = new DBWorks();
            qr.DBGenerateQR(id2qr);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ImportQR_Click(object sender, RoutedEventArgs e)
        {
            var qr = new DBWorks();
            qr.DBreadQR();
            LoadEntries();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Window3 p = new Window3();
            p.Show();
            

        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Window2 newWindow = new Window2();
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }
    }
}
