var colDef = [];
var tablesub;
var deleteEnabled = true;
var LoadAtReady = true;
// if we need to custom seetings
var isSpecial = true;
var isServerSide = true;

var isActiveIndex = undefined;
var modelHeaderTitle = "";

var attachment = undefined;
var userLink = undefined;

$(document).ready(function () {
    $('body').on("click", "#addItem", function () {
        Show_edit(null);
    });

    if (LoadAtReady) {
        LoadTable();
    }
});


function GenerateTableCustomColumn() {
    colDef = [];
    if (operationIndex != undefined) {
        colDef.push({
            "targets": operationIndex,
            "data": "download_link",
            "render": function (data, type, full) {
                var html = '<a href="#!" onclick="Show_edit(' + data + ')" class="tooltips" data-toggle="tooltip" data-original-title="' + editTT + '" tooltip-placement="top" tooltip="' + editTT + '"><i class="fa fa-pencil" style="color:#00c0ff;"></i></a>';

                if (deleteEnabled) {
                    html += '&nbsp;&nbsp;<a href="#!" onclick="DeletePopup(' + data + ')" class="tooltips"  data-toggle="tooltip" data-original-title="' + deleteTT + '" tooltip-placement="top" tooltip="' + deleteTT + '"><i class="fa fa-trash" style="color:red;"></i></a>';
                }

                return html;
            }
        });
    }
    if (isActiveIndex != undefined) {
        colDef.push({
            "targets": isActiveIndex,
            "data": "CurrentYear",
            "render": function (data, type, full) {
                if (data == true) {
                    return '<i class="fa fa-check-circle-o" style="color:green;"></i>';
                } else {
                    return '<i class="fa fa-ban" style="color:red;"></i>';
                }
            }
        });
    }


    if (userLink != undefined) {
        colDef.push({
            "targets": userLink,
            "data": "ImageId",
            "render": function (data, type, full) {

                return '<a href="#!" class="pointer ProfileEdit" data-id="' + full.Id + '" > ' + full.FullName + '</a>';

            }
        });
    }
}

function LoadTable() {
    tablesub = $('#tablelookups').DataTable({
        "processing": true,
        "initComplete": function () {
            $('[data-toggle="tooltip"]').tooltip();
        },
        "order": [[ 0, "desc" ]],
        "serverSide": isServerSide,
        "filter": true,
        "bFilter": false,
        "bLengthChange": false,
        "pageLength": TablePageSize,
        "displayStart": 0,
        "orderMulti": false,

        "Paginate": true,
        //"info": false,

        "pagingType": "full_numbers",
        "language": pagingData,
        "ajax": {
            "url": ActionLoadData,

            "type": "POST",
            "datatype": "json"
        },
        dom: 'Bfrtip',
        buttons: [
         'copy', 'excel', 'pdf', 'print'
        ],
        "columnDefs": colDef,

        "columns": colData,
    });

    $('#tablelookups tbody').on('click', 'button', function () {
    });
    $("#tablelookups").on('page.dt', function () {
        var info = tablesub.page.info();
        $("#DataPage").val(info.page);
    });

    $("#tablelookups").on('order.dt', function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

    $("#tablelookups").on('search.dt', function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
}

function Search() {
    var searchtokens = document.getElementById('searchToken').value;
    tablesub.ajax.url(ActionLoadData + "?searchtoken=" + searchtokens);
    tablesub.ajax.reload(null, false);
}



function Show_edit(id) {
    ShowLoader();
    $.ajax({
        url: ActionEditData,
        type: 'post',
        data: {
            id: id
        },
        success: function (data) {
            if (modelHeaderTitle != "") {
                $("#generalModelTitle").html(modelHeaderTitle);
            }
            $("#generalModelBody").html(data);
            $("#generalModal").modal("show");
            HideLoader();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}

function Delete(id) {
    $('.deleteConfirmationBtn').button('loading');
    $.ajax({
        url: ActionDeleteData,
        type: 'post',
        data: {
            id: id
        },
        success: function (data) {
            Notification(data.responseType, data.msg);

            ClearForm();
            $('.deleteConfirmationBtn').button('reset');
            $('#deletePopup').modal('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $('.deleteConfirmationBtn').button('reset');
            $('#deletePopup').modal('hide');
        }
    });

}

function ClearForm() {
    $(".EmptyClass").val("");
    $(".EmptyClassID").attr("value", "0");
    $("input:checkbox").prop('checked', false);

    Search();

    $("#generalModal").modal("hide");

    
}

function OnSucess(data) {
    HideLoader();

    Notification(data.responseType, data.ErrorMessegesStr || data.MessegesStr);

    if (data.IsDone) {
        ClearForm();
    }

}

function OnBegin() {
    if (!Validate()) {
        return false;
    }
    ShowLoader();
   
}

function Validate() {
    var result = true;
    
    $(".select222").each(function () {
        try {
            if (!$(this).valid()) {
                result = false;
            }
        } catch (e) {
        }
    });

    $(".select2").each(function () {
        try {
            if (!$(this).valid()) {
                result = false;
            }
        } catch (e) {
        }
    });

    if (!result) {
        return false;
    } else {
        return true;
    }
}

function OnComplete() {
    HideLoader();
}


////////////////////////////////////////////////////////////
//                                                       //
//            Helper script                             //
//          Created By: Islam Osman                    //
//          Date :26-1-2016                           //
//                                                   //
///////////////////////////////////////////////////////

function DeletePopup(id, id2, id3, id4) {
    $('.deleteConfirmationBtn').attr('onclick', 'Delete("' + id + '","' + id2 + '","' + id3 + '","' + id4 + '");');
    $('#deletePopup').modal('show');
}
