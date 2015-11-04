define(["shared/guajax", "shared/scatterSeriesModel"], function(guajax, scatterSeriesModel) {
    return function testModel(diseaseId) {
        var exports = this;

        exports.delegate = {
            showGraph: function(data) { },
            showValidity: function(data) { },
        }

        exports.diseaseName = ko.observable();
        exports.clusteredData = ko.observable();
        exports.diseaseProperties = ko.observable();
        exports.clusterPropertyIds = ko.observableArray();
        exports.includeAge = ko.observable();
        exports.calculateSilhouette = ko.observable();
        exports.searchValue = ko.observable();
        exports.searchResults = ko.observableArray();
        exports.personSelected = ko.observable();

        exports.showGraph = function() {
            guajax.post("api/cluster", {
                clusterDiseaseId: diseaseId,
                clusterPropertyIds: exports.clusterPropertyIds(),
                includeAge: exports.includeAge(),
                calculateSilhouette: exports.calculateSilhouette()

            })
            .then(function(response) {
                exports.clusteredData(response.data);

                var chartData = _.map(response.data, function(cluster) {
                    var chartSerie = new scatterSeriesModel(cluster);
                    return chartSerie;
                });

                exports.delegate.showGraph(chartData);
            });
        };

        exports.showValidity = function() {
            function calculateAverage(dataPoints) {
                var sum = 0;
                var numberOfDataPoints = 0;
                var data = _.each(dataPoints, function(dataPoint) {
                    sum += dataPoint.silhouette;
                    numberOfDataPoints++;
                });

                return sum / numberOfDataPoints;
            }

            var columnData = _.map(exports.clusteredData(), function(cluster) {
                var chartSerie = [cluster.name, calculateAverage(cluster.dataPoints)];
                return chartSerie;
            });
            
            exports.delegate.showValidity(columnData);
        };

        exports.search = function() {
            var results = [];
            _.each(exports.clusteredData(), function(cluster) {
                results.push(_.filter(cluster.dataPoints, function(person) {
                    return person.firstName == exports.searchValue() || person.lastName == exports.searchValue()
                        || person.firstName + ' ' + person.lastName == exports.searchValue();
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