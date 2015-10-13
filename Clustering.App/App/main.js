requirejs.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'knockout': '../Scripts/knockout-3.1.0',
        'jquery': '../Scripts/jquery-1.9.1'
        
    }
});

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