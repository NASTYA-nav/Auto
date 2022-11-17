var Auto = Auto || {};

Auto.cr34c_agreement = (function() {

    debugger;
    var hideFields = function(context){
        let formContext = context.getFormContext();
        formContext.getControl("cr34c_autoid").setVisible(false);
        formContext.getControl("cr34c_summa").setVisible(false);
        formContext.getControl("cr34c_fact").setVisible(false);
        formContext.getControl("cr34c_creditid").setVisible(false);
    }

    var visibleCreditTab = function(context){
        debugger;
        let formContext = context.getFormContext();
        let contactAttr = formContext.getAttribute("cr34c_contact");
        let autoidAttr = formContext.getAttribute("cr34c_autoid");
        if (contactAttr.getValue() != null && autoidAttr.getValue() != null){
            Xrm.Page.ui.tabs.get("tab_2_credit").setVisible(true);
        } else {
            Xrm.Page.ui.tabs.get("tab_2_credit").setVisible(false);
        }
    }

    var ableCreditFields = function(context){
        debugger;
        let formContext = context.getFormContext();
        let creditidAttr = formContext.getAttribute("cr34c_creditid");
        if (creditidAttr && Xrm.Page.ui.tabs.get("tab_2_credit")) {
            formContext.getControl("cr34c_factsumma").setDisabled(false);
            formContext.getControl("cr34c_paymentplandate").setDisabled(false);
        } else {
            formContext.getControl("cr34c_factsumma").setDisabled(true);
            formContext.getControl("cr34c_paymentplandate").setDisabled(true);
        }

    }

    return {
        onLoad : function(context){
            let formContext = context.getFormContext();
            let contactAttr = formContext.getAttribute("cr34c_contact");
            let autoidAttr = formContext.getAttribute("cr34c_autoid");
            let creditidAttr = formContext.getAttribute("cr34c_creditid");
            var formType = formContext.ui.getFormType();
            Xrm.Page.ui.tabs.get("tab_2_credit").setVisible(false);

            //if create
            if (formType == 1) {
                hideFields(context);
            }

            contactAttr.addOnChange( visibleCreditTab );
            autoidAttr.addOnChange( visibleCreditTab );
            creditidAttr.addOnChange( ableCreditFields );
        }
    }
})();