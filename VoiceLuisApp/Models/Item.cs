using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace VoiceLuisApp.Models
{

    
    public class LuisData
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public Entity[] entities { get; set; }
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }
    public class PLC_Display_class
    {
        public string PLC_Display { get; set; }
        public string D10_D19 { get; set; }
        public string D20_D29 { get; set; }

        public string D30_D39 { get; set; }
        public string D40_D49 { get; set; }
    }


}