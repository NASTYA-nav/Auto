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
        // Предоставляет доступ ко основным функциям dynamics
        private readonly IOrganizationService _service;

        public CheckAgreementInvoiceService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // Метод для проверки есть ли связанные с договором счета
        public void CheckAgreement(CodeActivityContext context, OutArgument<bool> IsContainsInvoice, InArgument<EntityReference> AgrementReference)
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
