using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSystem.Models
{
    internal class Orders
    {
        public int Id { get; set; }
        public string OrderDate { get; set; }
        public int TotalPrice {  get; set; }
        public string UserName { get; set; }
    }
}
