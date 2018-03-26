(function () {

    "use strict";

    angular.module("app")
        .controller("registrationController", registrationController);

    function registrationController($http, $timeout, $scope, $window) {

        var vm = this;

        var form = {
            FirstName : "",
            LastName : "",
            Email : "",
            EmailConfirm : "",
            "Password" : ""
        };

        var log = {
            Username : "",
            Password : ""
        };

        vm.Reg = form;
        vm.Log = log;
        vm.LoginSuccess = false;
       
        $(function () {
            $('#fullpage').fullpage();                   
        });

        vm.Login = function() {
            var isValid = true;

            if (vm.Log.Username.length < 1)
            {
                isValid = false;
                $('#loginEmail').addClass('is-invalid');
            }
            else
            {
                $('#loginEmail').removeClass('is-invalid');
            }

            if (vm.Log.Password.length < 1)
            {
                isValid = false;
                $('#loginPassword').addClass('is-invalid');
            }
            else
            {
                $('#loginPassword').removeClass('is-invalid');
            }

            /* post request for sign in */
            if (isValid)
            {
                /* redirect to dashboard */
                $('#loginButton').removeClass('is-invalid');

                $http.post("/login", vm.Log)
                .then(function(response) {             
                    if (response.data != "202")
                    {
                        /* show form errors */
                        $('#loginForm').addClass('is-invalid');
                    }
                    else 
                    {
                        $window.location.href = '/';
                    }
                });
            }

        }

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
                    if (response.data != "202")
                    {
                        /* show form errors */
                    }
                    else 
                    {
                        $window.location.href = '/';
                    }
                });
            }
        }
    };
})();