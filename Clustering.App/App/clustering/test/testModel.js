define(["shared/guajax", "shared/scatterSeriesModel"], function(guajax, scatterSeriesModel) {
    return function testModel(diseaseId) {
        var exports = this;

        exports.delegate = {
            showGraph: function(data) { },
            showValidity: function(data) { },
        }

        exports.clusteredData = ko.observable();

        exports.showGraph = function() {
            guajax.get("api/clustering/test")
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
    };
});