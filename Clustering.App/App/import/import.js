define(["import/importModel", "plugins/router", "durandal/app"], function(importModel, router, app) {
    var model = ko.observable();

    var activate = function() {
        model(new importModel());
    };

    return {
        model: model,
        activate: activate
    };
});