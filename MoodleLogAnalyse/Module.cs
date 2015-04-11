﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleLogAnalyse
{
    class Module
    {
        #region private attributes
        uint id;
        uint type;
        string name;
        uint totalAccesses;
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

        #region operator overloading
        public static Module operator ++(Module m)
        {
            m.totalAccesses++; // Increment the access count.
            return m;
        }

        public static Module operator --(Module m)
        {
            if(m.totalAccesses > 0) m.totalAccesses--; // Decrement the access count.
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
