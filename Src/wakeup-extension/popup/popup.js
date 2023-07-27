const sendMessageId = document.getElementById("sendmessageid");
if (sendMessageId) {
  sendMessageId.onclick = function() {
    chrome.tabs.query({ active: true, currentWindow: true }, function(tabs) {
      chrome.tabs.sendMessage(
        tabs[0].id,
        {
            url: chrome.extension.getURL("images/background.jpg"),
            audio: chrome.extension.getURL("assets/bensound-dreams.mp3"),
            favicon: chrome.extension.getURL("icons/wu128x128.png"),
            tabId: tabs[0].id
        },
        function(response) {
            console.log("message with url and audio sent");
            window.close();
        }
      );
    });
  };
}