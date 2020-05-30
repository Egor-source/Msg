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
  


    if (users == '') {
        $.ajax({
            //Метод обработки
            url: "Home/SidbarReboot",
            //Тип запроса
            type: "Get",

            success:Ins
        });

        return;
    }
    else {

        //Конвертирует JSON строку в js объект
        users = jQuery.parseJSON(users);

        $(".newFriends").empty();
        $('.subscriptions').empty();
        $('.friends').empty();


        //Проверяет были ли возвращены значения
        if (users.length === 0) {
            $(".status").text("Пользователь не найден");
            return;
        }



        for (var i = 0; i < users.length; i++) {

            var fId = '#' + users[i].Id;


            switch (users[i].Status) {
                case -1:
                    GenerateUser(users[i]);
                    break;
                case 0:
                    var cookie = $.cookie("User").split('Name');

                    //Парсит куки
                    var Id = cookie[0].split("=");

                    //Все еще парсит куки
                    Id = Id[1].slice(0, Id[1].length - 1);


                    if (users[i].HostRequestId == Id) {
                        GenerateNewFriendView(users[i]);
                    }
                    else {
                        GenerateDeletingUserView(users[i]);
                    }
                    break;
                case 1:
                    GenerateFriendView(users[i]);
                    break;
            }
        }
    }
}

function Ins(html) {

    $(".sidebar").empty();
    $(".sidebar").append(html);
    confirm();
    remove();
    usub();
}

    //Генерирует представления для найденных пользователей
function GenerateUser(user) {
    $(".users").append($('<div class="user"><img src="/Content/Photo/' + user.Photo + '" class="UserPhoto" /><div class="fio"> <p class="">' + user.Name + '</p> <p class="">' + user.Surname + '</p></div><button class="addFriend usersButton" id="' + user.Id + '" value="' + user.Id+'" ">Добавить</button></div > '));

    var id = '#' + user.Id;
    //Добавляет обработчик события click для кнопки добавления в друзья
    $(id).click(function () {
        //Вызывает метод сервера для обработки запроса добавления в друзья
        hub.server.friendshipRequest(user.Id);

        //Добавляет пользователя в подписки
        GenerateSubscriptions($(this).parent());

    });

    $('.user').click(function () {
        ShowDialog($(this).children('.UserButton').val());
    });
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

    $('.user').click(function () {
        ShowDialog($(this).children('.UserButton').val());
    });
}

