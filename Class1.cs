using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using ZXing;
using ZXing.QrCode;

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
                        var writer = new ZXing.BarcodeWriterPixelData
                        {
                            Format = ZXing.BarcodeFormat.QR_CODE,
                            Options = new QrCodeEncodingOptions
                            {
                                Height = 300,
                                Width = 300,
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
                } else
                {
                    MessageBox.Show("Обращение не найдено");
                }
                
            }
        }
        }
}
