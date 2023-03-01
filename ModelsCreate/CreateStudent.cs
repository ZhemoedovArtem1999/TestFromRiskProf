using test.Models;

namespace test.ModelsCreate
{
    public class CreateStudent
    {
        public List<Organization> Organizations { get; set; }
        public List<Employee> Employees { get; set; }

        public string Teacher { get; set; }
        public string Group { get; set; }

        public int IdOrganiz { get; set; }
        public int IdEmployee { get; set; }

        public int IdGroup { get; set; }

    }
}
