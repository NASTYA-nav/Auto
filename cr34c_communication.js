var Auto = Auto || {};

Auto.cr34c_communication = (function() {

    const type = {
        "phone": 290040000,
        "email": 290040001
        };

    var hideFields = function(context) {
        let formContext = context.getFormContext();
        formContext.getControl("cr34c_phone").setVisible(false);
        formContext.getControl("cr34c_email").setVisible(false);
    }

    var showCommunications = function(context) {
        let formContext = context.getFormContext();
        let typeAttr = formContext.getAttribute("cr34c_type").getValue();

        if (typeAttr == type.email) {
            formContext.getControl("cr34c_email").setVisible(true);
            formContext.getControl("cr34c_phone").setVisible(false);
        }

        if (typeAttr == type.phone) {
            formContext.getControl("cr34c_phone").setVisible(true);
            formContext.getControl("cr34c_email").setVisible(false);
        }

        if (typeAttr == null) {
            formContext.getControl("cr34c_phone").setVisible(false);
            formContext.getControl("cr34c_email").setVisible(false);
        }
    }

    return {
        onLoad : function(context){
            let formContext = context.getFormContext();
            let typeAttr = formContext.getAttribute("cr34c_type");

            //if create
            if (typeAttr == null) {
                hideFields(context);
            }

            typeAttr.addOnChange( showCommunications );
        }
    }
})();