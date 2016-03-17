using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleLogAnalyse
{
    class Section
    {
        public uint id { get; set; }// Moodle section id
        public string name { get; set; } // The name of the section
        public List<uint> modules = new List<uint>(); // List of modules in the section in display sequence
        public Section(uint sectionId, string sectionName, string sectionModules)
        {
            id = sectionId;
            name = sectionName;

            string [] moduleStrings = sectionModules.Split(',');
            foreach(string module in moduleStrings)
            {
                modules.Add(uint.Parse(module));
            }
        }
    }

}
