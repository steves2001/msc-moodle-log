//#define DETAILED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdsReadWrite;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;

namespace MoodleLogAnalyse
{
    static class Analyse 
    {
        #region private attributes
        private static OdsReaderWriter fileAccess = new OdsReaderWriter();  // Generic public domain ODS reader class

        private static bool dataPresent = false;  // Status variable set to indicate there is data to work with.

        #endregion

        #region public attributes

        public static DataSet moodleData;   // Dataset containing the log data for analysis:
                                            // id, instanceid, contextlevel, course, module, 
                                            // instance, type, description, time, action, 
                                            // url, userid, firstname, lastname, username

        public static List<uint> excludedStudents = new List<uint>();  // Students to be excluded from operations
        public static List<Student> studentList = new List<Student>();  // A list of all the students in the log.

        public static SortedList<uint, Module> moduleList = new SortedList<uint, Module>();  // A list of all the modules in the log.
        public static SortedList<uint, ModuleType> moduleTypeList = new SortedList<uint, ModuleType>();  // A list of all the modules types in the log.
        public static List<uint> sortedModuleKeys = new List<uint>(); // A list containing the current sort order for moudule output
        public static int selectedStudentCount { get { return activeStudentCount(); } }  // Returns the number of students who are flagged as active

        #endregion

        #region properties
        public static Boolean DataPresent  // Valid data has been loaded from the moodle log ods file
        {
            get { return dataPresent; }
            set
            {
                if (dataPresent != value)
                {
                    dataPresent = value;
                    NotifyStaticPropertyChanged("DataPresent");
                }
            }           
        }


        /// <summary>
        /// MVVM assistance code creates an event notification that a static property has changed its value to trigger updates in the UI
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged = delegate { };

        private static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region data extraction methods
        /// <summary>
        /// Creates a unique list of students and modules from a moodle log data ods file
        /// </summary>
        public static void extractData()
        {
            findStudents();
            findModules();
            findMaxModuleAccessCount();
        }

        /// <summary>
        /// Makes all studets active
        /// </summary>
        public static void selectAllStudents()
        {
            foreach (Student s in studentList)
                s.active = true;
        }

        /// <summary>
        /// Makes all students inactive
        /// </summary>
        public static void clearAllStudents()
        {
            foreach (Student s in studentList)
                s.active = false;
        }

        /// <summary>
        /// Flips students between active and none active
        /// </summary>
        public static void invertAllStudents()
        {
            foreach (Student s in studentList)
                s.active = !s.active;
        }

        /// <summary>
        /// Filters the list of students based on a grade defined in a filter strinf of comma separated values
        /// </summary>
        /// <param name="gradeString">A string of comma separated values</param>
        public static void selectStudentsOnGrade(string gradeString)
        {
            
            // Strip the grade string of spaces then split into and array of grades
            gradeString = Regex.Replace(gradeString, @"\s+", "");
            string[] grades = gradeString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Loop through the students setting them as active if their grade matches a grade in the grade array
            foreach (Student s in studentList)
                foreach (string grade in grades)
                    if (s.grade.Contains(grade))
                    {
                        s.active = true;
                        break;
                    }
                    else
                        s.active = false;
        }

        /// <summary>
        /// counts the number of students who are flagged as active in the student list
        /// </summary>
        /// <returns>the total amount of students who are active in the list</returns>
        private static int activeStudentCount()
        {
            int count = 0;
            foreach (Student s in studentList)
                if (s.active) count++;
            return count;

        }

