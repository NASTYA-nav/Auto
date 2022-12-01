using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Plugins.cr34c_agreement.Services
{
    /// <summary>
	/// Сервис отвечающий за бизнесс логику плагина PreAgreementCreate
	/// </summary>
    internal class cr34c_CreateAgreementService
    {
        private readonly IOrganizationService _service;

        public cr34c_CreateAgreementService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // При создании договора 
        public void CreateAgreement(Entity agreementEntity, ITracingService ts)
        {
            if (agreementEntity.Contains("cr34c_date") && agreementEntity.Contains("cr34c_contact"))
            {
                ts.Trace("1!");

                var date = (DateTime)agreementEntity["cr34c_date"];
                ts.Trace("2! " + date);

                var contactId = agreementEntity.GetAttributeValue<EntityReference>("cr34c_contact").Id;

                var query = new QueryExpression("cr34c_agreement");

                query.ColumnSet.AddColumns("cr34c_name", "cr34c_contact");

                var filter = new FilterExpression();

                // Выбираем все договоры контакта в создаваемом договоре
                filter.AddCondition("cr34c_contact", ConditionOperator.Equal, contactId);

                query.Criteria.AddFilter(filter);

                var results = _service.RetrieveMultiple(query);

                // Если у контакта еще не было договорров
                if (results.Entities.Count == 0)
                {
                    Entity contactToUpdate = new Entity("contact", contactId);

                    // Ставим дату первого договора контакту
                    contactToUpdate["cr34c_date"] = (DateTime)agreementEntity["cr34c_date"];

                    _service.Update(contactToUpdate);
                }
            }
        }
    }
}
