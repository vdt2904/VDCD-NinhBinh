$(document).ready(function () {
    getsp();
});
function getsp() {
    var str = "";
    $.ajax({
        url: 'https://localhost:7274/api/getanime',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            str = '<h3>Vũ Doanh Thái</h3>'
            $('.str').html(str);


        }
    });
}