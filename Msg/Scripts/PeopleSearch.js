
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
    }


}
    //Генерирует представления для найденных пользователей
function GenerateUser(user) {
    $(".users").append($('<div class="user"><input type="hidden" value="' + user.Id + '" class="id" /><img src="/Content/Photo/' + user.Photo + '" class="UserPhoto" /> <p class="">' + user.Name + '</p> <p class="">' + user.Surname + '</p></div > '));
}