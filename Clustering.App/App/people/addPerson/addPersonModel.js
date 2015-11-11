define(["shared/guajax"], function(guajax) {
    return function addPersonModel() {
        var exports = this;

        exports.delegate = {
            addedSuccessfully: function() { }
        };

        exports.genders = ko.observable(["Male", "Female", "Other"]);
        exports.selectedGender = ko.observable();

        exports.title = ko.observable();
        exports.firstName = ko.observable();
        exports.lastName = ko.observable();
        exports.dateOfBirth = ko.observable();

        exports.diseases = ko.observable();
        exports.diseasesAdded = ko.observableArray();

        exports.firstNameRequiredError = ko.observable();
        exports.lastNameRequiredError = ko.observable();
        exports.dobRequiredError = ko.observable();
        exports.diseaseAddedError = ko.observable();

        exports.selectedDiseaseId = ko.observable();
        exports.selectedDisease = ko.observable();
        exports.diseasePropertiesToAdd = ko.observableArray();

        exports.selectedDiseaseId.subscribe(function() {
            exports.diseaseAddedError(false);
            var disease = _.find(exports.diseases(), function(disease) {
                return disease.diseaseId == exports.selectedDiseaseId();
            });

            if (disease) {
                _.each(disease.diseaseProperties, function(property) {
                    property.score = ko.observable();
                });
            }
            exports.selectedDisease(disease);
        });

        exports.addDisease = function() {
            exports.diseaseAddedError(false);
            var alreadyAdded = _.filter(exports.diseasesAdded(), function(disease) {
                return disease.diseaseId == exports.selectedDiseaseId();
            });

            if (_.any(alreadyAdded)) {
                exports.diseaseAddedError(true);
                return;
            }

            _.map(exports.selectedDisease().diseaseProperties, function(property) {
                var newProperty = {
                    diseasePropertyId: property.diseasePropertyId,
                    score: property.score()
                };

                exports.diseasePropertiesToAdd.push(newProperty);
            });

            exports.diseasesAdded.push(exports.selectedDisease());
        };

        exports.removeDisease = function(diseaseId) {
            var diseaseToRemove = _.find(exports.diseasesAdded(), function(disease) {
                return disease.diseaseId == diseaseId;
            });

            exports.diseasesAdded.remove(diseaseToRemove);
        };

        exports.addPerson = function() {
            var isValid = validatePerson();

            if (!isValid) return;

            guajax.post("api/people", {
                title: exports.title(),
                firstName: exports.firstName(),
                lastName: exports.lastName(),
                dateOfBirth: moment(exports.dateOfBirth()).format(),
                gender: exports.selectedGender(),
                diseaseProperties: exports.diseasePropertiesToAdd()
            })
            .then(function(response) {
                exports.delegate.addedSuccessfully();
            });
        };

        var _fetchDiseases = function() {
            guajax.get("api/diseases")
            .then(function(response) {
                exports.diseases(response.data.results);
            });
        };

        exports.init = function() {
            _fetchDiseases();
        }

        function validatePerson() {
            exports.firstNameRequiredError(false);
            exports.lastNameRequiredError(false);
            exports.dobRequiredError(false);

            if (!exports.firstName()) exports.firstNameRequiredError(true);
            if (!exports.lastName()) exports.lastNameRequiredError(true);
            if (!exports.dateOfBirth()) exports.dobRequiredError(true);

            return !exports.firstNameRequiredError() && !exports.lastNameRequiredError() && !exports.dobRequiredError();
        }
    };
});