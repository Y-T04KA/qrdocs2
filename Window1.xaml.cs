using System;
using System.Collections.Generic;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private string username;
        private string supervisorname;
        private string adress;
        private string themes;
        private string content;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            content = SubmissionText.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SubmissionModeSendButton_Click(object sender, RoutedEventArgs e)
        {
            var append = new DBWorks();
            if (username=="" || username == null) { MessageBox.Show("Поле отправителя не может быть пустым!"); return; }
            if (supervisorname=="" || supervisorname==null) { MessageBox.Show("Поле получателя не может быть пустым!"); return; }
            if (adress == "" || adress == null) { MessageBox.Show("Поле адреса не может быть пустым!"); return; }
            if (themes == "" || themes == null) { MessageBox.Show("Поле темы не может быть пустым!"); return; }
            if (content == "" || content == null) { MessageBox.Show("Текст обращения не может быть пустым!"); return; }
            append.DBAppend(username, supervisorname, adress, themes, content);
        }

        private void SenderName_TextChanged(object sender, TextChangedEventArgs e)
        {
            username = SenderName.Text;
        }

        private void AddresseeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            supervisorname = AddresseeName.Text;
        }

        private void SenderAdress_TextChanged(object sender, TextChangedEventArgs e)
        {
            adress = SenderAdress.Text;
        }

        private void SubmissionHeader_TextChanged(object sender, TextChangedEventArgs e)
        {
            themes = SubmissionHeader.Text; 
        }
    }
}
