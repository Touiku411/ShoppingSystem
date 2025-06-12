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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ShoppingSystem.Forms
{
    public partial class HistoryForm: Form
    {
        private List<Order> orders = new List<Order>();
        public HistoryForm(string userName)
        {
            InitializeComponent();
            LoadOrderHistory(userName);
            DisplayOrders();
        }
        private void LoadOrderHistory(string userName)
        {
            string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = C:\Users\tengy\source\repos\ShoppingSystem\ShoppingSystem\Database.mdf;
                Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                // 先取 Orders 資料
                string sqlOrder = "SELECT Id, OrderDate FROM Orders WHERE UserName = @userName ORDER BY OrderDate DESC";
                using (SqlCommand cmd = new SqlCommand(sqlOrder, conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                Items = new List<CartItem>()
                            });
                        }
                    }
                }
                //對每筆訂單，載入對應的 OrderItems 和 Products
                foreach (var order in orders)
                {
                    string sqlItems = @"SELECT O.ProductId, O.Quantity, O.UnitPrice, P.Name, P.Category, P.ImagePath
                                    FROM OrderItems O JOIN Products P ON O.ProductId = P.Id
                                    WHERE O.OrderId = @orderId";
                    using (SqlCommand cmd = new SqlCommand(sqlItems, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderId", order.Id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new Product
                                {
                                    Id = (int)reader["ProductId"],
                                    Name = reader["Name"].ToString(),
                                    Price = (int)reader["UnitPrice"],
                                    Category = reader["Category"].ToString(),
                                    ImagePath = reader["ImagePath"].ToString()
                                };
                                order.Items.Add(new CartItem
                                {
                                    Product = product,
                                    Quantity = (int)reader["Quantity"]
                                });
                            }
                        }
                    }
                    order.TotalPrice = order.Items.Sum(i => i.Product.Price * i.Quantity);
                }
            }
        }
        private void DisplayOrders()
        {

            if (orders.Count == 0)
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
                OrderId = o.Id,
                Date = o.OrderDate,
                TotalPrice = o.TotalPrice

            }).ToList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                int selectedRow = (int)dgvOrders.Rows[e.RowIndex].Cells[0].Value;
                var selectedOrder = orders.FirstOrDefault(o=>o.Id == selectedRow);

                if(selectedOrder != null)
                {
                    OrderDetailsForm f = new OrderDetailsForm(selectedOrder);
                    f.ShowDialog();
                }
            }
        }
    }
}
