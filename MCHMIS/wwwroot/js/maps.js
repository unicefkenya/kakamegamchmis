var app = angular.module("clientApp", ["AxelSoft", "ngSanitize"]);
app.controller('clientCtrl', ['$scope', '$http', '$filter', function (scope, http, $filter) {
    scope.count = 1;
    scope.total = 1;
    scope.indicator = 1;

    scope.donor = 'All';
    scope.categoryId = '0';
    scope.period = '';
    scope.itemsPerPage = 20;
    var root = $('#RootUrl').val();
}]).filter('inArray', function ($filter) {
    return function (list, arrayFilter, element) {
        if (arrayFilter) {
            return $filter("filter")(list, function (listItem) {
                return arrayFilter.indexOf(listItem[element]) != -1;
            });
        }
    };
});

var rootUrl = $('#RootUrl').val();

function BeneficiariesMaps(data) {
    $('.ajax-loader ').fadeOut();
    var period = $("#reporting-period option:selected").text();
    period = period === 'Select' ? '' : period;

    var title = $("#indicators option:selected").val() === '5'
        ? "Total Volume of Grants Paid by end of" + ' ' + period
        : $("#indicators option:selected").text() + ' ' + period;

    Highcharts.mapChart('map-container', {
        title: {
            text: title
        },
        //subtitle: {
        //    text: "Test Subtite"
        //},

        mapNavigation: {
            enabled: true,
        },
        tooltip: {
            headerFormat: '',
            pointFormat: '{point.properties.CASWARD}' + '<br>' +
                'Beneficiaries: {point.value}'
        },
        colorAxis: {
            tickPixelInterval: 100,
            min: 0,
            minColor: '#d9f0a3',
            maxColor: '#005a32'
        },
        series: [{
            data: data,
            mapData: JSON.parse($('#geojson').val()),
            joinBy: ["CAWID", 0],
            keys: ["CAWID", 'value'],
            name: 'Ward Data',
            states: {
                hover: {
                    color: '#a4edba'
                }
            },
            dataLabels: {
                enabled: true,
                format: '{point.properties.CASWARD}'
            }
        }]
    });
}

function showLoader() {
    $('.ajax-loader ').show();
}

function hideLoader() {
    $('.ajax-loader ').fadeOut();
}
var ajaxError = function (object) {
    console.log("An error has occured processing your request.");
    hideLoader();
};
var ajaxAlways = function (object) {
    hideLoader();
};