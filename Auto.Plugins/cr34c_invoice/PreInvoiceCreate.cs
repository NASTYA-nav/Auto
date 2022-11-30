using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Remoting.Contexts;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
	/// 
	/// </summary>
    public sealed class PreInvoiceCreate : BaseInvoicePlugin
    {
        public override void ExecuteInternal(IServiceProvider service)
        {
            try
            {
                var target = (Entity)PluginExecutionContext.InputParameters["Target"];

                cr34c_CreateInvoiceService invoiceService = new cr34c_CreateInvoiceService(OrganizationService);
                invoiceService.CreateInvoice(TracingService, target);
            }
            catch (Exception exc)
            {
                TracingService.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
