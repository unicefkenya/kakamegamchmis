$(document).ready(function () {
    if ($(window).width() < 767) {
        $('body').removeClass('nav-sm');
        $('body').addClass('nav-md');
        $('.ajax-loader').hide();
    } else {
        $('body').addClass('nav-sm');
        $('body').removeClass('nav-md');
    }

    $('.lnk-modal').fadeIn();
    var n = '<div class="modal-dialog"><div class="modal-content"><div class="modal-header bg-success"><h4 class="modal-title text-center">Loading....<\/h4><\/div><div class="modal-body"><p class="text-center"> <i class="fa  fa-cog fa-spin fa-3x fa-fw"><\/i><span class="sr-only">Loading...<\/span> <\/p><\/div><div class="modal-footer"><button type="button" class="btn btn-default pull-left" data-dismiss="modal">Close<\/button><\/div><\/div><\/div>';

    $(document).on("click", ".modal-link,.lnk-modal", function (t) {
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
    $('.select-searchable,.filter-list').select2();

   $('.no-typing').keydown(function (e) {
        e.preventDefault();
        return false;
    });

    $('.date').datetimepicker({
        format: 'YYYY/MM/DD',
        // maxDate: moment(),
        useCurrent: false
    });
    $(".months").datetimepicker({
        format: 'MMM YYYY',
        viewMode: 'months'
    });
  
});

function showLoader() {
    $('.ajax-loader ').fadeIn();
}

function hideLoader() {
    $('.ajax-loader ').fadeOut();
}