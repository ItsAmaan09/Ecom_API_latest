using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
    public class ConfirmOrderResponseDTO
    {
        public int OrderId { get; set; }
        public bool IsConfirmed { get; set; }
        public string Message { get; set; }
    }
}