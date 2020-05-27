$(document).ready(function () {

    var main = $.connection.mainHub;

    var cookie = $.cookie("User").split('Name');

    var Id = cookie[0].split("=");

    main.client.friendshepRequest = function (jsonUser) {

        var sendingRequestUser = JSON.parse(jsonUser);
    }

    $.connection.hub.start().done(function () {

        main.server.connect(Id[1]);
    });

});

function GetFriendRequest(user) {
    $(".newFriends").append($('<div class="user"><input type="hidden" value="' + user.Id + '" class="id" /><img src="/Content/Photo/' + user.Photo + '" class="UserPhoto" /> <p class="">' + user.Name + '</p> <p class="">' + user.Surname + '</p><button>Добавить</button></div > '));
}