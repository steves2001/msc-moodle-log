using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleLogAnalyse
{
    /// <summary>
    /// A basic Module class for the storage of unique instances of the modules in the course.
    /// </summary>
    class Module
    {

        #region private attributes
        public uint id { get; set; }// Moodle module id
        public uint type { get; set; } // The moodle module type id
        public string name { get; set; } // The name of the module
        public uint totalAccesses { get; set; } // total accesses for this module
        public uint uniqueAccesses { get { return (uint)uniqueStudents.Count(); } }
        
        private List<uint> uniqueStudents = new List<uint>(); // List of unique students id that accessed the module
        #endregion

        #region constructors

        public Module()
        {
            id = 0;
            type = 0;
            name = "Module Name";
            totalAccesses = 0;
        }

        public Module(uint iD, uint moduleType, string moduleName)
        {
            id = iD;
            type = moduleType;
            name = moduleName;
            totalAccesses = 0;
        }

        #endregion

        #region public methods
        public void addStudent(uint studentId)
        {
            if (!uniqueStudents.Contains(studentId)) uniqueStudents.Add(studentId);
        }

        #endregion

        #region operator overloading
        public static Module operator ++(Module m)
        {
            m.totalAccesses++; // Increment the access count.
            return m;
        }

        public static Module operator --(Module m)
        {
            if (m.totalAccesses > 0) m.totalAccesses--; // Decrement the access count.
            return m;
        }
        #endregion

        #region overides
        public override string ToString()
        {
            return id + ", " + type + ", " + name + ", " + totalAccesses;
        }
        #endregion


    }
}
