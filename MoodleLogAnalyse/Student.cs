using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleLogAnalyse
{
    /// <summary>
    /// A basic student class for use in a list.
    /// </summary>
    class Student
    {
        #region private attributes
        uint id;
        string firstname;
        string lastname;
        string username;
        bool active;
        #endregion

        #region constructors
        public Student()
        {
            id = 0;
            firstname = "Firstname";
            lastname = "Lastname";
            username = "me@collegemail.ac.uk";
            active = true;
        }
        /// <summary>
        /// Constructs a student object with the basic details
        /// </summary>
        /// <param name="iD">The numeric Moodle id</param>
        /// <param name="firstName">The students first name</param>
        /// <param name="lastName">The students last name</param>
        /// <param name="userName">The users login name on the Moodle system</param>
        public Student(uint iD, string firstName, string lastName, string userName)
        {
            id = iD;
            firstname = firstName;
            lastname = lastName;
            username = userName;
            active = true;
        }
        #endregion
        #region overides
        public override string ToString()
        {
            return id + ", " + firstname + ", " + lastname + ", " + username + ", " + active.ToString();
        }
        #endregion
    } // End Student
}
