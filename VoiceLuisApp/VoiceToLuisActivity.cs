using System;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;
using System.Collections.Generic;
using System.Linq;
using Android.Speech;
using System.Net.Http;

using System.Threading.Tasks;
using VoiceLuisApp.Models;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.ObjectModel;




namespace VoiceLuisApp
{
    [Activity(Label = "AIIOT語意辨識電梯控制", MainLauncher = true, Icon = "@drawable/icon")]

    public class VoiceToLuisActivity : Activity , TextToSpeech.IOnInitListener

    {
        ObservableCollection<PLC_Display_class> PLC_Data_Class; //宣告變數 SCHOOLDATA 的抽屜放STRING
        int PLC_connect_ststus { get; set; }
        public String REC_PLC { get; set; }



        TextToSpeech textToSpeech;
        Context context;
        private readonly int MyCheckCode = 101, NeedLang = 103;
        Java.Util.Locale lang;

        public bool isRecording;
        private readonly int VOICE = 10;
        private TextView textBox;
        private TextView LuisBox_Intent,LuisBox_Entiti;
        private Button recButton;
        public string LuisIntentResult, LuisEntitiResult;

        public Int32 PollingIndex = 0, VoiceIndex = 0;

        public Boolean Check1 = false, Check2 = false, Check3 = false, Check4 = false,Check5 = false, Check6 =false;
        public int  kk=0 ;
        public Boolean VoiceStringCheck;
        public int Return_VoiceIndex;
        public string VoiceTextInput;
        public int ActiveCycleTime = 15;
        public  string LiftControlFloor;
        public String Save_Floor="1";
        ////////////////////////////////////////////////////////
        public string LIUS_Entry_check;
        const string MyLuisKey = "49fe969fec144c659b261a2afdd2898c";
        public const string questionStr = "ON";
        ///////////////////////////////////////////////////////

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);;
            string TxtToVoice = this.Intent.GetStringExtra("TxtToVoice");
            int para2 = this.Intent.GetIntExtra("DelayTime", -1);
            isRecording = false;
            SetContentView(Resource.Layout.TextToLuisMain);
    
            textBox = FindViewById<TextView>(Resource.Id.textYourText);
            LuisBox_Intent = FindViewById<TextView>(Resource.Id.LuisIntentText);
            LuisBox_Entiti = FindViewById<TextView>(Resource.Id.LuisEntitiText);

            int para3 = this.Intent.GetIntExtra("para3", -1);
            if (para3 >= 0)
            {
                VoiceIndex = para3; para3 = 0;
                PollingJobTime(300, para3);
            }
            else

            PollingJobTime(300, 0);
            textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
            var langAvailable = new List<string>{ "Default" };

