using System;
using Auto.Plugins.cr34c_agreement.Services;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_agreement
{
    /// <summary>
	/// Плагин на событие пре-обновления Договора
	/// </summary>
    public sealed class PreAgreementUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var ts = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                cr34c_UpdateAgreementService invoiceService = new cr34c_UpdateAgreementService(service);
                invoiceService.UpdateAgreement((Entity)context.InputParameters["Target"]);
            }
            catch (Exception exc)
            {
                ts.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
