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

        $scope.progressbar(function () {
            var sensorType = "W";

            var sensordata = sensordata1;
            if (sensorType == 'W')
                if (sensordata == 0) {
                    window.alert("width: 0%");
                    return ("width: 0%");
                }
                else {
                    window.alert("width: 100%");
                    return ("width: 100%");
                }
            else if (sensorType == 'R') {
                window.alert("width: " + (sensordata / 25 * 100) + "%");
                return ("width: " + (sensordata / 25 * 100) + "%");
            }
            else if (sensordata == 'S')
                if (sensordata <= 0) {
                    window.alert("width: 0%");
                    return ("width: 0%");
                }
                else if (sensordata >= 90) {
                    window.alert("width: 100%");
                    return ("width: 100%");
                }
                else {
                    window.alert("width: " + (sensordata / 90 * 100) + "%");
                    return ("width: " + (sensordata / 90 * 100) + "%");
                }
            setTimeout(progressbar, 5000);

        });
    };
})();

