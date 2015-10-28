define(["plugins/router"], function(router) {

    return {
        router: router,
        activate: function() {
            router.map([
              { route: ["", "disease/:id"], title: "Clustering", moduleId: "clustering/clustering", nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});