var Auto = Auto || {};

// Кнопка на форме Пересчитать редит
Auto.cr34c_agreement_ribbon = (function() {

    var showAlert = function(message) {  
        var alertStrings = { 
            confirmButtonLabel: "Ok", 
            text: message, 
            title: "Внимание!" 
        };
            var alertOptions = { height: 120, width: 260 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function (success) {
                    console.log("Alert dialog closed");
                },
                function (error) {
                    console.log(error.message);
                }
            );
    }

    return {
        // Функция расчитывает поля Сумма кредита и Полная стоимость кредита на форме договора
        calculate: function(primaryControl){
            var formContext = primaryControl;
            var creditamountAttribute = formContext.getAttribute("cr34c_creditamount");            
            var fullcreditamountAttribute = formContext.getAttribute("cr34c_fullcreditamount");
            var summaAttribute = formContext.getAttribute("cr34c_summa");
            var initialfeeAttribute = formContext.getAttribute("cr34c_initialfee");
            var creditperiodAttribute = formContext.getAttribute("cr34c_initialfee");
            var creditIdAttribute = formContext.getAttribute("cr34c_creditid");

            if (initialfeeAttribute == null || creditperiodAttribute == null 
                || creditIdAttribute == null || summaAttribute == null) {
                    showAlert("Отсутствуют поля на форме, необходимые для расчета кредита!");
                    return;
            }

            if (summaAttribute.getValue() == 0 || initialfeeAttribute.getValue() == 0 
                || creditperiodAttribute.getValue() == 0 || creditIdAttribute.getValue() == null) {
                    showAlert("Отсутствуют данные в полях, необходимые для расчета кредита!");
                    return;
            }

            var creditId = creditIdAttribute.getValue()[0].id

            Xrm.WebApi.retrieveRecord("cr34c_credit", creditId, "?$select=cr34c_percent")
            .then(
                function(result){
                    // Проверка наличия поля процента с сервера
                    if (result.cr34c_percent != 0) {
                        // Сумма кредита = [Договор].[Сумма] – [Договор].[Первоначальный взнос]
                        creditamountAttribute.setValue(summaAttribute.getValue() - initialfeeAttribute.getValue());
                        var creditamount = creditamountAttribute.getValue();

                        // Полная стоимость кредита = ([Кредитная Программа].[Ставка]/100 * [Договор].[Срок кредита]  * [Договор].[Сумма кредита] ) 
                        // + [Договор].[Сумма кредита]
                        var fullcreditamount = (result.cr34c_percent/100 * creditperiodAttribute.getValue() * creditamount) + creditamount;
                        fullcreditamountAttribute.setValue(fullcreditamount);

                        showAlert("Сумма кредита и полная стоимость расчитаны!");
                    }
                },
                function(error){
                    console.log(error.message);
                }
            );
        }
    }
})();