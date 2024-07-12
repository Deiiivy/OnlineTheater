using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Entities
{
    public class RatingMovie : Entity
    {
         [Required]
        public Movie Movie { get; set; }
        
        [Required]
        public Customer Customer { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
