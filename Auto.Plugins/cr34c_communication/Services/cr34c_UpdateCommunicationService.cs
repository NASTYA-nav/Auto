using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_communication.Services
{
    /// <summary>
    /// Сервис отвечающий за бизнесс логику плагина PreCommunicationUpdate
    /// </summary>
    public class cr34c_UpdateCommunicationService : BaseService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="service">Сервис</param>
        public cr34c_UpdateCommunicationService(IOrganizationService service) : base(service)
        {
        }

        /// <summary>
        /// Вызов базового метода для проверки контакта при изменении средства связи
        /// </summary>
        /// <param name="entity">Средство связи</param>
        public void UpdateCommunication(Entity entity)
        {
            CheckCommunication(entity);
        }
    }
}
