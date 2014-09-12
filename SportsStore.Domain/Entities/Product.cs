using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities
{
    public class Product
    {
        [HiddenInput(DisplayValue = false)]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please input a name")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please input the price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price.")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Please input the category")]
        public string Category { get; set; }
    }
}
