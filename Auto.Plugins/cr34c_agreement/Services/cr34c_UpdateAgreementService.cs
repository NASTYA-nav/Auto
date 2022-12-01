using System;
using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_agreement.Services
{
    /// <summary>
	/// Сервис отвечающий за бизнесс логику плагина PreAgreementUpdate
	/// </summary>
    internal class cr34c_UpdateAgreementService
    {
        private readonly IOrganizationService _service;

        public cr34c_UpdateAgreementService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // Изменяет обьект договора
        public void UpdateAgreement(Entity agreementEntity)
        {
            if (agreementEntity.Contains("cr34c_summa") 
                && agreementEntity.Contains("cr34c_factsumma") 
                && agreementEntity["cr34c_summa"] == agreementEntity["cr34c_factsumma"])
            {
                var agreementToUpdate = new Entity(agreementEntity.LogicalName, agreementEntity.Id);

                // Договор оплачен если сумма договора равна оплаченной сумме
                agreementToUpdate["cr34c_fact"] = true;

                _service.Update(agreementToUpdate);
            }
        }
    }
}
