define(["shared/guajax"], function(guajax) {
    return function addPersonModel() {
        var exports = this;

        exports.selectedFileName = ko.observable();
        exports.diseaseNameRequired = ko.observable();
        exports.diseaseName = ko.observable();

        exports.fileData = ko.observable({
            file: ko.observable()
        });

        exports.fileData().file.subscribe(function(file) {
            exports.selectedFileName(file.name);
        });

        exports.importData = function() {
            var isValid = validateImport();

            if (!isValid) return;

            if (exports.fileData().file()) {
                guajax.postFile("api/import", {
                    diseaseName: exports.diseaseName() ? exports.diseaseName() : null,
                    file: exports.fileData().file()
                })
                .then(function(response) {

                })
                .fail(function(response) {

                });
            }
        };

        function validateImport() {
            exports.diseaseNameRequired(false);

            if (!exports.diseaseName()) exports.diseaseNameRequired(true);

            return !exports.diseaseNameRequired();
        }
    };
});