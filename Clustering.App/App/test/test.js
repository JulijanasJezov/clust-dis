define(["test/testModel"], function(testModel) {
    var model = ko.observable(new testModel());

    var activate = function() {

        model().delegate.showGraph = function(data) {
            new Highcharts.Chart({
                chart: {
                    type: 'scatter',
                    zoomType: 'xy',
                    renderTo: "chart"
                },
                title: {
                    text: "Test"
                },
                subtitle: {
                    text: "Subtitle"
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
        }
    };

    

    return {
        activate: activate,
        model: model
    };
});