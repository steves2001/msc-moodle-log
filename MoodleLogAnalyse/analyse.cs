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
        private static OdsReaderWriter fileReader = new OdsReaderWriter();  // Generic public domain ODS reader class

        public static DataSet moodleData;  // Dataset containing the log data for analysis

        public static SortedList<uint,Student> studentList = new SortedList<uint,Student>();  // A list of all the students in the log.

        public static void getData(string filename)
        {
           
            moodleData = fileReader.ReadOdsFile(filename);  // Read the ODS File with the data

            // Transfer first row of the table to the column names and then remove that row

            int col = 0;  // temp column counter 

            foreach (DataColumn logColumn in moodleData.Tables[0].Columns)
            {
                logColumn.ColumnName = moodleData.Tables[0].Rows[0][col++].ToString();
            }

            moodleData.Tables[0].Rows[0].Delete();

            // Loop through the data extracting unique students into the sorted studentList data structure

            uint moodleId;  // Moodle unique id for the user

            foreach(DataRow logRow in moodleData.Tables[0].Rows)
            { 
                moodleId = uint.Parse( logRow["userid"].ToString());

                if (!studentList.ContainsKey(moodleId))
                {
                    studentList.Add(moodleId,
                        new Student(moodleId,
                            logRow["firstname"].ToString(),
                            logRow["lastname"].ToString(),
                            logRow["username"].ToString()
                            )
                    );
#if DEBUG                    
                    Console.WriteLine(studentList[moodleId].ToString());
#endif
                }
            }

            
        }
    }
}
