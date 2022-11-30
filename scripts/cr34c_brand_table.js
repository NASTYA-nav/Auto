// Функция вызываемая из формы Марки для отображения таблицы во фрейме
function OuterCall(formContext) {
    var creditId = formContext.data.entity.getId();
    var fetchXml = "?fetchXml=<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' >" +
                "<entity name='cr34c_credit' >" +
                    "<attribute name='cr34c_creditid' />" +
                    "<attribute name='cr34c_name' />" +
                    "<attribute name='cr34c_creditperiod' />" +
                    "<order attribute='cr34c_name' descending='false' />" +
                    "<link-entity name='cr34c_agreement' from='cr34c_creditid' to='cr34c_creditid' link-type='inner' alias='by' >" +
                        "<link-entity name='cr34c_auto' from='cr34c_autoid' to='cr34c_autoid' link-type='inner' alias='bz' >" +
                            "<filter type='and' >" +
                                "<condition attribute='cr34c_brandid' operator='eq' uiname='new' uitype='cr34c_brand' value='" + creditId +"' />" +
                            "</filter>" +
                            "<link-entity name='cr34c_model' from='cr34c_modelid' to='cr34c_modelid' link-type='inner' alias='ca' >" +
                            "<attribute name='cr34c_name' />" +
                            "<attribute name='cr34c_modelid' />" +
                            "<filter type='and' >" +
                                    "<condition attribute='cr34c_name' operator='not-null' />" +
                                "</filter>" +
                            "</link-entity>" +
                        "</link-entity>" +
                    "</link-entity>" +
                "</entity>" +
            "</fetch>";

        // Берем все кредитные программы, в которых участвуют автомобили этой марки
        Xrm.WebApi.retrieveMultipleRecords("cr34c_credit", fetchXml).then(
            function success(result) {
                for (var i in result.entities) {
                    debugger;
                    var creditTableBody = $('#creditTableBody');
                    // Вызов нажатия сущьности в таблице
                    creditTableBody.append(`<tr>
                    <td><a href="#" onclick='onRecordClick("cr34c_credit", "${result.entities[i].cr34c_creditid}")'>${result.entities[i].cr34c_name}</a></td>
                    <td><a href="#" onclick='onRecordClick("cr34c_model", "${result.entities[i]["ca.cr34c_modelid"]}")'>${result.entities[i]["ca.cr34c_name"]}</a></td>
                    <td>"${result.entities[i].creditperiod ?? null}"</td>
                    </tr>`);
                }                  
            },
            function (error) {
                console.log(error.message);
            }
        );
    }

    // Функция для открытия сущьности в новом окне
    var onRecordClick = function(entityName, entityId) {
        var pageInput = {
            pageType: "entityrecord",
            entityName: entityName,
            entityId: entityId
        };
        var navigationOptions = {
            target: 2,
            height: {value: 80, unit:"%"},
            width: {value: 70, unit:"%"},
            position: 1
        };
        Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(
            function (success) {
                console.log(success);
            },
            function (error) {
                console.log(error);
            });
    }