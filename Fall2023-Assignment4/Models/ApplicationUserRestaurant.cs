using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Fall2023_Assignment4.Data;
namespace Fall2023_Assignment4.Models

{
    public class ApplicationUserRestaurant
    {
        public string? ApplicationUserId { get; set; }

        public string? RestaurantId { get; set; }

    }
}

