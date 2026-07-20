using System;
using System.Collections.Generic;
using System.Text;

namespace Store_System.Models
{
    internal abstract class Person
    {
        //the Annotations best usecase in Web API's and Entity Framework, in my case the annoations will create the obj even if its not valid
        public string Id { get; protected set; } = Guid.NewGuid().ToString();

        public string FullName { get; protected set; }

        public string Email { get; protected set; }   

        public Person(string fullName, string email)
        {
            //isnullorempty: doesn't check the white spaces
            //isnullorwhitespace: check the white speces, most of the time this we use this one
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new Exception("INVALID NAME: IS_NULL_OR_EMPTY VALIDATION FAILED");
            }

            FullName = fullName;


            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new Exception("INVALID EMAIL: MUST CONTAIN '@'");
            }

            Email = email ;
        }

        public abstract void DisplayInfo();
    }
}
