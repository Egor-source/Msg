$(document).ready(function () {

    var friend = $.connection.friendHub;

    friend.client.friendshepRequest = function (jsonUser) {

        var sendingRequestUser = JSON.parse(jsonUser);
    }

    $.connection.hub.start().done(function () {

        friend.server.connect();
    });

});

function GetFriendRequest(user) {
    $(".newFriends").append($('<div class="user"><input type="hidden" value="' + user.Id + '" class="id" /><img src="/Content/Photo/' + user.Photo + '" class="UserPhoto" /> <p class="">' + user.Name + '</p> <p class="">' + user.Surname + '</p><button>Добавить</button></div > '));
}