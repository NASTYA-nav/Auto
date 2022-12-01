var Auto = Auto || {};

Auto.cr34c_agreement = (function() {

    // Некоторые системные роли
    const role = {
        "System Administrator": "7f578b67-7f5c-ed11-9561-000d3adf4eb1",
        "Basic User": "e068886d-7f5c-ed11-9561-000d3adf4eb1"
    };

    const creditCustomViewGuid = "00000000-0000-0000-0000-000000000001";

    // Функция для проверки роли пользователя
    var checkRole = function(executionContext, roleName) {
        var formContext = executionContext.getFormContext();
        if (formContext !== null && formContext != undefined) {
            var hasRole = false;
            var roles = Xrm.Utility.getGlobalContext().userSettings.roles;
    
            if (roles != null) {
                for (var x in roles._collection) 
                {
                    if (role[`${roleName}`] != undefined && x === role[`${roleName}`]) {
                        hasRole = true;
                        return;
                    }
                }
            }
        }
        return hasRole;
    }

    var disableForm = function(context) {
        // Вся форма ридонли
        var formContext = context.getFormContext();
        var controls = formContext.ui.controls;
        for (var i in controls._collection) 
        {
            formContext.getControl(i).setDisabled(true);
        }
    }

    var hideFields = function(context) {
        // Поля скрываются с формы
        var formContext = context.getFormContext();
        formContext.getControl("cr34c_autoid").setVisible(false);
        formContext.getControl("cr34c_summa").setVisible(false);
        formContext.getControl("cr34c_fact").setVisible(false);
        formContext.getControl("cr34c_creditid").setVisible(false);
        Xrm.Page.ui.tabs.get("tab_2_credit").setVisible(false);
    }

    var visibleCreditTab = function(context) {
        var formContext = context.getFormContext();
        var contactAttr = formContext.getAttribute("cr34c_contact");
        var autoidAttr = formContext.getAttribute("cr34c_autoid");

        // Если выбраны контак и авто показывать вкладку Кредит
        if (contactAttr.getValue() != null && autoidAttr.getValue() != null){
            Xrm.Page.ui.tabs.get("tab_2_credit").setVisible(true);
        } else {
            Xrm.Page.ui.tabs.get("tab_2_credit").setVisible(false);
        }
    }

    var showAlert = function() {  
        var alertStrings = { 
            confirmButtonLabel: "Ok", 
            text: "Кредитная программа просрочена!", 
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

    var ableCreditFields = function(context) {
        var formContext = context.getFormContext();
        var creditidAttr = formContext.getAttribute("cr34c_creditid").getValue();

        // Если кредитная программа выбрана поля оплаченная сумма и график платежей разлочены
        if (creditidAttr != null && Xrm.Page.ui.tabs.get("tab_2_credit")) {
            formContext.getControl("cr34c_factsumma").setDisabled(false);
            formContext.getControl("cr34c_paymentplandate").setDisabled(false);
        } else {
            // Иначе залочены
            formContext.getControl("cr34c_factsumma").setDisabled(true);
            formContext.getControl("cr34c_paymentplandate").setDisabled(true);
        }

    }

    var isAgreementExpired = function(context) {
        var formContext = context.getFormContext();
        var creditId = formContext.getAttribute("cr34c_creditid").getValue()[0].id;
        var agreementDate = formContext.getAttribute("cr34c_date").getValue();

        Xrm.WebApi.retrieveRecord("cr34c_credit", creditId, "?$select=cr34c_dateend")
        .then(
            function(result){
                // Договор истек если дата договора > даты окончания кредитной программы
                var creditEndDate = new Date(result.cr34c_dateend);
                if (agreementDate > creditEndDate) {
                    showAlert();
                    return true;
                }
            },
            function(error){
                console.log(error.message);
            }
        );
    }

    var creditIdOnChange = function(context) {
        var formContext = context.getFormContext();
        var creditidAttr = formContext.getAttribute("cr34c_creditid").getValue();
        var numberAgreementAttribute = formContext.getAttribute("cr34c_creditperiod");
        if (creditidAttr != null) {
            isAgreementExpired(context);
            ableCreditFields(context);
        
            Xrm.WebApi.retrieveRecord("cr34c_credit", creditidAttr[0].id, "?$select=cr34c_creditperiod")
            .then(
                function(result){
                    // Creditperiod обьекта кредит подставлять в поле creditperiod на вкладке кредит обьекта договор  
                    numberAgreementAttribute.setValue(result.cr34c_creditperiod);
                },
                function(error){
                    console.log(error.message);
                }
            );
        }
    }

    var creditFilter = function(context) {
        var formContext = context.getFormContext();
        var autoidAttr = formContext.getAttribute("cr34c_autoid");
        var autoid = autoidAttr.getValue()[0].id;
        fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
            "<entity name='cr34c_credit'>" +
                "<attribute name='cr34c_creditid' />" +
                "<attribute name='cr34c_name' />" +
                "<attribute name='createdon' />" +
                "<order attribute='cr34c_name' descending='false' />" +
                    "<link-entity name='cr34c_agreement' from='cr34c_creditid' to='cr34c_creditid' link-type='inner' alias='af'>" +
                        "<filter type='and'>" +
                        "<condition attribute='cr34c_autoid' operator='eq' uiname='auto' uitype='cr34c_auto' value='" + autoid +"' />" +
                        "</filter>" +
                    "</link-entity>" +
            "</entity>" +
        "</fetch>";

        var layoutXml = "<grid name='resultset' jump='cr34c_name' select='1' icon='1' preview='1' >" +
                "<row name='result' id='cr34c_creditid' >" +
                    "<cell name='cr34c_name' width='300' />" +
                    "<cell name='cr34c_datestart' width='100' />" +
                    "<cell name='cr34c_dateend' width='100' />" +
                    "<cell name='cr34c_percent' width='100' />" +
                "</row>" +
            "</grid>";

        var creditidControl = formContext.getControl("cr34c_creditid");
        
        // Выбираем только кредитные программы связанные с выбранным атомобилем
        creditidControl.addPreSearch(function () {
            creditidControl.addCustomView(creditCustomViewGuid, "cr34c_credit", "myworkview", fetchXml, layoutXml, true);    
        });
    }

    var autoidOnChange = function(context) {
        var formContext = context.getFormContext();
        var autoIdAttr = formContext.getAttribute("cr34c_autoid").getValue();
        var summaAttribute = formContext.getAttribute("cr34c_summa");

        if (autoIdAttr != null) {
            // Фильтр для лукапа с кредитными программами
            creditFilter(context);
            
            // Берем выбранное авто
            Xrm.WebApi.retrieveRecord("cr34c_auto", autoIdAttr[0].id, "?$select=cr34c_used,cr34c_amount,cr34c_modelid")
            .then(auto => { return auto; })
            .then( auto => {
                if (auto.cr34c_used) {
                    // Если авто с пробегом то сумма = сумме авто
                    summaAttribute.setValue(auto.cr34c_amount);
                } else if (auto.cr34c_modelid != null && auto.cr34c_modelid != undefined) {
                    Xrm.WebApi.retrieveRecord("cr34c_model", auto.cr34c_modelid, "?$select=cr34c_recommendedamount")
                    .then(model => {
                        // Если без пробега то сумма = цена модели
                        summaAttribute.setValue(model.cr34c_recommendedamount);
                    });
                }
            })
            .catch(function genericError(error) {
                console.log(error.message);
            });
        }
    }

    var numberFormatting = function(context) {
        var formContext = context.getFormContext();
        var numberAgreementAlldValue = formContext.getAttribute("cr34c_number_agreement").getValue();

        if (numberAgreementAlldValue != null) {
            // Оставлять только цифры и тире в поле Номер договора
            formContext.getAttribute("cr34c_number_agreement").setValue(numberAgreementAlldValue.replace(/[^0-9,-]/g,""));
        }   
    }

    return {
        onLoad: function(context) {
            var formContext = context.getFormContext();
            var contactAttr = formContext.getAttribute("cr34c_contact");
            var autoidAttr = formContext.getAttribute("cr34c_autoid");
            var creditidAttr = formContext.getAttribute("cr34c_creditid");
            var numberAgreementAttr = formContext.getAttribute("cr34c_number_agreement");
            var formType = formContext.ui.getFormType();
            visibleCreditTab(context);

            // Скрывать поля на форме создания
            if (formType == 1) {
                hideFields(context);
            }

            // Лочить форму редактирования для всех пользователей кроме System Administrator
            if (formType == 2 && checkRole(context, "System Administrator") == false) {
                disableForm(context); 
            }

            // Изменение контакта
            contactAttr != null ?? contactAttr.addOnChange(visibleCreditTab);
            // Изменение авто
            autoidAttr != null ?? autoidAttr.addOnChange(autoidOnChange);
            autoidAttr != null ?? autoidAttr.addOnChange(visibleCreditTab);
            // Изменение кредитной программы
            creditidAttr != null ?? creditidAttr.addOnChange(creditIdOnChange);
            // Форматирование поля Номер договора по окончанию ввода
            numberAgreementAttr != null ?? numberAgreementAttr.addOnChange(numberFormatting);
        },
        onSave: function(context){
            var formContext = context.getFormContext();
            var creditId = formContext.getAttribute("cr34c_creditid").getValue();
            var agreementExpired = isAgreementExpired(context);
            if (creditId != null && agreementExpired == true) {
                // Если выбранный кредит заэкспарился не сохранять форму
                context.getEventArgs().preventDefault();
            }
        }
    }
})();