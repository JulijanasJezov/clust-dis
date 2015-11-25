define(["clustering/cluster/clusterModel"], function(clusterModel) {
    var model = ko.observable();

    var activate = function(diseaseId) {
        model(new clusterModel(diseaseId));

        model().delegate.showGraphs = function(clusteredData, calculateSilhouette) {
            var clustersData = _.map(clusteredData, function(cluster) {
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

            var deviationData = _.map(clusteredData, function(cluster) {
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

            if (calculateSilhouette) {
                var xs = [];
                var ys = [];

                _.each(clusteredData, function(cluster) {
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
            }

            var clustersLayout = {
                height: 500,
                width: 800,
                showlegend: true,
                xaxis: {
                    title: 'PC1'
                },
                yaxis: {
                    title: 'PC2'
                }
            };

            Plotly.newPlot('clusters-chart', clustersData, clustersLayout);

            var deviationLayout = {
                height: 500,
                width: 800,
                showlegend: true,
                xaxis: {
                    title: 'Standard Deviation'
                },
                yaxis: {
                    title: 'Mean'
                }
            };

            Plotly.newPlot('deviation-chart', deviationData, deviationLayout);

            if (silhouetteData) {
                Plotly.newPlot('validity-chart', silhouetteData);
            }
        };

        model().init();
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

    return {
        model: model,
        activate: activate
    };
});