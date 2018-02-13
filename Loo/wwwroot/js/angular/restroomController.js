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

        });
    };
})();