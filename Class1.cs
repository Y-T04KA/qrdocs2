using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.IO;
using ZXing.QrCode;
using Microsoft.Win32;
using System.Drawing;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.IO.Image;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET;
using System.Drawing.Imaging;
using iText.Kernel.Font;
using ZXing;
using System.Text;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace qrdocs
{
    public class DBWorks
    {
        //private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\podelki\legacy\qrdocs\Database1.mdf;Integrated Security=True;Connect Timeout=30";
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rudb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public async void DBAppend(string eusername, string esupervisorname, string eadress, string ethemes, string econtent)
        {//e stands for external
            string sqlExpression = "INSERT INTO appdata (username, supervisorname, adress, themes, content, appstatus) VALUES (@username,@supervisorname,@adress,@themes,@content,0)";
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression,connection);
                SqlParameter nameParam = new SqlParameter("username", eusername);
                command.Parameters.Add(nameParam);
                SqlParameter snameParam = new SqlParameter("supervisorname", esupervisorname);
                command.Parameters.Add(snameParam);
                SqlParameter adressParam = new SqlParameter("adress", eadress);
                command.Parameters.Add(adressParam);
                SqlParameter themesParam = new SqlParameter("themes", ethemes);
                command.Parameters.Add(themesParam);
                SqlParameter contentParam = new SqlParameter("content", econtent);
                command.Parameters.Add(contentParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number > 0)
                {
                    MessageBox.Show("Заявление добавлено");
                }
                else {
                    MessageBox.Show("Ошибка");
                }
            }   
        }
        public async void DBAppendFull(string eusername, string esupervisorname, string eadress, string ethemes, string econtent, string resolution, int appstatus, string note)
        {
            string sqlExpression = "INSERT INTO appdata (username, supervisorname, adress, themes, content, resolution, appstatus, note) VALUES (@username,@supervisorname,@adress,@themes,@content,@resolution,@appstatus,@note)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter nameParam = new SqlParameter("username", eusername);
                command.Parameters.Add(nameParam);
                SqlParameter snameParam = new SqlParameter("supervisorname", esupervisorname);
                command.Parameters.Add(snameParam);
                SqlParameter adressParam = new SqlParameter("adress", eadress);
                command.Parameters.Add(adressParam);
                SqlParameter themesParam = new SqlParameter("themes", ethemes);
                command.Parameters.Add(themesParam);
                SqlParameter contentParam = new SqlParameter("content", econtent);
                command.Parameters.Add(contentParam);
                SqlParameter resParam = new SqlParameter("resolution", resolution);
                command.Parameters.Add(resParam);
                SqlParameter statusParam = new SqlParameter("appstatus", appstatus);
                command.Parameters.Add(statusParam);
                SqlParameter noteParam = new SqlParameter("note", note);
                command.Parameters.Add(noteParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number > 0)
                {
                    MessageBox.Show("Заявление добавлено");
                }
                else
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
        public async void DBGenerateQR(int id)
        {
            string sqlExpression = "SELECT * FROM appdata WHERE id=@id";
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
             await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter idParam = new SqlParameter("@id", id);
                command.Parameters.Add(idParam);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        object id2 = reader["id"];
                        object username = reader["username"];
                        object supervisorname = reader["supervisorname"];
                        object adress = reader["adress"];
                        object themes = reader["themes"];
                        object content = reader["content"];
                        object resolution = reader["resolution"];
                        object appstatus = reader["appstatus"];
                        object note = reader["note"];
                        string qrstring = String.Format("{0}#;{1}#;{2}#;{3}#;{4}#;{5}#;{6}#;{7}#;{8}",id2,username,supervisorname,adress,themes,content,resolution,appstatus,note);
                        MakeQR(qrstring, id2);
                        MessageBoxResult res = MessageBox.Show("Создать файл для печати?", "", MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK) { MakePDF(qrstring,id); }
                       }
                } else
                {
                    MessageBox.Show("Обращение не найдено");
                }
                
            }
        }
        private void MakePDF(string qrstring, int n)
        {
            string fil = String.Format("D://test/{0}.png",n);
            string dest = String.Format("D://test/{0}.pdf",n);
            ///
            string[] data = qrstring.Split("#;");
            string id = String.Format("Номер заявления — {0}", data[0]);
            string username = String.Format("Имя заявителя - {0}", data[1]);
            string supervisorname = String.Format("Имя руководителя - {0}",data[2]);
            string adress = String.Format("Адрес - {0}",data[3]);
            string themes = String.Format("Тема заявления - {0}",data[4]);
            string content = String.Format("Текст заявления - {0}",data[5]);
            string resolution;
            if (data[6] == "")
            {
                resolution = "Резолюция - отсутствует";
            } else
            {
                resolution = String.Format("Резолюция - {0}", data[6]);
            }
            string appstatus = data[7];
            switch (appstatus)
            {
                case "0":
                    appstatus = "Статус - Принято";
                    break; 
                case "1":
                    appstatus = "Статус - Рассмотрено";
                    break ;
                case "2": 
                    appstatus = "Статус - Отклонено";
                    break;
            }
            string note;
            if (data[8] == "")
            {
                note = "Примечание - отсутствует";
            }
            else
            {
                note = String.Format("Примечание - {0}", data[8]);
            }
            //
            if (fil == "") { return; } else
            {
                PdfWriter writer = new PdfWriter(dest);
                PdfDocument pdfDoc = new PdfDocument(writer);
                pdfDoc.AddNewPage();
                
                Document document = new Document(pdfDoc);
                ImageData picture = ImageDataFactory.Create(fil);
                iText.Layout.Element.Image img = new iText.Layout.Element.Image(picture);
                document.Add(img);
                PdfFont font = PdfFontFactory.CreateFont(@"C:\\Windows\Fonts\times.ttf", "Identity-H");
                iText.Layout.Element.List list = new iText.Layout.Element.List();
                list.Add(id);
                list.Add(username);
                list.Add(supervisorname);
                list.Add(adress);
                list.Add(themes);
                list.Add(content);
                list.Add(resolution);
                list.Add(appstatus);
                list.Add(note);
                list.SetFont(font);
                document.Add(list);
                
                document.Close();

            }
        }
        private void MakeQR(string qrstring1,object id2)
        {
            string qrstring = Encoding.UTF8.GetString(Encoding.Default.GetBytes(qrstring1));
            QRCodeWriter qrEncode = new QRCodeWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();    //для колекции поведений
            hints.Add(EncodeHintType.CHARACTER_SET, "utf-8");   //добавление в коллекцию кодировки utf-8
            BitMatrix qrMatrix = qrEncode.encode(   //создание матрицы QR
                qrstring,                 //кодируемая строка
                BarcodeFormat.QR_CODE,  //формат кода, т.к. используется QRCodeWriter применяется QR_CODE
                450,                    //ширина
                450,                    //высота
                hints);                 //применение колекции поведений

            BarcodeWriter qrWrite = new BarcodeWriter();    //класс для кодирования QR в растровом файле
            Bitmap qrImage = qrWrite.Write(qrMatrix);   //создание изображения
            string filename = String.Format("D://test/{0}.png", id2);
            qrImage.Save(filename, System.Drawing.Imaging.ImageFormat.Png);//сохранение изображения
            
        }
        
        public void DBreadQR()
        {
            var reader = new ZXing.Windows.Compatibility.BarcodeReader();
            OpenFileDialog filed = new OpenFileDialog();
            filed.ShowDialog();
            string fil = filed.FileName;
            if (fil.Length == 0) return;
            string[] check = fil.Split("."); 
            int lenght = check.Length;
            if (check[lenght-1] != "png") {
                int dpi = 300;
                string workaround = @"D:\test\temp.png";
                GhostscriptVersionInfo gvi = new GhostscriptVersionInfo(@"D:\test\gsdll64.dll");
                using (var rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(fil, gvi, false);
                    for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                    {
                        var pageFilePath = Path.Combine(@"D:\test\", string.Format("temp.png", pageNumber));
                        var img = rasterizer.GetPage(dpi, pageNumber);
                        img.Save(pageFilePath, ImageFormat.Png);
                    }
                    }
                var barcodeBitmap = (Bitmap)Image.FromFile(workaround);
                reader.AutoRotate = true;
                var result0 = reader.Decode(barcodeBitmap);
                string result;
                if (result0 != null) { result = result0.ToString(); } else { MessageBox.Show("С QR-кодом что-то не так"); return; }
                string[] data = new string[8];
                data = result.Split("#;");
                try
                {
                    int q = data[5].Length;
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show("Информация для обработки не найдена");
                    return;
                }
                int id;
                int.TryParse(data[0], out id);
                string username = data[1];
                string supervisorname = data[2];
                string adress = data[3];
                string themes = data[4];
                string content = data[5];
                string resolution = data[6];
                int appstatus;
                int.TryParse(data[7], out appstatus);
                string note = data[8];
                int id2 = id;
                if (IDcheck(id2))
                {
                    MessageBoxResult res = MessageBox.Show("В базе уже есть запись с таким номером. Перезаписать?", "Обновить?", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) { DBUpdate(id, username, supervisorname, adress, themes, content, resolution, appstatus, note); } else return;
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Будет добавлена новая запись", "Добавить?", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) { DBAppendFull(username, supervisorname, adress, themes, content, resolution, appstatus, note); } else return;
                }
            }
            else
            {
                var barcodeBitmap = (Bitmap)Image.FromFile(fil);
                reader.AutoRotate = true;
                var result0 = reader.Decode(barcodeBitmap);
                string result;
                if (result0 != null) { result = result0.ToString(); } else { MessageBox.Show("С QR-кодом что-то не так"); return; }
                string[] data = new string[8];
                data = result.Split("#;");
                try
                {
                    int q = data[5].Length;
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show("Информация для обработки не найдена");
                    return;
                }                         
                    int id;
                    int.TryParse(data[0], out id);                    
                    int appstatus;
                    int.TryParse(data[7], out appstatus);                  
                    if (IDcheck(id))
                    {
                        MessageBoxResult res = MessageBox.Show("В базе уже есть запись с таким номером. Перезаписать?", "Обновить?", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes) { DBUpdate(id, data[1], data[2], data[3], data[4], data[5], data[6], appstatus, data[8]); } else return;
                    }
                    else
                    {
                        MessageBoxResult res = MessageBox.Show("Будет добавлена новая запись", "Добавить?", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes) { DBAppendFull(data[1], data[2], data[3], data[4], data[5], data[6], appstatus, data[8]); } else return;
                    }
                
                
            }
        }
        public bool IDcheck(int id)
        {
            string sqlExpression = "SELECT id FROM appdata WHERE id=@id";
                        using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter idParam = new SqlParameter("@id", id);
                command.Parameters.Add(idParam);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }
        public async void DBUpdate(int id, string username, string supervisorname, string adress, string themes, string content, string resolution, int appstatus, string note)
        {
            string SqlExpression = "UPDATE appdata SET username=@username,supervisorname=@supervisorname, adress=@adress, themes=@themes, content=@content, resolution=@resolution, appstatus=@appstatus,note=@note WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("username", username);
                command.Parameters.Add(nameParam);
                SqlParameter snameParam = new SqlParameter("supervisorname", supervisorname);
                command.Parameters.Add(snameParam);
                SqlParameter adressParam = new SqlParameter("adress", adress);
                command.Parameters.Add(adressParam);
                SqlParameter themesParam = new SqlParameter("themes", themes);
                command.Parameters.Add(themesParam);
                SqlParameter contentParam = new SqlParameter("content", content);
                command.Parameters.Add(contentParam);
                SqlParameter resParam = new SqlParameter("resolution", resolution);
                command.Parameters.Add(resParam);
                SqlParameter statusParam = new SqlParameter("appstatus", appstatus);
                command.Parameters.Add(statusParam);
                SqlParameter noteParam = new SqlParameter("note", note);
                command.Parameters.Add(noteParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number > 0)
                {
                    MessageBox.Show("Заявление добавлено");
                }
                else
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
        public async void DBUpdateUsername(int id, string username)
        {
            string SqlExpression = "UPDATE appdata SET username=@username WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("username", username);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении Заявителя"); }
            }
        }
        public async void DBUpdateSname(int id, string supervisorname)
        {
            string SqlExpression = "UPDATE appdata SET supervisorname=@supervisorname WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("supervisorname", supervisorname);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении Адресата"); }
            }
        }
        public async void DBUpdateAdress(int id, string adress)
        {
            string SqlExpression = "UPDATE appdata SET adress=@adress WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("adress", adress);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении адреса"); }
            }
        }
        public async void DBUpdateThemes(int id, string themes)
        {
            string SqlExpression = "UPDATE appdata SET themes=@themes WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("themes", themes);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении темы"); }
            }
        }
        public async void DBUpdateContent(int id, string content)
        {
            string SqlExpression = "UPDATE appdata SET content=@content WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("content", content);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении текста"); }
            }
        }
        public async void DBUpdateResolution(int id, string resolution)
        {
            string SqlExpression = "UPDATE appdata SET resolution=@resolution WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("resolution", resolution);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении решения"); }
            }
        }
        public async void DBUpdateStatus(int id, int status)
        {
            string SqlExpression = "UPDATE appdata SET appstatus=@appstatus WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("appstatus", status);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении статуса"); }
            }
        }
        public async void DBUpdateNote(int id, string note)
        {
            string SqlExpression = "UPDATE appdata SET note=@note WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(SqlExpression, connection);
                SqlParameter idParam = new SqlParameter("id", id);
                command.Parameters.Add(idParam);
                SqlParameter nameParam = new SqlParameter("note", note);
                command.Parameters.Add(nameParam);
                int number = await command.ExecuteNonQueryAsync();
                if (number == 0) { MessageBox.Show("Ошибка при изменении примечания"); }
            }
        }
        public int maxID()
        {
            string sqlExpression = "SELECT MAX(id) FROM appdata";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                return (int)command.ExecuteScalar();
            }
        }   
    }
}
