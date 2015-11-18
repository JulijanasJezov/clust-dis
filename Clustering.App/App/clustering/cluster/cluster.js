define(["clustering/cluster/clusterModel"], function(clusterModel) {
    var model = ko.observable();

    var activate = function(diseaseId) {
        model(new clusterModel(diseaseId));

        model().delegate.showGraph = function(data) {
            //$('#chart').highcharts({
            //    chart: {
            //        type: 'scatter',
            //        zoomType: 'xy'
            //    },
            //    title: {
            //        text: model().diseaseName()
            //    },
            //    xAxis: {
            //        title: {
            //            enabled: true,
            //            text: 'Age'
            //        },
            //        startOnTick: true,
            //        endOnTick: true,
            //        showLastLabel: true
            //    },
            //    yAxis: {
            //        title: {
            //            text: 'Depression Level'
            //        }
            //    },
            //    legend: {
            //        layout: 'vertical',
            //        align: 'left',
            //        verticalAlign: 'top',
            //        x: 100,
            //        y: 70,
            //        floating: true,
            //        backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF',
            //        borderWidth: 1
            //    },
            //    plotOptions: {
            //        scatter: {
            //            marker: {
            //                radius: 5,
            //                states: {
            //                    hover: {
            //                        enabled: true,
            //                        lineColor: 'rgb(100,100,100)'
            //                    }
            //                }
            //            },
            //            states: {
            //                hover: {
            //                    marker: {
            //                        enabled: false
            //                    }
            //                }
            //            },
            //            tooltip: {
            //                headerFormat: '<b>{series.name}</b><br>',
            //                pointFormat: '{point.x} years, {point.y} dl'
            //            }
            //        }
            //    },
            //    series: data
            //});

            debugger;
            var layout = {
                shapes: [
                    {
                        type: 'circle',
                        xref: 'x',
                        yref: 'y',
                        x0: _.min(data[0].x),
                        y0: _.min(data[0].y),
                        x1: _.max(data[0].x),
                        y1: _.max(data[0].y),
                        opacity: 0.2,
                        fillcolor: 'blue',
                        line: {
                            color: 'blue'
                        }
                    },
                    {
                        type: 'circle',
                        xref: 'x',
                        yref: 'y',
                        x0: _.min(data[1].x),
                        y0: _.min(data[1].y),
                        x1: _.max(data[1].x),
                        y1: _.max(data[1].y),
                        opacity: 0.2,
                        fillcolor: 'orange',
                        line: {
                            color: 'orange'
                        }
                    },
                    {
                        type: 'circle',
                        xref: 'x',
                        yref: 'y',
                        x0: _.min(data[2].x),
                        y0: _.min(data[2].y),
                        x1: _.max(data[2].x),
                        y1: _.max(data[2].y),
                        opacity: 0.2,
                        fillcolor: 'green',
                        line: {
                            color: 'green'
                        }
                    },
                    {
                        type: 'circle',
                        xref: 'x',
                        yref: 'y',
                        x0: _.min(data[3].x),
                        y0: _.min(data[3].y),
                        x1: _.max(data[3].x),
                        y1: _.max(data[3].y),
                        opacity: 0.2,
                        fillcolor: 'red',
                        line: {
                            color: 'red'
                        }
                    }
                ],
                height: 500,
                width: 800,
                showlegend: false
            }
            debugger;
            Plotly.newPlot('chart', data, layout);
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