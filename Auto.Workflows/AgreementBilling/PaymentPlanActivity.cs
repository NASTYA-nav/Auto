using System;
using System.Activities;
using Auto.Workflows.AgreementBilling.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Auto.Workflows.AgreementBilling
{
    public class PaymentPlanActivity : CodeActivity
    {
        [Input("Agrement")]
        [RequiredArgument]
        [ReferenceTarget("cr34c_agreement")]
        public InArgument<EntityReference> AgrementReference { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var tracingService = context.GetExtension<ITracingService>();

            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            var service = serviceFactory.CreateOrganizationService(null);

            try
            {
                service.Delete("cr34c_invoice", Guid.Parse("0a6357f8-816e-ed11-9560-000d3a6939ee"));
                throw new Exception();
                PaymentPlanService paymentService = new PaymentPlanService(service);
                paymentService.CreatePayment(context, AgrementReference);
            }
            catch (Exception exc)
            {
                tracingService.Trace(exc.ToString() + exc.StackTrace);
                throw new InvalidPluginExecutionException(exc.Message + exc.StackTrace);
            }
        }
    }
}
