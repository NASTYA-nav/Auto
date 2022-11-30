using Auto.Workflows.AgreementBilling.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace Auto.Workflows.AgreementBilling
{
    public class CheckAgreementInvoiceActivity : CodeActivity
    {
        [Input("Agrement")]
        [RequiredArgument]
        [ReferenceTarget("cr34c_agreement")]
        public InArgument<EntityReference> AgrementReference { get; set; }

        [Output("Is agreement has invoice")]
        public OutArgument<bool> IsContainsInvoice { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var tracingService = context.GetExtension<ITracingService>();

            var wfContext = context.GetExtension<IWorkflowContext>();

            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            var service = serviceFactory.CreateOrganizationService(null);

            try
            {
                CheckAgreementInvoiceService agreementService = new CheckAgreementInvoiceService(service);
                agreementService.CheckAgreement(context, IsContainsInvoice, AgrementReference);
            }
            catch (Exception exc)
            {
                tracingService.Trace(exc.ToString());
                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
