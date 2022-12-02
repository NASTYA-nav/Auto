using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// Сервис отвечающий за бизнесс логику плагина PreInvoiceUpdate
	/// </summary>
    public class cr34c_UpdateInvoiceService : BaseService
    {
        public cr34c_UpdateInvoiceService(IOrganizationService service) : base(service)
        {
        }

        // Метод вызываемый при обновлении счета
        public void UpdateInvoice(Entity invoiceEntity, ITracingService ts)
        {
            ChangeInvoice(invoiceEntity, ts);
        }
    }
}
