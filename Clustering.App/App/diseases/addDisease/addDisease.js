define(["diseases/addDisease/addDiseaseModel", "plugins/router", "durandal/app"], function(addDiseaseModel, router, app) {
    var model = ko.observable();

    var activate = function() {
        model(new addDiseaseModel());

        model().delegate.addedSuccessfully = function() {
            app.trigger("disease:message", "The disease was added successfully.");
            router.navigate("#diseases");
        };
    };

    return {
        model: model,
        activate: activate
    };
});