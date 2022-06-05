using System;
using System.Collections.Generic;
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
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        private int id;
        private string username;
        private string supervisorname;
        private string adress;
        private string themes;
        private string content;
        private string resolution;
        private int appstatus;
        private string note;

        public Window3()
        {
            InitializeComponent();
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

        private void usernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            username = Convert.ToString(usernameBox.Text);
        }

        private void usernameBox_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {
            supervisorname = usernameBox_Copy.Text;
        }

        private void adressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            adress = adressBox.Text;
        }

        private void themeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            themes = themeBox.Text;
        }

        private void contentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            content = contentBox.Text;
        }

        private void statusBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                appstatus = Convert.ToInt32(statusBox.Text);
            }
            catch (FormatException)
            {
                statusBox.Text = "0";
            }
            
        }

        private void resolutionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            resolution = resolutionBox.Text;
        }

        private void noteBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            note = noteBox.Text;
        }
        private void idValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void StatusValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-2]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            var DB = new DBWorks();
            int check = DB.maxID();
            if (id > check)
            {
                //add new entry
                if (username == "" || username == null) { MessageBox.Show("Поле отправителя не может быть пустым!"); return; }
                if (supervisorname == "" || supervisorname == null) { MessageBox.Show("Поле получателя не может быть пустым!"); return; }
                if (adress == "" || adress == null) { MessageBox.Show("Поле адреса не может быть пустым!"); return; }
                if (themes == "" || themes == null) { MessageBox.Show("Поле темы не может быть пустым!"); return; }
                if (content == "" || content == null) { MessageBox.Show("Текст обращения не может быть пустым!"); return; }
                string promt = String.Format("Будет создана новая запись под номером {0}", check+1);
                MessageBoxResult res = MessageBox.Show(promt, "Создать новую запись?", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes) { DB.DBAppendFull(username, supervisorname, adress, themes, content, resolution, appstatus, note); }
                else { return; }
                MessageBox.Show("Была создана новая запись");
                this.Close();
            } else
            {
                //edit existing entry
                string c1 = "", c2 = "", c3 = "", c4 = "", c5 = "", c6 = "", c7 = "",c8="";
                if (username != "" && username != null) { DB.DBUpdateUsername(id,username);c1 = "Имя Заявителя, "; }
                if (supervisorname != "" && supervisorname != null) { DB.DBUpdateSname(id, supervisorname); c2 = "Адресат, "; }
                if (adress != "" && adress != null) { DB.DBUpdateAdress(id, adress); c3 = "Адрес, "; }
                if (themes != "" && themes != null) { DB.DBUpdateThemes(id, themes); c4 = "Темы, "; }
                if (content != "" && content != null) { DB.DBUpdateContent(id, content); c5 = "Текст обращения, "; }
                if (resolution != "" && resolution != null) { DB.DBUpdateResolution(id, resolution); c6 = "Резолюция, "; }
                { DB.DBUpdateStatus(id, appstatus); c7 = "Статус, "; }
                if (note != "" && note != null) { DB.DBUpdateNote(id, note); c8 = "Примечание."; }
                string promt = String.Format("Были изменены следующие записи: {0}{1}{2}{3}{4}{5}{6}{7}", c1, c2,c3, c4, c5, c6, c7, c8);
                MessageBox.Show(promt);
                this.Close();

            }
        }
    }
}
