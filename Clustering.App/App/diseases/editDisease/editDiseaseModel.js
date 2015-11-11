define(["shared/guajax"], function(guajax) {
    return function testModel(diseaseId) {
        var exports = this;

        exports.delegate = {
            updateSuccessful: function() { }
        };

        exports.diseaseName = ko.observable();
        exports.propertyName = ko.observable();

        exports.properties = ko.observableArray();
        exports.propertyExists = ko.observable();

        exports.propertiesRequiredError = ko.observable();
        exports.nameRequiredError = ko.observable();

        exports.addProperty = function() {
            if (!exports.propertyName()) return;

            var property = { name: exports.propertyName() };
            exports.propertyExists(_.find(exports.properties(), function(existingProperty) {
                return existingProperty.name === property.name;
            }));

            if (!exports.propertyExists()) {
                exports.properties.push(property);
            }
        };

        exports.removeProperty = function(item) {
            exports.properties.remove(item);
        };

        exports.updateDisease = function() {
            var isValid = validateDisease();

            if (!isValid) return;

            guajax.patch("api/diseases/" + diseaseId, {
                diseaseName: exports.diseaseName(),
                properties: exports.properties()

            })
            .then(function(response) {
                exports.delegate.updateSuccessful();
            });
        };

        function validateDisease() {
            exports.nameRequiredError(false);
            exports.propertiesRequiredError(false);

            if (!exports.diseaseName()) exports.nameRequiredError(true);
            if (exports.properties().length < 2) exports.propertiesRequiredError(true);

            return !exports.nameRequiredError() && !exports.propertiesRequiredError();
        }

        var _fetchDisease = function() {
            guajax.get("api/diseases/" + diseaseId)
           .then(function(response) {
               exports.diseaseName(response.data.diseaseName);
               exports.properties(response.data.diseaseProperties);
           });
        };

        exports.init = function() {
            _fetchDisease();
        };
    };
});