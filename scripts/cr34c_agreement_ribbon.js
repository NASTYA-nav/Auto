var Auto = Auto || {};

Auto.cr34c_agreement_ribbon = (function() {

    return {
        calculate: function(context){
            var formContext = context.getFormContext();
            var creditamountAttribute = formContext.getAttribute("cr34c_creditamount");
            var fullcreditamountAttribute = formContext.getAttribute("cr34c_fullcreditamount");
            var summaAttribute = formContext.getAttribute("cr34c_summa");
            var initialfeeAttribute = formContext.getAttribute("cr34c_initialfee");
            var creditperiod = formContext.getAttribute("cr34c_initialfee").getValue();
            var creditamount = creditamountAttribute.getValue();
            var creditId = formContext.getAttribute("cr34c_creditid").getValue();
            var percent;
            Xrm.WebApi.retrieveRecord("cr34c_credit", creditId, "?$select=cr34c_percent")
            .then(
                function(result){
                    debugger;
                    percent = result;
                },
                function(error){
                    console.log(error.message);
                }
            );

            // Сумма кредита = [Договор].[Сумма] – [Договор].[Первоначальный взнос]
            creditamountAttribute.setValue(summaAttribute.getValue() - initialfeeAttribute.getValue());

            // Полная стоимость кредита = ([Кредитная Программа].[Ставка]/100 * [Договор].[Срок кредита]  * [Договор].[Сумма кредита] ) 
            // + [Договор].[Сумма кредита]
            var fullcreditamount = (percent/100 * creditperiod * creditamount) + creditamount;
            fullcreditamountAttribute.setValue(fullcreditamount);
        }
    }
})();