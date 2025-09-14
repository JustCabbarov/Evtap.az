using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvTap.Domain.Enums;

namespace EvTap.Domain.Entities
{
    public class Listing : BaseEntity
    {
      
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public AdvertType AdvertType { get; set; }     // Satılır / Kirayə / Günlük Kirayə

        public int? Rooms { get; set; }          // Ev üçün
        public int? Floor { get; set; }          // Ev / Ofis üçün
        public int? TotalFloors { get; set; }    // Ev / Ofis üçün
        public double? Area { get; set; }        // Sahə (Ev, Torpaq, Ofis)
     
        public RenovationType Renovation { get; set; }   // Təmirli / Təmiriz
        public ListingType CreatorType { get; set; } 
        public bool IsPremium { get; set; }           // Premium elan
        public DateTime? PremiumExpireDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } // Elan tipi (ev, torpaq, ofis)

        public int? AgencyId { get; set; }        
        public Agency? Agency { get; set; }
     
        public int UserId { get; set; }
        public ApplicationUser   User { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public ICollection<ListingImage> Images { get; set; }
        public ICollection<ListingFutures> Features { get; set; }
    }
}
