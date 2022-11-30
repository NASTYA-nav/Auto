using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.IdentityModel.Metadata;
using System.Runtime.Remoting.Contexts;

namespace Auto.Workflows.AgreementBilling.Services
{
    /// <summary>
    /// 
    /// </summary>
    // TODO разделить ответственноссть класса
    public class PaymentPlanService
    {
        private readonly IOrganizationService _service;

        public PaymentPlanService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void CreatePayment(CodeActivityContext context, InArgument<EntityReference> AgrementReference)
        {
            var agrementRef = AgrementReference.Get(context);

            if (CanCreatePayment(agrementRef.Id))
            {
                DeleteAgrementAutoInvoices(agrementRef.Id);

                CreatePaymentPlan(agrementRef.Name, agrementRef.Id);

                SetPaymentPlanDate(agrementRef.Name, agrementRef.Id);
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
            isBilling.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.Equal, agrementRefId));
            isBilling.Conditions.Add(new ConditionExpression("cr34c_fact", ConditionOperator.Equal, true));

            //	Если с договором связан счет с типом = [Вручную]
            var isManualExist = new FilterExpression(LogicalOperator.And);
            isManualExist.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.Equal, agrementRefId));
            isManualExist.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)InvoiceType.Manual));

            filter.AddFilter(isBilling);
            filter.AddFilter(isManualExist);

            query.Criteria.AddFilter(filter);

            var results = _service.RetrieveMultiple(query);

            if (results.Entities.Count != 0)
            {
                // Если есть счета то не создавать счет
                canCreatePayment = false;
            }

            return canCreatePayment;
        }

        private void SetPaymentPlanDate(string agrementRefName, Guid agrementRefId)
        {
            Entity dogovorToUpdate = new Entity(agrementRefName, agrementRefId);

            // Установить на договоре поле [Дата графика платежей] =Текущей датой + 1 день
            dogovorToUpdate["cr34c_paymentplandate"] = DateTime.UtcNow.AddDays(1);

            _service.Update(dogovorToUpdate);
        }

        private void DeleteAgrementAutoInvoices(Guid agrementRefId)
        {
            var query = new QueryExpression("cr34c_invoice");

            query.ColumnSet.AddColumns("cr34c_dogovorid", "cr34c_type");

            var filter = new FilterExpression(LogicalOperator.And);
            filter.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.Equal, agrementRefId));
            filter.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)InvoiceType.Auto));

            var results = _service.RetrieveMultiple(query);

            // Удалить все связанные с договором счета с типом=[Автоматически]
            foreach (var invoice in results.Entities)
            {
                _service.Delete(invoice.LogicalName, invoice.Id);
            }
        }

        private void CreatePaymentPlan(string agrementRefName, Guid agrementRefId)
        {
            var columnSet = new ColumnSet("cr34c_creditperiod", "cr34c_creditamount");

            var agreementFromCrm = _service.Retrieve(agrementRefName, agrementRefId, columnSet);

            int creditPeriod;

            decimal creditAmount;

            if (agreementFromCrm.Contains("cr34c_creditperiod") 
                && agreementFromCrm["cr34c_creditperiod"] != null
                && agreementFromCrm.Contains("cr34c_creditamount")
                && agreementFromCrm["cr34c_creditamount"] != null)
            {
                creditPeriod = agreementFromCrm.GetAttributeValue<int>("cr34c_creditperiod");
                creditAmount = agreementFromCrm.GetAttributeValue<Money>("cr34c_creditamount").Value;
            } else
            {
                throw new Exception("В договоре не указаны Срок кредиита, Сумма кредита!");
            }

            var invoicesCount = creditPeriod * 12;

            // Сумма ежемесячного платежа
            var amount = creditAmount / (creditPeriod * 12);
            var paydate = DateTime.UtcNow;

            Entity invoiceToCreate = new Entity(agrementRefName);

            invoiceToCreate["cr34c_name"] = "Счет на оплату";
            invoiceToCreate["cr34c_date"] = DateTime.UtcNow;
            invoiceToCreate["cr34c_dogovorid"] = agrementRefId;
            invoiceToCreate["cr34c_type"] = (int)InvoiceType.Auto;
            invoiceToCreate["cr34c_amount"] = amount;

            // Создание счетов на каждый месяц всего периода кредита договора
            while (invoicesCount > 0)
            {
                invoiceToCreate["cr34c_paydate"] = paydate;

                _service.Create(invoiceToCreate);

                paydate.AddMonths(1);
                invoicesCount--;
            }
        }
    }
}
