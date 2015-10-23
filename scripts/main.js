var app = angular.module('app', ['ngRoute', 'angulartics', 'angulartics.google.analytics']);

app.config(function($routeProvider) {
	$routeProvider
		.when('/', {
			templateUrl: 'pages/home.html'
		})
		.when('/download', {
			templateUrl: 'pages/download.html'
		})
		.when('/features', {
			templateUrl: 'pages/features.html'
		})
		.when('/docs', {
			templateUrl: 'pages/docs.html'
		})
		.when('/resources', {
			templateUrl: 'pages/resources.html'
		});
})
app.controller('mainController', ['$scope', function($scope) {

}]);