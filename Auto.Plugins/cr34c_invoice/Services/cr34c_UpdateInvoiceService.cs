using Microsoft.Xrm.Sdk;
using System;


namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// 
	/// </summary>
    public class cr34c_UpdateInvoiceService : BaseService
    {
        public cr34c_UpdateInvoiceService(IOrganizationService service) : base(service)
        {
        }

        public void UpdateInvoice(Entity invoiceEntity, ITracingService ts)
        {
            var isPayed = invoiceEntity.GetAttributeValue<bool>("cr34c_fact");

            if (isPayed)
            {
                invoiceEntity["cr34c_paydate"] = DateTime.UtcNow;

                ChangeInvoice(invoiceEntity, ts);
            }
        }
    }
}
