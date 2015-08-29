using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Devdog.InventorySystem;
using Devdog.InventorySystem.UI;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    public class CurrencyEditor : InventorySystemEditorCrudBase<InventoryCurrency>
    {
        protected override List<InventoryCurrency> crudList
        {
            get { return new List<InventoryCurrency>(InventoryEditorUtility.selectedDatabase.currencies); }
            set { InventoryEditorUtility.selectedDatabase.currencies = value.ToArray(); }
        }

        private UnityEditorInternal.ReorderableList currencyConversionList;


        public CurrencyEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {}


        public override void EditItem(InventoryCurrency item)
        {
            base.EditItem(item);
            
            currencyConversionList = new UnityEditorInternal.ReorderableList(item.currencyConversions, typeof(InventoryCurrencyConversionLookup), true, true, true, true);
            currencyConversionList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Currency conversions");
            currencyConversionList.drawElementCallback += (rect, index, active, focused) =>
                {
                    var r = rect;
                    r.height = 18;
                    r.y += 2;

                    var conversion = item.currencyConversions[index];

                    r.width /= 3;
                    r.width -= 5;
                    EditorGUIUtility.labelWidth = 100;
                    conversion.factor = EditorGUI.FloatField(r, "1 " + item.singleName + " to ", conversion.factor);
                    EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;

                    r.x += r.width + 5;

                    var db = InventoryEditorUtility.GetItemDatabase(true, false);

                    //r.width /= 2;
                    var c = InventoryEditorUtility.PopupField(r, "", db.pluralCurrenciesStrings, db.currencies, o => o.ID == item.currencyConversions[index].currencyID);
                    if (c != null)
                        conversion.currencyID = c.ID;

                    r.x += r.width + 5;
                    EditorGUIUtility.labelWidth = 80;
                    conversion.useInAutoConversion = EditorGUI.Toggle(r, "auto conv.", conversion.useInAutoConversion);
                    EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;

                };
            currencyConversionList.onAddCallback += list =>
                {
                    var l = new List<InventoryCurrencyConversionLookup>(item.currencyConversions);
                    l.Add(new InventoryCurrencyConversionLookup());
                    item.currencyConversions = l.ToArray();

                    list.list = item.currencyConversions;
                };
            currencyConversionList.onRemoveCallback += list =>
                {
                    var l = new List<InventoryCurrencyConversionLookup>(item.currencyConversions);
                    l.RemoveAt(list.index);
                    item.currencyConversions = l.ToArray();

                    list.list = item.currencyConversions;
                };

        }


        protected override bool MatchesSearch(InventoryCurrency currency, string searchQuery)
        {
            if (currency == null)
                return false;

            string search = searchQuery.ToLower();
            return (currency.ID.ToString().Contains(search) || currency.singleName.ToLower().Contains(search) || currency.pluralName.ToLower().Contains(search));
        }

        protected override void CreateNewItem()
        {
            var item = new InventoryCurrency();
            item.ID = (crudList.Count > 0) ? crudList.Max(o => o.ID) + 1 : 0;
            AddItem(item, true);
        }

        protected override void DrawSidebarRow(InventoryCurrency item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.singleName, 260);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(InventoryCurrency currency, int itemIndex)
        {
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
            EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;
            var db = InventoryEditorUtility.GetItemDatabase(true, false);

            EditorGUILayout.LabelField("#" + currency.ID);
            currency.singleName = EditorGUILayout.TextField("Single name", currency.singleName);
            currency.pluralName = EditorGUILayout.TextField("Pural name", currency.pluralName);
            currency.description = EditorGUILayout.TextField("Description", currency.description);
            currency.allowFractions = EditorGUILayout.Toggle("Allow fractions", currency.allowFractions);

            currency.icon = InventoryEditorUtility.SimpleObjectPicker<Sprite>("Icon", currency.icon, false, false);

            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("You can use string.Format elements to define the text formatting of the currency: ", InventoryEditorStyles.labelStyle);
            EditorGUILayout.LabelField("{0} = The current amount");
            EditorGUILayout.LabelField("{1} = Min min amount");
            EditorGUILayout.LabelField("{2} = The max amount");
            EditorGUILayout.LabelField("{3} = Single / plural name (depends on amount)");
            GUI.color = Color.white;
            currency.stringFormat = EditorGUILayout.TextField("String format", currency.stringFormat);

            EditorGUILayout.LabelField("Format example (single): ", currency.GetFormattedString(1, 0.0f, 10f));
            EditorGUILayout.LabelField("Format example (pural): ", currency.GetFormattedString(100f, 0.0f, 100f));

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Conversions", InventoryEditorStyles.titleStyle);


            EditorGUILayout.LabelField("Currencies can be converted between one another. For example convert 1 Japanese yen to 0.008 dollars.", InventoryEditorStyles.labelStyle);
            if(currency.currencyConversions.Any(o => o.currencyID == currency.autoConvertOnMaxCurrencyID) == false && currency.autoConvertOnMax)
                EditorGUILayout.HelpBox("Auto convert on max currency " + currency.autoConvertOnMaxCurrency.pluralName + " not in list. Can't convert currency", MessageType.Error);

            if (currency.currencyConversions.Any(o => o.currencyID == currency.autoConvertFractionsToCurrencyID) == false && currency.autoConvertFractions && currency.allowFractions == false)
                EditorGUILayout.HelpBox("Auto convert on fractions " + currency.autoConvertFractionsToCurrency.pluralName + " not in list. Can't convert currency", MessageType.Error);

            EditorGUILayout.LabelField("Make sure you order conversions based on priority.", InventoryEditorStyles.labelStyle);
            currencyConversionList.DoLayoutList();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Automatic conversions", InventoryEditorStyles.titleStyle);

            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("When a currency hits the maximum amount auto. convert it to another currency.", InventoryEditorStyles.labelStyle);
            EditorGUILayout.LabelField("For example, in some games you have copper, silver and gold. You can never have more than 99 coper, because it's converted to silver.", InventoryEditorStyles.labelStyle);
            GUI.color = Color.white;

            currency.autoConvertOnMax = EditorGUILayout.Toggle("Auto convert on max", currency.autoConvertOnMax);
            if (currency.autoConvertOnMax)
            {
                currency.autoConvertOnMaxAmount = EditorGUILayout.FloatField("Auto convert on max amount", currency.autoConvertOnMaxAmount);

                if (currency.autoConvertOnMaxCurrencyID == currency.ID)
                    GUI.color = Color.red;

                var c = InventoryEditorUtility.PopupField("Auto convert to currency", db.pluralCurrenciesStrings, db.currencies, o => o.ID == currency.autoConvertOnMaxCurrencyID);
                if (c != null)
                    currency.autoConvertOnMaxCurrencyID = c.ID;

                GUI.color = Color.white;
                if (currency.autoConvertOnMaxCurrencyID == currency.ID)
                {
                    EditorGUILayout.HelpBox("Can't auto convert to self", MessageType.Error);
                }
            }

            if (currency.allowFractions == false)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUI.color = Color.yellow;
                EditorGUILayout.LabelField("When a fraction is introduced convert it to a lower currency.");
                EditorGUILayout.LabelField("For example, in some games you have copper, silver and gold. When an attempt is done to add 1.1 silver, add 1 silver and 10 copper (converting the fraction to 10 copper).", InventoryEditorStyles.labelStyle);
                EditorGUILayout.LabelField("When this option is disabled fractions will be discarded.");
                GUI.color = Color.white;

                currency.autoConvertFractions = EditorGUILayout.Toggle("Auto convert fractions", currency.autoConvertFractions);
                if (currency.autoConvertFractions)
                {
                    var c = InventoryEditorUtility.PopupField("Auto convert fractions to currency", db.pluralCurrenciesStrings, db.currencies, o => o.ID == currency.autoConvertFractionsToCurrencyID);
                    if (c != null)
                        currency.autoConvertFractionsToCurrencyID = c.ID;

                }
            }



            EditorGUILayout.EndVertical();
        }

        protected override bool IDsOutOfSync()
        {
            return false;
        }

        protected override void SyncIDs()
        {

        }
    }
}
