// Ссылка на автоматически-сгенерированный прокси хаба
var hub = $.connection.usersHub;

//срабатывает при нажатии на клавишу в input .search
function Search(key,id) {
    //Проверяет код нажатой клавиши
    if (key >= 48 && key <= 90 || key == 13 || key == 8 || key >= 186 && key <= 222) {
        //Перадает значения input .search
        let man = $(".search").val();
        //Очищает элемент .users
        $(".users").empty();
        //Отправляет ajax запрос
        $.ajax({
            //Метод обработки
            url: "Home/GetPeople",
            //Тип запроса
            type: "POST",
            //Передаваеймые в метод переменные
            data: { man: man, id: id },
            //Тип передаваемых данных
            dataType: "html",
            //Метод,срабатывающий перед отправкой запроса
            beforeSend: Before,
            //Медот,срабатывающий после выполнения запроса 
            success: Success
        });
    }
}
//Выводит статус поиска пользователей
function Before() {
    $(".status").text("Поиск...");
}

function Success(users) {

    //Очищает элемент .status
    $(".status").empty();
    //Конвертирует JSON строку в js объект
    users = jQuery.parseJSON(users);

    //Проверяет были ли возвращены значения
    if (users.length === 0) {
        $(".status").text("Пользователь не найден");
        return;
    }

    for (var i = 0; i < users.length; i++) {
        GenerateUser(users[i]);
        var id ='#' +users[i].Id;
            //Добавляет обработчик события click для кнопки добавления в друзья
        $(id).click(function () {
            //Вызывает метод сервера для обработки запроса добавления в друзья
            hub.server.friendshipRequest($(this).val());

            //Добавляет пользователя в подписки
            GenerateSubscriptions($(this).parent());

        });
    }

}
    //Генерирует представления для найденных пользователей
function GenerateUser(user) {
    $(".users").append($('<div class="user"><img src="/Content/Photo/' + user.Photo + '" class="UserPhoto" /> <p class="">' + user.Name + '</p> <p class="">' + user.Surname + '</p><button class="addFriend usersButton" id="' + user.Id + '" value="' + user.Id+'" ">Добавить в друзья</button></div > '));
   
}

function GenerateSubscriptions(user) {

    //Удаляет пользователя из поиска
    $(user).remove();


    //Добавляет пользователя на страницу в категорию подписки
    $(user).children(".addFriend").text("Отписаться");
    $(user).children(".addFriend").addClass("unsubscribe");
    $(user).children(".addFriend").removeClass("addFriend");

    $(".subscriptions").append($('<div class="user">' + $(user).html() + '</div>'));

    var id = '#' + $(user).children(".unsubscribe").val();

    //Добавляет обработчик события click
    $(id).click(function () {

        //Вызывает на сервере метод для отписки от пользователя
       hub.server.unsubscribe($(this).val());
        //Удаляет представление пользователя из подписок
        $(this).parent().remove();
    });
}

