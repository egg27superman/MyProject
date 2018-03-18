$(function () {
    $(".btnDelete").click(function () {
        var thiscontrol = $(this);
        var controlcount = $("#controlsCount").val();
        
        if (controlcount >= 2) {
            controlcount--;
            $("#controlsCount").val(controlcount);

            var num = thiscontrol.attr("value");
            $("#div" + num).remove();
        }
        else {
            alert("can not remove all upload control");
        }
    });

    $("#btnAdd").click(function () {
        var maxnum = $("#maxNum").val();
        maxnum++;

        var control = $("#upload div:first");

        var copycontrol = control.clone(true).attr("id", "div" + maxnum);
        $(copycontrol).find("button[type='button']").attr("value", maxnum);
        $(copycontrol).find("input[type='file']").attr("name", "uploadfile" + maxnum);
        $("#upload").append(copycontrol);

        var controlcount = $("#controlsCount").val();
        controlcount++;
        $("#controlsCount").val(controlcount);

        $("#maxNum").val(maxnum);

    });

    $("#uploadNewsImage").change(function () {
        //alert($("#uploadNewsImage").val());
        $("#newsImage").attr("src", "");
    });
})


