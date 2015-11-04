define(["diseases/addDisease/addDiseaseModel"], function(addDiseaseModel) {
    var model = ko.observable();

    var activate = function() {
        model(new addDiseaseModel());

    };

    return {
        model: model,
        activate: activate
    };
});