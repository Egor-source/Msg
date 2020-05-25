

function Search(key) {

    if (key >= 48 && key <= 90 || key == 13 || key==8 || key >= 186 && key <= 222) {
        let man = $(".search").val();
        $(".users").empty();
        $.ajax({
            url: "Home/GetPeople",
            type: "POST",
            data: ({ man: man }),
            dataType: "html",
            beforeSend: Before,
            success: Success
        });
    }
}

function Before() {
    $(".status").text("Поиск...");
}

function Success(users) {

    $(".status").empty();
    users = jQuery.parseJSON(users);

    if (users.length === 0) {
        $(".status").text("Пользователь не найден");
    }

    for (var i = 0; i < users.length; i++) {
        $(".users").append($('<div class="user"><img src="/Content/Photo/' + users[i].Photo + '" class="UserPhoto" /> <p class="">' + users[i].Name + '</p> <p class="">' + users[i].Surname + '</p></div > '));   
                         
    }
}