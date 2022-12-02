﻿using System;
using System.Activities;
using Auto.Workflows.AgreementBilling.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Auto.Workflows.AgreementBilling
{
    /// <summary>
    /// Бизнесс процесс на проверку связки счетов с договором
    /// </summary>
    public class CheckAgreementInvoiceActivity : CodeActivity
    {
        // Входной параметр Договор
        [Input("Agrement")]
        [RequiredArgument]
        [ReferenceTarget("cr34c_agreement")]
        public InArgument<EntityReference> AgrementReference { get; set; }

        // Исходщий параметр факт наличия у договора счетов
        [Output("Is agreement has invoice")]
        public OutArgument<bool> IsContainsInvoice { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Пишет в лог информацию для помощи в деббаге при исключении
            var tracingService = context.GetExtension<ITracingService>();

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
