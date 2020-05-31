$(document).ready(function () {

    $(".messageInput").bind("input",function () {

        if (event.keyCode == 13) {
            console.log(321);
        }

        var crutch = $('</p>').text($(".messageInput").val()).css({ "font-size": "20px" });
        $(".dialog").append($(crutch).css({ "display": "none" }));

        var textWidth = $(crutch).width();

        $(".dialog").remove(crutch);

        var heigth = Math.trunc((textWidth / $(".messageInput").width()));

        if (heigth <= 5) {
            var input = 1 + heigth;
            $(".messageInput").css({ 'overflow-y': 'hidden' });
            $(".dialog").css({ 'grid-template-rows': '30fr ' + input + 'fr ' });
        }
        else {
            $(".messageInput").css({ 'overflow-y': 'scroll' });
        }

    });

    $(".messageInput").keyup(function () {


    });
});

