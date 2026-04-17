using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SimpleStore.Application.Dto.Product
{
    public record ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public string CategoryName { get; set; }
    }
}
