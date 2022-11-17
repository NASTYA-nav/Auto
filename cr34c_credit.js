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
        let formContext = context.getFormContext();
        let dateStartAttr = formContext.getAttribute("cr34c_datestart").getValue();
        let dateEndAttr = formContext.getAttribute("cr34c_dateend").getValue();
        var startDate = new Date(dateStartAttr); 
        startDate.setFullYear(startDate.getFullYear() + 1);

        if (dateEndAttr == startDate || dateEndAttr > startDate) {
            return true;
        } else {
           return false;
        }
    }

    var onDateChange = function(context) {
        let isEndBiggerThanStart = isEndDateBigger(context);

        if (isEndBiggerThanStart == false) {
            showAlert();
        }
    }

    return {
        onLoad : function(context){
            let formContext = context.getFormContext();
            let dateStartAttr = formContext.getAttribute("cr34c_datestart");
            let dateEndAttr = formContext.getAttribute("cr34c_dateend");

            dateStartAttr.addOnChange( onDateChange );
            dateEndAttr.addOnChange( onDateChange );
        },
        onSave : function(context){
            debugger;
            let formContext = context.getFormContext();
            let isEndBiggerThanStart = isEndDateBigger(context);
            if (isEndBiggerThanStart == false) {
                showAlert();
                context.getEventArgs().preventDefault();
            } 
        }
    }
})();