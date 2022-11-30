using Auto.App.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Auto.Plugins.cr34c_invoice.Serviseces
{
    /// <summary>
	/// 
	/// </summary>
    public class cr34c_DeleteInvoiceService
    {
        private readonly IOrganizationService _service;

        public cr34c_DeleteInvoiceService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void DeleteInvoice(EntityReference invoiceEntity)
        {
            var invoiceFromCrm = _service.Retrieve("cr34c_invoice", invoiceEntity.Id, new ColumnSet(App.Entities.cr34c_invoice.Fields.cr34c_dogovorid));

            var dogovorId = invoiceFromCrm.GetAttributeValue<EntityReference>("cr34c_dogovorid").Id;

            var agrementFromCrm = _service.Retrieve("cr34c_agreement", dogovorId, new ColumnSet(
                App.Entities.cr34c_agreement.Fields.cr34c_factsumma, 
                App.Entities.cr34c_agreement.Fields.cr34c_summa, 
                App.Entities.cr34c_agreement.Fields.cr34c_date));
            
            decimal factSumma = agrementFromCrm.Attributes.Contains("cr34c_factsumma")
                ? agrementFromCrm.GetAttributeValue<Money>("cr34c_factsumma").Value
                : new Money().Value = new decimal(0.0);

            decimal amount = agrementFromCrm.Attributes.Contains("cr34c_factsumma") ?
                invoiceFromCrm.GetAttributeValue<Money>("cr34c_amount").Value
                : new Money().Value = new decimal(0.0);

            // Уменьшение оплаченной суммы в договоре
            var resultSumma = factSumma - amount;

            var dogovorToUpdate = new Entity(agrementFromCrm.LogicalName, agrementFromCrm.Id);

            dogovorToUpdate["cr34c_factsumma"] = resultSumma;

            _service.Update(dogovorToUpdate);


        }
    }
}
