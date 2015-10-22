define([], function() {
    return function columnSeriesModel(data) {
        var exports = this;

        exports.data = mapDataPoints(data.dataPoints);

        function mapDataPoints(dataPoints) {
            var sum = 0;
            var numberOfDataPoints = 0;
            var data = _.each(dataPoints, function(dataPoint) {
                sum += dataPoint.silhouette;
                numberOfDataPoints++;
            });

            return sum / numberOfDataPoints;
        }

    };
});