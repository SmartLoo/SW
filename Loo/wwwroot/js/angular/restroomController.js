(function () {

    "use strict";

    angular.module("app")
        .controller("restroomController", restroomController);

    function restroomController($http, $timeout, $scope, $interval) {
        var vm = this;
        vm.SelectedRestroom = "";
        vm.Verification = [];
       
        $(".verification-input").keyup(function() {
          var len = $(this.value.length);
          if (len.length > 0)
          {
            var inputs = $(this).closest('form').find(':input');
            inputs.eq( inputs.index(this)+ 1 ).focus();
          }
        });

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

        vm.progressBar = function(sensorId, sensorValue) {
                var sensorType = sensorId[0].slice(0,1);

                //TODO: Include input for calibration parameter
                var calibrationValue = 3600;

                if (sensorType == 'W') //water
                    if (sensorValue == 0) {
                        return (0);
                    }
                    else {
                        return (100);
                    }
                else if (sensorType == 'R') { //trash can
                    return (((85-sensorValue) / 85) * 100);
                }
                else if (sensorType == 'P') //paper
                {
                    return (((5-sensorValue) / 5) * 100);
                }
                else if (sensorType == 'S') //soap        
                    return (((calibrationValue - sensorValue) / calibrationValue) * 100);    
                else
                    return (2);
            };
    };
})();