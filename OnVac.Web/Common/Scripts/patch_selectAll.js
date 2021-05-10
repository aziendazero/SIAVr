function registerCheckClick(idChkAll) {
    var chk = document.getElementById(idChkAll);
    if (chk != null) {
        chk.onclick = function () { wzMsDatagrid_SelDesellAll(chk, 0); };
    }
}