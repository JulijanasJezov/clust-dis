define(["shared/guajax"], function(guajax) {
    return function clusterModel(diseaseId) {
        var exports = this;

        exports.delegate = {
            showGraphs: function(clustersData, deviationData, silhouetteData) { }
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

                var clustersData = _.map(response.data, function(cluster) {
                    var xs = [];
                    var ys = [];
                    var text = []
                    _.each(cluster.dataPoints, function(dp) {
                        xs.push(dp.xValue);
                        ys.push(dp.yValue);
                        text.push(dp.firstName + " " + dp.lastName);
                    });
                    return {
                        x: xs,
                        y: ys,
                        text: text,
                        mode: 'markers',
                        name: cluster.name
                    };
                });

                var deviationData = _.map(response.data, function(cluster) {
                    var xs = [];
                    var ys = [];
                    var text = []
                    _.each(cluster.dataPoints, function(dp) {
                        xs.push(dp.standardDeviation);
                        ys.push(dp.mean);
                        text.push(dp.firstName + " " + dp.lastName);
                    });
                    return {
                        x: xs,
                        y: ys,
                        text: text,
                        mode: 'markers',
                        name: cluster.name
                    };
                });

                var silhouetteData = null;

                if (exports.calculateSilhouette()) {
                    var xs = [];
                    var ys = [];

                    _.each(response.data, function(cluster) {
                        xs.push(cluster.name);
                        ys.push(calculateAverage(cluster.dataPoints));
                    });

                    silhouetteData = [
                      {
                          x: xs,
                          y: ys,
                          type: 'bar'
                      }
                    ];

                    exports.silhouetteCalculated(true);
                }

                exports.delegate.showGraphs(clustersData, deviationData, silhouetteData);
            });
        };

        function calculateAverage(dataPoints) {
            var sum = 0;
            var numberOfDataPoints = 0;
            var data = _.each(dataPoints, function(dataPoint) {
                sum += dataPoint.silhouette;
                numberOfDataPoints++;
            });

            return sum / numberOfDataPoints;
        }

        exports.search = function() {
            if (!exports.searchValue()) return;
            var results = [];
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