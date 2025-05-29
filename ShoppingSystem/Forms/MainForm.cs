using ShoppingSystem.Forms;
using ShoppingSystem.Models;
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
using ShoppingSystem.Data;

namespace ShoppingSystem
{
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();
            //LoadSampleProducts();

        }

        private List<Product> products = new List<Product>();
        private List<CartItem> cartItems = new List<CartItem>();
        private List<Order> orderHistory = new List<Order>();

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            var filter = products.FindAll(p=>p.Name.ToLower().Contains(keyword));
            DisplayProducts(filter);
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string category = cmbCategory.SelectedItem.ToString();
            if (category == "全部")
                DisplayProducts(products);
            else
                DisplayProducts(products.FindAll(p => p.Category == category));
        }

        private void btnCart_Click(object sender, EventArgs e)
        {
            using(CheckoutForm form = new CheckoutForm(cartItems))
            {
                // 建立副本，保證資料不會被 CheckoutForm 清掉
                var cartSnapshot = cartItems.Select(item => new CartItem
                {
                    Product = item.Product,
                    Quantity = item.Quantity
                }).ToList();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    // 這裡才建立訂單（使用 cartSnapshot，而不是 cartItems）
                    var newOrder = new Order
                    {
                        OrderId = orderHistory.Count + 1,
                        Date = DateTime.Now,
                        Items = cartSnapshot
                    };
                    orderHistory.Add(newOrder);

                    cartItems.Clear();
                    UpdateCartButtonText();
                }
            }
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("管理員功能待實作");
        }

        private void SetupCategoryComboBox()
        {
            cmbCategory.Items.Add("全部");
            var categories = products.Select(p=>p.Category).Distinct().ToList();
            foreach( var category in categories)
            {
                cmbCategory.Items.Add(category);
            }

            cmbCategory.SelectedIndex = 0;
        }

        private void DisplayProducts(List<Product> productList)
        {
            flpProducts.Controls.Clear();

            foreach (var p in productList)
            {
                Panel panel = new Panel
                {
                    Width = 180,
                    Height = 220,
                    Margin = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                PictureBox pic = new PictureBox
                {
                    Width = 160,
                    Height = 120,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Top = 10,
                    Left = 10,
                };

                try
                {
                    pic.Image = Image.FromFile(p.ImagePath);
                }
                catch
                {
                    // 圖片讀取失敗可忽略
                }
                panel.Controls.Add(pic);

                Label lblName = new Label
                {
                    Text = p.Name,
                    Top = 140,
                    Left = 10,
                    Width = 160
                };
                panel.Controls.Add(lblName);

                Label lblPrice = new Label
                {
                    Text = $"價格: ${p.Price}",
                    Top = 165,
                    Left = 10,
                    Width = 160
                };
                panel.Controls.Add(lblPrice);

                Button btnAddCart = new Button
                {
                    Text = "加入購物車",
                    Top = 190,
                    Left = 10,
                    Width = 160,
                    Tag = p  // 利用 Tag 儲存對應商品
                };
                btnAddCart.Click += BtnAddCart_Click;
                panel.Controls.Add(btnAddCart);

                flpProducts.Controls.Add(panel);
            }
        }
        private void BtnAddCart_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag is Product p)
            {
                var existing = cartItems.Find(c => c.Product.Id == p.Id);
                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    cartItems.Add(new CartItem { Product = p, Quantity = 1 });
                }
                UpdateCartButtonText();
                MessageBox.Show($"已加入購物車：{p.Name}");
            }
        }

        private void UpdateCartButtonText()
        {
            int count = 0;
            foreach (var item in cartItems)
            {
                count += item.Quantity;
            }
            btnCart.Text = $"購物車({count})";
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            using(HistoryForm form = new HistoryForm(orderHistory))
            {
                form.ShowDialog();
            }
        }
        SqlConnection sqlDb = null;
        private void MainForm_Load(object sender, EventArgs e)
        {
            string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = |DataDirectory|\Database.mdf;";
            try
            {
                sqlDb = new SqlConnection(cntStr);
                sqlDb.Open();

                string sqlStr = "SELECT * FROM Products";

                SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlDb);
                SqlDataReader sqlDr = sqlCmd.ExecuteReader();

                products.Clear(); // 先清空舊資料
                flpProducts.Font = new Font("微軟正黑體", 10);

                while (sqlDr.Read())
                {
                    Product p = new Product
                    {
                        Id = Convert.ToInt32(sqlDr["Id"]),
                        Name = sqlDr["Name"].ToString(),
                        Price = Convert.ToInt32(sqlDr["Price"]),
                        Category = sqlDr["Category"].ToString(),
                        ImagePath = sqlDr["ImagePath"].ToString()
                    };
                    products.Add(p);
                }

                sqlDr.Close();

                //dgvProducts.Font = new Font("微軟正黑體", 10);
                //int rowIndex = 0;
                //for (int i = 0; i < sqlDr.FieldCount; i++)
                //{
                //    dgvProducts.Columns.Add("column" + (i + 1).ToString(), sqlDr.GetName(i));
                //}
                //while (sqlDr.Read() != false)
                //{
                //    dgvProducts.Rows.Add();
                //    for (int i = 0; i < sqlDr.FieldCount; i++)
                //    {
                //        dgvProducts.Rows[rowIndex].Cells[i].Value = sqlDr.GetValue(i).ToString();
                //    }
                //    rowIndex++;
                //}
                sqlDr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //var repo = new ProductRepository(cntStr);
            //products = repo.GetAll();
 
            DisplayProducts(products);
            SetupCategoryComboBox();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(sqlDb != null)
            {
                sqlDb.Close();
            }
        }
    }
}
