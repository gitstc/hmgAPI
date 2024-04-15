using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace hmgAPI.Entities
{
    public class AppMerchant
    {
        [Key]
        public int MerchantId { get; set; }
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CoRegistrationNo { get; set; }


        //merchant can have many users
        //navigation prop to config relationship
        public ICollection<AppUser> Users { get; set; }


    }
}