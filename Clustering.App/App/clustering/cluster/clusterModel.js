define(["shared/guajax"], function(guajax) {
    return function clusterModel(diseaseId) {
        var exports = this;

        exports.delegate = {
            showGraphs: function(clusteredData, calculateSilhouette) { }
        }

        // page data
        exports.diseaseName = ko.observable();
        exports.clusteredData = ko.observable();
        exports.diseaseProperties = ko.observable();
        exports.silhouetteCalculated = ko.observable(false);

        // data to cluster
        exports.clusterPropertyIds = ko.observableArray();
        exports.includeAge = ko.observable();
        exports.calculateSilhouette = ko.observable();
        exports.numberOfClusters = ko.observable();

        // search data
        exports.searchValue = ko.observable();
        exports.searchResults = ko.observableArray();
        exports.personSelected = ko.observable();

        // validation
        exports.numberOfClustersError = ko.observable(false);

        var isValid = function() {
            exports.numberOfClustersError(false);

            if (isNaN(exports.numberOfClusters()) || exports.numberOfClusters() < 2) {
                exports.numberOfClustersError(true);
                return false;
            }

            return true;
        };

        exports.showGraph = function() {
            if (!isValid()) return;
            exports.silhouetteCalculated(false);

            guajax.post("api/cluster", {
                clusterDiseaseId: diseaseId,
                clusterPropertyIds: exports.clusterPropertyIds(),
                includeAge: exports.includeAge(),
                calculateSilhouette: exports.calculateSilhouette(),
                numberOfClusters: exports.numberOfClusters()

            })
            .then(function(response) {
                exports.clusteredData(response.data);

                if (exports.calculateSilhouette()) {
                    exports.silhouetteCalculated(true);
                }

                exports.delegate.showGraphs(exports.clusteredData(), exports.calculateSilhouette());
            });
        };

        exports.search = function() {
            if (!exports.searchValue()) return;
            var results = [];

            // search for the person in all of the clusters
            _.each(exports.clusteredData(), function(cluster) {
                results.push(_.filter(cluster.dataPoints, function(person) {
                    var firstName = person.firstName.toLowerCase();
                    var lastName = person.lastName.toLowerCase();
                    var searchVal = exports.searchValue().toLowerCase();
                    return firstName == searchVal || lastName == searchVal
                        || firstName + ' ' + lastName == searchVal;
                }));
            });

            results = _.flatten(results);
            exports.searchResults(results);
        };

        exports.init = function() {
            var _fetchDiseaseProperties = function() {
                guajax.get("api/diseases/" + diseaseId + "/properties")
                .then(function(response) {
                    exports.diseaseProperties(response.data);
                    exports.diseaseName(_.first(response.data).disease.name);
                });
            };

            if (diseaseId) _fetchDiseaseProperties();
        };
    };
});