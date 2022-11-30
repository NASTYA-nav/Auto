using Auto.App.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace Auto.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "AuthType=Office365;Url=https://org350db84c.crm4.dynamics.com/;username=november11822@november11822trialnastya.onmicrosoft.com;password=Gladison69.;";
            CrmServiceClient client = new CrmServiceClient(connectionString);

            if(client.LastCrmException != null)
            {
                Console.WriteLine(client.LastCrmException);
            }

            var servise = (IOrganizationService)client;
            var agrementFromCrm = servise.Retrieve("cr34c_agreement", Guid.Parse("6bf4a976-e465-ed11-9561-000d3ab78121"), new ColumnSet(cr34c_agreement.Fields.cr34c_name, cr34c_agreement.Fields.cr34c_factsumma, cr34c_agreement.Fields.cr34c_summa, cr34c_agreement.Fields.cr34c_date));
            var i = agrementFromCrm.GetAttributeValue<string>("cr34c_name");
            decimal factSumma = agrementFromCrm.Attributes.Contains("cr34c_factsumma")
                    ? agrementFromCrm.GetAttributeValue<Money>("cr34c_factsumma").Value
                    : new Money().Value = new decimal(0.0);

            // Увеличивать оплаченную сумму договора если счет оплачен
            decimal maxSumma = agrementFromCrm.Attributes.Contains("cr34c_summa")
                ? agrementFromCrm.GetAttributeValue<Money>("cr34c_summa").Value
                : new Money().Value = new decimal(0.0);

            QueryExpression query = new QueryExpression("cr34c_credit");
            query.ColumnSet = new ColumnSet(SystemUser.EntityLogicalName);
            query.TopCount = 1;
            //query.Criteria.AddCondition("cr34c_name", ConditionOperator.Like, "%test");
            //query.Criteria.AddFilter("cr34c_name", ConditionOperator.Like, "%test");
            var result = servise.RetrieveMultiple(query);
            foreach (var entity in result.Entities)
            {
                Console.WriteLine(entity.Id);
            }

            Console.Read();
        }
    }
}
