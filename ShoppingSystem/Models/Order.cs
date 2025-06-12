using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int TotalPrice { get; set; }

    }
}
