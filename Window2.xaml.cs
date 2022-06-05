﻿using Microsoft.Data.SqlClient;
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
        public void LoadEntries()
        {
            string sqlExpression = "SELECT * FROM appdata";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\podelki\legacy\qrdocs\Database1.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlExpression, connection);
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                Submissions.ItemsSource = ds.DefaultView;

            }
        }
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Submissions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
    }
}
