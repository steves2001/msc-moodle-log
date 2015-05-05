//#define DETAILED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdsReadWrite;
using System.Data;
using System.Text.RegularExpressions;

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
        public static List<uint> excludedStudents = new List<uint>();  // Students to be excluded from operations
        public static List<Student> studentList = new List<Student>();  // A list of all the students in the log.
        public static SortedList<uint, Module> moduleList = new SortedList<uint, Module>();  // A list of all the modules in the log.
        public static SortedList<uint, ModuleType> moduleTypeList = new SortedList<uint, ModuleType>();  // A list of all the modules types in the log.
        public static int selectedStudentCount { get { return activeStudentCount(); } } 
        #endregion

        #region data extraction methods
        public static void extractData()
        {
           findStudents();
           findModules();
        }

        public static void selectAllStudents()
        {
            foreach (Student s in studentList)
                s.active = true;
        }

        public static void clearAllStudents()
        {
            foreach (Student s in studentList)
                s.active = false;
        }

        public static void invertAllStudents()
        {
            foreach (Student s in studentList)
                s.active = !s.active;
        }

        public static void selectStudentsOnGrade(string gradeString)
        {

            gradeString = Regex.Replace(gradeString, @"\s+", "");
            string[] grades = gradeString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (Student s in studentList)
                foreach(string grade in grades)
                    if (s.grade.Contains(grade))
                    {
                        s.active = true;
                        break;
                    }
                    else
                        s.active = false;
        }


        private static int activeStudentCount()
        {
            int count = 0;
            foreach (Student s in studentList)
                if (s.active) count++;
            return count;

        }

        public static void findExcludedStudents()
        {
            excludedStudents.Clear();

            foreach(Student s in studentList)
            {
                if(!s.active)
                {
                    excludedStudents.Add(s.id);
                }
            }

        }

        /// <summary>
        /// Loop through the data extracting unique students into the sorted studentList data structure
        /// </summary>
        public static void findStudents()
        {
            uint moodleId;  // Moodle unique id for the user


            studentList.Clear(); // New

            foreach (DataRow logRow in moodleData.Tables[0].Rows)
            {
                moodleId = uint.Parse(logRow["userid"].ToString());


                if (studentList.FindIndex(sId => sId.id == moodleId) < 0)
                {
                    studentList.Add(
                        new Student(moodleId,
                            logRow["firstname"].ToString(),
                            logRow["lastname"].ToString(),
                            logRow["username"].ToString()
                            )
                    );
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

                    // If current module does not exist in the module list create it
                    if (!moduleList.ContainsKey(moodleId))
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
#if DETAILED           
                    Console.WriteLine(moduleList[moodleId].ToString());  
#endif
                }
                // if the current log entry is for a ignored student don't increment the access counts.
                if (excludedStudents.Contains(uint.Parse(logRow["userid"].ToString()))) continue; // If an excluded user skip this access
                
                // only increment the counts if there is an allowed action e.g. add, complete
                if (moduleTypeList[typeId].trackAction(logRow["action"].ToString()))
                {
                    moduleList[moodleId].addStudent(uint.Parse(logRow["userid"].ToString()));
                    moduleList[moodleId]++; // Increment the module access count.
                    moduleTypeList[typeId]++;  // Total Accesses for that type of module
                }
            }
#if DETAILED
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
           
            // Filter out the none student accesses
            int rowCounter = 0;
            List<int> deleteRows = new List<int>();

            foreach (DataRow logRow in moodleData.Tables[0].Rows)
            {
                if (logRow["username"].ToString().IndexOf("@student") <= 0)
                {
                    deleteRows.Add(rowCounter);
                    rowCounter++;
                    continue;
                }
                rowCounter++;
            }

            foreach (int row in deleteRows.OrderByDescending(item => item).ToList())
                moodleData.Tables[0].Rows[row].Delete();

            //extractData();
        }
        #endregion
    }
}
