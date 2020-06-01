// Ссылка на автоматически-сгенерированный прокси хаба
var hub = $.connection.usersHub;

//Получает cookie пользователя
var cookie = $.cookie("User").split('&');

var obj = {
    id: null,
    Name: null,
    Surname: null,
    Photo: null
};

for (var i = 0; i < cookie.length; i++) {
    var k = cookie[i].split('=');
    obj[k[0]] = k[1];
}

//Выполняется при загрузки страницы
$(document).ready(function () {

    
    $('.user').click(function () {
        ShowDialog($(this).children('.usersButton').val());
    });

    confirm();

    remove();

    usub();

    //Метод,вызываемый сервером при отправке запроса на дружбу
    hub.client.friendshepRequest = function (jsonUser) {

        //Сереализует JSON строку в массив объектов
        var sendingRequestUser = jQuery.parseJSON(jsonUser);

        for (var i = 0; i < sendingRequestUser.length; i++) {
            GenerateNewFriendView(sendingRequestUser[i]);
        }
    }
    //Метод,вызываемый сервером при отписки от пользователя
    hub.client.unsubscribe = function (UserId) {
        var id = '#' + UserId;

        $(id).parent().remove();
    }

    //Метод,вызываесый сервером при подтверждении дружбы
    hub.client.confirmFriendship = function (jsonUser) {
        //Сереализует JSON строку в массив объектов
        var confirmedUser = jQuery.parseJSON(jsonUser);

        for (var i = 0; i < confirmedUser.length; i++) {

            var id = '#' + confirmedUser[i].Id;

            //Удаляет представление пользователя из подписок
            $(id).parent().remove();

            GenerateFriendView(confirmedUser[i]);
        }

    }

    //Метод,вызываемый сервером при удалении пользователя из друзей
    hub.client.removeFriend = function (jsonUser) {
         //Сереализует JSON строку в массив объектов
        var deletingUser = jQuery.parseJSON(jsonUser);

        for (var i = 0; i < deletingUser.length; i++) {
            GenerateDeletingUserView(deletingUser[i]);
        }

    }

    //Метод,вызываемый сервером при получении сообщения
    hub.client.getMessage = function (json) {
        //Сереализует JSON строку в массив объектов
        var message = jQuery.parseJSON(json);

        console.log(message.senderId);

       

        for (var i = 0; i < message.length; i++) {

            if (message[i].senderId == $('.interlocutorId').val()) {
                GenerateMessageView(message[i]);
            }
        }

            //Прокручивает диалог в конец
            var messages = document.querySelector('.messages');

            $('.messages').scrollTop(messages.scrollHeight);
        
    }

    //Подключение к серверу
    $.connection.hub.start().done(function () {
        //Метод,вызываемый на сервере после подключения пользователя
        hub.server.connect(obj.id);
    });

});

     
//Добавляет на страницу представление для нового друга 
function GenerateNewFriendView(newFriend) {
    //Создает представление для нового друга
    $(".newFriends").append($('<div class="newFriend ' + newFriend.Id + ' user" ><img src="/Content/Photo/' + newFriend.Photo + '" class="UserPhoto"/><div class="fio"><p class="Name">' + newFriend.Name + '</p><p class="Surname">' + newFriend.Surname + '</p></div> <button class="usersButton" id="' + newFriend.Id + '">Добавить</button> </div>'));
    var id = '#' + newFriend.Id;
    //Добавляет обработчик события click для кнопки подтверждения друга
    $(id).click(function () {
        //Вызывает на сервере метод подтверждения дружбы 
        hub.server.confirmFriendship(newFriend.Id);

        $(this).parent().remove();

        GenerateFriendView(newFriend);

    });

    $('.user').click(function () {
        ShowDialog($(this).children('.usersButton').val());
    });
}

