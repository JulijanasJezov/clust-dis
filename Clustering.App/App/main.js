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

define(['durandal/system', 'durandal/app', 'shared/guajax'], function(system, app, guajax) {
    system.debug(true);

    app.title = "Clustering";
    system.debug(true);
    app.configurePlugins({
        router: true,
        dialog: true
    });

    app.start()
    .then(function() {
        app.setRoot("shell");
        guajax.get("api/seed");
    });
});