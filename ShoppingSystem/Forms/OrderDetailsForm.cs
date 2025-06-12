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
    public partial class OrderDetailsForm : Form
    {
        public OrderDetailsForm(Order order)
        {
            InitializeComponent();
            LoadDetails(order);
        }
        private void LoadDetails(Order order)
        {
            dgvDetails.AutoGenerateColumns = false;
            dgvDetails.Rows.Clear();
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "品名", DataPropertyName = "ProductName" });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "單價", DataPropertyName = "UnitPrice" });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "數量", DataPropertyName = "Quantity" });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "小計", DataPropertyName = "Subtotal" });

            var data = order.Items.Select(i => new
            {
                ProductName = i.Product.Name,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity,
                Subtotal = i.Product.Price * i.Quantity
            }).ToList();
            
            dgvDetails.DataSource = data;

            lblTotal.Text = $"總金額{order.TotalPrice}元";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
