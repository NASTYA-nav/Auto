using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Remoting.Contexts;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
	/// 
	/// </summary>
    public sealed class PreInvoiceDelete : BaseInvoicePlugin
    {
        public override void ExecuteInternal(IServiceProvider service)
        {
            try
            {
                var target = (EntityReference)PluginExecutionContext.InputParameters["Target"];

                cr34c_DeleteInvoiceService invoiceService = new cr34c_DeleteInvoiceService(OrganizationService);
                invoiceService.DeleteInvoice(target);
            }
            catch (Exception exc)
            {
                TracingService.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
