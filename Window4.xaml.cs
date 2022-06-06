using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace qrdocs
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4()
        {
            InitializeComponent();
            LoadEntries();
        }

        private SqlDataAdapter adapter;
        private DataTable ds;
        public string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rudb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public void LoadEntries()
        {
            string sqlExpression = "SELECT * FROM appdata";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\podelki\legacy\qrdocs\Database1.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                adapter = new SqlDataAdapter(sqlExpression, connection);
                ds = new DataTable();
                adapter.Fill(ds);
                Submissions.ItemsSource = ds.DefaultView;


            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Window4 newWindow = new Window4();
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }
        private int id;
        private int appstatus;
        private string resolution;
        private string note;

        private void AcknowledgedRB_Checked(object sender, RoutedEventArgs e)
        {
            appstatus = 1;
        }

        private void RejectedRB_Checked(object sender, RoutedEventArgs e)
        {
            appstatus = 2;
        }

        private void ResolutionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            resolution = ResolutionBox.Text;
        }

        private void NoteBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            note = NoteBox.Text;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            DBWorks db = new DBWorks();
            if (id > db.maxID())
            {
                MessageBox.Show("Номер не найден");
                return;
            }
            if (resolution == null || resolution == "")
            {
                MessageBox.Show("Поле Резолюция не может быть пустым");
                return;
            }
            if (note == null || note == "")
            {
                db.DBUpdateStatus(id,appstatus);
                db.DBUpdateResolution(id, resolution);
            }
            else
            {
                db.DBUpdateStatus(id, appstatus);
                db.DBUpdateResolution(id, resolution);
                db.DBUpdateNote(id, note);
            }

        }

        private void idBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                id = Convert.ToInt32(idBox.Text);
            }
            catch (FormatException)
            {
                idBox.Text = "0";
            }          
        }
        private void idValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ResetButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            MainWindow p = new MainWindow();
            p.Show();
            Close();
        }
    }
}
