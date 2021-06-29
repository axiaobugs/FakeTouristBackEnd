using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XiechengAPI.Moldes;

namespace XiechengAPI.Dtos
{
    public class ShoppingCartDto
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ICollection<LineItemDto> ShoppingCartItems { get; set; }
    }
}
