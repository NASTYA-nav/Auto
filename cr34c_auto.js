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
        var km = formContext.getControl("cr34c_km");
        var ownerscount = formContext.getControl("cr34c_ownerscount");
        var isdamaged = formContext.getControl("cr34c_isdamaged");

        if (usedAttr == true) {
            km.setVisible(true);
            ownerscount.setVisible(true);
            isdamaged.setVisible(true);
        } else {
            km.setVisible(false);
            ownerscount.setVisible(false);
            isdamaged.setVisible(false);
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