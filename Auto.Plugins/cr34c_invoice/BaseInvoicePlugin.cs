using System;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
    /// Базовый плагин с общей логикой для плагинов PreInvoiceCreate, PreInvoiceDelete, PreInvoiceUpdate
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
