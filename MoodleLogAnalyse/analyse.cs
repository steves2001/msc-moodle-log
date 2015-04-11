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
        #region private attributes
        private static OdsReaderWriter fileReader = new OdsReaderWriter();  // Generic public domain ODS reader class
        #endregion

        #region public attributes
        public static DataSet moodleData;  // Dataset containing the log data for analysis
        //id, instanceid, contextlevel, course, module, instance, type, description, time, action, url, userid, firstname, lastname, username

        public static SortedList<uint,Student> studentList = new SortedList<uint,Student>();  // A list of all the students in the log.
        public static SortedList<uint, Module> moduleList = new SortedList<uint, Module>();  // A list of all the modules in the log.
        public static SortedList<uint, string> moduleTypeList = new SortedList<uint, string>();  // A list of all the modules types in the log.
        #endregion

        #region data extraction methods
        public static void extractData()
        {
           findStudents();
           findModules();
        }

        /// <summary>
        /// Loop through the data extracting unique students into the sorted studentList data structure
        /// </summary>
        public static void findStudents()
        {
            uint moodleId;  // Moodle unique id for the user

            foreach (DataRow logRow in moodleData.Tables[0].Rows)
            {
                moodleId = uint.Parse(logRow["userid"].ToString());

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
        }  // End findStudents

        /// <summary>
        /// Loop through the data extracting unique modules into the sorted moduleList data structure
        /// </summary>
        public static void findModules()
        {
            uint moodleId;  // Moodle unique id for the module
            uint typeId;  // Moodle unique id for a module type

            foreach (DataRow logRow in moodleData.Tables[0].Rows)
            {
                moodleId = uint.Parse(logRow["instanceid"].ToString());

                if (moduleList.ContainsKey(moodleId))
                {
                    moduleList[moodleId]++; // Increment the module access count.
                }
                else
                {
                    moduleList.Add(moodleId,
                        new Module(moodleId,
                            uint.Parse(logRow["module"].ToString()),
                            logRow["description"].ToString()
                            )
                    );

                    // Populate the list of module types in the course
                    typeId = uint.Parse(logRow["module"].ToString());
                    
                    if (!moduleTypeList.ContainsKey(typeId))
                    {
                        moduleTypeList.Add(typeId, logRow["type"].ToString());
                    }
#if DEBUG           
                    Console.WriteLine(moduleList[moodleId].ToString());  
#endif
                }
            }
        }  // End findModules
        #endregion

        #region data access methods
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

            extractData();
        }
        #endregion
    }
}
