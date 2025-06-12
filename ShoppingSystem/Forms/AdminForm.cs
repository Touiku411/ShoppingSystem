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
        //private string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
        //        @"AttachDBFilename = |DataDirectory|\Database.mdf;Integrated Security=True;"
        private string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = C:\Users\tengy\source\repos\ShoppingSystem\ShoppingSystem\Database.mdf;Integrated Security=True;"; 
        private List<Product> products = new List<Product>();
        private List<Order> orders = new List<Order>();
        public AdminForm()
        {
            InitializeComponent();
            //LoadProducts();
            SetupCategoryComboBox();
            if(tabControls.SelectedTab == tabPageProducts)
            {
                LoadProducts();
            }
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
                    var p = new Order()
                    {
                        Id  = (int)reader["Id"],
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        TotalPrice = Convert.ToInt32(reader["TotalPrice"]),
                        UserName = reader["UserName"].ToString(),
                    };
                    orders.Add(p);
                }
                foreach(var p in orders)
                {
                    dgvOrders.Rows.Add(p.Id,p.OrderDate,p.TotalPrice,p.UserName);
                }
            }
        }
        private void SetupCategoryComboBox()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("Fruit");
            cmbCategory.Items.Add("Drink");
            cmbCategory.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            //string category = txtCategory.Text.Trim();
            string category = cmbCategory.SelectedItem.ToString();
            string imagepath = txtImagePath.Text.Trim();
            if (!int.TryParse(txtPrice.Text.Trim(), out int price))
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



        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text.Trim(), out int id))
            {
                MessageBox.Show("請輸入有效的商品 ID！");
                return;
            }
            string name = txtName.Text.Trim();
            string category = cmbCategory.SelectedItem.ToString();
            string imagepath = txtImagePath.Text.Trim();

            if(!int.TryParse(txtPrice.Text.Trim(),out int price)){
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
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(!int.TryParse(txtId.Text.Trim(),out int id))
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
        }
    }
}
