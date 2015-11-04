define(["diseases/listDiseases/listDiseasesModel"], function(listDiseasesModel) {
    var model = ko.observable();

    var activate = function() {
        model(new listDiseasesModel());

        model().init();
    };

    return {
        model: model,
        activate: activate
    };
});