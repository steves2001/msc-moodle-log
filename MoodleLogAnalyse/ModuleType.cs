using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleLogAnalyse
{
    class ModuleType
    {
        public uint id { get; set; }
        public uint totalAccesses { get; set; }
        public string name { get; set; }

        public ModuleType()
        {
            id = 0;
            totalAccesses = 0;
            name = "Blank";

        }

        public ModuleType(uint moduleId, string moduleName)
        {
            id = moduleId;
            totalAccesses = 0;
            name = moduleName;

        }

        #region operator overloading
        public static ModuleType operator ++(ModuleType m)
        {
            m.totalAccesses++; // Increment the access count.
            return m;
        }

        public static ModuleType operator --(ModuleType m)
        {
            if (m.totalAccesses > 0) m.totalAccesses--; // Decrement the access count.
            return m;
        }
        #endregion

        #region overides
        //public override string ToString()
        //{
        //    return name;
        //}
        #endregion

    }
}
