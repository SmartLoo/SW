(function () {

    "use strict";

    angular.module("app")
        .controller("registrationController", registrationController);

    function registrationController($http, $timeout, $scope) {

        var vm = this;

        var form = {
            FirstName : "",
            LastName : "",
            Email : "",
            EmailConfirm : "",
            "Password" : ""
        };

        vm.Reg = form;
       
        $(function () {
                                     
        });

        vm.Register = function() {

            var isValid = true;

            /* check if emails match */
            if (vm.Reg.Email.length < 1 || vm.Reg.Email != vm.Reg.EmailConfirm)
            {
                isValid = false;
                $('#registerEmail').addClass('is-invalid');
                $('#registerEmailConfirm').addClass('is-invalid');
            }
            else 
            {
                $('#registerEmail').removeClass('is-invalid');
                $('#registerEmailConfirm').removeClass('is-invalid');
            }

            if (vm.Reg.FirstName.length < 1)
            {
                isValid = false;
                $('#firstName').addClass('is-invalid');
            }
            else
            {
                $('#firstName').removeClass('is-invalid');
            }

            if (vm.Reg.LastName.length < 1)
            {
                isValid = false;
                $('#lastName').addClass('is-invalid');
            }   
            else
            {
                $('#lastName').removeClass('is-invalid');
            }       

            /* post request to api to register client */
            if (isValid)
            {
                $http.post("/register", vm.Reg)
                .then(function(response) {
                    console.log(response);
                });
            }
        }
    };
})();