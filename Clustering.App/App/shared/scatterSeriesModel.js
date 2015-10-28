define([], function() {
    return function scatterSeriesModel(data) {
        var exports = this;

        exports.name = data.name;
        exports.data = mapDataPoints(data.dataPoints);

        function mapDataPoints(dataPoints) {
            var data = _.map(dataPoints, function(dataPoint) {
                return {
                    x: dataPoint.properties.age,
                    y: dataPoint.properties.depressionLevel,
                    name: dataPoint.firstName + dataPoint.lastName
                };
            });

            return data;
        }
    };
});