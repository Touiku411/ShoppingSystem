using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoppingSystem.Forms
{
    public partial class AdminLoginForm : Form
    {
        public bool IsAuthenticated { get; private set; } = false;
        public AdminLoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = C:\Users\tengy\source\repos\ShoppingSystem\ShoppingSystem\Database.mdf;Integrated Security=True;";

            using(SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM AdminUsers WHERE Username = @username AND Password = @password;";

                using(SqlCommand  cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        IsAuthenticated = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("帳號或密碼錯誤", "登入失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            //if (username == "Admin" && password == "1234")
            //{
            //    IsAuthenticated = true;
            //    this.DialogResult = DialogResult.OK;
            //    this.Close();
            //}
            //else
            //{
            //    MessageBox.Show("帳號或密碼錯誤", "登入失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }
    }
}
