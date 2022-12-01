using Microsoft.Xrm.Sdk;

namespace Auto.Plugins.cr34c_communication.Services
{
    /// <summary>
    /// Сервис отвечающий за бизнесс логику плагина PreCommunicationUpdate
    /// </summary>
    public class cr34c_UpdateCommunicationService : BaseService
    {
        public cr34c_UpdateCommunicationService(IOrganizationService service) : base(service)
        {
        }

        public void UpdateCommunication(Entity entity)
        {
            CheckCommunication(entity);
        }
    }
}
