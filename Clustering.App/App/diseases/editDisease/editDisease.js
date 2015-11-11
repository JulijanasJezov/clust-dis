define(["diseases/editDisease/editDiseaseModel", "plugins/router", "durandal/app"], function(editDiseaseModel, router, app) {
    var model = ko.observable();

    var activate = function(diseaseId) {
        model(new editDiseaseModel(diseaseId));

        model().delegate.updateSuccessful = function() {
            app.trigger("disease:message", "The disease was updated successfully.");
            router.navigate("#diseases");
        };

        model().init();
    };

    return {
        model: model,
        activate: activate
    };
});