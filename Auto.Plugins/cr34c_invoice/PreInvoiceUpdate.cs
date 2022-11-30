using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Remoting.Contexts;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
	/// 
	/// </summary>
    public sealed class PreInvoiceUpdate : BaseInvoicePlugin
    {
        public override void ExecuteInternal(IServiceProvider service)
        {
            try
            {
                var target = (Entity)PluginExecutionContext.InputParameters["Target"];
                
                var isPayed = target.GetAttributeValue<bool>("cr34c_fact");


                if (isPayed)
                {
                    var invoiceEntity = new Entity(target.LogicalName, target.Id);

                    invoiceEntity["cr34c_paydate"] = DateTime.UtcNow;

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
