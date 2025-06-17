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


namespace ShoppingSystem.Forms
{
    public partial class CheckoutForm: Form
    {
        private List<CartItem> cartItems;
        private string currentUser;
        public CheckoutForm(List<CartItem> cartItems, string currentUser)
        {
            InitializeComponent();
            this.cartItems = cartItems;
            InitializeCartView();
            this.currentUser = currentUser;
        }

        private void InitializeCartView()
        {
            dgvCart.DataSource = null;
            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "商品名稱", DataPropertyName = "ProductName", Name = "商品名稱" });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "單價", DataPropertyName = "Price" });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "數量", DataPropertyName = "Quantity" });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "小計", DataPropertyName = "Subtotal" });

            dgvCart.DataSource = cartItems.Select(c => new
            {
                ProductName = c.Product.Name,
                Price = c.Product.Price,
                Quantity = c.Quantity,
                Subtotal = c.Product.Price * c.Quantity
            }).ToList();

            UpdateTotalLabel();
        }
        private void UpdateTotalLabel()
        {
            int total = cartItems.Sum(item => item.Product.Price * item.Quantity);
            lblTotal.Text = $"總金額：${total}";
        }
  
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("購物車是空的！");
                return;
            }

            int totalPrice = cartItems.Sum(item => item.Product.Price * item.Quantity);
            string userName = currentUser;

            string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = |DataDirectory|\Database.mdf;
                Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    //寫入Orders
                    string sqlOrder = "INSERT INTO Orders([OrderDate ],TotalPrice,UserName) VALUES(GETDATE(),@totalPrice,@userName); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdOrder = new SqlCommand(sqlOrder, conn, tx);
             
                    cmdOrder.Parameters.AddWithValue("@totalPrice", totalPrice);
                    cmdOrder.Parameters.AddWithValue("@userName", userName);
                    int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());


                    //寫入OrderItmes
                    foreach (var item in cartItems)
                    {
                        string sqlItem = "INSERT INTO OrderItems(OrderId,ProductId,Quantity,UnitPrice) VALUES(@orderId, @productId, @quantity, @unitPrice);";
                        SqlCommand cmdItem = new SqlCommand(sqlItem, conn, tx);

                        cmdItem.Parameters.AddWithValue("@orderId", orderId);
                        cmdItem.Parameters.AddWithValue("@productId", item.Product.Id);
                        cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                        cmdItem.Parameters.AddWithValue("@unitPrice", item.Product.Price);
                        cmdItem.ExecuteNonQuery();
                    }
                    tx.Commit();
                    MessageBox.Show("訂單儲存成功！");
                }
                catch (Exception ex)
                {
                    {
                        tx.Rollback();
                        MessageBox.Show("訂單儲存失敗：" + ex.Message);
                    }
                }
            }
            cartItems.Clear(); // 清空購物車
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvCart_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string selectedProductName = dgvCart.Rows[e.RowIndex].Cells["商品名稱"].Value.ToString();
            DialogResult result = MessageBox.Show($"是否移除 {selectedProductName}？", "確認移除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var itemToRemove = cartItems.FirstOrDefault(c => c.Product.Name == selectedProductName);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                    InitializeCartView();
                }
            }
        }
    }
}
