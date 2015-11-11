define(["diseases/listDiseases/listDiseasesModel", "durandal/app"], function(listDiseasesModel, app) {
    var model = ko.observable();
    var message = ko.observable();
    var hideMessageOnNextExecute;
    var showMessage = ko.observable();

    var activate = function() {
        model(new listDiseasesModel());

        model().delegate.deleteSuccessful = function() {
            message("The Disease was deleted successfully");
            hideMessageOnNextExecute = true;
        };

        model().init();

        if (hideMessageOnNextExecute) {
            showMessage(false);
            message(null);
            hideMessageOnNextExecute = false;
        } else if (showMessage()) {
            hideMessageOnNextExecute = true;
        }
    };

    app.on("disease:message")
    .then(function(msg) {
        message(msg);
        showMessage(true);
    });

    return {
        model: model,
        activate: activate,
        message: message
    };
});