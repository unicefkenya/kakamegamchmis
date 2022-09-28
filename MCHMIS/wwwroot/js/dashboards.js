var app = angular.module("clientApp", ["AxelSoft", "ngSanitize"]);
app.controller('clientCtrl', ['$scope', '$http', '$filter', function (scope, http, $filter) {
    scope.count = 1;
    scope.total = 1;
    scope.indicator = 1;
    scope.paymentIndicator = 5;
    scope.donor = 'All';
    scope.categoryId = '0';
    scope.itemsPerPage = 20;

    var root = $('#RootUrl').val();
    scope.sourceofFundsChanged = function () {
        if (scope.donor === 'All') {
            scope.data.Beneficiaries = scope.allBeneficiaries;
        }
        else if (scope.donor === 'Government of Uganda') {
            scope.data.Beneficiaries = $filter('filter')(scope.allBeneficiaries, { DonorId: 1 });
            /*
            scope.data.Beneficiaries = $filter("filter")(scope.allBeneficiaries,
                function (listItem) {
                    return scope.GoUDistricts.indexOf(listItem.DistrictName) != -1;
                }
            );*/
        } else {
            scope.data.Beneficiaries = $filter('filter')(scope.allBeneficiaries, { DonorId: 2 });
            /*
            scope.data.Beneficiaries = $filter("filter")(scope.allBeneficiaries,
                function (listItem) {
                    return scope.GoUDistricts.indexOf(listItem.DistrictName) === -1;
                }
            );
            */
        }
        var male = 0;
        var female = 0;

        for (var i = 0; i < scope.data.Beneficiaries.length; i++) {
            var item = scope.data.Beneficiaries[i];
            male += (item.Male);
            female += (item.Female);
        }

        scope.data.Male = male;
        scope.data.Female = female;
        scope.data.Total = male + female;
        BeneficiaryChats(scope.data);
    };// End Function

    scope.filterComplaints = function () {
        scope.data.Complaints = scope.allComplaints;

        if (scope.categoryId === '1') {
            scope.data.Complaints = $filter('filter')(scope.data.Complaints, { CategoryId: 1 });
        } else if (scope.categoryId === '2') {
            scope.data.Complaints = $filter('filter')(scope.data.Complaints, { CategoryId: 2 });
        }
        if (scope.donor === '1') {
            scope.data.Complaints = $filter('filter')(scope.data.Complaints, { DonorId: 1 });
            /*
            scope.data.Complaints = $filter("filter")(scope.allComplaints,
                function (listItem) {
                    return scope.GoUDistricts.indexOf(listItem.DistrictName) != -1;
                }
            );
            */
        } else if (scope.donor === '2') {
            scope.data.Complaints = $filter('filter')(scope.data.Complaints, { DonorId: 2 });
            /*
            scope.data.Complaints = $filter("filter")(scope.allComplaints,
                function (listItem) {
                    return scope.GoUDistricts.indexOf(listItem.DistrictName) === -1;
                }
            );
            */
        }

        var resolved = 0;
        var registered = 0;

        for (var i = 0; i < scope.data.Complaints.length; i++) {
            var item = scope.data.Complaints[i];
            registered += (item.Registered);
            resolved += (item.Resolved);
        }

        scope.data.Registered = registered;
        scope.data.Resolved = resolved;
        scope.data.Total = registered + resolved;

        var total = 0;
        for (var i = 0; i < scope.data.Complaints.length; i++) {
            var item = scope.data.Complaints[i];
            total += (item.Resolved + item.Registered);
        }
        scope.total = total;

        ComplaintsCharts(scope.data);
    };// End Function

    scope.refreshBeneficiaryData = function () {
        var url = root + 'Beneficiaries/RefreshData';

        if (scope.period !== 0) {
            showLoader();
            http.post(url, { IndicatorId: scope.indicator, CycleCode: scope.period }).success(function (data) {
                scope.data = data;
                scope.allBeneficiaries = data.Beneficiaries;
                if (scope.donor === 'All') {
                    BeneficiaryChats(scope.data);
                }
                else {
                    scope.sourceofFundsChanged();
                }

                hideLoader();
            }).error(ajaxError);
        }
    }

    scope.refreshPaymentData = function () {
        var url = 'Payments/RefreshData';

        if (scope.period !== 0) {
            showLoader();
            http.post(url, { IndicatorId: scope.indicator, CycleCode: scope.period }).success(function (data) {
                scope.data = data;
                scope.allPayments = data.Payments;
                PaymentsChats(scope.data);

                hideLoader();
            }).error(ajaxError);
        }
    }
    scope.refreshComplaintData = function () {
        var url = root + 'Complaints/RefreshData';
        scope.donor = 'All';
        showLoader();
        http.post(url, { IndicatorId: scope.paymentIndicator, CycleCode: scope.period }).success(function (data) {
            scope.data = data;
            scope.allComplaints = data.Complaints;
            ComplaintsCharts(scope.data);
            hideLoader();
        }).error(ajaxError);
    }
    scope.indexCount = function (newPageNumber) {
        alert();
        scope.serial = newPageNumber * 10 - 9;
    }
}]).filter('inArray', function ($filter) {
    return function (list, arrayFilter, element) {
        if (arrayFilter) {
            return $filter("filter")(list, function (listItem) {
                return arrayFilter.indexOf(listItem[element]) != -1;
            });
        }
    };
});

