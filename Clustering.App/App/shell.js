define(["plugins/router"], function(router) {

    return {
        router: router,
        activate: function() {
            router.map([
              { route: ["", "cluster/diseases/:id"], title: "Clustering", moduleId: "clustering/clustering", nav: true },
              { route: ["diseases"], title: "Diseases", moduleId: "diseases/listDiseases/listDiseases", nav: true },
              { route: ["diseases/add"], title: "Diseases", moduleId: "diseases/addDisease/addDisease", nav: false },
              { route: ["diseases/:id/edit"], title: "Diseases", moduleId: "diseases/editDisease/editDisease", nav: false },
              { route: ["people"], title: "People", moduleId: "people/listPeople/listPeople", nav: true },
              { route: ["people/add"], title: "People", moduleId: "people/addPerson/addPerson", nav: false },
              { route: ["people/:id/edit"], title: "People", moduleId: "people/editPerson/editPerson", nav: false },
              { route: ["import"], title: "Import Data", moduleId: "import/import", nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});