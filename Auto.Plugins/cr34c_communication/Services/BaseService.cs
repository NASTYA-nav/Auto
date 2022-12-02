using System;
using Auto.Plugins.cr34c_communication.Enums;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Auto.Plugins.cr34c_communication.Services
{
    /// <summary>
	/// Базовый сервис с общей логикой проверки на наличие у контакта основных средств связи
	/// </summary>
    public class BaseService
    {
        /// <summary>
        /// Предоставляет доступ ко основным функциям dynamics
        /// </summary>
        private readonly IOrganizationService _service;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BaseService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Проверяет контакт на наличие основного средства связи
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        public void CheckCommunication(Entity entity)
        {
            if (entity.Contains("cr34c_contactid") && entity["cr34c_contactid"] != null
                && entity.Contains("cr34c_type") && entity.Contains("cr34c_main"))
            {
                var type = (OptionSetValue)entity["cr34c_type"];

                var contactId = (EntityReference)entity["cr34c_contactid"];

                var typeValue = type.Value;

                // Если тип связи почта и основная почта уже есть то ошибка
                if (typeValue == (int)CommunicationType.Email && (bool)entity["cr34c_main"] == true && IsMainCommunicationExist(contactId.Id, CommunicationType.Email))
                {
                    throw new InvalidPluginExecutionException("У контакта уже есть основное средство связи E-mail!");
                }

                // Если тип связи телефон и основной телефон уже есть то ошибка
                if (typeValue == (int)CommunicationType.Phone && (bool)entity["cr34c_main"] == true && IsMainCommunicationExist(contactId.Id, CommunicationType.Phone))
                {
                    throw new InvalidPluginExecutionException("У контакта уже есть основное средство связи Телефон!");
                }
            }
        }

        /// <summary>
        /// Проверяет есть ли уже у контакта данный тип средства связи как основной
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsMainCommunicationExist(Guid contactId, CommunicationType type)
        {
            var communicationExist = false;

            var query = new QueryExpression("cr34c_communication");

            query.ColumnSet.AddColumns("cr34c_name");

            var filter = new FilterExpression(LogicalOperator.And);

            // Средства связи данного контакта
            var isContact = new FilterExpression(LogicalOperator.And);
            isContact.Conditions.Add(new ConditionExpression("cr34c_contactid", ConditionOperator.NotNull));
            isContact.Conditions.Add(new ConditionExpression("cr34c_contactid", ConditionOperator.Equal, contactId));
            filter.AddFilter(isContact);

            // Средства связи контакта и 
            if (type.Equals(CommunicationType.Email))
            {
                // Если почта выбираем основную
                var isEmailMain = new FilterExpression(LogicalOperator.And);
                isEmailMain.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)CommunicationType.Email));
                isEmailMain.Conditions.Add(new ConditionExpression("cr34c_main", ConditionOperator.Equal, true));

                filter.AddFilter(isEmailMain);

            }
            if (type.Equals(CommunicationType.Phone))
            {
                // Если телефон выбираем оновной
                var isPhoneMain = new FilterExpression(LogicalOperator.And);
                isPhoneMain.Conditions.Add(new ConditionExpression("cr34c_type", ConditionOperator.Equal, (int)CommunicationType.Phone));
                isPhoneMain.Conditions.Add(new ConditionExpression("cr34c_main", ConditionOperator.Equal, true));

                filter.AddFilter(isPhoneMain);
            }

            query.Criteria.AddFilter(filter);

            var results = _service.RetrieveMultiple(query);

            if (results.Entities.Count != 0)
            {
                //  Если есть элементы, то основное средство связи есть
                communicationExist = true;
            }

            return communicationExist;
        }
    }
}
