using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBScript
{
    public class CreateOrderResponseDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsCreated { get; set; }
    }
}