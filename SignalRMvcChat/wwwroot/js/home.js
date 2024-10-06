function loadChat(name) {
    $(".chat-wrapper.shown").removeClass("shown");
    $("#select-chat-div").hide();

    $chat = $('*[data-recipient="' + name + '"]');
    $chat.addClass("shown");
    scrollToBottom();
}

$(".write input").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btn-send").click();
    }
});

$("#sendButton").on("click", function () {
    var $input = $("#messageInput");
    var text = $input.val();
    var username = $input.data("username");
    var conversationId = $input.data("conversationId");

    sendMessage(text, username, conversationId);
    $input.val("");
})

function getTimeNow() {
    return new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function scrollToBottom() {
    $(".chat-wrapper.shown .chat").animate({ scrollTop: $('.chat-wrapper.shown .chat').prop("scrollHeight") }, 500);
}

function displayMessage(text, timeStamp, sender) {
    const username = $("#username").val();
    const msgDiv = $("<div>").addClass("media mb-3");
    if (sender === username) msgDiv.addClass("d-flex justify-content-end");
    const msgBody = $("<div>").addClass("media-body");
    msgBody.append(`<h5 class="mt-0">${sender}</h5><p>${text}</p><small class="text-muted">${formatTimeStamp(timeStamp)}</small>`);
    msgDiv.append(msgBody);
    $("#messages").append(msgDiv);
}

function updateSidebar(text, timeStamp, conversationId){
    const conv = $(`#${conversationId}`)
    conv.find('.last-message-text').text(text);
    conv.find('.timestamp').text(new Date(timeStamp).toLocaleTimeString('en-US', {hour: '2-digit', minute: '2-digit'}));
}

function displayIncomingMessage(text, timeStamp, conversationId, username) {
    var $input = $("#messageInput");
    var $username = $input.data("username");
    var $conversationId = $input.data("conversationId");
    var currentUsername = $("#username").val();

    updateSidebar(text, timeStamp, conversationId);

    if (currentUsername === username) return;

    if (username == $username || conversationId == $conversationId) {
        displayMessage(text, timeStamp, username);
    }
}

function formatTimeStamp(timeStamp) {
    var date = new Date(timeStamp);

    var dateOptions = { day: 'numeric', month: 'long' };
    var timeOptions = { hour: '2-digit', minute: '2-digit' };

    var formattedDate = date.toLocaleDateString('en-US', dateOptions);
    var formattedTime = date.toLocaleTimeString('en-US', timeOptions);

    return `${formattedDate}, ${formattedTime}`;
}