using System;
using Auto.Plugins.cr34c_agreement.Services;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_agreement
{
    /// <summary>
	/// Плагин на событие пре-создания Договора
	/// </summary>
    public sealed class PreAgreementCreate : IPlugin
    {
        /// <summary>
        /// Логика создания договора
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        public void Execute(IServiceProvider serviceProvider)
        {
            // Пишет в лог информацию для помощи в деббаге при исключении
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                cr34c_CreateAgreementService invoiceService = new cr34c_CreateAgreementService(service);
                invoiceService.CreateAgreement((Entity)context.InputParameters["Target"]);
            }
            catch (Exception exc)
            {
                tracingService.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
