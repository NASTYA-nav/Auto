﻿using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// Сервис отвечающий за бизнесс логику плагина PreInvoiceDelete
	/// </summary>
    public class cr34c_DeleteInvoiceService
    {
        private readonly IOrganizationService _service;

        public cr34c_DeleteInvoiceService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void DeleteInvoice(ITracingService ts, EntityReference invoiceEntity)
        {
            var invoiceFromCrm = _service.Retrieve("cr34c_invoice", invoiceEntity.Id, new ColumnSet("cr34c_dogovorid", "cr34c_fact", "cr34c_amount"));

            var isPayed = invoiceFromCrm.GetAttributeValue<bool>("cr34c_fact");

            // Все изменения с договором в cлчае оплаты счета
            if (isPayed)
            {
                var agreementId = invoiceFromCrm.GetAttributeValue<EntityReference>("cr34c_dogovorid").Id;

                var agrementFromCrm = _service.Retrieve("cr34c_agreement", agreementId, new ColumnSet("cr34c_factsumma", "cr34c_summa", "cr34c_date"));

                var factSumma = agrementFromCrm.GetAttributeValue<Money>("cr34c_factsumma").Value;

                var amount = invoiceFromCrm.GetAttributeValue<Money>("cr34c_amount").Value;

                // Уменьшение оплаченной суммы в договоре
                var resultSumma = factSumma - amount;

                var agreementToUpdate = new Entity(agrementFromCrm.LogicalName, agrementFromCrm.Id);

                agreementToUpdate["cr34c_factsumma"] = resultSumma;

                _service.Update(agreementToUpdate);
            }
        }
    }
}
