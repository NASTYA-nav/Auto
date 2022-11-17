var Auto = Auto || {};

Auto.cr34c_agreement = (function() {

    var hideFields = function(context) {
        let formContext = context.getFormContext();
        formContext.getControl("cr34c_autoid").setVisible(false);
        formContext.getControl("cr34c_summa").setVisible(false);
        formContext.getControl("cr34c_fact").setVisible(false);
        formContext.getControl("cr34c_creditid").setVisible(false);
    }

    var visibleCreditTab = function(context) {
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

    var ableCreditFields = function(context) {
        debugger;
        let formContext = context.getFormContext();
        let creditidAttr = formContext.getAttribute("cr34c_creditid").getValue();

        if (creditidAttr != null && Xrm.Page.ui.tabs.get("tab_2_credit")) {
            formContext.getControl("cr34c_factsumma").setDisabled(false);
            formContext.getControl("cr34c_paymentplandate").setDisabled(false);
        } else {
            formContext.getControl("cr34c_factsumma").setDisabled(true);
            formContext.getControl("cr34c_paymentplandate").setDisabled(true);
        }

    }

    var creditFilter = function(context) {
        debugger;
        let formContext = context.getFormContext();
        let autoidAttr = formContext.getAttribute("cr34c_autoid");
        let autoid = autoidAttr.getValue()[0].id;
        fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
    "<entity name='cr34c_credit'>" +
      "<attribute name='cr34c_creditid' />" +
      "<attribute name='cr34c_name' />" +
      "<attribute name='createdon' />" +
      "<order attribute='cr34c_name' descending='false' />" +
      "<link-entity name='cr34c_agreement' from='cr34c_creditid' to='cr34c_creditid' link-type='inner' alias='af'>" +
        "<filter type='and'>" +
          "<condition attribute='cr34c_autoid' operator='eq' uiname='auto' uitype='cr34c_auto' value='" + autoid + "' />" +
        "</filter>" +
      "</link-entity>" +
    "</entity>" +
  "</fetch>";
        let creditidControl = formContext.getControl("cr34c_creditid");
        
        creditidControl.addPreSearch(function () {
            creditidControl.addCustomFilter(fetchXml);    
        });
    }

    var autoidOnChange = function(context) {
        debugger;
        let formContext = context.getFormContext();
        let autoidAttr = formContext.getAttribute("cr34c_autoid");
        autoidAttr.addOnChange( visibleCreditTab );
        if (autoidAttr.getValue() != null) {
            creditFilter(context);
        }
    }

    var numberFormatting = function(context) {
        debugger;
        let formContext = context.getFormContext();
        let numberAgreementAlldValue = formContext.getAttribute("cr34c_number_agreement").getValue();

        if (numberAgreementAlldValue != null) {
            formContext.getAttribute("cr34c_number_agreement").setValue(numberAgreementAlldValue.replace(/[^0-9,-]/g,""));
        }
        
    }

    return {
        onLoad : function(context){
            debugger;
            let formContext = context.getFormContext();
            let contactAttr = formContext.getAttribute("cr34c_contact");
            let autoidAttr = formContext.getAttribute("cr34c_autoid");
            let creditidAttr = formContext.getAttribute("cr34c_creditid");
            let numberAgreementAttr = formContext.getAttribute("cr34c_number_agreement");
            var formType = formContext.ui.getFormType();
            Xrm.Page.ui.tabs.get("tab_2_credit").setVisible(false);

            //if create
            if (formType == 1) {
                hideFields(context);
            }

            contactAttr.addOnChange( visibleCreditTab );
            autoidAttr.addOnChange( autoidOnChange );
            creditidAttr.addOnChange( ableCreditFields );
            numberAgreementAttr.addOnChange( numberFormatting );
        }
    }
})();