//Добавляет на страницу представление для друга
function GenerateFriendView(friend) {
    //Создает представление для друга
    $(".friends").append($('<div class="user"><img src="/Content/Photo/' + friend.Photo + '" class="UserPhoto"/><div class="fio"><p class="Name">' + friend.Name + '</p><p class="Surname">' + friend.Surname + '</p></div> <button class="usersButton" id="' + friend.Id + '"  value="' + friend.Id + '">Удалить</button> </div>'));
    var id = '#' + friend.Id;

    //Добавляет обработчик события click для кнопки удаления из друзей
    $(id).click(function () {

        //Вызывает на сервере метод удаления друга
        hub.server.removeFriend(friend.Id);

        $(this).parent().remove();

        GenerateNewFriendView(friend);
    });

    $('.user').click(function () {
        ShowDialog($(this).children('.usersButton').val());
    });
}

//Добавляет представление для удаленного из друзей пользователя
function GenerateDeletingUserView(user) {
    var id = '#' + user.Id;

    $(id).parent().remove();

    $(".subscriptions").append($('<div class="user"><img src="/Content/Photo/' + user.Photo + '" class="UserPhoto" /> <div class="fio"><p class="Name">' + user.Name + '</p> <p class="Surname">' + user.Surname + '</p></div><button class="unsubscribe usersButton" id="' + user.Id + '" value="' + user.Id + '" >Отписаться</button></div > '));

    $(id).click(function () {

        //Вызывает на сервере метод для отписки от пользователя
        hub.server.unsubscribe($(this).val());
        //Удаляет представление пользователя из подписок
        $(this).parent().remove();
    });

    $('.user').click(function () {
        ShowDialog($(this).children('.usersButton').val());
    });
}

function confirm() {
    //Обработчик события click для подтверждения дружбы
    $(".confirmFriend").click(function () {

        //Вызывает на сервере метод для подтверждения дружбы
        hub.server.confirmFriendship($(this).val());

        var photo = $(this).parent().children('.UserPhoto').attr('src').split('/');

        //Информация о пользователи,получаемая из представления,стригеревшего метод
        var obj = {
            Id: $(this).val(),
            Name: $(this).parent().children('.Name').text(),
            Surname: $(this).parent().children('.Surname').text(),
            Photo: photo[photo.length - 1]
        };

        $(this).parent().remove();

        GenerateFriendView(obj);
    });
}


function remove() {
    $('.friend').click(function () {
        //Вызывает на сервере метод удаления друга
        hub.server.removeFriend($(this).val());


        var photo = $(this).parent().children('.UserPhoto').attr('src').split('/');

        //Информация о пользователи,получаемая из представления,стригеревшего метод
        var obj = {
            Id: $(this).val(),
            Name: $(this).parent().children('.Name').text(),
            Surname: $(this).parent().children('.Surname').text(),
            Photo: photo[photo.length - 1]
        };

        $(this).parent().remove();

        GenerateNewFriendView(obj);
    });
}

function usub() {
    //Обработчик события click для отписки от пользователя
    $('.unsubscribe').click(function () {

        //Вызывает на сервере метод для отписки от пользователя
        hub.server.unsubscribe($(this).val());
        //Удаляет представление пользователя из подписок
        $(this).parent().remove();
    });
}

//Метод для вывода на экран всех сообщений с выбраным пользователем
function ShowDialog(interlocutorId){

    $('.dialog').css({'grid-template-rows':'1fr 30fr 1fr'});

        $.ajax({
            //Метод обработки
            url: "Home/ShowDialog",
            //Тип запроса
            type: "POST",
            //Передаваеймые в метод переменные
            data: { userId: obj.id, interlocutorId: interlocutorId },
            //Тип передаваемых данных
            dataType: "html",
            success: function (dialog) {
                $('.dialog').empty();
                $('.dialog').append(dialog);
                if ($('.message').length == 0) {
                    $(".messages").css({ 'display': 'grid' });
                }

            }
        });

   
}


//Генерирует представление для полученного сообщения
function GenerateMessageView(message) {

    var date = new Date(message.sendingTime);
    $(".messages").css({ 'display': 'block' });
    $(".noMessages").css({ 'display': 'none' });

    $(".messages").append('<div class="message sender"><img src="/Content/Photo/' + message.Photo + '" class="UserPhoto"><div class="info"><div class="messagefio"> <p class="Name">' + message.Name + '</p><p class="Surname">' + message.Surname + '</p><div class="date">' + date.getHours() + ":" + date.getMinutes() + '</div></div><p class="text">' + message.Text + '</p></div></div>');
}

