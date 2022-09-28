$(document).ready(function () {
    $('.ajax-loader').hide();
    $('.modal-link').fadeIn();
    $("form:not(.form-paginated)").off('submit').on("submit",
        function (e) {
            if ($("form").valid()) {
                $('.btn-success').prop('disabled', 'disabled');
                $('.ajax-loader-next').show();
            } else {
                return false;
            }
        });
    if ($(window).width() < 767) {
        $('body').removeClass('nav-sm');
        $('body').addClass('nav-md');
        $('.ajax-loader').hide();
    } else {
        $('body').addClass('nav-sm');
        $('body').removeClass('nav-md');
    }
    setTimeout(function () {
        $('.col-md-3.left_col').height($('.main_container').height());
    }, 1000);

    $('.lnk-modal').fadeIn();
    var n = '<div class="modal-dialog"><div class="modal-content"><div class="modal-header bg-success preloader"><h4 class="modal-title text-center">Loading....<\/h4><\/div><div class="modal-body"><p class="text-center"> <i class="fa  fa-cog fa-spin fa-3x fa-fw"><\/i><span class="sr-only">Loading...<\/span> <\/p><\/div><div class="modal-footer"><button type="button" class="btn btn-default pull-left" data-dismiss="modal">Close<\/button><\/div><\/div><\/div>';
    $('.no-typing').keydown(function (e) {
        e.preventDefault();
        return false;
    });
    $('body').on("click", ".btn-approve-status", function () {
        $('#action').val('4');
    });
    $('body').on("click", ".btn-reject-status", function () {
        if ($('#Notes').val() === '') {
            $('.notes-error').fadeIn();
            return false;
        }
        $('.notes-error').fadeOut();
        $('#action').val('5');
    });
    $('body').on("click", ".btn-save-with-notes", function () {
        if ($(this).parents('form').find('#Notes').val() === '') {
            $('.notes-error').fadeIn();
            return false;
        }
        $('.notes-error').fadeOut();
        $('#action').val('3');
    });
    $('body').on("click", ".btn-approve", function () {
        $('#action').val('3');
    });

    $('.btn-filter-report').on("click", function () {
        $('#Option').val('filter');
        // alert($('#Option').val());
    });
    $('.btn-export ').on("click", function () {
        $('#Option').val('export');
        //alert($('#Option').val());
    });

    $('body').on("click", ".btn-reject", function () {
        if ($('#Notes').val() === '') {
            $('.notes-error').fadeIn();
            return false;
        }
        $('.notes-error').fadeOut();
        $('#action').val('4');
    });
    $('.modal-link,.lnk-modal').off('click').on("click", function (t) {
        t.preventDefault();
        $("#popup").html(n);
        $("#popup").load($(this).attr("href"), function () {
            $(".checkbox").each(function () {
                if ($(this).prop("checked") === false) {
                    $(this).after('<input type="hidden" name="' + $(this).attr("name") + '" value="false">');
                }
            });
        });
        $("#popup").modal("show");
        $("#popup").modal({ keyboard: 0 });
    });
    $("#check-all").click(function () {
        if ($(this).prop("checked") === true) {
            $('.table input:checkbox').prop('checked', true);
        } else {
            $('.table input:checkbox').prop('checked', false);
        }
    });
    $('.select-searchable,.filter-list').select2();

    var col1Heigth = $('.col-one').height();
    var col2Heigth = $('.col-two').height();
    if (col1Heigth > col2Heigth) {
        $('.col-two').height(col1Heigth);
    } else {
        $('.col-one').height(col2Heigth);
    }

    //$('.date').datetimepicker({
    //    format: 'YYYY/MM/DD',
    //    // maxDate: moment(),
    //    useCurrent: false
    //});
    $('.date').datetimepicker({
        format: 'YYYY/MM/DD',
        maxDate: moment().add(1, 'h'),
        useCurrent: false
    });
    var maxDate = moment().add(14, 'd');
   
    $('.visit-date').datetimepicker({
        format: 'YYYY/MM/DD',
        maxDate: moment().add(14, 'd'),
        minDate: moment().add(-30, 'd'),
        useCurrent: false
    });
    
    $('.date-future').datetimepicker({
        format: 'YYYY/MM/DD',
        useCurrent: false
    });
    $(".months").datetimepicker({
        format: 'MMM YYYY',
        viewMode: 'months'
    });
    //$('select[multiple]').multiselect({
    //    enableFiltering: false,
    //    includeSelectAllOption: true
    //});

    $('.pagination a:not(.get .pagination a)').on('click', function () {
        var page = $.urlParam($(this).attr('href'), 'page');
        if (page !== 0) {
            $('#pagination-page').val(page);
            $('#Option').val('filter');
            $('.form-paginated').trigger('submit');
        }
        return false;
    });
    $.urlParam = function (url, shows) {
        var results = new RegExp('[\\?&]' + shows + '=([^&#]*)').exec(url);
        if (!results) { return 0; } return results[1] || 0;
    }
    $('.nav.side-menu .parent')
        .hover(function () {
            $(this).find('ul').stop().slideDown();
        }, function () {
            $(this).find('ul').stop().slideUp();
        });
    $('.child_menu').removeAttr('style');
    $('#sidebar-menu li.active').addClass('current-page');
    $('#sidebar-menu li.current-page').removeClass('active');
    $("#check-all").click(function () {
        if ($(this).prop("checked") === true) {
            $('.table input:checkbox').prop('checked', true);
        } else {
            $('.table input:checkbox').prop('checked', false);
        }
    });
    $('.btn-reset').on('click',
        function (e) {
            e.preventDefault();
            $('.filter-list').val('').trigger('change');
            $('.filter-list option').removeAttr('selected');
            $('.select-searchable').val('').trigger('change');
            $('.select-searchable option').removeAttr('selected');

            $(':input').each(function () {
                var type = this.type;
                var tag = this.tagName.toLowerCase(); // normalize case
                // to reset the value attr of text inputs,
                // password inputs, fileUpload and textareas
                if (type === 'text' || type === 'password' || tag === 'textarea' || type === 'file')
                    this.value = "";
                // checkboxes and radios need to have their checked state cleared
                //else if (type == 'radio')
                //    this.checked = false;
                else if (type === 'checkbox')
                    this.checked = false;
                // select elements need to have their 'selectedIndex' property set to -1
                // (this works for both single and multiple select elements)
                else if (tag === 'select') {
                    this.selectedIndex = 0;
                }
            });
            $(':input.multiple').each(function () {
                this.selectedIndex = -1;
                $('.multiselect-selected-text').text('None selected');
            });
            return false;
        });
    //if ($('#DisabilityTypeId').val() === null) {
    //    $('#DisabilityRequires24HrCareId,#DisabilityCaregiverId').prop('disabled', 'disabled');
    //} else {
    //    $('#DisabilityRequires24HrCareId,#DisabilityCaregiverId').prop("disabled", false);
    //}
    $('.with-other').off('change').on('change', function () {
        var selected = $(this).find('option:selected').text();

        if (selected.indexOf('Other') > 0) {
            $(this).parent().find('.form-control-other').prop("disabled", false);
        } else {
            $(this).parent().find('.form-control-other').prop('disabled', 'disabled');
        }
    });
    $('.change-form,.complaint-form').on('submit', function (e) {
        var failed = false;

        if ($('.supporting-document').val() !== undefined && $('.supporting-document').val().toLowerCase().indexOf(".pdf") < 0) {
            $('.form-error').fadeIn();
            failed = true;
        } else {
            $('.form-error').fadeOut();
        }

        if (failed === true) {
            e.preventDefault();

            $(this).parent().find('.btn-success').prop("disabled", false);
            return false;
        }
    });
    $('.edit-change-form,.edit-complaint-form').on('submit', function (e) {
        var failed = false;
        if ($('.supporting-document').val().length !== 0 && $('.supporting-document').val().toLowerCase().indexOf(".pdf") < 0) {
            $('.form-error').fadeIn();
            failed = true;
        } else {
            $('.form-error').fadeOut();
        }

        if (failed === true) {
            e.preventDefault();
            return false;
        }
    });
    $('#table,#table-only').DataTable({
        // dom: 'lBfrtip',
        dom: 'rtip',
        "pageLength": 10,
        "order": [],
        "ordering": false,
        "oLanguage": {
            "oPaginate": {
                "sPrevious": "<i class='fa fa-angle-double-left'></i>",
                "sNext": "<i class='fa fa-angle-double-right'></i>"
            }
        }
    });
    $('#table-filter,.table-filter').DataTable({
        // dom: 'lBfrtip',
        dom: 'lfrtip',

        "pageLength": 10,
        "order": [],
        "ordering": false,
        "oLanguage": {
            "oPaginate": {
                "sPrevious": "<i class='fa fa-angle-double-left'></i>",
                "sNext": "<i class='fa fa-angle-double-right'></i>"
            }
        },
    });
    $('#table-filter-sort').DataTable({
        // dom: 'lBfrtip',
        dom: 'lfrtip',

        "pageLength": 10,
        "order": [],
        "ordering": true,
        "oLanguage": {
            "oPaginate": {
                "sPrevious": "<i class='fa fa-angle-double-left'></i>",
                "sNext": "<i class='fa fa-angle-double-right'></i>"
            }
        },
    });

    // Handle form submission event
    $('.datatable-form').on('submit', function (e) {
        var form = this;
        // Encode a set of form elements from all pages as an array of names and values
        var table = $('#table-filter').DataTable();
        var params = table.$('input,select,textarea').serializeArray();
        // Iterate over all form elements
        $.each(params, function () {
            // If element doesn't exist in DOM
            if (!$.contains(document, form[this.name])) {
                // Create a hidden element
                $(form).append(
                    $('<input>')
                        .attr('type', 'hidden')
                        .attr('name', this.name)
                        .val(this.value)
                );
            }
        });
    });
}); /*Closing document.ready*/

var app = angular.module("clientApp", ["AxelSoft", "ngSanitize"]);
app.controller('clientCtrl', ['$scope', '$http', '$filter', function (scope, http, filter) {
    var rootUrl = $('#RootUrl').val();
    var ajaxError = function (object) {
        console.log("An error has occured processing your request.");
        hideLoader();
    };
    var ajaxAlways = function (object) {
        hideLoader();
    };
    scope.duration = -1;
    scope.count = $('#count').val();
}]);

function showLoader() {
    $('.ajax-loader ').fadeIn();
}

function hideLoader() {
    $('.ajax-loader ').fadeOut();
}
function indexByKeyValue(arraytosearch, key, valuetosearch) {
    for (var i = 0; i < arraytosearch.length; i++) {
        if (arraytosearch[i][key] === valuetosearch) {
            return i;
        }
    }
    return null;
}