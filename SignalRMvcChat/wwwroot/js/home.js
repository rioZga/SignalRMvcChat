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

var sendMessage = function (recipient) {
    event.preventDefault();

    var $text = $("#message-text").val();

    $.ajax({
        type: 'POST',
        url: '/Chat/SendMessage',
        data: { username: recipient, text: $text },
        cache: false,
        success: function () {
            $(".write textarea").val('');
            $(".chat-wrapper.shown .chat").append('<div class="bubble me"><span class="message-text">' + $text + ' </span>'
                + '<span class="message-time">' + getTimeNow() + '</span></div>');
            scrollToBottom();
        },
        error: function () {
            alert("Failed to send message!");
        }
    });
}

function getTimeNow() {
    return new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function scrollToBottom() {
    $(".chat-wrapper.shown .chat").animate({ scrollTop: $('.chat-wrapper.shown .chat').prop("scrollHeight") }, 500);
}