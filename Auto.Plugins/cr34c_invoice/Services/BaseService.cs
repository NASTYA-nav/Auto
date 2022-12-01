using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// Базовый сервис с общей логикой для изменения счета
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
            // Проставляется дата оплаты текущая
            invoiceEntity["cr34c_paydate"] = DateTime.UtcNow;

            var query = new QueryExpression("cr34c_invoice");

            query.ColumnSet.AddColumns("cr34c_amount", "cr34c_dogovorid", "cr34c_fact");

            var filter = new FilterExpression();

            filter.AddCondition("cr34c_invoiceid", ConditionOperator.Equal, invoiceEntity.Id);

            query.Criteria.AddFilter(filter);

            var invoiceFromCrm = _service.RetrieveMultiple(query);

            if (invoiceEntity.Contains("cr34c_dogovorid")
                || (invoiceFromCrm.Entities != null && invoiceFromCrm.Entities.Count != 0))
            {
                
                    // Изменяется оплаченная сумма в договоре если указан договор
                    ChangeFactSumma(invoiceEntity, invoiceFromCrm.Entities, ts);
                
            }
        }

        private void ChangeFactSumma(Entity invoiceEntity, DataCollection<Entity> invoiceFromCrm, ITracingService ts)
        {
            Guid agrementId;
            decimal amount;

            // Если поля не заданы брать из сущьности в бд
            if (invoiceEntity.Contains("cr34c_dogovorid"))
            {
                agrementId = invoiceEntity.GetAttributeValue<EntityReference>("cr34c_dogovorid").Id;
            } else
            {
                agrementId = invoiceFromCrm.Count != 0 
                    ? invoiceFromCrm[0].GetAttributeValue<EntityReference>("cr34c_dogovorid").Id 
                    : throw new InvalidPluginExecutionException("Отсутствует Договор!");
            }

            if (invoiceEntity.Contains("cr34c_amount"))
            {
                amount = invoiceEntity.GetAttributeValue<Money>("cr34c_amount").Value;
            } else
            {
                amount = invoiceFromCrm.Count != 0 
                    ? invoiceFromCrm[0].GetAttributeValue<Money>("cr34c_amount").Value
                    : new decimal(0);
            }

            var agreementFromCrm = _service.Retrieve("cr34c_agreement", agrementId, new ColumnSet(
                App.Entities.cr34c_agreement.Fields.cr34c_factsumma, 
                App.Entities.cr34c_agreement.Fields.cr34c_summa, 
                App.Entities.cr34c_agreement.Fields.cr34c_date));

            var agreementToUpdate = new Entity(agreementFromCrm.LogicalName, agreementFromCrm.Id);

            decimal factSumma = agreementFromCrm.Attributes.Contains("cr34c_factsumma")
                ? agreementFromCrm.GetAttributeValue<Money>("cr34c_factsumma").Value
                : new Money().Value = new decimal(0.0);

            // Увеличивать оплаченную сумму договора суммой этого опаченного счета
            var resultSumma = factSumma + amount;

            decimal maxSumma = agreementFromCrm.Attributes.Contains("cr34c_summa")
                ? agreementFromCrm.GetAttributeValue<Money>("cr34c_summa").Value
                : new Money().Value = new decimal(0.0);

            // Сообщение об ошибке, если сумма оплаты по факту превышает максимально допустимую в договоре если она есть
            if (agreementFromCrm.Attributes.Contains("cr34c_summa") && maxSumma <= resultSumma)
            {
                throw new InvalidPluginExecutionException("Сумма договора превышена! Счет не может быть сохранен.");
            }

            agreementToUpdate["cr34c_factsumma"] = resultSumma;

            _service.Update(agreementToUpdate);
        }
    }
}
