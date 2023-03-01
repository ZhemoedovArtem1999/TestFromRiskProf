using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using test.Models;

//using System.Data.SqlClient;
using Npgsql;
using test.Models;
using test.ModelsView;
using System.Data;
using test.ModelsCreate;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string connectionString = "Host=localhost;Port=5432;Database=test;Username=postgres;Password=postgres";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
        

            StudyGroupModelView studyGroupModelView = new StudyGroupModelView();
            studyGroupModelView.getStudyGroup();
            return View(studyGroupModelView.getStudyGroup()); ;
        }

        public IActionResult CreateGroup()
        {
            CreateGroup createGroup = new CreateGroup();
            List<TeacherModel> teachers = new List<TeacherModel>();

            string sqlExpression = "select * from TeacherViewFun()";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

            string sqlFunction = "GroupViewFun";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.Text;
                //NpgsqlParameter res = new NpgsqlParameter("table",.)

                //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    //Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

                    while (reader.Read())
                    {
                        TeacherModel teacherModel = new TeacherModel();
                        teacherModel.ID = reader.GetInt32(0);
                        teacherModel.FIO = reader.GetString(1);

                        teachers.Add(teacherModel);
                        //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                    }
                }
                reader.Close();
            }


            createGroup.teachers = teachers;


            return View(createGroup); ;


            
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupModel group)
        {
            string sqlExpression = "select * from TeacherViewFun()";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

            string sqlProc = "CreateGroup";
            int id = -1;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(sqlProc, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                int groupId = -1;
                //NpgsqlParameter res = new NpgsqlParameter("table",.)
                command.Parameters.AddWithValue("id_group1", groupId);
                command.Parameters.AddWithValue("title1", group.Title);
                command.Parameters.AddWithValue("id_teacher", group.Teacher.ID);

                //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    //Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

                    while (reader.Read())
                    {
                       
                        id = reader.GetInt32(0);
                        //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                    }
                }
                reader.Close();
            }


            
            // db.Users.Add(user);
            //await db.SaveChangesAsync();
            return RedirectToAction("EditGroup", new { id = id });
        }


        public async Task<IActionResult> EditGroup(int? id)
        {
            if (id != null)
            {
                EditViewGroup editViewGroup = new EditViewGroup();
                StudyGroupModelView? group = new StudyGroupModelView();

                string sqlExpression = "select * from TeacherViewFun()";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

                string sqlProc = "GroupView";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand(sqlProc, connection);
                    // указываем, что команда представляет хранимую процедуру
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    int groupId = -1;
                    //NpgsqlParameter res = new NpgsqlParameter("table",.)
                    command.Parameters.AddWithValue("title1", "");
                    command.Parameters.AddWithValue("teacher", "");
                    command.Parameters.AddWithValue("id1", id);

                    //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        //Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

                        while (reader.Read())
                        {
                            group.Id = id;
                            group.Title = reader.GetString(0);
                            group.Teacher = reader.GetString(1);
                            //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                        }
                    }
                    reader.Close();
                }

                editViewGroup.Study = group;
               
                sqlExpression = $"select * from StudentsFun({id})";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

                List<Student> students = new List<Student>();

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
                    // указываем, что команда представляет хранимую процедуру
                    command.CommandType = System.Data.CommandType.Text;
                    int groupId = -1;
                    //NpgsqlParameter res = new NpgsqlParameter("table",.)
               ;

                    //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        //Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

                        while (reader.Read())
                        {
                            Student student = new Student();
                            student.Id = reader.GetInt32(2);
                            student.FIO = reader.GetString(0);
                            student.Organization = reader.GetString(1);

                            students.Add(student);
                            //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                        }
                    }
                    reader.Close();
                }

                editViewGroup.Students = students;




                return View(editViewGroup);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditGroup(EditViewGroup group, int id)
        {
            string sqlProc = "updGroup";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(sqlProc, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                int groupId = -1;
                //NpgsqlParameter res = new NpgsqlParameter("table",.)
                command.Parameters.AddWithValue("id_group1", id);
                command.Parameters.AddWithValue("title", group.Study.Title);
               

                //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                var reader = command.ExecuteReader();

                
                reader.Close();
            }

            // db.Users.Update(user);
            //await db.SaveChangesAsync();
            return RedirectToAction("EditGroup", new {id = id});
        }

        public IActionResult AddStudent(int? id)
        {
            if (id!=null)
            {
                CreateStudent createStudent = new CreateStudent();
                string sqlExpression = $"select * from OrganizFun({id})";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

            

                List<Organization> organizations = new List<Organization>();

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
                    // указываем, что команда представляет хранимую процедуру
                    command.CommandType = System.Data.CommandType.Text;
                    int groupId = -1;
                    //NpgsqlParameter res = new NpgsqlParameter("table",.)
                    ;
                    command.Parameters.AddWithValue("id_group1", id);

                    //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        //Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));



                        while (reader.Read())
                        {
                            Organization organization = new Organization();
                            organization.Id = reader.GetInt32(0);
                            organization.Title = reader.GetString(1);
                            createStudent.Teacher = reader.GetString(3);
                            createStudent.Group = reader.GetString(2);
                            createStudent.IdGroup = reader.GetInt32(4);

                            organizations.Add(organization);
                            //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                        }
                        
                    }
                    reader.Close();
                }

                createStudent.Organizations = organizations;
               

               // "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";



              


                
                return View(createStudent);
            }
            return NotFound();
        }


        
        public ActionResult GetEmployee(int id, int id_group)
        {
            string sqlExpression = $"select * from EmployeeFun({id},{id_group})";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

            CreateStudent createStudent = new CreateStudent();

            List<Employee> employees = new List<Employee>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.Text;
                int groupId = -1;
                //NpgsqlParameter res = new NpgsqlParameter("table",.)
                ;
                command.Parameters.AddWithValue("id1", id);
                command.Parameters.AddWithValue("id_group", id_group);

                //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {



                    while (reader.Read())
                    {
                        Employee employee = new Employee();
                        employee.Id = reader.GetInt32(0);
                        employee.Id_Organiz = reader.GetInt32(1);
                        employee.FIO = reader.GetString(2);


                        employees.Add(employee);
                        //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                    }
                }
                reader.Close();
                createStudent.Employees = employees;
            }

            return PartialView(createStudent);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(CreateStudent student, int id)
        {
            string sqlExpression = "select * from TeacherViewFun()";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

            string sqlProc = "addStudent";
            
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(sqlProc, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                int groupId = -1;
                //NpgsqlParameter res = new NpgsqlParameter("table",.)
                command.Parameters.AddWithValue("id_group1", id);
                command.Parameters.AddWithValue("id_employee", student.IdEmployee);
 

                //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                var reader = command.ExecuteReader();

             
                reader.Close();
            }
            return RedirectToAction("EditGroup", new {id=id});
        }


       
        public async Task<IActionResult> delStudent(int id)
        {
            string sqlExpression = "select * from TeacherViewFun()";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

            string sqlProc = "delStudent";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand(sqlProc, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                
                //NpgsqlParameter res = new NpgsqlParameter("table",.)
                command.Parameters.AddWithValue("id_stud", id);
                

                //SqlParameter resParam = new SqlParameter(,SqlDbType.)
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }

                reader.Close();
            }
            return RedirectToAction("EditGroup", new { id = id });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}