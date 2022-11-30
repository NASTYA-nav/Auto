using Auto.Plugins.cr34c_communication.Services;
using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;
using System;

namespace Auto.Plugins.cr34c_communication
{
    /// <summary>
	/// 
	/// </summary>
    public sealed class PreCommunicationCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var ts = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                cr34c_CreateCommunicationService invoiceService = new cr34c_CreateCommunicationService(service);
                invoiceService.CreateCommunication((Entity)context.InputParameters["Target"]);
            }
            catch (Exception exc)
            {
                ts.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
