//Выполняется при загрузки страницы
$(document).ready(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var user = $.connection.usersHub;

    //Получает куки пользователя
    var cookie = $.cookie("User").split('Name');

    //Парсит куки
    var Id = cookie[0].split("=");

    //Все еще парсит куки
    Id = Id[1].slice(0, Id[1].length - 1);

    //Метод,вызываемый сервером при отправке запроса на дружбу
    user.client.friendshepRequest = function (jsonUser) {

        //Сереализует JSON строку в массив объектов
        var sendingRequestUser = jQuery.parseJSON(jsonUser);

        for (var i = 0; i < sendingRequestUser.length; i++) {
            GetNewFriendView(sendingRequestUser[i]);
        }
    }

    //Подключение к серверу
    $.connection.hub.start().done(function () {
        //Метод,вызываемый на сервере после подключения пользователя
        user.server.connect(Id);
    });

});

     
//Добавляет на страницу представление для нового друга 
function GetNewFriendView(friend) {
    $(".newFriends").append($('<div class="newFriend ' + friend.Id + '" "><img src="/Content/Photo/' + friend.Photo + '" class="UserPhoto"/"><p class="">' + friend.Name + '</p><p class="">' + friend.Surname + '</p> <button class="usersButton" id="' + friend.Id + '">Добавить друга</button> </div>'));
    var id = '#' + friend.Id;
    $(id).click(function () {
        $.ajax({
            url: "Home/AddFriend",
            //Тип запроса
            type: "POST",
            //Передаваеймые в метод переменные
            data: { id: friend.Id },
            //Тип передаваемых данных
            dataType: "html"
        });
    });
}