            // our spinner only wants to contain the languages supported by the tts and ignore the rest
            var localesAvailable = Java.Util.Locale.GetAvailableLocales().ToList();
            foreach (var locale in localesAvailable)
            {
                LanguageAvailableResult res = textToSpeech.IsLanguageAvailable(locale);
                switch (res)
                {
                    case LanguageAvailableResult.Available:
                        langAvailable.Add(locale.DisplayLanguage);
                        break;
                    case LanguageAvailableResult.CountryAvailable:
                        langAvailable.Add(locale.DisplayLanguage);
                        break;
                    case LanguageAvailableResult.CountryVarAvailable:
                        langAvailable.Add(locale.DisplayLanguage);
                        break;
                }
            }
            langAvailable = langAvailable.OrderBy(t => t).Distinct().ToList();

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, langAvailable);
            // set up the speech to use the default langauge
            // if a language is not available, then the default language is used.
            lang = Java.Util.Locale.Default;
            textToSpeech.SetLanguage(lang);

            // set the speed and pitch
            textToSpeech.SetPitch(.9f);
            textToSpeech.SetSpeechRate(.9f);


            // check to see if we can actually record - if we can, assign the event to the button
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert
                var alert = new AlertDialog.Builder(recButton.Context);
                alert.SetTitle("You don't seem to have a microphone to record with");
                alert.SetPositiveButton("OK", (sender, e) =>
                {
                    textBox.Text = "No microphone present";
                    recButton.Enabled = false;
                    return;
                });

                alert.Show();
            }
           
        }
        public Tuple<string, string> test11111(string text)
        {
            return new Tuple<string, string>($"II1111=:{text}", "II22222");
        }
            public void Start_Recording()
        {
                        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Application.Context.GetString(Resource.String.messageSpeakNow));
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 25000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 25000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                        StartActivityForResult(voiceIntent, VOICE);
          }

    public void TextToVoiceDelayTime(int DelayValue,string TextToVoice)
        {
            System.Timers.Timer Timer1 = new System.Timers.Timer();
            Timer1.Interval = DelayValue;
            Timer1.Enabled = true;
            Timer1.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(TextToVoice))
                    textToSpeech.Speak(TextToVoice, QueueMode.Flush,null,"");
                Timer1.Stop();
            };
            Timer1.Start();
        }


        public void PollingJobTime(int DelayValue,int JampValue)
        {
            System.Timers.Timer Timer2 = new System.Timers.Timer();
            Timer2.Interval = DelayValue;
            Timer2.Enabled = true;
           
            Timer2.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (PollingIndex >= ActiveCycleTime)
                {
                    if (JampValue>0) { VoiceIndex = JampValue; JampValue = 0; }
                    switch (VoiceIndex)
                    {
                        case 0: TextToVoiceDelayTime(50, "歡迎使用電梯語意辨識控制系統"); VoiceIndex++; break;
                        case 1: TextToVoiceDelayTime(50, "接下來身分識別請看著我微笑"); VoiceIndex++; break;
                        case 2:
                            if (Check1 == false)
                            {
                                Intent intent = new Intent(this, typeof(CameraActivity));
                                StartActivity(intent);
                                Check1 = true;
                            }
                            break;
                        case 3:
                            TextToVoiceDelayTime(50, "請告訴我您要乘坐電梯至哪樓層");
                            VoiceIndex++; Check1 = false; break; 
                        case 4:
                            if (Check1 == false )
                            {
                                VoiceTextInput = "";
                                Check1 = true;
                                Start_Recording();
                            }
                            ActiveCycleTime = 15;
                            VoiceIndex++;
                            break;
                        case 5:
                            if (Check2 == false& VoiceTextInput.Length >1)
                           {
                                Check2 = true;
                                LIUS_Entry_check = VoiceTextInput;
                                textBox.Text = VoiceTextInput; //Recoder End Print
                                TextToVoiceDelayTime(50, "收到您說" + VoiceTextInput);
                                var result = LIUS_API(VoiceTextInput);
                                VoiceIndex++;
                            }
                            ActiveCycleTime = 22;
                            break;
                        case 6:
                            if (LuisIntentResult==(null))
                            {
                                TextToVoiceDelayTime(30, "語音無法辨識請重新輸入");
                                Check2 = false;
                                PollingIndex = 0;
                                VoiceIndex = 3;
                                break;
                            }

                            if (LuisIntentResult.Length > 1)
                            {
                                Check3 = true;
                                VoiceIndex++;
                            }
                            break;
                        case 7:
                            if (Check4 == false)
                            {
                                TextToVoiceDelayTime(50, "接下來電梯控制執行" + LuisEntitiResult + "運轉控制");
                                switch (LuisEntitiResult)
                                  {
                                    case "一 樓": LiftControlFloor = "0001"; break;
                                    case "二 樓": LiftControlFloor = "0002"; break;
                                    case "三 樓": LiftControlFloor = "0003"; break;
                                    case "四 樓": LiftControlFloor = "0004"; break;
                                    case "五 樓": LiftControlFloor = "0005"; break;
                                    case "六 樓": LiftControlFloor = "0006"; break;
                                    case "七 樓": LiftControlFloor = "0007"; break;
                                    case "八 樓": LiftControlFloor = "0008"; break;
                                };

                                if (LiftControlFloor != (null))
                                {
                                 Intent intent = new Intent(this, typeof(PLCActivity));
                                 intent.PutExtra("LiftControl", LiftControlFloor);
                                 intent.PutExtra("LuisIntentResult", LuisIntentResult);
                                 intent.PutExtra("LuisEntitiResult", LuisEntitiResult);
                                    StartActivity(intent);
                                Check4 = true;
                                }
                            }
                            break; 
                        case 8:
                            break;
                            if (Check5 == false)
                            {
                                Intent intent = new Intent(this, typeof(PLCActivity));
                                intent.PutExtra("Read_PLC", "01FF000A4420000000000100");
                                StartActivity(intent);
                                Check5 = true;
                                ActiveCycleTime = 3;
                            }

                            VoiceIndex++;
                            break;


                        case 9:
                           if (Check6 == false)
                          
                            {
                                string REC_PLC = this.Intent.GetStringExtra("REC_PLC");
                                if (Save_Floor != REC_PLC.Substring(11, 1) & REC_PLC.Substring(11, 1)!= REC_PLC.Substring(7, 1))
                                {
                                    TextToVoiceDelayTime(30, "電梯開始移動從" + REC_PLC.Substring(11,1) + "樓移動至" + REC_PLC.Substring(7, 1) + "樓");
                                    Save_Floor = REC_PLC.Substring(11, 1);
                                    REC_PLC = "";
                                }
                                
                                Check6 = true;
                                ActiveCycleTime = 3;
                                VoiceIndex--;
                            }
                            break;   
                    };

                    PollingIndex = 0;
                }
                PollingIndex++;
                if (VoiceIndex > 10) VoiceIndex = 0;
            };
            Timer2.Start();
        }
        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            if (status == OperationResult.Error)
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
            if (status == OperationResult.Success)
                textToSpeech.SetLanguage(lang);
        }

        protected override void OnActivityResult(int req, Result res, Intent data)
        {
            if (req == NeedLang)//撥放語音
            {
                var installTTS = new Intent();
                installTTS.SetAction(TextToSpeech.Engine.ActionInstallTtsData);
                StartActivity(installTTS);
            }

            if (req == VOICE)//語音轉字串
            {
                if (res == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        VoiceTextInput = matches[0]; 
                        if (VoiceTextInput.Length > 500)
                         VoiceTextInput = VoiceTextInput.Substring(0, 500);
                    }
                    else
                    textBox.Text = "No speech was recognised";
                   
                }
            }

            base.OnActivityResult(req, res, data);

        }
             public async Task LIUS_API(string VoiceTextInput)

        {
         
            var questionInput = Console.ReadLine();
            var result = await LuisRequestToPublicIotApiAsync(VoiceTextInput);
            LuisBox_Entiti.Text = "";
            LuisIntentResult = result.Item1.ToString();
            LuisBox_Intent.Text = LuisIntentResult;
            LuisEntitiResult= result.Item2.ToString();
            LuisBox_Entiti.Text = LuisEntitiResult;
            TextToVoiceDelayTime(20, "語意辨識分數" + LuisIntentResult); 
        }


        private async static Task<Tuple<double, string>> LuisRequestToPublicIotApiAsync(string qstr)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/7b8468e2-fd09-4374-b2e9-ed7563bbecc9")
            };
            if (string.IsNullOrEmpty(qstr))
            {
                qstr = questionStr;
            }
            var resultStr = await httpClient.GetStringAsync($"?subscription-key={MyLuisKey}&q={qstr}");
            var luisResult = Newtonsoft.Json.JsonConvert.DeserializeObject<LuisData>(resultStr);
            if (luisResult != null)
            {
                return new Tuple<double, string>(luisResult.topScoringIntent.score, luisResult.entities[0].entity.ToString());
            }
            return null;
        }


    }
}


