var Auto = Auto || {};

Auto.cr34c_auto = (function() {

    var hideFields = function(context) {
        let formContext = context.getFormContext();
        formContext.getControl("cr34c_km").setVisible(false);
        formContext.getControl("cr34c_ownerscount").setVisible(false);
        formContext.getControl("cr34c_isdamaged").setVisible(false);
    }

    var showUsedFields = function(context) {
        let formContext = context.getFormContext();
        let usedAttr = formContext.getAttribute("cr34c_used").getValue();
        if (usedAttr == true) {
            formContext.getControl("cr34c_km").setVisible(true);
            formContext.getControl("cr34c_ownerscount").setVisible(true);
            formContext.getControl("cr34c_isdamaged").setVisible(true);
        } else {
            formContext.getControl("cr34c_km").setVisible(false);
            formContext.getControl("cr34c_ownerscount").setVisible(false);
            formContext.getControl("cr34c_isdamaged").setVisible(false);
        }

    }

    return {
        onLoad : function(context){
            let formContext = context.getFormContext();
            let usedAttr = formContext.getAttribute("cr34c_used");

            if (formContext.getAttribute("cr34c_used").getValue() == false) {
                hideFields(context);
            }

            usedAttr.addOnChange( showUsedFields );
        }
    }
})();