$(function () {
    $.datepicker.setDefaults({
        dateFormat: 'dd/mm/yy'
    })

    $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy' }).val();

    $("#tabs").tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
    $("#tabs li").removeClass("ui-corner-top").addClass("ui-corner-left");

    $(".tableClass tr:nth-child(odd)").addClass("odd-row");
    $(".tableClass td:first-child, table th:first-child").addClass("first");
    $(".tableClass td:last-child, table th:last-child").addClass("last");

    //if ($("#Menu").length > 0)
    //{
        
    //    //$('#Menu').easytree();
    //    //alert(1);
    //}

    //$("#Menu").menu({
    //    items: "> :not(.ui-widget-header)"
    //    //select: function (event, ui) {
    //    //    $('.menu_item_selected', this).removeClass('menu_item_selected');
    //    //    ui.item.addClass('menu_item_selected');
    //    //}
    //});

    //$("#Menu").find("a").click(function () {
    //    alert(1);
    //    $("#Menu").find("a").removeClass("ui-state-focus");//remove if something was selected
    //    $(this).addClass("ui-state-focus");//add a selected class
    //});

    //if ($("#Menu").length>0)
    //{
    //    $('#Menu').easytree();
    //}

    //$("#btnUpdateNewsImage").click(function () {
    //    $('#btnUpdateNewsImage').toggle();
    //    $('#btnGiveupNewsImage').toggle();
    //    $('#newsImage').toggle();
    //    $('#upload').toggle();
    //});

    //$("#btnGiveupNewsImage").click(function () {
    //    $('#btnUpdateNewsImage').toggle();
    //    $('#btnGiveupNewsImage').toggle();
    //    $('#newsImage').toggle();
    //    $('#upload').toggle();
    //});

    //$("#btnUpdateNewsImage").click();

    //$(function () {
    //    $("#tabs").tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
    //    $("#tabs li").removeClass("ui-corner-top").addClass("ui-corner-left");
    //})



    // 动态增加news content
    //$("#btnAddNewsContent").click(function () {
    //    var maxnum = $("#maxNum").val();
    //    maxnum++;

    //    var control = $("#NewsDetail div:first");

    //    var copycontrol = control.clone(true).attr("id", "NewsDetail" + maxnum);
    //    $(copycontrol).find("button[type='button']").attr("value", maxnum);
    //    $("#NewsDetail").append(copycontrol);

    //    var controlcount = $("#controlsCount").val();
    //    controlcount++;
    //    $("#controlsCount").val(controlcount);

    //    $("#maxNum").val(maxnum);

    //});


    //$(".btnDeleteNewsDetail").click(function () {
    //    var thiscontrol = $(this);
        
    //    var controlcount = $("#controlsCount").val();

    //    if (controlcount >= 2) {
    //        controlcount--;
    //        $("#controlsCount").val(controlcount);

    //        var num = thiscontrol.attr("value");
    //        alert("#NewsDetail" + num);
    //        $("#NewsDetail" + num).remove();
    //    }
    //    else {
    //        alert("can not remove all news content");
    //    }
    //});

});


