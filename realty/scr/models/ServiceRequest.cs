namespace ServiceDesk.Api.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = null!;
        public int EquipmentId { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string Priority { get; set; } = null!;
        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? AssignedTo { get; set; }

        public string? Comment { get; set; }
    }

}
