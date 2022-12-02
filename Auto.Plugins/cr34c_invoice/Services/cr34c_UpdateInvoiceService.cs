using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// Сервис отвечающий за бизнесс логику плагина PreInvoiceUpdate
	/// </summary>
    public class cr34c_UpdateInvoiceService : BaseService
    {
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="service">Сервис</param>
        public cr34c_UpdateInvoiceService(IOrganizationService service) : base(service)
        {
        }

        /// <summary>
        /// Метод вызываемый при обновлении счета
        /// </summary>
        /// <param name="invoiceEntity">Счет</param>
        public void UpdateInvoice(Entity invoiceEntity)
        {
            ChangeInvoice(invoiceEntity);
        }
    }
}
