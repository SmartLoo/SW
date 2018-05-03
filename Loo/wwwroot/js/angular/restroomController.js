﻿(function () {

    "use strict";

    angular.module("app")
        .controller("restroomController", restroomController);

    function restroomController($http, $timeout, $scope, $interval) {
        var vm = this;
        vm.SelectedRestroom = "";
        vm.Validation = [];
        vm.CurrentStep = 0;
       
        $(".verification-input").keyup(function() {
          var len = $(this.value.length);
          if (len.length > 0)
          {
            var inputs = $(this).closest('form').find(':input');
            inputs.eq( inputs.index(this)+ 1 ).focus();
          }
        });

        $(function () {
            $http.get("/api/buildings")
                .then(function(response) {
                    vm.Buildings = response.data;

                    $http.get("/api/restrooms?buildingName=" + vm.Buildings[0])
                        .then(function(response) {
                            vm.Restrooms = response.data;
                            vm.SelectedRestroom = vm.Restrooms[0];

                            $http.get("/api/restroom?clientId=BostonU&buildingName=" + vm.Buildings[0] + "&restroomName=" + vm.SelectedRestroom)
                                .then(function(response) {
                                    var sensors = response.data;
                                    var i;
                                    for (i = 0; i < sensors.length; i++) { 
                                        if (sensors[i].SensorId[0] == 'L')
                                        {
                                            sensors[i].SortOrder = 10000;
                                        }
                                        else 
                                        {
                                            sensors[i].SortOrder = vm.sortOrder(sensors[i]);
                                        }
                                    } 

                                    vm.Sensors = sensors;
                            });
                    });
                });
                $interval(updateSensors, 100000);                                                                                            
        });


        function updateSensors(){
            $http.get("/api/restroom?clientId=BostonU&buildingName=" + vm.Buildings[0] + "&restroomName=" + vm.Restrooms[0])
                .then(function(response) {
                    var sensors = response.data;
                    var i;
                    for (i = 0; i < sensors.length; i++) { 
                        if (sensors[i].SensorId[0] == 'L')
                        {
                            sensors[i].SortOrder = 10000;
                        }
                        else 
                        {
                            sensors[i].SortOrder = vm.sortOrder(sensors[i]);
                        }
                    } 

                    vm.Sensors = sensors;
            });
        }


        vm.ValidateAccessory = function()
        {
            var accessoryCode = vm.Validation.join('');
            $http.get("/api/validate" + "?accessoryCode=" + accessoryCode)
                .then(function(response) {
                if (response.data != "Invalid"){
                    vm.CurrentStep++;
                    vm.NewAccessory = response.data;
                    $http.get("/api/bridge" + "?bridgeId=" + vm.NewAccessory.BridgeId)
                        .then(function(response) {
                            if (response.data != [])
                            {
                                var ref = response.data;
                                // we found another sensor with same bridge id
                                vm.NewAccessory.BuildingName = ref.BuildingName;
                                vm.NewAccessory.BuildingCode = ref.BuildingCode;
                                vm.NewAccessory.LocationName = ref.LocationName;
                                vm.NewAccessory.LocationCode = ref.LocationCode;
                                vm.AdvanceStep(3); // deprecate on next rev
                            }
                            else
                            {
                                vm.AdvanceStep(2);  // deprecate on next rev
                            }
                        });
                }
                else
                {
                    vm.Validation = [];
                }
            });
        }

        vm.progressBar = function(s) {
                var sensorType = s.SensorId[0].slice(0,1);

                //TODO: Include input for calibration parameter

                if (sensorType == 'L') //liquid
                    if (s.SensorValue == 0) {
                        return (0);
                    }
                    else {
                        return (100);
                    }
                else if (sensorType == 'T') { //trash can
                    return s.SensorValue;
                }
                else if (sensorType == 'P') //paper
                {
                    return s.SensorValue;
                }
                else if (sensorType == 'S') //soap        
                    return ((s.CMinDist - s.SensorValue) / s.CMinDist) * 100;  
                else
                    return (2);
            };

        vm.sortOrder = function(s) {
            var sensorType = s.SensorId[0].slice(0,1);

            //TODO: Include input for calibration parameter

            if (sensorType == 'L') //liquid
                if (s.SensorValue == 0) {
                    return (0);
                }
                else {
                    return (100);
                }
            else if (sensorType == 'T') { //trash can
                return (100 - s.SensorValue);
            }
            else if (sensorType == 'P') //paper
            {
                return s.SensorValue;
            }
            else if (sensorType == 'S') //soap        
                return ((s.CMinDist - s.SensorValue) / s.CMinDist) * 100;  
            else
                return (2);
        };

        vm.AdvanceStep = function(forceStep) 
        {
            if (vm.CurrentStep == 0)
            {
                vm.ValidateAccessory();
                return;
            }

            vm.PreviousStep = vm.CurrentStep;
            var stepId = '#step' + vm.CurrentStep.toString();
            $(stepId).addClass('dissolve');

            if (forceStep != null)
            {
                vm.CurrentStep = forceStep;
            }
            else
            {
                vm.CurrentStep++;
            }
        }

        vm.Calibrate = function()
        {
            vm.NewAccessory.CInitialDist = vm.NewAccessory.SensorValue;
        }

        vm.RegisterAccessory = function()
        {
            $http.post("/api/add_accessory", vm.NewAccessory)
                .then(function(response) {
                   
                });
        }

        function removeElement(event) {
          if (event.animationName === 'slide-out') {
            event.target.parentNode.removeChild(event.target);
            var stepId = '#step' + vm.CurrentStep.toString();
            $(stepId).removeClass('hidden-step');
            $(stepId).addClass('slide-in');
          }

          if (event.animationName === 'dissolve') {
              var stepId = '#step' + vm.PreviousStep.toString();
              $(stepId).removeClass('dissolve');
              $(stepId).addClass('slide-out');
              $(stepId).removeClass('slide-in');
          }
        }

        document.body.addEventListener('animationend',removeElement);
        document.body.addEventListener('webkitAnimationEnd', removeElement);    
    };
})();