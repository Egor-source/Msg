$(document).ready(function () {

    var messages = document.querySelector('.messages');

    $('.messages').scrollTop(messages.scrollHeight);

    $(".messageInput").bind("input",function () {


        var crutch = $('</p>').text($(".messageInput").val()).css({ "font-size": "20px" });
        $(".dialog").append($(crutch).css({ "display": "none" }));

        var textWidth = $(crutch).width();

        $(".dialog").remove(crutch);

        var heigth = Math.trunc((textWidth / $(".messageInput").width()));

        if (heigth <= 4) {
            var input = 1 + heigth;
            $(".messageInput").css({ 'overflow-y': 'hidden' });
            $(".dialog").css({ 'grid-template-rows': '30fr ' + input + 'fr ' });
        }
        else {
            $(".messageInput").css({ 'overflow-y': 'scroll' });
        }

    });

    $(".messageInput").keyup(function () {

        if (event.keyCode == 13) {

            hub.server.sendMessage(obj.id, $('.interlocutorId').val(), $(this).val());

            var date = new Date();

            $(".messages").append('<div class="message sender"><img src="/Content/Photo/' + obj.Photo + '" class="UserPhoto"> <p class="Name">' + obj.Name + '</p><p class="Surname">' + obj.Surname + '</p><div class="date">' + date.getHours() + ":" + date.getMinutes() +'</div><p>' + $(this).val() + '</p></div>');

            var messages = document.querySelector('.messages');

            $('.messages').scrollTop(messages.scrollHeight);
        }
    });

    
});




