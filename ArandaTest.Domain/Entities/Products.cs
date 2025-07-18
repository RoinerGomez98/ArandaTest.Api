namespace ArandaTest.Domain.Entities
{
    public class Products
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool Status { get; set; } = true;
        public virtual Guid? CategoryId { get; set; } = null!;
        public virtual Guid? CreatedBy { get; set; } = null!;
        public virtual Category? Category { get; set; } = null!;
        public virtual Users? Author { get; set; } = null!;
    }
}
