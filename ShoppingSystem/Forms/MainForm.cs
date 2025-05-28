using ShoppingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoppingSystem
{
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();
            btnSearch.Click += btnSearch_Click;
            cmbCategory.SelectedIndexChanged += cmbCategory_SelectedIndexChanged;
            btnCart.Click += btnCart_Click;
            btnAdmin.Click += btnAdmin_Click;

            LoadSampleProducts();
            SetupCategoryComboBox();
            DisplayProducts(products);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
     
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
            MessageBox.Show("購物車功能待實作");
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("管理員功能待實作");
        }

        //簡易商品類別
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public string Category { get; set; }
            public string ImagePath { get; set; }
        }
        // 購物車項目類別
        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }

        private List<Product> products = new List<Product>();
        private List<CartItem> cartItems = new List<CartItem>();

        private void LoadSampleProducts()
        {
            products.Add(new Product { Id = 1, Name = "蘋果", Price = 30, Category = "水果", ImagePath = "Resources/蘋果.png" });
            products.Add(new Product { Id = 2, Name = "香蕉", Price = 25, Category = "水果", ImagePath = "Resources/香蕉.png" });
            products.Add(new Product { Id = 3, Name = "牛奶", Price = 50, Category = "飲料", ImagePath = "Resources/牛奶.png" });
        }
        private void SetupCategoryComboBox()
        {
            cmbCategory.Items.Add("全部");
            cmbCategory.Items.Add("水果");
            cmbCategory.Items.Add("飲料");
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
    }
}
