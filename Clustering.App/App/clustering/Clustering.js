define(["plugins/router", "durandal/app", "shared/guajax"], function(router, app, guajax) {
    var activeItem = router.activeItem();
    var childRouter = router.createChildRouter();
    var activeTab = ko.observable();
    var selectedDiseaseTabId = ko.observable();
    var diseaseTabs = ko.observable();

    childRouter.makeRelative({
        moduleId: "clustering",
        fromParent: true
    })
    .map([
        {
            route: ["", ":id"],
            moduleId: "cluster/cluster",
            title: "Cluster Data",
            tab: "cluster"
        }
    ]);

    childRouter.mapUnknownRoutes(function(instruction) {
    });

    childRouter.on("router:route:activating", function(instance, instruction) {
        activeTab(instruction.config.tab);
    });

    var activate = function(diseaseId) {
        selectedDiseaseTabId(diseaseId);

        _fetchDiseaseTabs();
    };

    var _fetchDiseaseTabs = function() {
        return guajax.get("api/diseases")
        .then(function(response) {
            diseaseTabs(response.data);

            if (!selectedDiseaseTabId()) {
                var defaultTab = _.first(diseaseTabs()).diseaseId;
                router.navigate("cluster/diseases/" + defaultTab);
            }
        });
    };

    return {
        router: childRouter,
        activeTab: activeTab,
        activate: activate,
        selectedDiseaseTabId: selectedDiseaseTabId,
        diseaseTabs: diseaseTabs
    };
});