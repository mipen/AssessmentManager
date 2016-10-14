using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using static AssessmentManager.CONSTANTS;

namespace AssessmentManager
{
    public class CourseManager
    {

        private List<Course> courses = new List<Course>();
        private TreeView tree;
        private static CourseManager instance;
        private static string coursesDir = "";

        public CourseManager()
        {
        }

        #region Properties

        public List<Course> Courses => courses;
        public static CourseManager Instance => instance;

        #endregion

        #region Methods

        public void Initialise(TreeView tv, CourseManager instance)
        {
            //Record the instance of itself for access elsewhere
            CourseManager.instance = instance;
            //Record the tree view to be used
            tree = tv;
            string p = Application.StartupPath + "\\" + COURSES_FOLDER_NAME;
            //Create 'Courses' folder if doesn't already exist
            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }
            coursesDir = p;

            //Read all courses and load them into the list.
            LoadAllCourses(p);
            courses.Sort((Course c1, Course c2) => c1.CourseTitle.CompareTo(c2.CourseTitle));

            //Fill the tree view.
            RebuildTreeView();
        }

        private void LoadAllCourses(string coursesPath)
        {
            string[] courseDirs = Directory.GetDirectories(coursesPath);
            if (courseDirs.Count() > 0)
            {
                for (int i = 0; i < courseDirs.Count(); i++)
                {
                    //Get all the files in the course dir
                    string[] files = Directory.GetFiles(courseDirs[i]);
                    string path = null;
                    try
                    {
                        path = (from t in files
                                where Path.GetExtension(t) == COURSE_EXT
                                select t).First();
                    }
                    catch { }
                    if (!path.NullOrEmpty())
                    {

                        Course c = LoadCourse(path);
                        if (c != null)
                        {
                            c.ResetAssessments();
                            courses.Add(c);
                            //Load the assessments for this course
                            string[] assessmentDirs = Directory.GetDirectories(courseDirs[i]);
                            if (assessmentDirs.Count() > 0)
                            {
                                foreach (string a in assessmentDirs)
                                {
                                    string assessmentPath = null;
                                    try
                                    {
                                        assessmentPath = (from t in Directory.GetFiles(a)
                                                          where Path.GetExtension(t) == ASSESSMENT_SESSION_EXT
                                                          select t).First();
                                    }
                                    catch { }
                                    if (!assessmentPath.NullOrEmpty())
                                    {
                                        AssessmentSession session = LoadSession(assessmentPath);
                                        if (session != null)
                                        {
                                            c.Assessments.Add(session);
                                            session.FolderPath = a;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RegisterNewCourse(Course course)
        {
            course.SetIdDirect(RandomCourseID());
            SerialiseCourse(course);
            courses.Add(course);
            RebuildTreeView();
        }

        public void SerialiseCourse(Course course)
        {
            string path = course.GetCoursePath();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                string filePath = course.GetFilePath();
                using (FileStream s = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(s, course);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save course information: \n\n" + ex.Message);
            }
        }

        public bool SerialiseSession(AssessmentSession session, string filePath)
        {
            try
            {
                using (FileStream s = File.Open(@filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(s, session);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save assessment session: \n\n" + ex.Message);
                return false;
            }

            return true;
        }

        public AssessmentSession LoadSession(string path)
        {
            AssessmentSession s = null;
            try
            {
                using (FileStream fs = File.Open(@path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    s = (AssessmentSession)bf.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load assessment session: \n\n" + ex.Message);
            }
            return s;
        }

        public string CreateAssessmentDir(AssessmentSession session, string coursePath)
        {
            try
            {
                string dirName = Path.Combine(@coursePath, RandomAssessmentID(session, @coursePath));
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);
                return dirName;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Course LoadCourse(string path)
        {
            Course c = null;
            try
            {
                using (FileStream s = File.Open(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    c = (Course)bf.Deserialize(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load course information: \n\n" + ex.Message);
            }
            return c;
        }

        public void RebuildTreeView()
        {
            tree.Nodes.Clear();
            if (courses.Count > 0)
            {
                foreach (var c in courses)
                {
                    CourseNode cn = BuildCourseNodeFor(c);
                    tree.Nodes.Add(cn);
                }
                tree.Sort();
            }
        }

        public void RebuildTreeView(string criteria)
        {
            tree.Nodes.Clear();
            if (courses.Count > 0)
            {
                foreach (var c in courses.Where(co => (co.CourseTitle.ToLower().Contains(criteria.ToLower())) || (co.ID.Contains(criteria))))
                {
                    CourseNode cn = BuildCourseNodeFor(c);
                    tree.Nodes.Add(cn);
                }
                tree.Sort();
            }
        }

        public CourseNode BuildCourseNodeFor(Course course)
        {
            CourseNode node = new CourseNode(course);
            if (course.Assessments.Count > 0)
            {
                foreach (var s in course.Assessments)
                {
                    node.Nodes.Add(new AssessmentSessionNode(s));
                }
            }
            return node;
        }

        public void AddAssessmentSession(AssessmentSession session)
        {
            Course c = (from t in Courses
                        where t.ID == session.CourseID
                        select t).First();
            if (c != null)
                c.Assessments.Add(session);
        }

        public string RandomCourseID()
        {
            //Check to make sure that no other course has this same id
            string id = "";
            do
            {
                id = Util.RandomString(6);
            } while (id.NullOrEmpty() || courses.Where(c => c.ID == id).Any());
            return id;
        }

        public string RandomAssessmentID(AssessmentSession session, string coursePath)
        {
            if (Directory.Exists(@coursePath))
            {
                string[] dirs = Directory.GetDirectories(@coursePath);
                if (dirs.Count() > 0)
                {
                    string id = "";
                    do
                    {
                        id = Util.RandomString(6);
                    } while (id == "" || dirs.Where(c => c.EndsWith(id)).Any());

                    return session.AssessmentInfo.AssessmentName + " - " + id;
                }
                else
                {
                    string id = Util.RandomString(6);
                    return session.AssessmentInfo.AssessmentName + " - " + id;
                }
            }
            else
                throw new ArgumentException("Cannot find specified path: \n\n" + coursePath, "coursePath");
        }

        public DialogResult DeleteCourse(Course course)
        {
            //Delete the course's folder
            try
            {
                FileSystem.DeleteDirectory(course.GetCoursePath(), UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
            catch (OperationCanceledException)
            {
                return DialogResult.No;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error removing course with ID: " + course.ID + "\n\n" + e.Message, "Error");
            }
            //Remove the course from the list
            courses.Remove(course);
            return DialogResult.Yes;
        }

        public void DeleteSession(AssessmentSession session)
        {
            try
            {
                //Delete session backup files
                if (Directory.Exists(session.FolderPath))
                    FileSystem.DeleteDirectory(session.FolderPath, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error removing session backup files: \n\n" + e.Message);
            }
            //Delete any deployed files
            if (Directory.Exists(session.DeploymentTarget))
            {
                string[] accountDirs = Directory.GetDirectories(session.DeploymentTarget);
                if (accountDirs.Count() > 0)
                {
                    foreach (var accountDir in accountDirs)
                    {
                        //Delete the files
                        string[] files = Directory.GetFiles(accountDir);
                        if (files.Count() > 0)
                        {
                            foreach (var file in files)
                            {
                                try
                                {
                                    string fileName = Path.GetFileName(file);
                                    string scriptName = session.AssessmentInfo.AssessmentName + ASSESSMENT_SCRIPT_EXT;
                                    if (fileName == scriptName)
                                    {
                                        FileSystem.DeleteFile(file);
                                        continue;
                                    }
                                    else if (session.AdditionalFiles.Count > 0 && session.AdditionalFiles.Contains(fileName))
                                    {
                                        FileSystem.DeleteFile(file);
                                        continue;
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("Error removing session files: \n\n" + e.Message);
                                }
                            }
                        }
                        string[] subDirs = Directory.GetDirectories(accountDir);
                        if (subDirs.Count() > 0)
                        {
                            //Delete the autosaves folder
                            try
                            {
                                string autosavesDir = (from t in subDirs
                                                       where new DirectoryInfo(t).Name == AUTOSAVE_FOLDER_NAME(session.AssessmentInfo.AssessmentName)
                                                       select t).First();
                                if (!autosavesDir.NullOrEmpty())
                                {
                                    if (Directory.Exists(autosavesDir))
                                        FileSystem.DeleteDirectory(autosavesDir, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("Error removing autosaves directory: \n\n " + e.Message);
                            }
                        }
                    }
                }
            }
            Course c = FindCourseByID(session.CourseID);
            if (c != null)
            {
                c.Assessments.Remove(session);
            }
        }

        public string PathForCourse(string ID)
        {
            if (!Courses.Where(c => c.ID == ID).Any())
                throw new ArgumentException($"No course with ID: {ID} registered", "ID");

            string path = Path.Combine(coursesDir, ID);
            if (Directory.Exists(path))
                return path;
            else
                throw new ArgumentException($"Cannot find directory for course with ID: {ID}", "ID");
        }

        public Course FindCourseByID(string ID)
        {
            try
            {
                return Courses.Where(c => c.ID == ID).First();
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
