(function () {

    "use strict";

    angular.module("app")
        .controller("setupController", setupController);

    function setupController($http, $timeout, $scope, $window) {

        var vm = this;
        vm.CurrentStep = 1;

        vm.AdvanceStep = function() {
            $('figure').addClass('dissolve');
            vm.CurrentStep++;
        }


        function removeElement(event) {
          if (event.animationName === 'slide-out') {
            event.target.parentNode.removeChild(event.target);

            var stepId = '#step' + vm.CurrentStep.toString();
            $(stepId).removeClass('hidden-step');
            $(stepId).addClass('slide-in');
          }

          if (event.animationName === 'dissolve') {
              $('#step1').addClass('slide-out');
          }
        }

        document.body.addEventListener('animationend',removeElement);
        document.body.addEventListener('webkitAnimationEnd', removeElement);

    };


})();