define(["clustering/cluster/clusterModel"], function(clusterModel) {
    var model = ko.observable();

    var activate = function(diseaseId) {
        model(new clusterModel(diseaseId));

        model().delegate.showGraphs = function(clustersData, deviationData, silhouetteData) {
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

    return {
        model: model,
        activate: activate
    };
});