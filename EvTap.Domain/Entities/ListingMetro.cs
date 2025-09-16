using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvTap.Domain.Entities
{
    public class ListingMetro : BaseEntity
    {
        public int ListingId { get; set; }
        public Listing Listing { get; set; }

        public int MetroStationId { get; set; }
        public MetroStation MetroStation { get; set; }
    }
}
