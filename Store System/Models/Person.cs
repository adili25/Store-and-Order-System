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
                throw new ArgumentException("INVALID NAME: IS_NULL_OR_EMPTY VALIDATION FAILED", nameof(fullName));
            }
            FullName = fullName;

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("INVALID EMAIL: IS_NULLOR_EMPTY VALIDATION FAILED", nameof(email));
            }
            if (!email.Contains('@'))
            {
                throw new ArgumentException("INVALID EMAIL: MUST CONTAIN '@'", nameof(email));
            }
            Email = email ;
        
        }

        public abstract void DisplayInfo();
    }
}
