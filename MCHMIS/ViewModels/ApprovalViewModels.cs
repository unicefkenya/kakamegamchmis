using MCHMIS.Models;

namespace MCHMIS.ViewModels
{
    public class ApprovalViewModel
    {
        public string Id { get; set; }
        public string Notes { get; set; }
        public int StatusId { get; set; }
    }

    public class SendForApprovalViewModel
    {
        public int Count { get; set; }
        public string[] Ids { get; set; }
    }
}