define(["plugins/router"], function(router) {

    return {
        router: router,
        activate: function() {
            router.map([
              { route: "", title: "Home", moduleId: "test/test", nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});