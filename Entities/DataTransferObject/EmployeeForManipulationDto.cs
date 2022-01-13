using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObject
{
    public abstract class EmployeeForManipulationDto
    {
        [Required(ErrorMessage = "Employee Name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximun Length for the name is 30 characters.")]
        public string Name { get; set; }

        [Range(18, int.MaxValue, ErrorMessage = "Age is required and can't be lower than 18")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Employee Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximun Length for the name is 20 characters.")]
        public string Position { get; set; }
    }
}
