using System.Text.Json.Serialization;
using ArandaTest.Domain.Enums;

namespace ArandaTest.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public UsersDto? Author { get; set; }
        public bool Status { get; set; }
    }
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        [JsonIgnore]
        public string ImageUrl { get; set; } = string.Empty;
        [JsonIgnore]
        public Guid? CreatedBy { get; set; }
    }

    public class ProducUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public bool Status { get; set; }
    }

    public class ProductFilterDto
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Category { get; set; }
        public SortBy SortBy { get; set; } = SortBy.Name;
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
