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


namespace ShoppingSystem.Forms
{
    public partial class CheckoutForm: Form
    {
        private List<CartItem> cartItems;
        public CheckoutForm(List<CartItem> cartItems)
        {
            InitializeComponent();
            this.cartItems = cartItems;
            InitializeCartView();
        }

        private void InitializeCartView()
        {
            dgvCart.DataSource = null;
            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "商品名稱", DataPropertyName = "ProductName" });
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

            MessageBox.Show("訂單已成立！");
            cartItems.Clear(); // 清空購物車
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
