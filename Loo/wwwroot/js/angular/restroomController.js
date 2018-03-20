(function () {

    "use strict";

    angular.module("app")
        .controller("restroomController", restroomController);

    function restroomController($http, $timeout, $scope, $interval) {
        var vm = this;
        vm.SelectedRestroom = "";
       
        $(function () {
            $http.get("/api/buildings?clientId=BostonU")
                .then(function(response) {
                    vm.Buildings = response.data;

                    $http.get("/api/restrooms?clientId=BostonU&buildingName=" + vm.Buildings[0])
                        .then(function(response) {
                            vm.Restrooms = response.data;
                            vm.SelectedRestroom = vm.Restrooms[0];

                            $http.get("/api/restroom?clientId=BostonU&buildingName=" + vm.Buildings[0] + "&restroomName=" + vm.SelectedRestroom)
                                .then(function(response) {
                                    vm.Sensors = response.data;
                            });
                    });
                });

                $interval(updateSensors, 5000);
                                                                                            
        });


        function updateSensors(){
            $http.get("/api/restroom?clientId=BostonU&buildingName=" + vm.Buildings[0] + "&restroomName=" + vm.Restrooms[0])
                .then(function(response) {
                    vm.Sensors = response.data;
            });
        }

        vm.progressBar = function(SensorId, SensorValue) {
                var sensorType = SensorId[0].slice(0,1);
                var sensordata = SensorValue;
                if (sensorType == 'W')
                    if (sensordata == 0) {
                        return (0);
                    }
                    else {
                        return (100);
                    }
                else if (sensorType == 'R') {
                    return (sensordata / 25 * 100);
                }
                else if (sensordata == 'S')
                    /*
                    normalizedValue = (SensorValue / 1714) * 100;
                    return normalizedValue;
                    */           
                    if (sensordata <= 0) {
                        return (0);
                    }
                    else if (sensordata >= 90) {
                        return (100);
                    }
                    else {
                        return (sensordata / 90 * 100);
                    }
                else
                    return (2);
            };
    };
})();