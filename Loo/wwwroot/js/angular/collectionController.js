﻿(function () {

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
       
        $(function () {
            $http.get("/api/client?clientId=5a70eace54cdbbebe2440c4c")
                .then(function(response) {
                    vm.Client = response.data;
                });
        });
    };

})();