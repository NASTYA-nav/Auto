using System;
using Auto.Plugins.cr34c_communication.Services;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_communication
{
    /// <summary>
	/// Плагин на событие пре-обновления Средства связи
	/// </summary>
    public sealed class PreCommunicationUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var ts = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                cr34c_UpdateCommunicationService invoiceService = new cr34c_UpdateCommunicationService(service);
                invoiceService.UpdateCommunication((Entity)context.InputParameters["Target"]);
            }
            catch (Exception exc)
            {
                ts.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
