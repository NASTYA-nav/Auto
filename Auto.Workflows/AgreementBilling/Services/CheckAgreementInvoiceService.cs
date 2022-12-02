using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Workflows.AgreementBilling.Services
{
    /// <summary>
    /// Сервис отвечающий за бизнесс процесс CheckAgreementInvoiceActivity
    /// </summary>
    public class CheckAgreementInvoiceService
    {
        /// <summary>
        /// Предоставляет доступ ко основным функциям dynamics
        /// </summary>
        private readonly IOrganizationService _service;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="service">Сервис</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CheckAgreementInvoiceService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Проверяет есть ли у договора счета, если нет то устанавливать исходящий параметр IsContainsInvoice,
        /// в заисимости от значения
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <param name="IsContainsInvoice">Наличие счетов</param>
        /// <param name="AgrementReference">Договор</param>
        public void CheckAndSetAgreement(CodeActivityContext context, OutArgument<bool> IsContainsInvoice, InArgument<EntityReference> AgrementReference)
        {
            var agrementRef = AgrementReference.Get(context);

            var isContainsInvoice = true;

            var query = new QueryExpression("cr34c_invoice");

            query.ColumnSet.AddColumns("cr34c_dogovorid");

            var filter = new FilterExpression();

            filter.AddCondition("cr34c_dogovorid", ConditionOperator.Equal, agrementRef.Id);

            query.Criteria.AddFilter(filter);

            var results = _service.RetrieveMultiple(query);

            if (results.Entities.Count == 0)
            {
                // У данного договора нет счетов
                isContainsInvoice = false;
            }

            IsContainsInvoice.Set(context, isContainsInvoice);
        }
    }
}
