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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace ShoppingSystem.Forms
{
    public partial class LoginForm : Form
    {
        public bool IsAuthenticated { get; private set; } = false;
        public string LoggedInUsername { get; private set; }
        public string LoggedInRole { get; private set; }
        public LoginForm()
        {
            InitializeComponent();
        }
     
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = |DataDirectory|\Database.mdf;Integrated Security=True;";

            using(SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                string sql = "SELECT Username, Role FROM Users WHERE Username = @username AND Password = @password;";

                using(SqlCommand  cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LoggedInUsername = reader["Username"].ToString();
                            LoggedInRole = reader["Role"].ToString();

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
            }

        }
    }
}
