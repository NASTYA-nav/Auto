var Auto = Auto || {};

Auto.cr34c_communication = (function() {

    // Типы лукапа срества связи
    const type = {
        "phone": 290040000,
        "email": 290040001
    };

    var hideFields = function(context) {
        var formContext = context.getFormContext();
        formContext.getControl("cr34c_phone").setVisible(false);
        formContext.getControl("cr34c_email").setVisible(false);
    }

    var showCommunication = function(context) {
        var formContext = context.getFormContext();
        var typeAttr = formContext.getAttribute("cr34c_type").getValue();
        var phone = formContext.getControl("cr34c_phone");
        var email = formContext.getControl("cr34c_email");

        if (typeAttr == type.email) {
            email.setVisible(true);
            phone.setVisible(false);
        }

        if (typeAttr == type.phone) {
            phone.setVisible(true);
            email.setVisible(false);
        }

        if (typeAttr == null) {
            phone.setVisible(false);
            email.setVisible(false);
        }
    }

    return {
        onLoad : function(context){
            var formContext = context.getFormContext();
            var typeAttr = formContext.getAttribute("cr34c_type");
            var formType = formContext.ui.getFormType();

            // Если форма создания
            if (formType == 1) {
                hideFields(context);
            }

            // Показывать средство связи в зависимости от выбранного типа
            typeAttr.addOnChange( showCommunication );
        }
    }
})();