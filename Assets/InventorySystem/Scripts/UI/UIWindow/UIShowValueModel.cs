using System;
using UnityEngine;

namespace Devdog.InventorySystem.UI
{
    [System.Serializable]
    public partial class UIShowValueModel
    {
        [Header("Text")]
        public UnityEngine.UI.Text textField;
        public string textFormat = "{0}/{1}";
        public int roundToDecimals = 1;
        public bool clearTextWhenZero = true;

        [Header("Slider")]
        public UnityEngine.UI.Slider slider;

        [Header("Image fill")]
        public UnityEngine.UI.Image imageFill; // Used for fillAmount

        [Header("Audio")]
        public AudioClip activationClip;


        public void Repaint(float current, float max)
        {
            if (textField != null)
            {
                if (current <= 0.0001f && clearTextWhenZero)
                    textField.text = "";
                else
                    textField.text = string.Format(textFormat, System.Math.Round(current, roundToDecimals), System.Math.Round(max, roundToDecimals));
            }

            if (slider != null)
            {
                slider.minValue = 0.0f;
                slider.maxValue = max;

                // To avoid GC
                if (current != slider.value)
                    slider.value = current;
            }

            if (imageFill != null)
            {
                // To avoid GC
                float n = current / max;
                if (n != imageFill.fillAmount)
                    imageFill.fillAmount = n;
            }
        }

        /// <summary>
        /// An action is activated, show it.
        /// </summary>
        public void Activate()
        {
            if(activationClip != null)
                InventoryUtility.AudioPlayOneShot(activationClip);
        }
    }
}