        /// <summary>
        /// Builds a list of students who should be excluded from an analysis operation
        /// </summary>
        public static void findExcludedStudents()
        {
            excludedStudents.Clear();

            foreach (Student s in studentList)
            {
                if (!s.active)
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
            if (!DataPresent) return;  // Safety check
            uint moodleId;  // Moodle unique id for the user


            studentList.Clear(); // New

            foreach (DataRow logRow in moodleData.Tables[0].Rows)
            {
                moodleId = uint.Parse(logRow["userid"].ToString());

                // If we did not find the student in the student list add them
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
            if (!DataPresent) return;  // Safety check

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
            sortModulesByAccessCount();
        }  // End findModules

        /// <summary>
        /// Finds the module with the highest access count and returns the count or null if the list is empty
        /// </summary>
        /// <returns>The total accesses of the module with the highest module count or null if there are no modules</returns>
        public static uint? findMaxModuleAccessCount()
        {
            return moduleList.Max(module => module.Value.totalAccesses);
        } // End findMaxModuleAccessCount

        #endregion

        #region data access methods
        /// <summary>
        /// Sorts the module list by total access count
        /// </summary>
        public static void sortModulesByAccessCount()
        {
            List<Module> m = moduleList.Values.ToList<Module>();  // Get the modules to sort

            m.Sort(delegate(Module x, Module y)
            {
                return x.totalAccesses.CompareTo(y.totalAccesses); // Simple because i used uint (no need to check for nulls)
            });

            sortedModuleKeys.Clear();  // Clear the existing list elements

            foreach(Module module in m)
                sortedModuleKeys.Add(module.id);  // Copy the new order to the key list.
        }

        /*          Temp note how to sort a list using anon delegate
                    m.Sort(delegate(Module x, Module y)
                    {
                        if (x.totalAccesses == null && y.totalAccesses == null) return 0;
                        else if (x.totalAccesses == null) return -1;
                        else if (y.totalAccesses == null) return 1;
                        else return x.totalAccesses.CompareTo(y.totalAccesses);
                    });
         */

        /// <summary>
        /// Clears all the data from the class ready for repopulation
        /// </summary>
        public static void clearData()
        {
            try
            {
                moodleData.Clear();
                excludedStudents.Clear();
                studentList.Clear();
                moduleList.Clear();
                moduleTypeList.Clear();
                DataPresent = false;
            }
            catch { }
        }

        /// <summary>
        /// Stores the student data in an excel readable ods file
        /// </summary>
        /// <param name="filename">path and filename to store the data under</param>
        public static void storeStudentData(string filename)
        {
            DataTable studentInfo = new DataTable("StudentInfo");
            DataSet studentData = new DataSet();
            // Set up columns
            Student s = new Student();
            // Build the columns for the data
            studentInfo.Columns.Add("id", typeof(string));
            studentInfo.Columns.Add("firstname", typeof(string));
            studentInfo.Columns.Add("lastname", typeof(string));
            studentInfo.Columns.Add("username", typeof(string));
            studentInfo.Columns.Add("active", typeof(string));
            studentInfo.Columns.Add("grade", typeof(string));

            // Add a header row for readability
            studentInfo.Rows.Add("id", "firstname", "lastname", "username", "active", "grade");

            // Loop through the student list storing the data in the data table
            foreach (Student stu in studentList)
            {
                studentInfo.Rows.Add(stu.id.ToString(), stu.firstname, stu.lastname, stu.username, stu.active.ToString(), stu.grade);
            }

            // Add the table to the dataset
            studentData.Tables.Add(studentInfo);
            // Write the dataset as an ODS file
            fileAccess.WriteOdsFile(studentData, filename);
        }

        /// <summary>
        /// Reads a previously stored ods file containing the student grade information and updates the current list with
        /// the students grade information.
        /// </summary>
        /// <param name="filename">file name and path as a string</param>
        public static void getStudentData(string filename)
        {
            DataSet studentData;

            try
            {
                studentData = fileAccess.ReadOdsFile(filename);  // Open file or fail
            }
            catch { return; }

            // Check the file opened has the right data columns
            if(    studentData.Tables[0].Rows[0][0].ToString() != "id"
                && studentData.Tables[0].Rows[0][1].ToString() != "firstname"
                && studentData.Tables[0].Rows[0][2].ToString() != "lastname"
                && studentData.Tables[0].Rows[0][3].ToString() != "username"
                && studentData.Tables[0].Rows[0][4].ToString() != "active"
                && studentData.Tables[0].Rows[0][5].ToString() != "grade") { return; }

            int col = 0;  // temp column counter 

            // Set the column names
            foreach (DataColumn logColumn in studentData.Tables[0].Columns)
            {
                logColumn.ColumnName = studentData.Tables[0].Rows[0][col++].ToString();
            }

            studentData.Tables[0].Rows[0].Delete();

            uint stuId = 0;
            int stuIndex = -1;

            // Loop through all students in the sheet matching id in datafile with students in the loaded list
            foreach (DataRow stu in studentData.Tables[0].Rows)
            {
                stuId = uint.Parse(stu["id"].ToString());  // Get student ID from the file data
                
                stuIndex = studentList.FindIndex(sId => sId.id == stuId); // Find the list index for that student -1 if not found
                if (stuIndex >= 0)
                {
                    studentList[stuIndex].grade = stu["grade"].ToString();  // Copy the grade from the data file to the student in the list
                }

            }

        }

        /// <summary>
        /// Reads a moodle log datafile in ods format created by a custom report from moodle
        /// </summary>
        /// <param name="filename">file name and path as a string</param>
        public static void getData(string filename)
        {
            try
            {
                moodleData = fileAccess.ReadOdsFile(filename);  // Read the ODS File with the data
            }
            catch { DataPresent = false; }

            if (moodleData == null) { DataPresent = false; return; }

            // Transfer first row of the table to the column names and then remove that row

            int col = 0;  // temp column counter 
            try
            {
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
                if (moodleData.Tables[0].Rows.Count > 0) DataPresent = true; else DataPresent = false;
            }
            catch { DataPresent = false; }
        }
        #endregion
    }
}
