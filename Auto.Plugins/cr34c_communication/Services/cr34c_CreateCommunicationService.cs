﻿using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_communication.Services
{
    /// <summary>
	/// Сервис отвечающий за бизнесс логику плагина PreCommunicationCreate
	/// </summary>
    internal class cr34c_CreateCommunicationService : BaseService
    {
        public cr34c_CreateCommunicationService(IOrganizationService service) : base(service)
        {
        }

        // Вызов базового метода для проверки контакта при создании средства связи
        public void CreateCommunication(Entity entity)
        {
            CheckCommunication(entity);
        }
    }
}
