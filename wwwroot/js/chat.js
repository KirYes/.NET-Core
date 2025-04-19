"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

function appendMessage(text){
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = text;
}
connection.on("ReceiveMessage", function (user, message) {
    appendMessage(`${user} says ${message}`);
});
connection.on("Answer", function (user, message) {
    appendMessage(`GPT Answer: ${message}`);
});
connection.on("Question", function (user, message) {
    appendMessage(`${user}'s question to GPT: ${message}`);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
