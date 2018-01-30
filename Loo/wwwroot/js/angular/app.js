(function () {

    // Creating module
    var app = angular.module("app", [])
        .run($run);

    // Safely instantiate dataLayer
    var dataLayer = window.dataLayer = window.dataLayer || [];

    $run.$inject = ['$rootScope', '$location'];

    function $run($rootScope, $location) {

        $rootScope.$on('$routeChangeSuccess', function () {

            dataLayer.push({
                event: 'ngRouteChange',
                attributes: {
                    route: $location.path()
                }
            });

        });

    }
})();