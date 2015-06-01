namespace TrafalgarSquare.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Text { get; set; }

        [Required]
        [ForeignKey("Sender")]
        public string SenderId { get; set; }

        public virtual User Sender { get; set; }

        public DateTime SendDateTime { get; set; }

        [Required]
        public string RecepientId { get; set; }

        public virtual User Recepient { get; set; }

        public bool IsSeen { get; set; }
    }
}
