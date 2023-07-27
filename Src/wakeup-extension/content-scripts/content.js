chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {

document.getElementsByTagName('html')[0].innerHTML = '<head></head><body></body>';

var script = document.createElement('script'); 
script.src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js'; 
document.head.appendChild(script);

var stringinject = `
    <link rel="shortcut icon" href="${request.favicon}" type="image/png" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.css">
    <style>

        @import url(https://fonts.googleapis.com/css?family=PT+Mono);

        body {
            background-color: #999;
            width: 100%;          
            height: 100%;
            background-image: url("${request.url}");
            background-position: center center;
            background-repeat: no-repeat; 
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
            background-attachment: fixed;
            overflow-x: hidden;
        }

        #timer, #watch, #clock {
            background-color: #333;
            border-radius: 30vh;
            height: 30vh;
            width: 30vh;
            position: relative;
            margin: 2.5vh auto;
        }

        .screen {
            width: 22vh;
            height: 6vh;
            background-color: #444;
            background-image: linear-gradient(300deg, rgba(255,255,255,0.1), rgba(255,255,255,0.1) 20%, rgba(255,255,255,0.5) 10%, rgba(255,255,255,0.1), rgba(255,255,255,0.1));
            border-radius: 2vh;
            margin: auto;
            position: relative;
            top: 12vh;
            text-align: center;
            font-family: 'PT Mono', monospace;
            color: #fff;
            line-height: 6vh;
            font-size: 4vh;
        }

    </style>`;

document.getElementsByTagName('head')[0].innerHTML = stringinject;

stringinject = `
    <div id="watch">
        <div class="screen" id="actualhour"></div>
    </div>
    <div id="timer">
        <div class="screen" contenteditable="true" id="wakeuphour"></div>
    </div>
    <div id="clock">
        <div class="screen" id="sleephour"></div>
    </div>

    <script>

    var wd = 2;
    var wu = 2;

    var timetowakeup;
    
    const watchScreen = document.querySelector("#actualhour");
  
    const timerScreen = document.querySelector("#wakeuphour");

    const clockScreen = document.querySelector("#sleephour");

    document.title = "wakeup";

    setTime();
    setInterval(setTime, 500);

    function setTime() {
        const now = new Date();
        var hour = now.toLocaleTimeString();
        watchScreen.innerHTML = hour;
        if (watchScreen.innerHTML == timerScreen.innerHTML)
        {
	        if (wd <= 1) {
		        wd = wd + 1;
	        }
	        wu = 0;
        }
        else
        {
	        if (wu <= 1) {
		        wu = wu + 1;
	        }
	        wd = 0;
        }
        if (wd == 1) {
            sound();
            localStorage.setItem("wakeup", timerScreen.innerHTML);
        }
        var difftimems = parseReadableTimeIntoMilliseconds(timerScreen.innerHTML) - parseReadableTimeIntoMilliseconds(watchScreen.innerHTML);
        if (difftimems < 0 ) {
            difftimems = parseReadableTimeIntoMilliseconds("24:00:00") + difftimems;
        }
        var clock = parseMillisecondsIntoReadableTime(difftimems);
        clockScreen.innerHTML = clock;
    }
  
    function sound(){
        new Audio("${request.audio}").play();
    }

    var timetowakeup = localStorage.getItem("wakeup");

    if (timetowakeup != null) {
        timerScreen.innerHTML = timetowakeup;
    }
    else {
        timerScreen.innerHTML = "00:00:00";
    }

    function parseMillisecondsIntoReadableTime(milliseconds) {
        var seconds = milliseconds / 1000;
        var hours = parseInt(seconds / 3600);
        seconds = seconds % 3600;
        var minutes = parseInt(seconds / 60);
        seconds = seconds % 60;
        return (hours >= 10 ? hours : "0" + hours) + ":" + (minutes >= 10 ? minutes : "0" + minutes) + ":" + (seconds >= 10 ? seconds : "0" + seconds);
    }

    function parseReadableTimeIntoMilliseconds(hms) {
        var a = hms.split(":");
        var milliseconds = a[0] * 60 * 60 * 1000 + a[1] * 60 * 1000 + a[2] * 1000;
        return milliseconds;
    }

</script>`;

    (function () {
        // more or less stolen form jquery core and adapted by paul irish
        function getScript(url, success) {
            var script = document.createElement('script');
            script.src = url;
            var head = document.getElementsByTagName('head')[0],
                done = false;
            // Attach handlers for all browsers
            script.onload = script.onreadystatechange = function () {
                if (!done && (!this.readyState
                    || this.readyState == 'loaded'
                    || this.readyState == 'complete')) {
                    done = true;
                    success();
                    script.onload = script.onreadystatechange = null;
                    head.removeChild(script);
                }
            };
            head.appendChild(script);
        }
        getScript('https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js', function () {
            $(document).ready(function () {
                var script = document.createElement('script');
                script.src = 'https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js';
                document.head.appendChild(script);
                $('body').html(stringinject);
            });
        });
    })();

  sendResponse({ fromcontent: "This message is from content.js" });
});
