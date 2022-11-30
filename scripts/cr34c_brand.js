var Auto = Auto || {};

// Событие для формы Марки
Auto.cr34c_brand = (function() {

    return {
        onLoad: function(context){
            var formContext = context.getFormContext();
            var formType = formContext.ui.getFormType();

            // Отображать таблицу только не на форме создания
            if (formType != 1) {
                var resourseControl = formContext.getControl("WebResource_brand").getContentWindow();
                resourseControl.then(
                    function(contentWindow){
                        // Вызов скрипта из веб-ресурса таблицы
                        contentWindow.OuterCall(formContext);
                    },
                    function(error){
                        console.log(error.message);
                    }
                );
            }
        }
    }
})();