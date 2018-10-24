using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage="Input your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Input street")]
        [Display(Name="Street")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Input your city")]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Input your country")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        public bool GiftWrap { get; set; }
    }
}
