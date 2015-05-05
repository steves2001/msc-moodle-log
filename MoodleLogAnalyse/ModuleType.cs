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
        public Dictionary<string,bool> allowedActions ;
        public ModuleType()
        {
            id = 0;
            totalAccesses = 0;
            name = "Blank";
            allowedActions = new Dictionary<string, bool>() { { "submit", true }, { "update", true }, { "view", true } };
        }

        public bool trackAction(string action)
        {
            if(allowedActions.ContainsKey(action))
                return allowedActions[action];

            return false;
        }

        public ModuleType(uint moduleId, string moduleName)
        {
            id = moduleId;
            totalAccesses = 0;
            name = moduleName;
            switch(moduleName)
            {
                case "assign":
                    allowedActions = new Dictionary<string, bool>() { { "submit", true }, { "view", false }, { "add", false } };
                    break;
                case "checklist":
                    allowedActions = new Dictionary<string, bool>() { { "complete", true }, { "update checks", false }, { "view", false }, { "add", false } };
                    break;
                case "page":
                    allowedActions = new Dictionary<string, bool>() { { "submit", false }, { "view", true }, { "add", false } };
                    break;
                case "resource":
                    allowedActions = new Dictionary<string, bool>() { { "submit", false }, { "view", true }, { "add", false } };
                    break;
                case "url":
                    allowedActions = new Dictionary<string, bool>() { { "submit", false }, { "view", true }, { "add", false } };
                    break;
                case "workshop":
                    allowedActions = new Dictionary<string, bool>() { { "add assessment", true }, { "view", false } };
                    break;
                default:
                    allowedActions = new Dictionary<string, bool>() { { "submit", true }, { "update", true }, { "view", true } };
                    break;
            }

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
