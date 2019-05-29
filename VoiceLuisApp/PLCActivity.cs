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
    [Activity(Label = "電梯運轉控制")]
    public class PLCActivity : Activity 
    {
        ObservableCollection<PLC_Display_class> PLC_Data_Class; 
        int PLC_connect_ststus { get; set; }
        public String REC_PLC { get; set; }
        public object Timer2 { get; private set; }

        public int OnCallFloor;

        public string Write_SendMessage, m_SendMessage;
        public bool check1 = false,check2=false;
        public int PollCount=0,PLC_CMD_Index=0;
        private Button btnCall_1F, btnCall_2F, btnCall_3F, btnCall_4F,btnCall_5F, btnCall_6F, btnCall_7F, btnCall_8F;
        private TextView LiftFloorTextView, LuisIntentResultTextView, LuisBox_EntitiTextView;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           SetContentView(Resource.Layout.PLC_Main);
            LiftFloorTextView = FindViewById<TextView>(Resource.Id.textLiftFloor);
            LuisIntentResultTextView = FindViewById<TextView>(Resource.Id.textLuisIntentResult);
            LuisBox_EntitiTextView = FindViewById<TextView>(Resource.Id.textLuisEntitiResult);
            btnCall_1F = FindViewById<Button>(Resource.Id.btnCall_1F);
            btnCall_2F = FindViewById<Button>(Resource.Id.btnCall_2F);
            btnCall_3F = FindViewById<Button>(Resource.Id.btnCall_3F);
            btnCall_4F = FindViewById<Button>(Resource.Id.btnCall_4F);
            btnCall_5F = FindViewById<Button>(Resource.Id.btnCall_5F);
            btnCall_6F = FindViewById<Button>(Resource.Id.btnCall_6F);
            btnCall_7F = FindViewById<Button>(Resource.Id.btnCall_7F);
            btnCall_8F = FindViewById<Button>(Resource.Id.btnCall_8F);

            string LuisIntentResult = this.Intent.GetStringExtra("LuisIntentResult");
            string LuisEntitiResult = this.Intent.GetStringExtra("LuisEntitiResult");
            LuisIntentResultTextView.Text = LuisIntentResult;
            LuisBox_EntitiTextView.Text = LuisEntitiResult;


            string LiftControlCommand = this.Intent.GetStringExtra("LiftControl");
            string ReadPLCAddress = this.Intent.GetStringExtra("Read_PLC");
            if (ReadPLCAddress!=(null))
               { PLC_Connect("01FF000A4420000000000500"); }
            if (LiftControlCommand != (null))
            {
                Write_SendMessage = "03FF000A4420000000000100"+ LiftControlCommand;
                PLC_Connect(Write_SendMessage);
               }
            PollingPLCTime(1, "Read");
            btnCall_1F.Click += delegate { OnCallFloor = 1; };
            btnCall_2F.Click += delegate { OnCallFloor = 2; };
            btnCall_3F.Click += delegate { OnCallFloor = 3; };
            btnCall_4F.Click += delegate { OnCallFloor = 4; };
            btnCall_5F.Click += delegate { OnCallFloor = 5; };
            btnCall_6F.Click += delegate { OnCallFloor = 6; };
            btnCall_7F.Click += delegate { OnCallFloor = 7; };
            btnCall_8F.Click += delegate { OnCallFloor = 8; };

        }
        public void PLC_Connect(string m_SendMessage)
        {
            Socket m_SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
             string m_RemoteIP = "192.168.31.31";
            IPAddress m_RemoteAddress;
            while (true)
            {
                if (!IPAddress.TryParse(m_RemoteIP, out m_RemoteAddress))
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("關於IP位置格式");
                    alert.SetMessage("IP位置格式錯誤檢查IP再試試");
                    alert.Show();
                }
                else
                {
                    break;
                }
            }
            IPEndPoint m_IPPoint = new IPEndPoint(m_RemoteAddress, 4999);
            try
            {
                m_SocketClient.Connect(m_IPPoint);
                byte[] m_RecvBuffer = new byte[8192];
                byte[] m_SendBuffer;
                int m_RecvLen;
                m_SendBuffer = Encoding.Default.GetBytes(m_SendMessage);
                m_SocketClient.Send(m_SendBuffer, m_SendBuffer.Length, SocketFlags.None);
                m_RecvLen = m_SocketClient.Receive(m_RecvBuffer, SocketFlags.None);
                ShowMessage(m_RecvBuffer, m_RecvLen);
                m_SocketClient.Shutdown(SocketShutdown.Both);
                m_SocketClient.Close(); 
            }
            catch (Exception ex)
            {
                m_SocketClient.Close();
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("錯誤發生請確認PLC連線  "+ "異常訊息:"+ex.Message);
                alert.Show();
            }
            /*解碼並顯示伺服器傳回的訊息*/
            void ShowMessage(byte[] buffer, int len)
            {
                REC_PLC = Encoding.Default.GetString(buffer, 0, len);
             //   VoiceLuisApp.Models.PLC_Display_class D10_D19 = REC_PLC;
                PLC_Data_Class = new ObservableCollection<PLC_Display_class>();
                PLC_Data_Class.Add(new PLC_Display_class { PLC_Display = REC_PLC });
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                if (REC_PLC=="8300")
                {
                  // Write to PLC
                }
                else
                {
                  LiftFloorTextView.Text= "電梯目前移動從" + REC_PLC.Substring(11, 1) + "樓移動至" + REC_PLC.Substring(7, 1) + "樓";
                }
            }
        }


        public void PollingPLCTime(int DelayValue, string ReadWriteMode)
        {
            System.Timers.Timer Timer2 = new System.Timers.Timer();
            Timer2.Interval = DelayValue;
            Timer2.Enabled = true;
            Timer2.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {


                if (OnCallFloor > 0)
                {
                    if (OnCallFloor == 1){ ReadWriteMode = "Write_1F";   }
                    if (OnCallFloor == 2) { ReadWriteMode = "Write_2F";  }
                    if (OnCallFloor == 3) { ReadWriteMode = "Write_3F";  }
                    if (OnCallFloor == 4) { ReadWriteMode = "Write_4F";  }
                    if (OnCallFloor == 5) { ReadWriteMode = "Write_5F";  }
                    if (OnCallFloor == 6) { ReadWriteMode = "Write_6F";  }
                    if (OnCallFloor == 7) { ReadWriteMode = "Write_7F";  }
                    if (OnCallFloor == 8) { ReadWriteMode = "Write_8F";  }
                }
                else { ReadWriteMode = "Read"; }

                switch (ReadWriteMode)
                      {
                       case "Read":
                            PLC_Connect("01FF000A4420000000000500");
                            break;
                       case "Write_1F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0001";
                        PLC_Connect(Write_SendMessage);
                        OnCallFloor = 0;
                        break;
                       case "Write_2F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0002";
                        PLC_Connect(Write_SendMessage);
                        OnCallFloor = 0;
                        break;
                       case "Write_3F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0003";
                        PLC_Connect(Write_SendMessage);
                        OnCallFloor = 0;
                        break;
                        case "Write_4F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0004";
                        PLC_Connect(Write_SendMessage);
                        OnCallFloor = 0;
                        break;
                    case "Write_5F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0005";
                        PLC_Connect(Write_SendMessage);
                        OnCallFloor = 0;
                        break;
                    case "Write_6F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0006";
                        PLC_Connect(Write_SendMessage);
                        OnCallFloor = 0;
                        break;
                    case "Write_7F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0007";
                        PLC_Connect(Write_SendMessage);
                        break;
                    case "Write_8F":
                        Write_SendMessage = "03FF000A4420000000000100" + "0008";
                        PLC_Connect(Write_SendMessage);
                        OnCallFloor = 0;
                        break;
                };


            };
                Timer2.Start();
        }











    }
    }







