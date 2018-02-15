(function () {

    "use strict";

    angular.module("app")
        .controller("restroomController", restroomController);

    function restroomController($http, $timeout, $scope) {
        var vm = this;
       
        $(function () {
            $http.get("/api/buildings?clientId=BostonU")
                .then(function(response) {
                    vm.Buildings = response.data;

                    $http.get("/api/restrooms?clientId=BostonU&buildingName=" + vm.Buildings[0])
                        .then(function(response) {
                            vm.Restrooms = response.data;

                            $http.get("/api/restroom?clientId=BostonU&buildingName=" + vm.Buildings[0] + "&restroomName=" + vm.Restrooms[0])
                                .then(function(response) {
                                    vm.Sensors = response.data;
                            });
                    });
                });                                        
        });
    };
})();