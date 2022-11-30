using Auto.Plugins.cr34c_invoice.Serviseces;
using Microsoft.Xrm.Sdk;
using System;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
	/// 
	/// </summary>
    public abstract class BaseInvoicePlugin : IPlugin
    {
        public ITracingService TracingService { get; private set; }

        public IPluginExecutionContext PluginExecutionContext { get; set; }

        public IOrganizationService OrganizationService { get; set; }

        public void Execute(IServiceProvider serviceProvider)
        {
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            OrganizationService = serviceFactory.CreateOrganizationService(PluginExecutionContext.UserId);

            ExecuteInternal(serviceProvider);
        }
        public abstract void ExecuteInternal(IServiceProvider service);
    }
}
