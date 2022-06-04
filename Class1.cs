using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace qrdocs
{
    public class DBWorks
    {
        public struct DBStruct
        {
            public object id;
            public object username;
            public object supervisorname;
            public object adress;
            public object themes;
            public object content;
            public object resolution;
            public object appstatus;
            public object note;
        }
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
        /*public async void DBShow()
        {
            string sqlExpression = "SELECT * FROM appdata";
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
              await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                DBStruct DBS[] = new DBStruct;
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        Object[] values = new Object[9];
                        DBS = reader.GetValues(values);
                         
                    }
                }

            }
        }*/
        }
}
