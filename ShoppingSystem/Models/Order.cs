using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSystem.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int TotalPrice => Items.Sum(i => i.Product.Price * i.Quantity);

    }
}
