define(["people/editPerson/editPersonModel", "plugins/router", "durandal/app"], function(editPersonModel, router, app) {
    var model = ko.observable();

    var activate = function(personId) {
        model(new editPersonModel(personId));

        model().delegate.updateSuccessful = function() {
            app.trigger("people:message", "Person was updated successfully");
            router.navigate("#people");
        };

        model().init();
    };

    return {
        model: model,
        activate: activate
    };
});