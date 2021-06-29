using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XiechengAPI.Dtos
{
    public class LineItemDto
    {

        public int Id { get; set; }
        public Guid TouristRouteId { get; set; }
        public TouristRouteDto TouristRoute { get; set; }
        public Guid? ShoppingCartId { get; set; }
        // public Guid? OrderId { get; set; }
        public decimal OriPrice { get; set; }
        public double? DiscountPresent { get; set; }
    }
}
