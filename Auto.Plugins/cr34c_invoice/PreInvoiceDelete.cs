using System;
using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
    /// Плагин на событие пре-удаления счета
    /// </summary>
    public sealed class PreInvoiceDelete : BaseInvoicePlugin
    {
        public override void ExecuteInternal(IServiceProvider service)
        {
            try
            {
                var target = (EntityReference)PluginExecutionContext.InputParameters["Target"];

                cr34c_DeleteInvoiceService invoiceService = new cr34c_DeleteInvoiceService(OrganizationService);
                invoiceService.DeleteInvoice(TracingService, target);
            }
            catch (Exception exc)
            {
                TracingService.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
