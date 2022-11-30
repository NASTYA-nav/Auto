var checkRole = function(executionContext, roleName) {
    debugger;
    let formContext = executionContext.getFormContext();
    if (formContext !== null && formContext != undefined) {
        let hasRole = false;
        let roles = Xrm.Utility.getGlobalContext().userSettings.roles;

        if (roles != null) {
            for (var x in roles) 
            {
                if (x.name.toLowerCase() === roleName.toLowerCase()) {
                    hasRole = true;
                    return;
                }
            }
        }
    }
    return hasRole;
}