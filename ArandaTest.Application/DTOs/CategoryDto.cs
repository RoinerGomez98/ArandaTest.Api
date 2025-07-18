using ArandaTest.Domain.Enums;

namespace ArandaTest.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryFilterDto
    {
        public string? Name { get; set; }
        public SortBy SortBy { get; set; } = SortBy.Name;
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
