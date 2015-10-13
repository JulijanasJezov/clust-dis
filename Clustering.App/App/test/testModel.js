define(["shared/guajax", "shared/chartSeriesModel"], function(guajax, chartSeriesModel) {
    return function testModel() {
        var exports = this;

        exports.delegate = {
            showGraph: function(data) { }
        }

        exports.showGraph = function() {
            guajax.get("api/clustering/test")
            .then(function(response) {
                var chartData = _.map(response.data, function(cluster) {
                    var chartSerie = new chartSeriesModel(cluster);
                    return chartSerie;
                });

                exports.delegate.showGraph(chartData);
            })

            exports.delegate.showGraph();
        };
    };
});