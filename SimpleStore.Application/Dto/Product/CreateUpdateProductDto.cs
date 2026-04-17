using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SimpleStore.Application.Dto.Product
{
    public class CreateUpdateProductDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
    }
}
