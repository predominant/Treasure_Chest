using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public abstract class InventoryObjectPickerBase<T> : EditorWindow where T : class
    {
        #region Events & Delegates

        public delegate void PickObject(T obj);
        public delegate void CloseWindow();

        private List<PickObject> onPickObjectList = new List<PickObject>(2);

        private event PickObject _OnPickObject;
        public event PickObject OnPickObject
        {
            add
            {
                _OnPickObject += value;
                onPickObjectList.Add(value);
            }
            remove
            {
                _OnPickObject -= value;
                onPickObjectList.Remove(value);
            }
        }

        public event CloseWindow OnClose;

        #endregion


        
        public virtual EditorWindow window { get; protected set; }
        public string windowTitle { get; set; }
        
        // Not a list to add serialization
        protected IList<T> objects;
        protected Vector2 scrollPosition;
        public string searchQuery;

        private bool focusOnInput = true;
        public int selectionIndex;
        public Vector2 minSizeVec { get; set; }
        public bool isUtility { get; protected set; }

        public bool isSearching
        {
            get { return string.IsNullOrEmpty(searchQuery) == false && searchQuery != "Search..."; }
        }


        public InventoryObjectPickerBase() : base()
        {
            objects = new List<T>();
        }

        protected void NotifyPickedObject(T obj)
        {
            if (_OnPickObject != null)
                _OnPickObject(obj);

            Close(true);
        }


        /// <summary>
        /// Find objects of type in asset database.
        /// </summary>
        /// <returns></returns>
        protected abstract IList<T> FindObjects(bool searchProjectFolder);


        [Obsolete("Dont use parameterless Show()", true)]
        public new virtual void Show()
        {}

        /// <summary>
        /// Show the window, searches all available items in the asset database.
        /// </summary>
        public new virtual void Show(bool searchProjectFolder)
        {
            if(searchProjectFolder)
                Show(FindObjects(searchProjectFolder));
        }

        public virtual void Show(IList<T> objectsToPickFrom)
        {
            // Remove prev. events
            while (onPickObjectList.Count > 0)
            {
                _OnPickObject -= onPickObjectList[0];
                onPickObjectList.RemoveAt(0);
            }

            window = GetWindow(GetType(), isUtility);

            objects = objectsToPickFrom;
            selectionIndex = 0;
            focusOnInput = true;
            window.titleContent = new GUIContent(windowTitle);
            window.minSize = minSizeVec;

            window.Show();
        }

        [Obsolete("Use Close(true) instead!", true)]
        public new virtual void Close() { }

        public virtual void Close(bool uselessVariable)
        {
            if (OnClose != null)
                OnClose();
            
            if(window != null)
                window.Close();

            //base.Close();
        }


        public virtual void OnGUI()
        {
            searchQuery = InventoryEditorStyles.SearchBar(searchQuery, this, isSearching);
            if (focusOnInput)
            {
                EditorGUI.FocusTextInControl("SearchField");
                focusOnInput = false;
            }

            //EditorGUILayout.BeginHorizontal();
            //searchQuery = EditorGUILayout.TextField(searchQuery);
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();

            //EditorGUILayout.LabelField("ID", GUILayout.Width(20));
            //searchID = EditorGUILayout.Toggle(searchID);

            //EditorGUILayout.LabelField("Name", GUILayout.Width(40));
            //searchName = EditorGUILayout.Toggle(searchName);

            //EditorGUILayout.LabelField("Description", GUILayout.Width(60));
            //searchDesc = EditorGUILayout.Toggle(searchDesc);

            //EditorGUILayout.LabelField("Category", GUILayout.Width(50));
            //searchCategory = EditorGUILayout.Toggle(searchCategory);

            //EditorGUILayout.LabelField("Rarity", GUILayout.Width(50));
            //searchRarity = EditorGUILayout.Toggle(searchRarity);

            //EditorGUILayout.LabelField("Type", GUILayout.Width(35));
            //searchType = EditorGUILayout.Toggle(searchType);

            //EditorGUILayout.EndHorizontal();

            ShowInfoBox();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            int resultCount = 0;
            T selectedObject = null;
            foreach (var obj in objects)
            {
                EditorGUILayout.BeginHorizontal();

                if (isSearching)
                {
                    string search = searchQuery.ToLower();
                    if (MatchesSearch(obj, search))
                    {
                        if (resultCount == selectionIndex)
                        {
                            GUI.color = Color.green;
                            selectedObject = obj;
                        }

                        DrawObjectButton(obj);
                        resultCount++;
                    }
                }
                else
                {
                    if (resultCount == selectionIndex)
                    {
                        GUI.color = Color.green;
                        selectedObject = obj;
                    }

                    DrawObjectButton(obj);
                    resultCount++;
                }

                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();


            if (Event.current.isKey)
            {
                selectionIndex = Mathf.Clamp(selectionIndex, 0, resultCount - 1);

                // Keyboard movement
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    selectionIndex++;
                    Repaint();
                }
                else if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    selectionIndex--;
                    Repaint();
                }


                // When pressing enter, the selected item
                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (resultCount > 0 && selectedObject != null)
                    {
                        NotifyPickedObject(selectedObject);
                    }
                    else
                    {
                        EditorGUI.FocusTextInControl("SearchField");
                        Repaint();
                    }
                }
            }
        }


        protected virtual void DrawObjectButton(T obj)
        {
            if (GUILayout.Button(obj.ToString()))
                NotifyPickedObject(obj);
        }


        protected abstract bool MatchesSearch(T obj, string search);

        protected virtual void ShowInfoBox()
        {
            EditorGUILayout.HelpBox("Use the up and down arrow keys to select an item.\nHit enter to pick the highlighted item.", MessageType.Info);            
        }
    }
}