﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using E_shop_backend.Models;

namespace E_shop_backend.Dtos
{
    public class ReqProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Seasons { get; set; }
        public decimal Price { get; set; }
        public int RatingId { get; set; }
        public int StudioId { get; set; }
        public ICollection<int> ProductGenres { get; set; } = null!;
    }
}
