define([], function() {
    return function scatterSeriesModel(data) {
        var exports = this;

        exports.name = data.name;
        exports.color = getRandomColor();
        exports.data = mapDataPoints(data.dataPoints);

        function mapDataPoints(dataPoints) {
            var data = _.map(dataPoints, function(dataPoint) {
                return {
                    x: dataPoint.properties.x,
                    y: dataPoint.properties.y,
                    name: dataPoint.firstName + dataPoint.lastName
                };
            });

            return data;
        }

        function getRandomColor() {
            var letters = '0123456789ABCDEF'.split('');
            var color = '#';
            for(var i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color;
        }
    };
});