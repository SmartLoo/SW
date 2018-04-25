﻿(function () {

    "use strict";

    angular.module("app")
        .controller("restroomController", restroomController);

    function restroomController($http, $timeout, $scope, $interval) {
        var vm = this;
        vm.SelectedRestroom = "";
        vm.Validation = [];
        vm.CurrentStep = 1;
       
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


        vm.ValidateAccessory = function()
        {
            var accessoryCode = vm.Validation.join('');
            $http.get("/api/validate" + "?accessoryCode=" + accessoryCode)
                .then(function(response) {
                if (response.data != "Invalid"){
                    vm.NewAccessory = response.data;
                    vm.AdvanceStep();
                }
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

        vm.AdvanceStep = function() {
            var stepId = '#step' + vm.CurrentStep.toString();
            vm.CurrentStep++;
            $(stepId).addClass('dissolve');
        }


        function removeElement(event) {
          if (event.animationName === 'slide-out') {
            event.target.parentNode.removeChild(event.target);

            var stepId = '#step' + vm.CurrentStep.toString();
            $(stepId).removeClass('hidden-step');
            $(stepId).addClass('slide-in');
          }

          if (event.animationName === 'dissolve') {

              var prevStep = vm.CurrentStep -1;
              var stepId = '#step' + prevStep.toString();
              $(stepId).removeClass('dissolve');
              $(stepId).addClass('slide-out');
              $(stepId).removeClass('slide-in');
          }
        }

        document.body.addEventListener('animationend',removeElement);
        document.body.addEventListener('webkitAnimationEnd', removeElement);    
    };
})();