"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/roomhub").build();

connection.on("Update", function (roomId) {
    GetUpdatedRoomSongs(roomId);
});

connection.on("AllowTransfer", function (allowTransfer) {
    if (allowTransfer) {
        $("#playlistControlDiv").html('');
        $("#playlistControlDiv").html('<button class="btn btn-primary" type="button" id="startPlaylistButton">Start Playlist</button>');

        $("#startPlaylistButton").off('click').click(function () {
            StartPlaylist(player, $("[name='playerUri']").first().html());
        });
    }
    else {
        $("#playlistControlDiv").html('');

        $("#startPlaylistButton").off('click');
    }
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
            $("#listOfSongs").html(data);
            $("#listOfSongs").trigger("songAdded");
        },
        error: function (data) {
            console.log(data);
        }
    });
}