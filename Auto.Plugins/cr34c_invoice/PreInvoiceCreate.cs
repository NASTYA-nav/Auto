using System;
using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
    /// Плагин на событие пре-создания счета
    /// </summary>
    public sealed class PreInvoiceCreate : BaseInvoicePlugin
    {
        /// <summary>
        /// Логика создания счета
        /// </summary>
        /// <param name="service">Сервис</param>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        public override void ExecuteInternal(IServiceProvider service)
        {
            try
            {
                var target = (Entity)PluginExecutionContext.InputParameters["Target"];

                var isPayed = target.GetAttributeValue<bool>("cr34c_fact");

                // Все изменения с договором и есго счетом только в лчае оплаты счета
                if (isPayed)
                {
                    cr34c_CreateInvoiceService invoiceService = new cr34c_CreateInvoiceService(OrganizationService);
                    invoiceService.CreateInvoice(TracingService, target);
                }
            }
            catch (Exception exc)
            {
                TracingService.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
