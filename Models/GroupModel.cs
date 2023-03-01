namespace test.Models
{
    public class GroupModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public TeacherModel Teacher { get; set; }
    }
}
