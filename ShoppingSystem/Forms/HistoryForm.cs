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
    public partial class HistoryForm: Form
    {
        private List<Order> orders;
        public HistoryForm(List<Order> orderHistory)
        {
            InitializeComponent();
            this.orders = orderHistory;
            LoadOrderHistory();
        }
        private void LoadOrderHistory()
        {
            if(orders.Count == 0)
            {
                lblEmpty.Visible = true;
                dgvOrders.Visible = false;
                return;
            }

            lblEmpty.Visible = false;
            dgvOrders.Visible = true;

            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.Columns.Clear();

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "訂單編號", DataPropertyName = "OrderId" });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "日期", DataPropertyName = "Date" });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "總金額", DataPropertyName = "TotalPrice" });
            
            dgvOrders.DataSource = orders.Select(o => new
            {
                OrderId = o.OrderId,
                Date = o.Date,
                TotalPrice = o.TotalPrice

            }).ToList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
