define(["shared/guajax", "shared/pager"], function(guajax, pager) {
    return function listDiseasesModel(diseaseId) {
        var exports = this;

        exports.delegate = {
            deleteSuccessful: function() { }
        };

        exports.diseases = ko.observable();

        exports.deleteDisease = function(diseaseId) {
            guajax.del("api/diseases/" + diseaseId)
            .then(function(response) {
                _fetchDiseases();
                exports.delegate.deleteSuccessful();
            });
        };

        var _fetchDiseases = function() {
            guajax.getWithAbortPrevious("api/diseases", {
                pageNumber: exports.pager.currentPageNumber(),
                filterQuery: exports.pager.filterQuery()
            })
            .then(function(response) {
                exports.pager.updatePaging(response.data);
                exports.diseases(response.data.results);
            });
        };

        exports.pager = new pager(_fetchDiseases);

        exports.init = function() {
            _fetchDiseases();
        };
    };
});