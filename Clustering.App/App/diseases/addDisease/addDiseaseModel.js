﻿define(["shared/guajax"], function(guajax) {
    return function addDiseaseModel() {
        var exports = this;

        exports.delegate = {
            addedSuccessfully: function() { }
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

            if (!exports.propertyExists())
            {
                exports.properties.push(property);
            }
        };

        exports.removeProperty = function(item) {
            exports.properties.remove(item);
        };

        exports.addDisease = function() {
            var isValid = validateDisease();

            if (!isValid) return;

            guajax.post("api/diseases", {
                diseaseName: exports.diseaseName(),
                properties: exports.properties()

            })
            .then(function(response) {
                exports.delegate.addedSuccessfully();
            });
        };

        function validateDisease() {
            exports.nameRequiredError(false);
            exports.propertiesRequiredError(false);

            if (!exports.diseaseName()) exports.nameRequiredError(true);
            if (exports.properties().length < 2) exports.propertiesRequiredError(true);

            return !exports.nameRequiredError() && !exports.propertiesRequiredError();
        }
    };
});