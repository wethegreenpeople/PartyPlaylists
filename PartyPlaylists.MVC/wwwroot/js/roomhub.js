"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/roomhub").build();

connection.on("Update", function (roomId) {
    GetUpdatedRoomSongs(roomId);
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});

function GetUpdatedRoomSongs(roomId) {
    $.ajax({
        url: "/room/GetRoomSongs",
        type: "GET",
        data: {
            "roomId": roomId
        },
        headers: {

        },
        success: function (data) {
            $("#tableOfSongs").html(data);
        },
        error: function (data) {
            console.log(data);
        }
    });
}