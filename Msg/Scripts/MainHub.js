// Ссылка на автоматически-сгенерированный прокси хаба
var hub = $.connection.usersHub;

//Выполняется при загрузки страницы
$(document).ready(function () {
   

    //Получает куки пользователя
    var cookie = $.cookie("User").split('Name');

    //Парсит куки
    var Id = cookie[0].split("=");

    //Все еще парсит куки
    Id = Id[1].slice(0, Id[1].length - 1);

    $(".confirmFriend").click(function () {

        hub.server.confirmFriendship($(this).val());

        var photo = $(this).parent().children('.UserPhoto').attr('src').split('/');

        var obj = {
            Id: $(this).val(),
            Name: $(this).parent().children('.Name').text(),
            Surname: $(this).parent().children('.Surname').text(),
            Photo: photo[photo.length-1]
        };

        $(this).parent().remove();

        GenerateFriendView(obj);
    });

    $('.friend').click(function () {
        //Вызывает на сервере метод удаления друга
        hub.server.removeFriend($(this).val());

        var photo = $(this).parent().children('.UserPhoto').attr('src').split('/');

        var obj = {
            Id: $(this).val(),
            Name: $(this).parent().children('.Name').text(),
            Surname: $(this).parent().children('.Surname').text(),
            Photo: photo[photo.length - 1]
        };

        $(this).parent().remove();

        GenerateNewFriendView(obj);
    });

    $('.unsubscribe').click(function () {

        //Вызывает на сервере метод для отписки от пользователя
        hub.server.unsubscribe($(this).val());
        //Удаляет представление пользователя из подписок
        $(this).parent().remove();
    });

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

    hub.client.removeFriend = function (jsonUser) {

        console.log(1);
        var deletingUser = jQuery.parseJSON(jsonUser);

        console.log(jsonUser);

        for (var i = 0; i < deletingUser.length; i++) {
            console.log(3);
            GenerateDeletingUserView(deletingUser[i]);
        }

    }

    //Подключение к серверу
    $.connection.hub.start().done(function () {
        //Метод,вызываемый на сервере после подключения пользователя
        hub.server.connect(Id);
    });

});

     
//Добавляет на страницу представление для нового друга 
function GenerateNewFriendView(newFriend) {
    //Создает представление для нового друга
    $(".newFriends").append($('<div class="newFriend ' + newFriend.Id + ' user" ><img src="/Content/Photo/' + newFriend.Photo + '" class="UserPhoto"/><p class="Name">' + newFriend.Name + '</p><p class="Surname">' + newFriend.Surname + '</p> <button class="usersButton" id="' + newFriend.Id + '">Добавить друга</button> </div>'));
    var id = '#' + newFriend.Id;
    //Добавляет обработчик события click для кнопки подтверждения друга
    $(id).click(function () {
        //Вызывает на сервере метод подтверждения дружбы 
        hub.server.confirmFriendship(newFriend.Id);

        $(this).parent().remove();

        GenerateFriendView(newFriend);

    });
}

//Добавляет на страницу представление для друга
function GenerateFriendView(friend) {
    //Создает представление для друга
    $(".friends").append($('<div class="user"><img src="/Content/Photo/' + friend.Photo + '" class="UserPhoto"/><p class="Name">' + friend.Name + '</p><p class="Surname">' + friend.Surname + '</p> <button class="usersButton" id="' + friend.Id + '"  value="' + friend.Id + '">Удалить из друзей</button> </div>'));
    var id = '#' + friend.Id;

    //Добавляет обработчик события click для кнопки удаления из друзей
    $(id).click(function () {

        //Вызывает на сервере метод удаления друга
        hub.server.removeFriend(friend.Id);

        $(this).parent().remove();

        GenerateNewFriendView(friend);
    });
}

function GenerateDeletingUserView(user) {
    var id = '#' + user.Id;
    console.log(321);

    $(id).parent().remove();
    $(".subscriptions").append($('<div class="user"><img src="/Content/Photo/' + user.Photo + '" class="UserPhoto" /> <p class="Name">' + user.Name + '</p> <p class="Surname">' + user.Surname + '</p><button class="unsubscribe usersButton" id="' + user.Id + '" value="' + user.Id + '" >Отписаться</button></div > '));

    $(id).click(function () {

        //Вызывает на сервере метод для отписки от пользователя
        hub.server.unsubscribe($(this).val());
        //Удаляет представление пользователя из подписок
        $(this).parent().remove();
    });
}



