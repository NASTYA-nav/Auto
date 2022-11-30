using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Plugins.cr34c_communication.Services
{
    public class BaseService
    {
        private readonly IOrganizationService _service;

        public BaseService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void CreateCommunication(Entity entity)
        {
            if (entity.Contains("cr34c_contactid") && entity["cr34c_contactid"] != null)
            {
                OptionSetValue type = (OptionSetValue)entity["cr34c_type"];

                var contactId = (EntityReference)entity["cr34c_contactid"];

                var typeValue = type.Value;

                if (typeValue == (int)CommunicationType.Email && IsMainCommunicationExist(contactId.Id, CommunicationType.Email))
                {
                    throw new InvalidPluginExecutionException("У контакта уже есть основное средство связи E-mail!");
                }
                if (typeValue == (int)CommunicationType.Phone && IsMainCommunicationExist(contactId.Id, CommunicationType.Phone))
                {
                    throw new InvalidPluginExecutionException("У контакта уже есть основное средство связи Телефон!");
                }
            }
        }

        private bool IsMainCommunicationExist(Guid contactId, CommunicationType type)
        {
            var communicationExist = false;

            var query = new QueryExpression("cr34c_communication");

            query.ColumnSet.AddColumns("cr34c_type", "cr34c_contactid", "cr34c_main");

            var filter = new FilterExpression(LogicalOperator.And);

            var isContact = new FilterExpression(LogicalOperator.And);
            isContact.Conditions.Add(new ConditionExpression("cr34c_contactid", ConditionOperator.NotNull));
            isContact.Conditions.Add(new ConditionExpression("cr34c_contactid", ConditionOperator.Equal, contactId));
            filter.AddFilter(isContact);


            if (type.Equals(CommunicationType.Email))
            {
                var isEmailMain = new FilterExpression(LogicalOperator.And);
                isEmailMain.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)CommunicationType.Email));
                isEmailMain.Conditions.Add(new ConditionExpression("cr34c_main", ConditionOperator.Equal, true));

                filter.AddFilter(isEmailMain);

            }
            if (type.Equals(CommunicationType.Phone))
            {
                var isPhoneMain = new FilterExpression(LogicalOperator.And);
                isPhoneMain.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)CommunicationType.Phone));
                isPhoneMain.Conditions.Add(new ConditionExpression("cr34c_main", ConditionOperator.Equal, true));

                filter.AddFilter(isPhoneMain);
            }

            query.Criteria.AddFilter(filter);

            var results = _service.RetrieveMultiple(query);

            if (results.Entities.Count != 0)
            {
                communicationExist = true;
            }

            return communicationExist;
        }
    }
}
