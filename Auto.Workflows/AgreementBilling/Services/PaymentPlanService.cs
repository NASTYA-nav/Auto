using System;
using System.Activities;
using Auto.Workflows.AgreementBilling.Enums;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Workflows.AgreementBilling.Services
{
    /// <summary>
    /// Сервис отвечающий за бизнесс процесс PaymentPlanActivity
    /// </summary>
    public class PaymentPlanService
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
        public PaymentPlanService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Метод вызываемый бизнесс процессом для создания графика платежей
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <param name="AgrementReference">Договор</param>
        public void CreatePayment(CodeActivityContext context, InArgument<EntityReference> AgrementReference)
        {
            var agrementRef = AgrementReference.Get(context);

            // Если у договора есть оплаченный счет или ручной счет
            if (CanCreatePayment(agrementRef.Id))
            {
                // Удаление автоматически созданных счетов у договора
                DeleteAgrementAutoInvoices(agrementRef.Id);

                // Создание графика платежей
                CreatePaymentPlan(agrementRef.LogicalName, agrementRef.Id);

                // Устанавливаем поле график платежей
                SetPaymentPlanDate(agrementRef.LogicalName, agrementRef.Id);
            }
        }

        /// <summary>
        /// Проверка возможно ли создание графика платежей
        /// </summary>
        /// <param name="agrementRefId">Id договора</param>
        /// <returns></returns>
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

        /// <summary>
        /// Устанавливает дату графика платежей договору
        /// </summary>
        /// <param name="agrementRefName">Логическое навание сущьности договор</param>
        /// <param name="agrementRefId">Id договора</param>
        private void SetPaymentPlanDate(string agrementRefName, Guid agrementRefId)
        {
            Entity agreementToUpdate = new Entity(agrementRefName, agrementRefId);

            // Установить на договоре поле [Дата графика платежей] =Текущей датой + 1 день
            agreementToUpdate["cr34c_paymentplandate"] = DateTime.UtcNow.AddDays(1);

            _service.Update(agreementToUpdate);
        }

        /// <summary>
        /// Удаляет связанные с договором автоматически созданные счета
        /// </summary>
        /// <param name="agrementRefId">Id договора</param>
        private void DeleteAgrementAutoInvoices(Guid agrementRefId)
        {
            var query = new QueryExpression("cr34c_invoice");
            query.ColumnSet.AddColumns("cr34c_dogovorid", "cr34c_type");

            var filter = new FilterExpression(LogicalOperator.And);
            filter.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("cr34c_dogovorid", ConditionOperator.Equal, agrementRefId));
            filter.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)InvoiceType.Auto));

            query.Criteria.AddFilter(filter);

            var results = _service.RetrieveMultiple(query);

            // Удалить все связанные с договором счета с типом=[Автоматически]
            if (results.Entities != null && results.Entities.Count != 0) 
            {
                foreach (var invoice in results.Entities)
                {
                    _service.Delete(invoice.LogicalName, invoice.Id);
                }
            }
        }

        /// <summary>
        /// Создание графика платежей
        /// </summary>
        /// <param name="agrementRefName">Логическое навание сущьности договор</param>
        /// <param name="agrementRefId">Id договора</param>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        private void CreatePaymentPlan(string agrementRefName, Guid agrementRefId)
        {
            var columnSet = new ColumnSet("cr34c_creditperiod", "cr34c_creditamount");

            var agreementFromCrm = _service.Retrieve(agrementRefName, agrementRefId, columnSet);

            int creditPeriod;

            decimal creditAmount;

            if (agreementFromCrm.Contains("cr34c_creditperiod")
                && (int)agreementFromCrm["cr34c_creditperiod"] != 0
                && agreementFromCrm.Contains("cr34c_creditamount")
                && agreementFromCrm.GetAttributeValue<Money>("cr34c_creditamount").Value >= new decimal(0))
            {

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

            Entity invoiceToCreate = new Entity("cr34c_invoice");

            invoiceToCreate["cr34c_name"] = "Счет на оплату";
            invoiceToCreate["cr34c_date"] = DateTime.UtcNow;
            invoiceToCreate["cr34c_dogovorid"] = new EntityReference(agrementRefName, agrementRefId);
            invoiceToCreate["cr34c_type"] = new OptionSetValue((int)InvoiceType.Auto);
            invoiceToCreate["cr34c_amount"] = amount;

            // Создание счетов на каждый месяц всего периода кредита
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
