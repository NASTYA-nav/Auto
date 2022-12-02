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
        // Предоставляет доступ ко основным функциям dynamics
        private readonly IOrganizationService _service;

        public BaseService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // Базовый метод для изменения счета
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

            // Если указан договор в счете или есть сущьность в бд
            if (invoiceEntity.Contains("cr34c_dogovorid")
                || (invoiceFromCrm.Entities != null && invoiceFromCrm.Entities.Count != 0))
            {
                ChangeFactSumma(invoiceEntity, invoiceFromCrm.Entities, ts);
            }
        }

        // Изменяется оплаченная сумма в договоре
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
                // Если договор не указан в сущьности из бд то ошибка
                agrementId = invoiceFromCrm.Count != 0 
                    ? invoiceFromCrm[0].GetAttributeValue<EntityReference>("cr34c_dogovorid").Id 
                    : throw new InvalidPluginExecutionException("Отсутствует Договор!");
            }

            // Если поля не заданы брать из сущьности в бд
            if (invoiceEntity.Contains("cr34c_amount"))
            {
                amount = invoiceEntity.GetAttributeValue<Money>("cr34c_amount").Value;
            } else
            {
                // Если сумма не указан в сущьности из бд то ошибка
                amount = invoiceFromCrm.Count != 0 
                    ? invoiceFromCrm[0].GetAttributeValue<Money>("cr34c_amount").Value
                    : new decimal(0);
            }

            var agreementFromCrm = _service.Retrieve("cr34c_agreement", agrementId, new ColumnSet("cr34c_factsumma", "cr34c_summa", "cr34c_date"));

            var agreementToUpdate = new Entity(agreementFromCrm.LogicalName, agreementFromCrm.Id);

            // Оплаченная сумма
            decimal factSumma = agreementFromCrm.Attributes.Contains("cr34c_factsumma")
                ? agreementFromCrm.GetAttributeValue<Money>("cr34c_factsumma").Value
                : new Money().Value = new decimal(0.0);

            // Увеличивать оплаченную сумму договора суммой данного опаченного счета
            var resultSumma = factSumma + amount;

            // Сумма договора
            decimal maxSumma = agreementFromCrm.Attributes.Contains("cr34c_summa")
                ? agreementFromCrm.GetAttributeValue<Money>("cr34c_summa").Value
                : new Money().Value = new decimal(0.0);

            // Сообщение об ошибке, если сумма оплаты по факту превышает максимально допустимую в договоре если она есть
            if (agreementFromCrm.Attributes.Contains("cr34c_summa") && maxSumma <= resultSumma)
            {
                throw new InvalidPluginExecutionException("Сумма договора превышена! Счет не может быть сохранен.");
            }

            // Изменение оплаченной суммы договора
            agreementToUpdate["cr34c_factsumma"] = resultSumma;

            _service.Update(agreementToUpdate);
        }
    }
}
