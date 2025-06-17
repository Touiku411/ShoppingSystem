using ShoppingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoppingSystem.Forms
{
    public partial class AdminForm: Form
    {
        SqlConnection SqlDb = null;
        private string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = |DataDirectory|\Database.mdf;Integrated Security=True;"; 
        private List<Product> products = new List<Product>();
        private List<Order> orders = new List<Order>();
        private List<User> users = new List<User>();
        private MainForm mainForm;
        public AdminForm(MainForm form)
        {
            InitializeComponent();
            //LoadProducts();
            SetupCategoryComboBox();
            if(tabControls.SelectedTab == tabPageProducts)
            {
                LoadProducts();
            }
            this.mainForm = form;
        }
            
        private void LoadProducts()
        {
            products.Clear();
            dgvProducts.Rows.Clear();

            using (SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                string sql = "SELECT * FROM Products";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var p = new Product()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToInt32(reader["Price"]),
                        Category = reader["Category"].ToString(),
                        ImagePath = reader["ImagePath"].ToString()
                    };
                    products.Add(p);
                }
                foreach (var p in products)
                {
                    dgvProducts.Rows.Add(p.Id, p.Name, p.Price, p.Category, p.ImagePath);
                }
            }
        }
        private void LoadOrders()
        {
            orders.Clear();
            dgvOrders.Rows.Clear();

            using(SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                string sql = "SELECT * FROM Orders;";
                SqlCommand cmd = new SqlCommand(sql,conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var o = new Order()
                    {
                        Id  = (int)reader["Id"],
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        TotalPrice = Convert.ToInt32(reader["TotalPrice"]),
                        UserName = reader["UserName"].ToString(),
                    };
                    orders.Add(o);
                }
                foreach(var o in orders)
                {
                    dgvOrders.Rows.Add(o.Id,o.OrderDate,o.TotalPrice,o.UserName);
                }
            }
        }

        private void LoadUsers()
        {
            users.Clear();
            dgvUsers.Rows.Clear();

            using(SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                string sql = "SELECT * FROM Users;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var o = new User()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        Role = reader["Role"].ToString()
                    };
                    users.Add(o);
                }
                foreach(var o in users)
                {
                    dgvUsers.Rows.Add(o.Id,o.Username,o.Password,o.Role);
                }
            }
        }
        private void SetupCategoryComboBox()
        {
            cmbCategoryAdd.Items.Clear();
            cmbCategoryAdd.Items.Add("Fruit");
            cmbCategoryAdd.Items.Add("Drink");
            cmbCategoryAdd.Items.Add("Bakery");
            cmbCategoryUpdate.Items.Clear();
            cmbCategoryUpdate.Items.Add("Fruit");
            cmbCategoryUpdate.Items.Add("Drink");
            cmbCategoryUpdate.Items.Add("Bakery");

            cmbCategoryAdd.SelectedIndex = 0;
            cmbCategoryUpdate.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtNameAdd.Text.Trim();
            string category = cmbCategoryAdd.SelectedItem.ToString();
            string imagepath = txtImagePathAdd.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("商品名稱不可為空！");
                return;
            }
            else if(string.IsNullOrEmpty(category))
            {
                MessageBox.Show("請選擇商品種類！");
                return;
            }
            if (!int.TryParse(txtPriceAdd.Text.Trim(), out int price))
            {
                MessageBox.Show("價格格式錯誤！");
                return;
            }
            
            string sql = "INSERT INTO Products(Name,Price,Category,ImagePath) VALUES(@name,@price,@category,@imagepath);";
            SqlDb = new SqlConnection(cntStr);
            SqlDb.Open();
            using (SqlCommand cmd = new SqlCommand(sql,SqlDb))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@imagepath", imagepath);

                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "新增成功!" : "新增失敗!");
                LoadProducts();
            }

            mainForm.ReloadProduct();

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtIdUpdate.Text.Trim(), out int id))
            {
                MessageBox.Show("請輸入有效的商品 ID！");
                return;
            }
            string name = txtNameUpdate.Text.Trim();
            string category = cmbCategoryUpdate.SelectedItem.ToString();
            string imagepath = txtImagePathUpdate.Text.Trim();

            if(!int.TryParse(txtPriceUpdate.Text.Trim(),out int price)){
                MessageBox.Show("價格格式錯誤！");
                return;
            }
            string sql = "UPDATE Products SET Name = @name, Price = @price, Category = @category, ImagePath = @imagepath WHERE Id = @id;";
            SqlDb = new SqlConnection(cntStr);
            SqlDb.Open();
            using(SqlCommand cmd = new SqlCommand(sql, SqlDb))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@imagepath", imagepath);

                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "更新成功！" : "找不到該商品！");
                LoadProducts();
            }
            mainForm.ReloadProduct();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(!int.TryParse(txtIdDelete.Text.Trim(),out int id))
            {
                MessageBox.Show("請輸入有效的商品 ID！");
                return;
            }

            string sql = "DELETE FROM Products\r\nWHERE Id = @id;";
            SqlDb = new SqlConnection(cntStr);
            SqlDb.Open();
            using(SqlCommand cmd = new SqlCommand(sql, SqlDb))
            {
                cmd.Parameters.AddWithValue("@id", id);
                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "刪除成功！" : "找不到該商品！");
                LoadProducts();
            }
            mainForm.ReloadProduct();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControls.SelectedTab == tabPageProducts)
            {
                LoadProducts();
            }
            else if(tabControls.SelectedTab == tabPageOrders)
            {
                LoadOrders();
            }
            else if(tabControls.SelectedTab == tabPageUsers)
            {
                LoadUsers();
            }
        }

        private void btnAddUsers_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = txtRole.Text.Trim();

            string sql = "INSERT INTO Users (Username, Password, Role) VALUES(@username, @password, @role);";
            SqlDb = new SqlConnection(cntStr);
            SqlDb.Open();
            using (SqlCommand cmd = new SqlCommand(sql, SqlDb))
            {
                cmd.Parameters.AddWithValue ("@username", username);
                cmd.Parameters.AddWithValue ("@password", password);
                cmd.Parameters.AddWithValue ("@role", role);
                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "新增成功!" : "新增失敗!");
                LoadUsers();
            }
        }


        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtIdUser.Text.Trim(), out int id))
            {
                MessageBox.Show("請輸入有效的使用者 ID！");
                return;
            }

            string sql = "DELETE FROM Users WHERE Id = @id;";
            SqlDb = new SqlConnection(cntStr);
            SqlDb.Open();
            using (SqlCommand cmd = new SqlCommand(sql, SqlDb))
            {
                cmd.Parameters.AddWithValue("@id", id);
                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "刪除成功！" : "找不到使用者！");
                LoadUsers();
            }
        }
    }
}
