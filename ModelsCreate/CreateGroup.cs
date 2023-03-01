using test.Models;

namespace test.ModelsCreate
{
    public class CreateGroup
    {
        public GroupModel group { get; set; }
        public IEnumerable<TeacherModel> teachers { get; set; }
    }
}
