using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Arikan
{
    public class ShowLeftTimeOnText : MonoBehaviour
    {
        public enum UpdateMethod
        {
            OnFrame,
            EverySecond,
            EveryMinute,
            OnEnable,
        }

        public int Year = 2018;
        public int Month = 9;
        public int Day = 7;
        public int Hour = 12;
        public int Minute = 0;
        public int Second = 0;
        [Space]
        public string FormatString = "{0}/{1}/{2}/{3}";
        public UpdateMethod Method = UpdateMethod.EverySecond;

        Text Text;
        DateTime target;
        TimeSpan left => target - DateTime.UtcNow;

        private void OnValidate()
        {
            Text = GetComponent<Text>();
            if(enabled && Text)
                SetTime(new DateTime(Year, Month, Day, Hour, Minute, Second, DateTimeKind.Utc));
        }
        private void Start()
        {
            if (Method == UpdateMethod.EveryMinute)
                StartCoroutine(UpdateTextMinuteCoRo());
            if (Method == UpdateMethod.EverySecond)
                StartCoroutine(UpdateTextSecondsCoRo());
        }
        public void SetTime(DateTime dt)
        {
            target = dt;
            Year = dt.Year;
            Month = dt.Month;
            Day = dt.Day;
            Hour = dt.Hour;
            Minute = dt.Minute;
            Second = dt.Second;
            Text.text = String.Format(FormatString, ClampNotNegative((int)left.TotalDays), ClampNotNegative((int)left.Hours), ClampNotNegative((int)left.Minutes), ClampNotNegative((int)left.Seconds));
        }

        private void Update()
        {
            if(Method == UpdateMethod.OnFrame && left.Ticks > 0)
                Text.text = String.Format(FormatString, ClampNotNegative((int)left.TotalDays), ClampNotNegative((int)left.Hours), ClampNotNegative((int)left.Minutes), ClampNotNegative((int)left.Seconds));
        }


        IEnumerator UpdateTextMinuteCoRo()
        {
            while (left.Ticks > 0)
            {
                Text.text = String.Format(FormatString, ClampNotNegative((int)left.TotalDays), ClampNotNegative((int)left.Hours), ClampNotNegative((int)left.Minutes), ClampNotNegative((int)left.Seconds));
                yield return new WaitForSecondsRealtime(60);
            }
        }
        IEnumerator UpdateTextSecondsCoRo()
        {
            while (left.Ticks > 0)
            {
                Text.text = String.Format(FormatString, ClampNotNegative((int)left.TotalDays), ClampNotNegative((int)left.Hours), ClampNotNegative((int)left.Minutes), ClampNotNegative((int)left.Seconds));
                yield return new WaitForSecondsRealtime(1);
            }
        }

        int ClampNotNegative(int value) => Mathf.Clamp(value, 0, int.MaxValue);
    }
}
