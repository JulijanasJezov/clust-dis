define(["shared/guajax"], function(guajax, scatterSeriesModel) {
    return function testModel(diseaseId) {
        var exports = this;

        exports.diseases = ko.observable();

        exports.deleteDisease = function(diseaseId) {
            guajax.del("api/diseases/" + diseaseId)
            .then(function(response) {
                _fetchDiseases();
            });
        };

        var _fetchDiseases = function() {
            guajax.get("api/diseases")
            .then(function(response) {
                exports.diseases(response.data);
            });
        };

        exports.init = function() {
            _fetchDiseases();
        };
    };
});