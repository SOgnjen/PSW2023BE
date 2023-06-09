using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalLibrary.Core.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Emails { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public UserRole Role { get; set; }

        public string Address { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public int Jmbg { get; set; }

        public Gender Gender { get; set; }

        public List<int> BloodPressure { get; set; }

        public List<int> BloodSugar { get; set; }

        public List<int> BodyFat { get; set; }

        public List<int> Weight { get; set; }
    }
}
