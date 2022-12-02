﻿using System;
using System.Activities;
using Auto.Workflows.AgreementBilling.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Auto.Workflows.AgreementBilling
{
    /// <summary>
    /// Бизнесс процесс на создание графика платежей
    /// </summary>
    public class PaymentPlanActivity : CodeActivity
    {
        // Входящий параметр Договор
        [Input("Agrement")]
        [RequiredArgument]
        [ReferenceTarget("cr34c_agreement")]
        public InArgument<EntityReference> AgrementReference { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Пишет в лог информацию для помощи в деббаге при исключении
            var tracingService = context.GetExtension<ITracingService>();

            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            var service = serviceFactory.CreateOrganizationService(null);

            try
            {
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
