var Auto = Auto || {};

Auto.cr34c_credit = (function() {

    var showAlert = function() {  
        var alertStrings = { 
            confirmButtonLabel: "Ok", 
            text: "Дата окончания должна быть больше чем датаначала минимум на 1 год.", 
            title: "Ошибка!" };
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

    var isEndDateBigger = function(context) {
        var formContext = context.getFormContext();
        var dateStartAttr = formContext.getAttribute("cr34c_datestart").getValue();
        var dateEndAttr = formContext.getAttribute("cr34c_dateend").getValue();
        var startDate = new Date(dateStartAttr); 
        startDate.setFullYear(startDate.getFullYear() + 1);
        var isEndBiggerThanStart;

        if (dateEndAttr >= startDate) {
            isEndBiggerThanStart = true;
        } else {
            isEndBiggerThanStart = false;
        }

        return isEndBiggerThanStart;
    }

    var onDateChange = function(context) {
        var isEndBiggerThanStart = isEndDateBigger(context);

        // показывать предупредение если срок кредита меньше года
        if (isEndBiggerThanStart == false) {
            showAlert();
        }
    }

    return {
        onLoad : function(context){
            var formContext = context.getFormContext();
            var dateStartAttr = formContext.getAttribute("cr34c_datestart");
            var dateEndAttr = formContext.getAttribute("cr34c_dateend");

            dateStartAttr.addOnChange(onDateChange);
            dateEndAttr.addOnChange(onDateChange);
        },
        onSave : function(context){
            var isEndBiggerThanStart = isEndDateBigger(context);

            // Запрещать сохранение если срок кредита меньше года
            if (isEndBiggerThanStart == false) {
                showAlert();
                context.getEventArgs().preventDefault();
            } 
        }
    }
})();