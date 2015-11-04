define(["plugins/router"], function(router) {

    return {
        router: router,
        activate: function() {
            router.map([
              { route: ["", "cluster/diseases/:id"], title: "Clustering", moduleId: "clustering/clustering", nav: true },
              { route: ["diseases"], title: "Diseases", moduleId: "diseases/listDiseases/listDiseases", nav: true },
              { route: ["diseases/add"], title: "Diseases", moduleId: "diseases/addDisease/addDisease", nav: false }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});