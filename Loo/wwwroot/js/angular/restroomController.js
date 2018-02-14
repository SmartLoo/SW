(function () {

    "use strict";

    angular.module("app")
        .controller("restroomController", restroomController);

    function restroomController($http, $timeout, $scope) {

        // Reference to scope of controller
        var vm = this;
        vm.Building = "PHO";
        vm.Location = "1FLOORW";
        vm.SensorId = "302";
        vm.Sensors = [1,2,3,4,5,6,7,8,9,10,11,12];
       
        $(function () {
            $http.get("/api/client?clientId=5a70eace54cdbbebe2440c4c")
                .then(function(response) {
                    vm.Client = response.data;
                });
        });
    };
})();