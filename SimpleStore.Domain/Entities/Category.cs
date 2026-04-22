using System.ComponentModel.DataAnnotations;

namespace SimpleStore.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
