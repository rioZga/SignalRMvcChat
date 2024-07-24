"use strict"

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
$("#sendButton").disabled = true;

connection.on("ReceiveMessage", function (message, time) {
    console.log("Message received");
    $('.chat-wrapper.shown .chat').append('<div class="bubble other"><span class="message-text">' + message + ' </span>'
        + '<span class="message-time">' + time + '</span></div>');
    scrollToBottom();
});

connection.start().then(function () {
    $("#sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

$("#sendButton").click(function (event) {
    const user = $("#user").val();
    const message = $("#message").val();
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    })
    event.preventDefault();
})