using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using UnityEditor;

namespace Devdog.InventorySystem.Editors
{
    public abstract class InventoryEditorBase : Editor
    {
        protected SerializedProperty script;
        protected CustomOverrideProperty[] overrides;

        protected InventoryItemDatabase database
        {
            get { return InventoryEditorUtility.GetItemDatabase(true, false); }
        }


        public virtual void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
        }

        protected CustomOverrideProperty FindOverride(string name)
        {
            return overrides.FirstOrDefault(o => o.serializedName == name);
        }

        public override void OnInspectorGUI()
        {
            OnCustomInspectorGUI();
        }

        protected virtual void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            serializedObject.Update();

            if(script != null)
                EditorGUILayout.PropertyField(script);

            serializedObject.ApplyModifiedProperties();
        }

    }
}
