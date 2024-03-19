using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using NetFwTypeLib;
using EO.WebBrowser;
using NAudio.Lame;
using NAudio.Wave;
namespace Wakeup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        private AudioFileReader audioFileReader;
        private IWavePlayer waveOutDevice;
        private string song;
        public static Form1 form = (Form1)Application.OpenForms["Form1"];
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.pictureBox1.Dock = DockStyle.Fill;
            EO.WebEngine.BrowserOptions options = new EO.WebEngine.BrowserOptions();
            options.EnableWebSecurity = false;
            EO.WebBrowser.Runtime.DefaultEngineOptions.SetDefaultBrowserOptions(options);
            EO.WebEngine.Engine.Default.Options.AllowProprietaryMediaFormats();
            EO.WebEngine.Engine.Default.Options.SetDefaultBrowserOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            this.webView1.Create(pictureBox1.Handle);
            this.webView1.Engine.Options.AllowProprietaryMediaFormats();
            this.webView1.SetOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            this.webView1.Engine.Options.DisableGPU = false;
            this.webView1.Engine.Options.DisableSpellChecker = true;
            this.webView1.Engine.Options.CustomUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            Navigate("https://michaelfraniatte.github.io/ppia/");
            webView1.RegisterJSExtensionFunction("demoAbout", new JSExtInvokeHandler(WebView_JSDemoAbout));
        }
        private void webView1_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            Task.Run(() => LoadPage());
        }
        private void LoadPage()
        {
            string backgroundpath = "file:///" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace(@"file:\", "").Replace(Process.GetCurrentProcess().ProcessName + ".exe", "").Replace(@"\", "/").Replace(@"//", "") + "background.jpg";
            string backgroundcolor = "";
            string timerscolor = "";
            using (System.IO.StreamReader file = new System.IO.StreamReader("colors.txt"))
            {
                file.ReadLine();
                backgroundcolor = file.ReadLine();
                file.ReadLine();
                timerscolor = file.ReadLine();
                file.Close();
            }
            string stringinject;
            stringinject = @"
    <style>

        @import url(https://fonts.googleapis.com/css?family=PT+Mono);

        body {
            background-color: backgroundcolor;
            height: 100%;
            background-image: url('background.jpg');
            background-repeat: no-repeat;
            background-attachment: fixed;
            background-position: center;
            overflow-x: hidden;
        }

        #timer, #watch, #clock {
            background-color: timerscolor;
            border-radius: 200px;
            height: 200px;
            width: 200px;
            position: relative;
            margin: 12px auto;
        }

        .screen {
            width: 154px;
            height: 42px;
            background-color: #444;
            background-image: linear-gradient(300deg, rgba(255,255,255,0.1), rgba(255,255,255,0.1) 20%, rgba(255,255,255,0.5) 10%, rgba(255,255,255,0.1), rgba(255,255,255,0.1));
            border-radius: 4px;
            margin: auto;
            position: relative;
            top: 75px;
            text-align: right;
            font-family: 'PT Mono', monospace;
            color: #fff;
            line-height: 38px;
            font-size: 28px;
            padding: 5px 8px;
        }
    </style>
".Replace("\r\n", " ").Replace("backgroundcolor", backgroundcolor).Replace("timerscolor", timerscolor).Replace("background.jpg", backgroundpath);
            stringinject = @"""" + stringinject + @"""";
            stringinject = @"$(" + stringinject + @" ).appendTo('head');";
            this.webView1.EvalScript(stringinject);
            stringinject = @"

    <div id='watch'>
        <div class='screen' id='actualhour'></div>
    </div>
    <div id='timer'>
        <div class='screen' contenteditable='true' id='wakeuphour'></div>
    </div>
    <div id='clock'>
        <div class='screen' id='sleephour'></div>
    </div>

    <script>
    var wd = 2;
    var wu = 2;

    var timetowakeup;
    
    const watchScreen = document.querySelector('#actualhour');
  
    const timerScreen = document.querySelector('#wakeuphour');

    const clockScreen = document.querySelector('#sleephour');

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
            localStorage.setItem('wakeup', timerScreen.innerHTML);
        }
        var difftimems = parseReadableTimeIntoMilliseconds(timerScreen.innerHTML) - parseReadableTimeIntoMilliseconds(watchScreen.innerHTML);
        if (difftimems < 0 ) {
            difftimems = parseReadableTimeIntoMilliseconds('24:00:00') + difftimems;
        }
        var clock = parseMillisecondsIntoReadableTime(difftimems);
        clockScreen.innerHTML = clock;
    }
  
    function sound(){
        demoAbout(window.navigator.appVersion, 'assets/wakeup.mp3');
    }

    var timetowakeup = localStorage.getItem('wakeup');

    if (timetowakeup != null) {
        timerScreen.innerHTML = timetowakeup;
    }
    else {
        timerScreen.innerHTML = '00:00:00';
    }

    function parseMillisecondsIntoReadableTime(milliseconds) {
        var seconds = milliseconds / 1000;
        var hours = parseInt(seconds / 3600);
        seconds = seconds % 3600;
        var minutes = parseInt(seconds / 60); 
        seconds = seconds % 60;
        return (hours >= 10 ? hours : '0' + hours) + ':' + (minutes >= 10 ? minutes : '0' + minutes) + ':' + (seconds >= 10 ? seconds : '0' + seconds);
    }

    function parseReadableTimeIntoMilliseconds(hms) {
        var a = hms.split(':'); 
        var milliseconds = a[0] * 60 * 60 * 1000 + a[1] * 60 * 1000 + a[2] * 1000; 
        return milliseconds;
    }

</script>
".Replace("\r\n", " ");
            stringinject = @"""" + stringinject + @"""";
            stringinject = @"$(document).ready(function(){$('body').append(" + stringinject + @");});";
            this.webView1.EvalScript(stringinject);
        }
        private void Navigate(string address)
        {
            if (String.IsNullOrEmpty(address))
                return;
            if (address.Equals("about:blank"))
                return;
            if (!address.StartsWith("http://") & !address.StartsWith("https://"))
                address = "https://" + address;
            try
            {
                webView1.Url = address;
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }
        void WebView_JSDemoAbout(object sender, JSExtInvokeArgs e)
        {
            song = e.Arguments[1] as string;
            try
            {
                waveOutDevice.Stop();
                audioFileReader.Dispose();
                waveOutDevice.Dispose();
            }
            catch { }
            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader(song);
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.webView1.Dispose();
        }
    }
}
