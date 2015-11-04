define(["plugins/router"], function(router) {

    return {
        router: router,
        activate: function() {
            router.map([
              { route: ["", "cluster/diseases/:id"], title: "Clustering", moduleId: "clustering/clustering", nav: true },
              //{ route: ["diseases/add"], title: "Clustering", moduleId: "clustering/clustering", nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});