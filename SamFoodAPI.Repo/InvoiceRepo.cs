using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class InvoiceRepo:GenericRepo<Invoice>
    {
        public InvoiceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetBillCode()
        {
            string billCode =string.Empty;

            DateTime dateNow = DateTime.Now;
            string prefix = $"DH_{dateNow.ToString("ddMMyy")}_";

            var bills = GetAll(x => x.IsDeleted != true
                                    && x.BillDate.Value.Year == dateNow.Year
                                    && x.BillDate.Value.Month == dateNow.Month
                                    && x.BillDate.Value.Day == dateNow.Day)
                        .Select(x => new
                        {
                            x.ID,
                            x.BillCode,
                            Stt = string.IsNullOrWhiteSpace(x.BillCode) ? 1 : Convert.ToInt32(x.BillCode.Replace(prefix, "")),
                        }).ToList();

            int stt = bills.Count() <= 0 ? 1 : bills.Max(x => x.Stt);

            stt += 1;

            billCode = $"{prefix}{stt}";

            return billCode;
        }

        public InvoiceStatsDTO GetStats()
        {
            return SqlDapper<InvoiceStatsDTO>
                .ProcedureToModelAsync("spGetInvoiceStats", null)
                .GetAwaiter()
                .GetResult();
        }
    }
}
