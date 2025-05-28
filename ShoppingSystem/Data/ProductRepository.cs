using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingSystem.Models;
using System.Data.SqlClient;

namespace ShoppingSystem.Data
{
    internal class ProductRepository
    {
        private string cntStr = @"Data Source= (LocalDB)\MSSQLLocalDB;" +
                @"AttachDBFilename = |DataDirectory|\Database.mdf;";

        public ProductRepository(string cntStr)
        {
            this.cntStr = cntStr;
        }

        public List<Product> GetAll()
        {
            var list = new List<Product>();

            using(SqlConnection conn = new SqlConnection(cntStr))
            {
                conn.Open();
                string sql = "SELECT * FROM Products";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Product
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Price = (int)reader["Price"],
                        Category = reader["Category"].ToString(),
                        ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString()
                    });
                }
            }
            return list;
        }

    }
}
