using Auto.App.Entities;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;

namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// 
	/// </summary>
    public class BaseService
    {
        private readonly IOrganizationService _service;

        public BaseService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void ChangeInvoice(Entity invoiceEntity, ITracingService ts)
        {
            var isPayed = invoiceEntity.GetAttributeValue<bool>("cr34c_fact");

            if (isPayed)
            {
                invoiceEntity["cr34c_paydate"] = DateTime.UtcNow;

                ChangeFactSumma(invoiceEntity, ts);
            }
        }

        private void ChangeFactSumma(Entity invoiceEntity, ITracingService ts)
        {
            Guid agrementId;
            decimal amount;

            if (!invoiceEntity.Contains("cr34c_dogovorid"))
            {
                var invoiceFromCrm = _service.Retrieve("cr34c_invoice", invoiceEntity.Id, new ColumnSet("cr34c_amount", "cr34c_dogovorid"));

                agrementId = invoiceFromCrm.GetAttributeValue<EntityReference>("cr34c_dogovorid").Id;
                amount = invoiceFromCrm.GetAttributeValue<Money>("cr34c_amount").Value;
            } else
            {
                agrementId = invoiceEntity.GetAttributeValue<EntityReference>("cr34c_dogovorid").Id;
                amount = invoiceEntity.GetAttributeValue<Money>("cr34c_amount").Value;
            }

            var agreementFromCrm = _service.Retrieve("cr34c_agreement", agrementId, new ColumnSet(
                App.Entities.cr34c_agreement.Fields.cr34c_factsumma, 
                App.Entities.cr34c_agreement.Fields.cr34c_summa, 
                App.Entities.cr34c_agreement.Fields.cr34c_date));

            var agreementToUpdate = new Entity(agreementFromCrm.LogicalName, agreementFromCrm.Id);

            decimal factSumma = agreementFromCrm.Attributes.Contains("cr34c_factsumma")
                ? agreementFromCrm.GetAttributeValue<Money>("cr34c_factsumma").Value
                : new Money().Value = new decimal(0.0);

            // Увеличивать оплаченную сумму договора
            var resultSumma = factSumma + amount;

            decimal maxSumma = agreementFromCrm.Attributes.Contains("cr34c_summa")
                ? agreementFromCrm.GetAttributeValue<Money>("cr34c_summa").Value
                : new Money().Value = new decimal(0.0);
            ts.Trace("maxSumma" + maxSumma);
            ts.Trace("resultSumma" + resultSumma);

            if (maxSumma <= resultSumma)
            {
                throw new InvalidPluginExecutionException("Сумма договора превышена! Счет не может быть сохранен.");
            }

            agreementToUpdate["cr34c_factsumma"] = resultSumma;

            _service.Update(agreementToUpdate);
        }
    }
}
