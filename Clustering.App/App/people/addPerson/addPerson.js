define(["people/addPerson/addPersonModel", "plugins/router", "durandal/app"], function(addPersonModel, router, app) {
    var model = ko.observable();

    var activate = function() {
        model(new addPersonModel());

        model().delegate.addedSuccessfully = function() {
            app.trigger("people:message", "A new Person was added successfully");
            router.navigate("#people");
        };

        model().init();
    };

    return {
        model: model,
        activate: activate
    };
});