define([], function() {
    return function(fetchDataFunction) {
        var exports = this;

        var sortClassPrefix = "fa-caret-";
        var ascClass = "up";
        var descClass = "down";

        function callGetDataFunction() {
            fetchDataFunction();
        }

        exports.totalResults = ko.observable(0);
        exports.currentPageNumber = ko.observable();
        exports.itemsPerPage = ko.observable(0);
        exports.pagingFromNumber = ko.observable(0);
        exports.pagingToNumber = ko.observable(0);
        exports.hasPreviousPage = ko.observable(false);
        exports.hasNextPage = ko.observable(false);
        exports.orderBy = ko.observable();
        exports.orderByDirection = ko.observable();
        exports.filterQuery = ko.observable("");

        /// Updates the entire paging model
        exports.updatePaging = function(pageData) {
            exports.totalResults(pageData.total);
            exports.currentPageNumber(pageData.pageNumber);
            exports.itemsPerPage(pageData.itemsPerPage);
            exports.pagingFromNumber(pageData.fromNumber);
            exports.pagingToNumber(pageData.toNumber);
            exports.hasPreviousPage(pageData.previousPage);
            exports.hasNextPage(pageData.nextPage);
            exports.orderBy(pageData.orderBy);
            exports.orderByDirection(pageData.direction);

            var oldFilterQuery = _.isString(exports.filterQuery()) ? exports.filterQuery().trim() : "";
            var newFilterQuery = _.isString(pageData.filterQuery) ? pageData.filterQuery.trim() : "";
            if (newFilterQuery != oldFilterQuery) {
                exports.filterQuery(pageData.filterQuery);
            }
        };

        //Update model when filter query changes
        exports.filterQuery.subscribe(function() {
            exports.currentPageNumber(1);
            callGetDataFunction();
        });

        /// Loads the next page of results
        exports.requestNextPage = function() {
            if (exports.hasNextPage() && exports.currentPageNumber() <= exports.pagingToNumber()) {
                exports.currentPageNumber(exports.currentPageNumber() + 1);
                callGetDataFunction();
            }
        };

        /// Loads the previous page of results
        exports.requestPreviousPage = function() {
            if (exports.hasPreviousPage() && exports.currentPageNumber() > 1) {
                exports.currentPageNumber(exports.currentPageNumber() - 1);
                callGetDataFunction();
            }
        };

        /// Sorts the results by the specified column, first by asc then desc
        exports.setOrderBy = function(orderBy) {
            var toggleOrderByDirection = function() {
                if (exports.orderByDirection() === "asc") {
                    exports.orderByDirection("desc");
                } else {
                    exports.orderByDirection("asc");
                }
            };

            if (exports.orderBy() === orderBy) {
                toggleOrderByDirection();
            } else {
                exports.orderBy(orderBy);
                exports.orderByDirection("asc");
            }

            exports.currentPageNumber(1);
            callGetDataFunction();
        };

        /// Manually resets the paging model
        exports.resetPaging = function() {
            exports.currentPageNumber(1);
            exports.orderBy(null);
            exports.orderByDirection(null);
            exports.filterQuery("");
        };

        exports.resetPageNumber = function() {
            exports.currentPageNumber(1);
        };

        /// Returns the correct CSS class for a given column
        exports.getSortClass = function(columnName) {
            if (exports.orderBy() !== columnName) {
                return "";
            }

            if (exports.orderByDirection() === "asc") {
                return sortClassPrefix + ascClass;
            } else {
                return sortClassPrefix + descClass;
            }
        };
    };
});