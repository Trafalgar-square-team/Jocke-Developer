namespace TrafalgarSquare.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string MachineName { get; set; }

        public bool IsDisabled { get; set; }
    }
}
