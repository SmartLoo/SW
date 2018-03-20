(function () {

    "use strict";

    angular.module("app")
        .controller("registrationController", registrationController);

    function registrationController($http, $timeout, $scope) {

        var vm = this;

        vm.Reg = {};
       
        $(function () {
                                     
        });

        vm.Register = function() {

            /* check if emails match */

            /* check if password is valid */

            /* check if guid is valid */

            /* post request to api to register client */
            $http.post("/register", vm.Reg)
                .then(function(response) {
                    console.log(response);
            });
        }
    };
})();