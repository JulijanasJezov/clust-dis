define(["people/listPeople/listPeopleModel", "durandal/app"], function(listPeopleModel, app) {
    var model = ko.observable();
    var message = ko.observable();
    var hideMessageOnNextExecute;
    var showMessage = ko.observable();

    var activate = function() {
        model(new listPeopleModel());

        model().init();

        if (hideMessageOnNextExecute) {
            showMessage(false);
            message(null);
            hideMessageOnNextExecute = false;
        } else if (showMessage()) {
            hideMessageOnNextExecute = true;
        }
    };

    app.on("people:message")
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