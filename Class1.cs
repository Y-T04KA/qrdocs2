using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.IO;
using ZXing.QrCode;
using Microsoft.Win32;
using System.Drawing;

namespace qrdocs
{
    public class DBWorks
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\podelki\legacy\qrdocs\Database1.mdf;Integrated Security=True;Connect Timeout=30";

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
                        
                       }
                } else
                {
                    MessageBox.Show("Обращение не найдено");
                }
                
            }
        }
        private void MakeQR(string qrstring,object id2)
        {
            var writer = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 450,
                    Width = 450,
                    Margin = 0
                }
            };
            var pixeldata = writer.Write(qrstring);
            using (var bitmap = new System.Drawing.Bitmap(pixeldata.Width, pixeldata.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixeldata.Width, pixeldata.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixeldata.Pixels, 0, bitmapData.Scan0, pixeldata.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    string filename = String.Format("D://test/{0}.png", id2);
                    bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Файл создан");
                }
            }
        }
        public void DBreadQR()
        {
            var reader = new ZXing.Windows.Compatibility.BarcodeReader();
            OpenFileDialog filed = new OpenFileDialog();
            filed.ShowDialog();
            string fil = filed.FileName;
            if (fil == "") { return; }
            else
            {
                var barcodeBitmap = (Bitmap)Image.FromFile(fil);
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
                
                    if (IDcheck(id))
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
            string SqlExpression = "UPDATE appdata SET username=@username, adress=@adress, themes=@themes, content=@content, resolution=@resolution, appstatus=@appstatus,note=@note WHERE id=@id";
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
    }
}
