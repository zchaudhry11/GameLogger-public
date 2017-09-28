using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameLoggerIden.Models
{
    public class Phone
    {
        [Key]
        [Column("UserName", Order = 0)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You must provide a phone number!")]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number!")]
        public string PhoneNumber { get; set; }

        public bool ReceiveTexts { get; set; }

        public Phone()
        {
            ReceiveTexts = false;
        }
    }
}