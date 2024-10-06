"use strict"

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
$("#sendButton").disabled = true;

connection.on("ReceiveMessage", displayIncomingMessage);

connection.start().then(function () {
    $("#sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

$(".list-group-item").on("click", function (e) {
    $(this).removeClass("font-weight-bold");
    e.preventDefault();
    const conversationId = $(this).data("conversation-id");
    const username = $(this).data("username")

    const conversationHeader = $("#conversation-header");
    conversationHeader.empty();
    conversationHeader.append(`<h2>${username}</h2> <hr/>`);

    $("#messageInput").data("username", username);
    $("#messageInput").data("conversationId", conversationId);

    if (typeof conversationId !== "undefined") {
        loadChatMessages(conversationId, "conversation");
    } else {
        loadChatMessages(username, "username")
    }
});

function loadChatMessages(data, operation) {
    $("#messages").empty();

    let query;
    if (operation === 'username') {
        query = `username=${data}`;
    } else {
        query = `conversationId=${data}`;
    }

    $.ajax({
        url: `/Chat/GetConversation?${query}`,
        type: 'GET',
        success: function (conversation) {
            conversation.messages.forEach(msg => {
                displayMessage(msg.text, msg.timeStamp, msg.senderName);
            });
        },
    });
}

function sendMessage(text, username, conversationId) {
    $.ajax({
        type: "POST",
        url: "/Chat/SendMessage",
        cache: false,
        data: {
            text,
            username,
            conversationId,
        },
        success: function (data) {
            if ($("#messageInput").data("username") == username ||
                $("#messageInput").data("conversationId") == conversationId)
                displayMessage(data.text, data.time, data.username);
            updateSidebar(data.text, data.time, data.conversationId)
        },
        error: function () {
            console.error("error occurred");
        }
    })
}