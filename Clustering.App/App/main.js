requirejs.config({
    paths: {
        "Scripts": "../Scripts",
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins'
        
    }
});

define("knockout", ko);
define("jquery", [], function() { return jQuery; });

define(['durandal/system', 'durandal/app'], function(system, app) {
    system.debug(true);

    app.title = "Clustering";

    app.configurePlugins({
        router: true,
        dialog: true
    });

    app.start()
    .then(function() {
        app.setRoot("shell");
    });
});