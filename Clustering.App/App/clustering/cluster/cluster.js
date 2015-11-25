define(["clustering/cluster/clusterModel"], function(clusterModel) {
    var model = ko.observable();

    var activate = function(diseaseId) {
        model(new clusterModel(diseaseId));

        model().delegate.showGraph = function(data) {
            $('#chart').highcharts({
                chart: {
                    type: 'scatter',
                    zoomType: 'xy'
                },
                title: {
                    text: model().diseaseName()
                },
                xAxis: {
                    title: {
                        enabled: true,
                        text: 'Age'
                    },
                    startOnTick: true,
                    endOnTick: true,
                    showLastLabel: true
                },
                yAxis: {
                    title: {
                        text: 'Depression Level'
                    }
                },
                legend: {
                    layout: 'vertical',
                    align: 'left',
                    verticalAlign: 'top',
                    x: 100,
                    y: 70,
                    floating: true,
                    backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF',
                    borderWidth: 1
                },
                plotOptions: {
                    scatter: {
                        marker: {
                            radius: 5,
                            states: {
                                hover: {
                                    enabled: true,
                                    lineColor: 'rgb(100,100,100)'
                                }
                            }
                        },
                        states: {
                            hover: {
                                marker: {
                                    enabled: false
                                }
                            }
                        },
                        tooltip: {
                            headerFormat: '<b>{series.name}</b><br>',
                            pointFormat: '{point.x} years, {point.y} dl'
                        }
                    }
                },
                series: data
            });
        };

        model().delegate.showDeviationGraph = function(data) {
            var layout = {
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

            Plotly.newPlot('deviation-chart', data, layout);
        };

        model().delegate.showValidity = function(data) {
            $('#validityChart').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Silhouette'
                },
                xAxis: {
                    type: 'category',
                    labels: {
                        rotation: -45,
                        style: {
                            fontSize: '13px',
                            fontFamily: 'Verdana, sans-serif'
                        }
                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Silhouette'
                    }
                },
                legend: {
                    enabled: false
                },
                tooltip: {
                    pointFormat: 'Silhouette: <b>{point.y:.1f}</b>'
                },
                series: [{
                    name: 'Silhouette',
                    data: data
                }]
            });
        };

        model().init();
    };

    return {
        model: model,
        activate: activate
    };
});