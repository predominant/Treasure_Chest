using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Devdog.InventorySystem.Dialogs
{
    using Devdog.InventorySystem.UI;

    public delegate void IntValDialogCallback(int value);

    public partial class IntValDialog : InventoryUIDialogBase
    {
        [Header("UI int val")]
        // Leave blank if you don't want to use them
        public UnityEngine.UI.Button plusButton;
        // Leave blank if you don't want to use them
        public UnityEngine.UI.Button minusButton;

        /// <summary>
        /// Increase the amount when the keycode is pressed.
        /// </summary>
        public string plusMinusAxis = ""; // DPadHorizontal


        public int minValue { get; protected set; }
        public int maxValue { get; protected set; }


        [InventoryRequired]
        public UnityEngine.UI.InputField inputField;

        public UIShowValueModel valueVisualizer = new UIShowValueModel();


        protected IntValDialogCallback yesCallback { get; set; }
        protected IntValDialogCallback noCallback { get; set; }

        private bool axisFired = false;


        public override void Update()
        {
            base.Update();

            if (plusMinusAxis == "")
                return;

            var axis = Input.GetAxis(plusMinusAxis);
            if (axis != 0.0f && axisFired == false)
            {
                if (axis < 0.0f)
                {
                    AddToInputValue(-1);
                    axisFired = true;
                }
                else if(axis > 0.0f)
                {
                    AddToInputValue(1);
                    axisFired = true;
                }
            }

            if(Input.GetAxisRaw(plusMinusAxis) == 0.0f)
            {
                axisFired = false;
            }
        }


        /// <summary>
        /// Show this dialog.
        /// <b>Don't forget to call dialog.Hide(); when you want to hide it, this is not done auto. just in case you want to animate it instead of hide it.</b>
        /// </summary>
        /// <param name="title">Title of the dialog.</param>
        /// <param name="description">The description of the dialog.</param>
        /// <param name="yes">The name of the yes button.</param>
        /// <param name="no">The name of the no button.</param>
        /// <param name="minValue">The minimal value allowed to be selected.</param>
        /// <param name="maxValue">The max value allowed to be selected.</param> 
        /// <param name="yesCallback"></param>
        /// <param name="noCallback"></param>
        public virtual void ShowDialog(Transform caller, string title, string description, int min, int max, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            SetEnabledWhileActive(false);
            this.yesCallback = yesCallback;
            this.noCallback = noCallback;

            window.Show(); // Have to show it first, otherwise we can't use the elements, as they're disabled.


            minValue = min;
            maxValue = max;

            titleText.text = title;
            if (descriptionText != null)
                descriptionText.text = description;
            

            inputField.text = minValue.ToString();
            inputField.onValueChange.RemoveAllListeners();
            inputField.onValueChange.AddListener((string result) => NotifyAmountValueChanged());


            if (plusButton != null)
            {
                plusButton.onClick.RemoveAllListeners();
                plusButton.onClick.AddListener(() =>
                {
                    if (window.isVisible == false)
                        return;

                    if (Input.GetKey(KeyCode.LeftShift))
                        AddToInputValue(10);
                    else
                        AddToInputValue(1);
                });
            }
            if (minusButton != null)
            {
                minusButton.onClick.RemoveAllListeners();
                minusButton.onClick.AddListener(() =>
                {
                    if (window.isVisible == false)
                        return;

                    if (Input.GetKey(KeyCode.LeftShift))
                        AddToInputValue(-10);
                    else
                        AddToInputValue(-1);
                });
            }


            if (yesButton != null)
            {
                yesButton.onClick.RemoveAllListeners();
                yesButton.onClick.AddListener(AcceptAction);
            }

            if (noButton != null)
            {
                noButton.onClick.RemoveAllListeners();
                noButton.onClick.AddListener(DenyAction);
            }

            valueVisualizer.Repaint(GetInputValue(), maxValue);
            NotifyDialogShown(caller);
        }

        protected virtual void NotifyAmountValueChanged()
        {
            ValidateInputField(minValue, maxValue);

        }

        /// <summary>
        /// Show the dialog.
        /// <b>Don't forget to call dialog.Hide(); when you want to hide it, this is not done auto. just in case you want to animate it instead of hide it.</b>
        /// </summary>
        /// <param name="title">The title of the dialog. Note that {0} is the item ID and {1} is the item name.</param>
        /// <param name="description">The description of the dialog. Note that {0} is the item ID and {1} is the item name.</param>
        /// <param name="yes">The name of the yes button.</param>
        /// <param name="no">The name of the no button.</param>
        /// <param name="minValue">The minimal value allowed to be selected.</param>
        /// <param name="maxValue">The max value allowed to be selected.</param> 
        /// <param name="item">
        /// You can add an item, if you're confirming something for that item. This allows you to use {0} for the title and {1} for the description inside the title and description variables of the dialog.
        /// An example:
        /// 
        /// ShowDialog("Are you sure you want to drop {0}?", "{0} sure seems valuable..", ...etc..);
        /// This will show the item name at location {0} and the description at location {1}.
        /// </param>
        /// <param name="yesCallback"></param>
        /// <param name="noCallback"></param>
        public virtual void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, InventoryItemBase item, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            ShowDialog(caller, string.Format(string.Format(title, item.name, item.description)), string.Format(description, item.name, item.description), minValue, maxValue, yesCallback, noCallback);
        }


        public virtual void AcceptAction()
        {
            if (window.isVisible == false)
                return;

            if (ValidateInputField(minValue, maxValue) == false)
                return;

            SetEnabledWhileActive(true);
            valueVisualizer.Activate();
            yesCallback(GetInputValue());
            window.Hide();
        }

        public virtual void DenyAction()
        {
            if (window.isVisible == false)
                return;

            SetEnabledWhileActive(true);
            if (ValidateInputField(minValue, maxValue) == false)
                noCallback(-1);
            else
                noCallback(GetInputValue());

            window.Hide();
        }

        protected void AddToInputValue(int add)
        {
            if (inputField.text == "")
                inputField.text = minValue.ToString();

            inputField.text = (GetInputValue() + add).ToString();

            NotifyAmountValueChanged();
            valueVisualizer.Repaint(GetInputValue(), maxValue);
        }

        public int GetInputValue()
        {
            if (inputField.text == "")
                return minValue;

            return int.Parse(inputField.text);
        }

        protected virtual bool ValidateInputField(int minValue, int maxValue)
        {
            if (inputField.text == "")
                return false;

            int r = GetInputValue();
            if (r > maxValue)
                inputField.text = maxValue.ToString();
            else if (r < minValue)
                inputField.text = minValue.ToString();

            return true;
        }
    }
}