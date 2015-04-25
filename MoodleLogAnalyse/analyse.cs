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
        public static SortedList<uint, ModuleType> moduleTypeList = new SortedList<uint, ModuleType>();  // A list of all the modules types in the log.
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

            studentList.Clear();

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

            moduleList.Clear();

            foreach (DataRow logRow in moodleData.Tables[0].Rows)
            {
                moodleId = uint.Parse(logRow["instanceid"].ToString());
                typeId = uint.Parse(logRow["module"].ToString());

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
                    
                    
                    if (!moduleTypeList.ContainsKey(typeId))
                    // Type did not exist add it to the list.
                    {
                        moduleTypeList.Add(typeId, new ModuleType(typeId, logRow["type"].ToString()));
                        
                    }
#if DEBUG           
                    Console.WriteLine(moduleList[moodleId].ToString());  
#endif
                }
                moduleTypeList[typeId]++;  // Total Accesses for that type of module
            }
#if DEBUG
            Console.WriteLine("Highest Access Count = " + findMaxModuleAccessCount());
#endif
        }  // End findModules
        /// <summary>
        /// Finds the module with the highest access count and returns the count or null if the list is empty
        /// </summary>
        /// <returns>The total accesses of the module with the highest module count or null if there are no modules</returns>
        public static uint? findMaxModuleAccessCount()
        {
            return  moduleList.Max(module => module.Value.totalAccesses);
        } // End findMaxModuleAccessCount
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
