namespace MCHMIS.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }

    public class Reason
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
    }

    public class ApprovalStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NotesCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Notes : CreateFields
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public NotesCategory Category { get; set; }

        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }
    }
}