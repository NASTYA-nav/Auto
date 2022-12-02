using System;
using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
    /// Плагин на событие пре-обновления чета
    /// </summary>
    public sealed class PreInvoiceUpdate : BaseInvoicePlugin
    {
        public override void ExecuteInternal(IServiceProvider service)
        {
            try
            {
                var target = (Entity)PluginExecutionContext.InputParameters["Target"];

                var invoiceFromCrm = OrganizationService.Retrieve("cr34c_invoice", target.Id, new ColumnSet("cr34c_fact"));

                // Берем из crm факт оплаты, если нет в контексте
                var isPayed = target.Contains("cr34c_dogovorid")
                    ? target.GetAttributeValue<bool>("cr34c_fact")
                    : invoiceFromCrm.GetAttributeValue<bool>("cr34c_fact");

                // Все изменения с договором и есго счетом только в лчае оплаты счета
                if (isPayed)
                {
                    cr34c_UpdateInvoiceService invoiceService = new cr34c_UpdateInvoiceService(OrganizationService);
                    invoiceService.UpdateInvoice(target, TracingService);
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
