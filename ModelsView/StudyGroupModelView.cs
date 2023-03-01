using Npgsql;

namespace test.ModelsView
{
    public class StudyGroupModelView
    {
        public int? Id { get; set; }
        public string Title { get; set; }

        public string Teacher { get; set; }

        public int Count { get; set; }

        private string connectionString = "Host=localhost;Port=5432;Database=test;Username=postgres;Password=postgres";
        public List<StudyGroupModelView> getStudyGroup()
        {
            List<StudyGroupModelView> studyGroups = new List<StudyGroupModelView>();

            string sqlExpression = "select * from groupviewfun()";// "select distinct S.\"Title\",T.\"FIO\",count(\"ID_employee\") over (partition by st.\"ID_group\") from \"StudyGroup\" S left join \"Teachers\" T on T.\"ID_teacher\" = S.\"ID_teacher\" left join \"Students\" st on st.\"ID_group\" = s.\"ID_group\";";

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
                    Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

                    while (reader.Read())
                    {
                        StudyGroupModelView studyGroup = new StudyGroupModelView();
                        studyGroup.Id = reader.GetInt32(0);
                        studyGroup.Title = reader.GetString(1);
                        studyGroup.Teacher = reader.GetString(2);
                        studyGroup.Count = reader.GetInt32(3);

                        studyGroups.Add(studyGroup);
                        //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                    }
                }
                reader.Close();
            }

            return studyGroups;
        }

    }
}
