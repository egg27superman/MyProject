$(function () {

    //$(".datepicker").datepicker({
    //    dateFormat: 'dd/mm/yyyy'
    //}).val();

});


function formatFloat(num, pos) {
    var size = Math.pow(10, pos);
    return Math.round(num * size) / size;
}


function banBackSpace(e) {
    var ev = e || window.event;
    var obj = ev.target || ev.srcElement;

    var t = obj.type || obj.getAttribute('type');


    var vReadOnly = obj.getAttribute('readonly');
    var vEnabled = obj.getAttribute('enabled');

    vReadOnly = (vReadOnly == null) ? false : vReadOnly;
    vEnabled = (vEnabled == null) ? true : vEnabled;

    var flag1 = (ev.keyCode == 8 && (t == "password" || t == "text" || t == "textarea")
                && (vReadOnly == true || vEnabled != true)) ? true : false;

    var flag2 = (ev.keyCode == 8 && t != "password" && t != "text" && t != "textarea")
                ? true : false;

    if (flag2) {
        return false;
    }
    if (flag1) {
        return false;
    }
}