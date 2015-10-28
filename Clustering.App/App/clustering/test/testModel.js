define(["shared/guajax", "shared/scatterSeriesModel"], function(guajax, scatterSeriesModel) {
    return function testModel(diseaseId) {
        var exports = this;

        exports.delegate = {
            showGraph: function(data) { },
            showValidity: function(data) { },
        }

        exports.clusteredData = ko.observable();
        exports.diseaseProperties = ko.observable();
        exports.clusterPropertyIds = ko.observableArray();

        exports.showGraph = function() {
            guajax.post("api/clustering/test", {
                clusterDiseaseId: diseaseId,
                clusterPropertyIds: exports.clusterPropertyIds()

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

        exports.init = function() {
            var _fetchDiseaseProperties = function() {
                guajax.get("api/diseases/" + diseaseId + "/properties")
                .then(function(response) {
                    exports.diseaseProperties(response.data);
                });
            };

            _fetchDiseaseProperties();
        };
    };
});