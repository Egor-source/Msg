$(document).ready(function () {

    if ($('.messages').length != 0) {
        //Прокручивает диалог в конец
        var messages = document.querySelector('.messages');

        $('.messages').scrollTop(messages.scrollHeight);
    }

    //Обработчик события input для inputa для отправки сообщения
    $(".messageInput").bind("input",function () {

        //Получает длинну введенного текста 
        var crutch = $('</p>').text($(".messageInput").val()).css({ "font-size": "20px" });
        $(".dialog").append($(crutch).css({ "display": "none" }));

        var textWidth = $(crutch).width();

        $(".dialog").remove(crutch);

        //Вычисляет больше ли длинна сообщения длинны inputa
        var heigth = Math.trunc((textWidth / $(".messageInput").width()));

        if (heigth <= 4) {

            var input = 1 + heigth;

            //Уберает прокрутку inputa
            $(".messageInput").css({ 'overflow-y': 'hidden' });

            //Изменяет размер инпута
            $(".dialog").css({ 'grid-template-rows': '1fr 30fr ' + input + 'fr ' });

            //Прокручивает диалог в конец
            if ($('.messages').length != 0) {
                var messages = document.querySelector('.messages');

                $('.messages').scrollTop(messages.scrollHeight);
            }
        }
        else {
            //Добавляет прокрутку input
            $(".messageInput").css({ 'overflow-y': 'scroll' });
        }

    });

    //Обработчик события keyup для inputa
    $(".messageInput").keyup(function () {

        //Проверяет был ли нажат Enter
        if (event.keyCode == 13) {
            //Вызывает метод для отправки сообщения на сервере
            hub.server.sendMessage(obj.id, $('.interlocutorId').val(), $(this).val());

            $(".messages").css({ 'display': 'block' });
            $(".noMessages").css({ 'display': 'none' });

            var date = new Date();

            //Выводит сообщение на экран
            $(".messages").append('<div class="message sender"><img src="/Content/Photo/' + obj.Photo + '" class="UserPhoto"> <div class="info"><div class="messagefio"> <p class="Name">' + obj.Name + '</p><p class="Surname">' + obj.Surname + '</p><div class="date">' + date.getHours() + ":" + date.getMinutes() +'</div></div><p class="text">' + $(this).val() + '</p></div></div>');
            //Прокручивает диалог в конец
            if ($('.messages').length != 0) {
                var messages = document.querySelector('.messages');

                $('.messages').scrollTop(messages.scrollHeight);
            }

            $(".messageInput").val('');
        }
    });

    
});




