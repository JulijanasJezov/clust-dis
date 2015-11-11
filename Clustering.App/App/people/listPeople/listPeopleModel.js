define(["shared/guajax", "shared/pager"], function(guajax, pager) {
    return function listPeopleModel(diseaseId) {
        var exports = this;

        exports.people = ko.observable();

        var _fetchPeople = function() {
            guajax.getWithAbortPrevious("api/people", {
                pageNumber: exports.pager.currentPageNumber(),
                filterQuery: exports.pager.filterQuery()
            })
            .then(function(response) {
                exports.pager.updatePaging(response.data);
                exports.people(response.data.results);
            });
        };

        exports.pager = new pager(_fetchPeople);

        exports.init = function() {
            _fetchPeople();
        };
    };
});