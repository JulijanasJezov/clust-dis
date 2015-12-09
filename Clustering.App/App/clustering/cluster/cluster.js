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

            var clusterNames = _.map(clusteredData, function(cluster) {
                return cluster.name;
            });

            var deviationData = _.map(_.first(clusteredData).propertiesDetails, function(pd) {
                return {
                    x: clusterNames,
                    y: [],
                    name: pd.name,
                    error_y: {
                        type: "data",
                        array: [],
                        visible: true
                    },
                    type: "bar"
                };
            });

            _.map(clusteredData, function(cluster) {
                _.each(cluster.propertiesDetails, function(pd) {
                    var singleObject = _.findWhere(deviationData, { name: pd.name });
                    singleObject.y.push(pd.averageValue);
                    singleObject.error_y.array.push(pd.standardDeviation);
                });
            });

            var deviationLayout = {
                height: 500,
                width: 800,
                barmode: "group"
            };

            Plotly.newPlot('deviation-chart', deviationData, deviationLayout);

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