using Auto.Plugins.cr34c_invoice.Enums;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// Сервис отвечающий за бизнесс логику плагина PreInvoiceCreate
	/// </summary>
    public class cr34c_CreateInvoiceService : BaseService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="service">Сервис</param>
        public cr34c_CreateInvoiceService(IOrganizationService service) : base(service)
        {
        }

        /// <summary>
        /// Метод вызываемый при создании счета
        /// </summary>
        /// <param name="tracingService"></param>
        /// <param name="invoiceEntity"></param>
        public void CreateInvoice(ITracingService tracingService, Entity invoiceEntity)
        {
            if (!invoiceEntity.Contains("cr34c_type"))
            {
                // Ручное создание если не установлен типсчета
                invoiceEntity["cr34c_type"] = new OptionSetValue((int)InvoiceType.Manual);
            }
          
            ChangeInvoice(invoiceEntity, tracingService);
        }
    }
}
