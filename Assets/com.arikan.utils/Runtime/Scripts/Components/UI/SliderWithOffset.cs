using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arikan
{
    public class SliderWithOffset : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] float valueRange;
        [Range(-1, 1)] [SerializeField] float offsetRange;
        [SerializeField] float value;
        [SerializeField] float Offset;
        [Space]
        [SerializeField] float maxValue;
        [SerializeField] float minValue;
        [Space]
        public Color PositiveOffset = Color.green;
        public Color NegativeOffset = Color.red;
        public Slider Front;
        public Slider Back;

        public SliderValueEvent onValueChanged;
        public SliderColorEvent onColorChanged;
        public string ValueTextFormat = "{0}";
        public Text ValueText;
        public Text OffsetText;

        private void OnEnable() => SetValue(value);
        private void OnValidate() => UpdateUI();
        public void UpdateUI()
        {
            if (Front)
            {
                if (Back)
                {
                    Front.transform.Find("Background").gameObject.SetActive(false);

                    Front.maxValue = maxValue;
                    Front.minValue = minValue;
                    Back.maxValue = maxValue;
                    Back.minValue = minValue;

#if UNITY_EDITOR
                    value = minValue + valueRange * (maxValue - minValue);
                    Offset = offsetRange * (maxValue - minValue);
#endif
                    var color = Offset < 0 ? NegativeOffset : PositiveOffset;
                    Back.fillRect.GetComponent<Image>().color = color;

                    float backValue = Mathf.Clamp(Back.value + Offset, Back.minValue, Back.maxValue);
                    if (Offset < 0)
                    {
                        Front.value = Mathf.Clamp(value + Offset, minValue, maxValue);
                        Back.value = Mathf.Clamp(value, minValue, maxValue);
                    }
                    else
                    {
                        Back.value = Mathf.Clamp(value + Offset, minValue, maxValue);
                        Front.value = Mathf.Clamp(value, minValue, maxValue);
                    }
                    if (ValueText)
                        ValueText.text = string.Format(ValueTextFormat, value);
                    if (OffsetText)
                        OffsetText.text = Offset == 0 ? string.Empty : Offset.ToString();
                    if (OffsetText)
                        OffsetText.color = color;
                }
            }
        }

        public float SetValue(float newValue) => SetValue(newValue, newValue - this.value, minValue, maxValue);
        public float SetValue(float newValue, float offset) => SetValue(newValue, offset, minValue, maxValue);
        public float SetValue(float newValue, float offset, float min, float max)
        {
            Offset = offset;
            if (max < min)
            {
                minValue = max;
                maxValue = min;
            }
            else
            {
                minValue = min;
                maxValue = max;
            }
            this.value = Mathf.Clamp(newValue, minValue, maxValue);
            valueRange = (this.value - minValue) / (maxValue - minValue);
            offsetRange = this.Offset / (maxValue - minValue);
            UpdateUI();

            if (Application.isPlaying)
            {
                onValueChanged?.Invoke(this.value);
                onColorChanged?.Invoke(Offset < 0 ? NegativeOffset : PositiveOffset);
            }
            return this.value;
        }

        public class SliderValueEvent : UnityEvent<float> { }
        public class SliderColorEvent : UnityEvent<Color> { }
    }
}
