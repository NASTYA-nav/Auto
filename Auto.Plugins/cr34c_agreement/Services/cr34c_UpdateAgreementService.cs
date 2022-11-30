using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using Auto.App.Entities;
using System.IdentityModel.Metadata;


namespace Auto.Plugins.cr34c_agreement.Services
{
    /// <summary>
	/// 
	/// </summary>
    internal class cr34c_UpdateAgreementService
    {
        private readonly IOrganizationService _service;

        public cr34c_UpdateAgreementService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void UpdateAgreement(Entity agreementEntity)
        {
            if (agreementEntity.Contains("cr34c_summa") 
                && agreementEntity.Contains("cr34c_factsumma") 
                && agreementEntity["cr34c_summa"] == agreementEntity["cr34c_factsumma"])
            {
                var dogovorToUpdate = new Entity(agreementEntity.LogicalName, agreementEntity.Id);

                // Договор оплачен если сумма договора равна оплаченной сумме
                dogovorToUpdate["cr34c_fact"] = true;

                _service.Update(dogovorToUpdate);
            }
        }
    }
}
