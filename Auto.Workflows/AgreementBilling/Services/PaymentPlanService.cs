using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Workflows.AgreementBilling.Services
{
    /// <summary>
    /// Сервис отвечающий за бизнесс процесс PaymentPlanActivity
    /// </summary>
    public class PaymentPlanService
    {
        private readonly IOrganizationService _service;

        public PaymentPlanService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void CreatePayment(CodeActivityContext context, InArgument<EntityReference> AgrementReference, ITracingService ts)
        {

            var agrementRef = AgrementReference.Get(context);

            // Если у договора есть оплаченный счет или ручной счет
            if (CanCreatePayment(agrementRef.Id))
            {
                // Удаление автоматически созданных счетов у договора
                DeleteAgrementAutoInvoices(agrementRef.Id, ts);

                // Создание графика платежей
                CreatePaymentPlan(agrementRef.LogicalName, agrementRef.Id, ts);

                // Устанавливаем поле график платежей
                SetPaymentPlanDate(agrementRef.LogicalName, agrementRef.Id);
            }
        }

        private bool CanCreatePayment(Guid agrementRefId)
        {
            var canCreatePayment = true;

            var query = new QueryExpression("cr34c_invoice");

            query.ColumnSet.AddColumns("cr34c_dogovorid", "cr34c_fact", "cr34c_type");

            var filter = new FilterExpression(LogicalOperator.Or);

            // Если с договором связан любой счет со статусом оплачено
            var isBilling = new FilterExpression(LogicalOperator.And);
            isBilling.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.NotNull));
            isBilling.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.Equal, agrementRefId));
            isBilling.Conditions.Add(new ConditionExpression("cr34c_fact", ConditionOperator.Equal, true));

            //	ИЛИ если с договором связан счет с типом = [Вручную]
            var isManualExist = new FilterExpression(LogicalOperator.And);
            isBilling.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.NotNull));
            isManualExist.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.Equal, agrementRefId));
            isManualExist.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)InvoiceType.Manual));

            filter.AddFilter(isBilling);
            filter.AddFilter(isManualExist);

            query.Criteria.AddFilter(filter);

            var results = _service.RetrieveMultiple(query);

            if (results.Entities.Count != 0)
            {
                // Если есть элементы то не создавать счета
                canCreatePayment = false;
            }

            return canCreatePayment;
        }

        private void SetPaymentPlanDate(string agrementRefName, Guid agrementRefId)
        {
            Entity agreementToUpdate = new Entity(agrementRefName, agrementRefId);

            // Установить на договоре поле [Дата графика платежей] =Текущей датой + 1 день
            agreementToUpdate["cr34c_paymentplandate"] = DateTime.UtcNow.AddDays(1);

            _service.Update(agreementToUpdate);
        }

        private void DeleteAgrementAutoInvoices(Guid agrementRefId, ITracingService ts)
        {
            var query = new QueryExpression("cr34c_invoice");
            ts.Trace("query!!!!!!!!!");
            query.ColumnSet.AddColumns("cr34c_dogovorid", "cr34c_type");

            var filter = new FilterExpression(LogicalOperator.And);
            filter.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.Equal, agrementRefId));
            filter.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)InvoiceType.Auto));

            query.Criteria.AddFilter(filter);

            var results = _service.RetrieveMultiple(query);
            ts.Trace("results1!!!" + results.Entities.Count);

            // Удалить все связанные с договором счета с типом=[Автоматически]
            if (results.Entities != null && results.Entities.Count != 0) 
            {
                ts.Trace("Entities");

                foreach (var invoice in results.Entities)
                {
                    ts.Trace("invoice" + invoice.LogicalName + invoice.Id);

                    _service.Delete(invoice.LogicalName, invoice.Id);
                }
            }
        }

        private void CreatePaymentPlan(string agrementRefName, Guid agrementRefId, ITracingService ts)
        {
            var columnSet = new ColumnSet("cr34c_creditperiod", "cr34c_creditamount");
            ts.Trace("Entities");

            var agreementFromCrm = _service.Retrieve(agrementRefName, agrementRefId, columnSet);
            ts.Trace("Entities");

            int creditPeriod;

            decimal creditAmount;

            if (agreementFromCrm.Contains("cr34c_creditperiod")
                && (int)agreementFromCrm["cr34c_creditperiod"] != 0
                && agreementFromCrm.Contains("cr34c_creditamount")
                && agreementFromCrm.GetAttributeValue<Money>("cr34c_creditamount").Value >= new decimal(0))
            {
                ts.Trace("Entities");

                creditPeriod = agreementFromCrm.GetAttributeValue<int>("cr34c_creditperiod");
                creditAmount = agreementFromCrm.GetAttributeValue<Money>("cr34c_creditamount").Value;
            } else
            {
                throw new InvalidPluginExecutionException("Проверьте введены ли данные в полях Срок кредиита и Сумма кредита!");
            }

            var invoicesCount = creditPeriod * 12;

            // Сумма ежемесячного платежа
            var amount = creditAmount / (creditPeriod * 12);
            var paydate = DateTime.UtcNow;
            ts.Trace("Entities" + amount);

            Entity invoiceToCreate = new Entity("cr34c_invoice");

            invoiceToCreate["cr34c_name"] = "Счет на оплату";
            invoiceToCreate["cr34c_date"] = DateTime.UtcNow;
            invoiceToCreate["cr34c_dogovorid"] = new EntityReference(agrementRefName, agrementRefId);
            invoiceToCreate["cr34c_type"] = new OptionSetValue((int)InvoiceType.Auto);
            invoiceToCreate["cr34c_amount"] = amount;
            ts.Trace("Entities" + invoicesCount);

            // Создание счетов на каждый месяц всего периода кредита договора
            while (invoicesCount > 0)
            {
                ts.Trace("Entities" + invoicesCount);

                invoiceToCreate["cr34c_paydate"] = paydate;
                ts.Trace("Entities" + invoicesCount);

                _service.Create(invoiceToCreate);
                ts.Trace("Entities" + invoicesCount);

                paydate.AddMonths(1);
                invoicesCount--;
            }
        }
    }
}
