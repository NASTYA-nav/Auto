using System;
using Auto.Plugins.cr34c_communication.Services;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_communication
{
    /// <summary>
    /// Плагин на событие пре-создания Средства связи
    /// </summary>
    public sealed class PreCommunicationCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Пишет в лог информацию для помощи в деббаге при исключении
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

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
                tracingService.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
