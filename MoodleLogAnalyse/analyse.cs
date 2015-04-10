using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdsReadWrite;
using System.Data;

namespace MoodleLogAnalyse
{
    class Analyse
    {
        private static OdsReaderWriter fileReader = new OdsReaderWriter();
        public static DataSet moodleData;

        public static void getData(string filename)
        {
           
            moodleData = fileReader.ReadOdsFile(filename);
           
        }
    }
}
