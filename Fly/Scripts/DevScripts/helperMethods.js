$(document).ready(function () {
    $(".select2").select2();
});

function ShowLoader() {
    $("#loader-wrapper").show();
    $(".btnSave").attr("disabled", "disabled");
}

function HideLoader() {
$("#loader-wrapper").hide();
    $(".btnSave").removeAttr("disabled");
}

function Notification(status, msg) {
    //$(".activationMsg").html(msg);
    //$("#ActivationModel").modal("show");

    toastr.options = {
        closeButton: true,
        positionClass: "toast-top-right",
        onclick: null,
        showDuration: 1000,
        hideDuration: 3000,
        timeOut: 50000,
        extendedTimeOut: 1000,
        showEasing: "swing",
        hideEasing: "linear",
        showMethod: "fadeIn",
        hideMethod: "fadeOut"
    };

    if (status == 1) {
        toastr['success'](msg);
    } else if (status == 2) {
        toastr['error'](msg);
    } else if (status == 3) {
        toastr['info'](msg);
    } else {
        toastr['warning'](msg);
    }
}