// var colors = ['#006091', '#7B99A9', '#374E5A', '#839118', '#9C8C21', '#98918F'];
var colors = ['#5B9BD4', '#EC7C30', '#9B9B9B', '#8B6529', '#839118', '#5b9f35', '#98918F'];
var colors = ['#5B9BD4', '#EC7C30', '#9B9B9B', '#8B6529', '#839118', '#5b9f35', '#98918F'];
var colorsb = ['#9B9B9B', '#FFC000', '#EC7C30', '#5B9BD4'];

function BeneficiaryChats(data) {
    var title = 'Number of Registered Mothers';
    //var period = $("#PeriodId option:selected").text();
    //period = period === 'Select' ? '' : period;
    //var indicator = $("#IndicatorId option:selected").val();
    //var title = $("#IndicatorId option:selected").val() === '5'
    //    ? $("#IndicatorId option:selected").text() + ' ' + period
    //    : $("#IndicatorId option:selected").text() + ' ' + period;
    //if (indicator === '1' || indicator === '2') {
    //    title = $("#IndicatorId option:selected").text() + ' as at end of ' + period;
    //}
    //if (indicator === '8') {
    //    title = 'Total number of beneficiaries deactivated / Deregistered as at end of ' + period;
    //}

    $('.lbltitle').text(title);

    Highcharts.chart('registration-barchart', {
        chart: {
            type: 'column'
        },
        title: {
            text: title,
            /* style: {
                 fontWeight: 'bold',
                 fontSize: '14px'
             }*/
        },
        colors: colors,
        xAxis: {
            crosshair: true,
            categories: ['']
        },
        yAxis: {
            min: 0,
            labels: {
                formatter: function () {
                    return this.value;
                }
            },

            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'center',
            //x: -30,
            verticalAlign: 'bottom',
            y: 5,
            floating: false,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            headerFormat: ' ',
            pointFormat: '<b>{series.name}</b>: {point.y}'
        },
        plotOptions: {
            column: {
                pointWidth: 65,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'gray',
                    formatter: function () {
                        return (this.y);
                    }
                }
            }
        },
        series: [{
            name: 'Registered',
            data: [data.registered]
        },
        {
            name: 'Eligible Mothers',
            data: [data.eligible]
        },
        {
            name: 'Ineligible Mothers',
            data: [data.ineligible]
        },

        ]
    });

    Highcharts.chart('totals-piechart', {
        chart: {
            type: 'pie'
        },
        title: {
            text: '% ' + title
        },
        colors: colors,
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },

        plotOptions: {
            pie: {
                allowPointSelect: true,
                showInLegend: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        series: [{
            name: 'Total',
            colorByPoint: true,
            data: [
                {
                    name: 'Eligible Mothers',
                    y: data.eligible
                },
                {
                    name: 'Ineligible Mothers',
                    y: data.eligible
                }
            ]
        }]
    });

    setTimeout(function () {
        $('.highcharts-container').addClass('hvr-glow');
    }, 2000);
}
function CommunityValidationChats(data) {
    var colors = ['#008000', '#ce9a00', '#dc3545', '#5B9BD4'];
    var title = 'Number of Mothers';
    //var period = $("#PeriodId option:selected").text();
    //period = period === 'Select' ? '' : period;
    //var indicator = $("#IndicatorId option:selected").val();
    //var title = $("#IndicatorId option:selected").val() === '5'
    //    ? $("#IndicatorId option:selected").text() + ' ' + period
    //    : $("#IndicatorId option:selected").text() + ' ' + period;
    //if (indicator === '1' || indicator === '2') {
    //    title = $("#IndicatorId option:selected").text() + ' as at end of ' + period;
    //}
    //if (indicator === '8') {
    //    title = 'Total number of beneficiaries deactivated / Deregistered as at end of ' + period;
    //}

    $('.lbltitle').text(title);

    Highcharts.chart('cv-barchart', {
        chart: {
            type: 'column'
        },
        title: {
            text: title,
            /* style: {
                 fontWeight: 'bold',
                 fontSize: '14px'
             }*/
        },
        colors: colors,
        xAxis: {
            crosshair: true,
            categories: ['']
        },
        yAxis: {
            min: 0,
            labels: {
                formatter: function () {
                    return this.value;
                }
            },

            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'center',
            //x: -30,
            verticalAlign: 'bottom',
            y: 5,
            floating: false,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            headerFormat: ' ',
            pointFormat: '<b>{series.name}</b>: {point.y}'
        },
        plotOptions: {
            column: {
                pointWidth: 65,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'gray',
                    formatter: function () {
                        return (this.y);
                    }
                }
            }
        },
        series: [{
            name: 'Low Variance',
            data: [data.low]
        },
        {
            name: 'Medium Variance',
            data: [data.medium]
        },
        {
            name: 'High Variance',
            data: [data.high]
        }]
    });

    Highcharts.chart('totals-piechart', {
        chart: {
            type: 'pie'
        },
        title: {
            text: '% ' + title
        },
        colors: colors,
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },

        plotOptions: {
            pie: {
                allowPointSelect: true,
                showInLegend: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        series: [{
            name: 'Total',
            colorByPoint: true,
            data: [
                {
                    name: 'Low Variance',
                    y: data.low
                },
                {
                    name: 'Medium Variance',
                    y: data.medium
                },
                {
                    name: 'High Variance',
                    y: data.high
                }
            ]
        }]
    });

    setTimeout(function () {
        $('.highcharts-container').addClass('hvr-glow');
    }, 2000);
}
function EnrolmentChats(data) {
    var title = 'Number of Mothers';
    //var period = $("#PeriodId option:selected").text();
    //period = period === 'Select' ? '' : period;
    //var indicator = $("#IndicatorId option:selected").val();
    //var title = $("#IndicatorId option:selected").val() === '5'
    //    ? $("#IndicatorId option:selected").text() + ' ' + period
    //    : $("#IndicatorId option:selected").text() + ' ' + period;
    //if (indicator === '1' || indicator === '2') {
    //    title = $("#IndicatorId option:selected").text() + ' as at end of ' + period;
    //}
    //if (indicator === '8') {
    //    title = 'Total number of beneficiaries deactivated / Deregistered as at end of ' + period;
    //}

    $('.lbltitle').text(title);

    Highcharts.chart('enrolment-barchart', {
        chart: {
            type: 'column'
        },
        title: {
            text: title,
            /* style: {
                 fontWeight: 'bold',
                 fontSize: '14px'
             }*/
        },
        colors: colors,
        xAxis: {
            crosshair: true,
            categories: ['']
        },
        yAxis: {
            min: 0,
            labels: {
                formatter: function () {
                    return this.value;
                }
            },

            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'center',
            //x: -30,
            verticalAlign: 'bottom',
            y: 5,
            floating: false,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            headerFormat: ' ',
            pointFormat: '<b>{series.name}</b>: {point.y}'
        },
        plotOptions: {
            column: {
                pointWidth: 65,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'gray',
                    formatter: function () {
                        return (this.y);
                    }
                }
            }
        },
        series: [{
            name: 'Enrolled in the Period',
            data: [data.enrolled]
        },
        {
            name: 'On Waiting List',
            data: [data.waiting]
        },
        //{
        //    name: 'Peak Mothers Enrolled',
        //    data: [data.total]
        //}
        ]
    });

    Highcharts.chart('totals-piechart', {
        chart: {
            type: 'pie'
        },
        title: {
            text: '% ' + title
        },
        colors: colors,
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },

        plotOptions: {
            pie: {
                allowPointSelect: true,
                showInLegend: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        series: [{
            name: 'Total',
            colorByPoint: true,
            data: [
                {
                    name: 'Enrolled in the Period',
                    y: data.active
                },
                {
                    name: 'On Waiting List',
                    y: data.waiting
                }
            ]
        }]
    });

    setTimeout(function () {
        $('.highcharts-container').addClass('hvr-glow');
    }, 2000);
}
function PaymentsChats(data) {
    var period = $("#PeriodId option:selected").text();
    period = period === 'Select' ? '' : period;
    var indicator = $("#IndicatorId option:selected").val();

    var title = $("#IndicatorId option:selected").val();

    if (indicator == 338 || indicator == 339) {
        title = $("#IndicatorId option:selected").text() + ' as at end of ' + period;
    } else {
        title = $("#IndicatorId option:selected").text();
    }

    $('.lbltitle').text(title);

    Highcharts.chart('payments-barchart', {
        chart: {
            type: 'column'
        },
        title: {
            text: title,
            /* style: {
                 fontWeight: 'bold',
                 fontSize: '14px'
             }*/
        },
        colors: colors,
        xAxis: {
            crosshair: true,
            categories: ['']
        },
        yAxis: {
            allowDecimals: false,
            min: 0,
            labels: {
                formatter: function () {
                    return this.value;
                }
            },

            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'center',
            //x: -30,
            verticalAlign: 'bottom',
            y: 5,
            floating: false,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            headerFormat: ' ',
            pointFormat: '<b>Payment Point {series.name}</b>: {point.y}'
        },
        plotOptions: {
            column: {
                pointWidth: 65,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'gray',
                    formatter: function () {
                        return (this.y).toLocaleString();
                    }
                }
            }
        },
        series: [{
            name: '1st ANC',
            data: [data.stage1]
        },
       {
           name: 'Delivery',
           data: [data.stage2]
       },
       {
           name: '6th Week',
           data: [data.stage3]
       },
       {
           name: '6th Month',
           data: [data.stage4]
       },
       {
           name: '9th Month',
           data: [data.stage5]
       },
       {
           name: '18th Month',
           data: [data.stage6]
       }

        ]
    });

    Highcharts.chart('totals-piechart', {
        chart: {
            type: 'pie'
        },
        title: {
            text: '% ' + title
        },
        colors: colors,
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },

        plotOptions: {
            pie: {
                allowPointSelect: true,
                showInLegend: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        series: [{
            name: 'Total',
            colorByPoint: true,
            data: [
                {
                    name: '1st ANC',
                    y: data.stage1
                },
                {
                    name: 'Delivery',
                    y: data.stage2
                },
                {
                    name: '6th Week ',
                    y: data.stage3
                },
                {
                    name: '6th Month',
                    y: data.stage4
                },
                {
                    name: '9th Month',
                    y: data.stage5
                },
                {
                    name: '18th Month',
                    y: data.stage6
                }
            ]
        }]
    });

    setTimeout(function () {
        $('.highcharts-container').addClass('hvr-glow');
    }, 2000);
}
function CasesChats(data) {
    var title = 'Number of Mothers';

    $('.lbltitle').text(title);

    Highcharts.chart('cases-barchart', {
        chart: {
            type: 'column'
        },
        title: {
            text: title,
            /* style: {
                 fontWeight: 'bold',
                 fontSize: '14px'
             }*/
        },
        colors: colors,
        xAxis: {
            crosshair: true,
            categories: ['']
        },
        yAxis: {
            min: 0,
            labels: {
                formatter: function () {
                    return this.value;
                }
            },

            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'center',
            //x: -30,
            verticalAlign: 'bottom',
            y: 5,
            floating: false,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            headerFormat: ' ',
            pointFormat: '<b>{series.name}</b>: {point.y}'
        },
        plotOptions: {
            column: {
                pointWidth: 65,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'gray',
                    formatter: function () {
                        return (this.y);
                    }
                }
            }
        },
        series: [{
            name: 'Updated',
            data: [data.updated]
        },
        {
            name: 'Not Updated',
            data: [data.missed]
        }]
    });

    Highcharts.chart('totals-piechart', {
        chart: {
            type: 'pie'
        },
        title: {
            text: '% ' + title
        },
        colors: colors,
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },

        plotOptions: {
            pie: {
                allowPointSelect: true,
                showInLegend: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        series: [{
            name: 'Total',
            colorByPoint: true,
            data: [
                {
                    name: 'Updated',
                    y: data.updated
                },
                {
                    name: 'Not Updated',
                    y: data.missed
                }
            ]
        }]
    });

    setTimeout(function () {
        $('.highcharts-container').addClass('hvr-glow');
    }, 2000);
}
function ComplaintsChats(data) {
    var title = 'Number of Complaints';

    $('.lbltitle').text(title);

    Highcharts.chart('complaints-barchart', {
        chart: {
            type: 'column'
        },
        title: {
            text: title,
            /* style: {
                 fontWeight: 'bold',
                 fontSize: '14px'
             }*/
        },
        colors: colors,
        xAxis: {
            crosshair: true,
            categories: ['']
        },
        yAxis: {
            min: 0,
            labels: {
                formatter: function () {
                    return this.value;
                }
            },

            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'center',
            //x: -30,
            verticalAlign: 'bottom',
            y: 5,
            floating: false,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false,
            itemStyle: { "fontSize": "11px" }
        },
        tooltip: {
            headerFormat: ' ',
            pointFormat: '<b>{series.name}</b>: {point.y}'
        },
        plotOptions: {
            column: {
                pointWidth: 65,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'gray',
                    formatter: function () {
                        return (this.y);
                    }
                }
            }
        },
        series: [{
            name: 'Open Within SLA',
            data: [data.openWithinSLA]
        }, {
            name: 'Open Outside SLA',
            data: [data.openOutsideSLA]
        }, {
            name: 'Resolved Within SLA',
            data: [data.resolvedWithinSLA]
        },
        {
            name: 'Resolved Outside SLA',
            data: [data.resolvedOutsideSLA]
        }]
    });

    Highcharts.chart('totals-piechart', {
        chart: {
            type: 'pie'
        },
        title: {
            text: '% ' + title
        },
        colors: colors,
        legend: {
            itemStyle: { "fontSize": "11px" }
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },

        plotOptions: {
            pie: {
                allowPointSelect: true,
                showInLegend: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        series: [{
            name: 'Total',
            colorByPoint: true,
            data: [
                {
                    name: 'Open Within SLA',
                    y: data.openWithinSLA
                }, {
                    name: 'Open Outside SLA',
                    y: data.openOutsideSLA
                }, {
                    name: 'Resolved Within SLA',
                    y: data.resolvedWithinSLA
                },
        {
            name: 'Resolved Outside SLA',
            data: [data.resolvedOutsideSLA]
        }
            ]
        }]
    });

    setTimeout(function () {
        $('.highcharts-container').addClass('hvr-glow');
    }, 2000);
}

$(document).ready(function () {
    Highcharts.setOptions({
        lang: {
            thousandsSep: ','
        },
        credits: {
            enabled: false
        }
    });
    $(".btn-export").click(function () {
        var scope = angular.element(document.querySelector('body')).scope();
        scope.$apply(function (filter) {
            scope.OldPagination = scope.itemsPerPage;
            scope.itemsPerPage = '';

            setTimeout(function () {
                var fileName = $('.lbltitle').text();
                $(".table").table2excel({
                    // exclude CSS class
                    exclude: ".noExl,.ng-hide",
                    name: fileName + " Data",
                    filename: fileName//do not include extension
                });
                var scope = angular.element(document.querySelector('body')).scope();
                scope.$apply(function (filter) {
                    scope.itemsPerPage = scope.OldPagination;
                });
            }, 100);
        });
    });
    $("body").on('click', 'dir-pagination-controls a', function () {
        $('.to-top').click();
        $('html,body').animate({
            scrollTop: $(".panel-body").offset().top
        });
    });
});
function showLoader() {
    $('.ajax-loader ').fadeIn();
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