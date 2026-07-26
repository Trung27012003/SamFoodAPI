using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Model.DTO
{
    public class InvoiceDTO:Invoice
    {
        public List<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}
