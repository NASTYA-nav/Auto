using System;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_invoice
{
    /// <summary>
    /// Базовый плагин с общей логикой для плагинов PreInvoiceCreate, PreInvoiceDelete, PreInvoiceUpdate
    /// </summary>
    public abstract class BaseInvoicePlugin : IPlugin
    {
        /// <summary>
        /// Пишет в лог информацию для помощи в деббаге при исключении
        /// </summary>
        public ITracingService TracingService { get; private set; }

        /// <summary>
        /// Предоставляет доступ к контексту выполнения
        /// </summary>
        public IPluginExecutionContext PluginExecutionContext { get; private set; }

        /// <summary>
        /// Предоставляет доступ ко основным функциям dynamics
        /// </summary>
        public IOrganizationService OrganizationService { get; private set; }

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="serviceProvider">Сервис провайдер</param>
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
