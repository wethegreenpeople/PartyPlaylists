"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/roomhub").build();

connection.on("Update", function (roomId) {
    console.log(roomId);
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});