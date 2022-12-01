var Auto = Auto || {};

Auto.cr34c_auto = (function() {

    var hideFields = function(context) {
        var formContext = context.getFormContext();
        formContext.getControl("cr34c_km").setVisible(false);
        formContext.getControl("cr34c_ownerscount").setVisible(false);
        formContext.getControl("cr34c_isdamaged").setVisible(false);
    }

    var showUsedFields = function(context) {
        var formContext = context.getFormContext();
        var usedAttr = formContext.getAttribute("cr34c_used").getValue();
        var km = formContext.getControl("cr34c_km");
        var ownersCount = formContext.getControl("cr34c_ownerscount");
        var isDamaged = formContext.getControl("cr34c_isdamaged");

        if (usedAttr == true) {
            km == null ?? km.setVisible(true);
            ownersCount == null ?? ownersCount.setVisible(true);
            isDamaged == null ?? isDamaged.setVisible(true);
        } else {
            km == null ?? km.setVisible(false);
            ownersCount == null ?? ownersCount.setVisible(false);
            isDamaged == null ?? isDamaged.setVisible(false);
        }

    }

    return {
        onLoad : function(context){
            var formContext = context.getFormContext();
            var usedAttr = formContext.getAttribute("cr34c_used");

            if (formContext.getAttribute("cr34c_used").getValue() == false) {
                hideFields(context);
            }

            // на изменение статуса Спробегом показывать или скрывать поля для пробега
            usedAttr.addOnChange( showUsedFields );
        }
    }
})();