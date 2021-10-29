using SQLBulk.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBulk.UnitTests.Entities
{
    [Table("customer")]
    public class Customer
    {
        [PrimaryKey(false)]
        public string FirstName { get; set; }
        [PrimaryKey(false)]
        public string LastName { get; set; }
        [PrimaryKey(false)]
        public DateTime BirthDate { get; set; }
        public int NumberOfComplaints { get; set; }
        public decimal MoneySpent { get; set; }
        public char Type { get; set; }
        public char Gender { get; set; }

        public static List<Customer> GetCustomers(int numberOfRecords)
        {
            var customers = new List<Customer>();
            for (int i = 0; i < numberOfRecords; i++)
            {
                customers.Add(new Customer()
                {
                    BirthDate = DateTime.Now.AddYears(-10),
                    FirstName = string.Join(' ', "Henk", IntToString(i)),
                    Gender = 'M',
                    LastName = "World",
                    MoneySpent = 12M,
                    NumberOfComplaints = 10,
                    Type = 'F',
                });
            }
            return customers;
        }

        private static string IntToString(int i)
        {
            string s = i.ToString();
            while(s.Length != 20)
            {
                s = "0" + s;
            }
            return s;
        }
    }
}
