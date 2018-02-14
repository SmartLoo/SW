(function () {

    "use strict";

    angular.module("app")
        .controller("collectionController", collectionController);

    function collectionController($http, $timeout, $scope) {

        // Reference to scope of controller
        var vm = this;
        vm.Building = "PHO";
        vm.Location = "1FLOORW";
        vm.SensorId = "302";

        $(function () {
            $http.get("/api/buildings?clientId=BostonU")
                .then(function (response) {
                    vm.Buildings = response.data;

                    $http.get("/api/sensors?clientId=BostonU&buildingName=" + vm.Buildings[0])
                        .then(function (response) {
                            vm.Sensors = response.data;
                        });
                });

        });

      
    };
})();

