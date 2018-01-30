(function () {

    "use strict";

    // Retrieving inventory module
    angular.module("app")
        .controller("collectionController", collectionController);

    // code for controller itself
    function collectionController($http, $timeout, $scope) {

        // Reference to scope of controller
        var vm = this;
        vm.Building = "PHO";
        vm.Location = "1FLOORW";
        vm.SensorId = "302";

        setInterval(updateData, 5000);

        function updateData( )
        {
          $http.get("http://smartloo.azurewebsites.net/api/sensor/PHO/1FLOORW/302")
                .then(function(response) {
                    vm.SensorData = response.data;
                });
        }

        $(function () {
            $http.get("http://smartloo.azurewebsites.net/api/sensor/PHO/1FLOORW/302")
                .then(function(response) {
                    vm.SensorData = response.data;
                });
        });
    };

})();