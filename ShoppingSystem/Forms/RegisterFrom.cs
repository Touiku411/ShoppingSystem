using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoppingSystem.Forms
{
    public partial class RegisterFrom : Form
    {
        private string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
               @"AttachDBFilename = |DataDirectory|\Database.mdf;Integrated Security=True;";
        public RegisterFrom()
        {
            InitializeComponent();
        }

        private void btnComfirm_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("請輸入帳號與密碼！");
                return;
            }
            using(SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();

                //檢查重複
                string checksql = "SELECT COUNT(*) FROM Users WHERE Username = @username;";
                SqlCommand cmd = new SqlCommand(checksql, conn);
                cmd.Parameters.AddWithValue("@username",username);
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("此帳號已存在！");
                    return;
                }

                //寫入資料表
                string insertsql = "INSERT INTO Users (Username, Password) VALUES(@username, @password);";
                SqlCommand sqlCommand = new SqlCommand(insertsql, conn);
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters.AddWithValue("@password", password);
                int rows = sqlCommand.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "註冊成功！" : "註冊失敗！");
                if (rows > 0)
                    this.DialogResult = DialogResult.OK;
            }
        }
    }
}
