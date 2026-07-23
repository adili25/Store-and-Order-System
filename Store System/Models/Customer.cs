using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal class Customer : Person
    {
        public CustomerType CustomerType { get; private set; }

        public DateTime RegistrationDate { get; private set; }

        //base(...) passing the props to the Person class
        public Customer(string name, string email, CustomerType type) : base(name, email)
        {
            RegistrationDate = DateTime.Now;

            CustomerType = type;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine(@$"
==========Customer Information==========
Id: {this.Id}
Full name: {this.FullName}
Email: {this.Email}
Type: {this.CustomerType}
Registration Date: {this.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss")}
========================================
");
        }

        public decimal GetDiscountPercentage() => this.CustomerType == CustomerType.Regular ? 0m : 0.10m;
    }
}
