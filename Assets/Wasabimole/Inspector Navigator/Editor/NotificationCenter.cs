using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

// ---------------------------------------------------------------------------------------------------------------------------
// Notification Center - Info dialog for plugin updates - © 2014, 2015 Wasabimole http://wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------
// Please send your feedback and suggestions to mailto://contact@wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------

namespace Wasabimole.InspectorNavigator.Editor
{
    [Serializable]
    public class NotificationCenter
    {
        string CurrentNotification = string.Empty;
        WWW www;
        string tname, key;
        string keyPrev, keyCurr, keyHas, keyUsg, keyRuns, keyLim;
        int packId, usages;
        bool showUpd, showInf;
        int currentVer, minLim;
        Action repDlg;

        public int RunCount;
        public bool Blink;
        public bool HasNotification;
        public bool HasPreviousNotification { get { return EditorPrefs.HasKey(keyPrev); } }

        string url;

        bool DEBUG_MODE = false;

        public NotificationCenter()
        {
            HasNotification = false;
            Blink = false;
        }

        public NotificationCenter(int currentVer, string tname, int packId, Action repDlg, int usages, int minLim)
            : this(currentVer, tname, packId, repDlg, usages, true, true, true) { this.minLim = minLim; }

        public NotificationCenter(int currentVer, string tname, int packId, Action repDlg, int usages, bool showUpd, bool showInf, bool mode)
            : this(currentVer, tname, packId, repDlg, usages, showUpd, showInf, mode, 3, "http://wasabimole.com/update/") { }

        public NotificationCenter(int currentVer, string tname, int packId, Action repDlg, int usages, bool showUpd, bool showInf, bool mode, int period, string baseURL)
        {
            this.currentVer = currentVer;
            this.tname = tname;
            this.packId = packId;
            this.repDlg = repDlg;
            this.showUpd = showUpd;
            this.showInf = showInf;
            this.usages = usages;
            key = tname.Replace(" ", string.Empty);
            keyPrev = key + "Str";
            keyCurr = key + "Curr";
            keyHas = key + "Has";
            keyUsg = key + "Usages";
            keyRuns = key + "Runs";
            keyLim = key + "Lim";
            System.TimeSpan ts = System.DateTime.Now - new System.DateTime(2014, 11, 1);
            int days = (int)ts.TotalDays;
            int diff = days;
            if (EditorPrefs.HasKey(key)) diff -= EditorPrefs.GetInt(key);
            url = baseURL + key + (mode ? ".tinf" : ".info");
            if ((diff >= period || DEBUG_MODE) && (showUpd || showInf))
            {
                EditorPrefs.SetInt(key, days);
                www = new WWW(url);
            }
            if (mode)
            {
                if (EditorPrefs.HasKey(keyLim)) this.usages = EditorPrefs.GetInt(keyLim);
                else EditorPrefs.SetInt(keyLim, usages);
            }
            if (EditorPrefs.HasKey(keyRuns))
                RunCount = EditorPrefs.GetInt(keyRuns);
            EditorPrefs.SetInt(keyRuns, RunCount + 1);
            if (EditorPrefs.HasKey(keyHas))
                HasNotification = EditorPrefs.GetBool(keyHas);
            if (EditorPrefs.HasKey(keyCurr))
                CurrentNotification = EditorPrefs.GetString(keyCurr);
        }

        public bool Update()
        {
            if (www != null)
            {
                try
                {
                    if (www.isDone)
                    {
                        if (string.IsNullOrEmpty(www.error)) CheckNotifications(www.text);
                        www = null;
                    }
                }
                catch { }
            }

            if (HasNotification)
            {
                bool tmp = ((int)EditorApplication.timeSinceStartup) % 2 == 0;
                if (tmp != Blink) { Blink = tmp; return true; }
            }
            return false;
        }

        bool isForceGet = false;

        public void ForceGetNotification()
        {
            if (www != null) return;
            www = new WWW(url);
            isForceGet = true;
        }

        void CheckNotifications(string values)
        {
            var newNotification = values.Replace("\\n", "\n").Replace("<br/>", "\n");
            string[] split = newNotification.Split('|');

            if (split.Length < 6) return;

            int oldHash = 0;
            if (HasPreviousNotification)
            {
                string tmp = EditorPrefs.GetString(keyPrev);
                int.TryParse(tmp.Substring(0, tmp.IndexOf('|')), out oldHash);
            }

            int newHash = 0;
            int.TryParse(split[0], out newHash);

            int newVersion = 0;
            int.TryParse(split[1], out newVersion);

            if (DEBUG_MODE || isForceGet || (newVersion == 0 && oldHash != newHash && showInf) || (newVersion > currentVer && showUpd))
            {
                if (HasNotification && !string.IsNullOrEmpty(CurrentNotification))
                    EditorPrefs.SetString(keyPrev, CurrentNotification);
                else
                    EditorPrefs.SetBool(keyHas, HasNotification = true);
                EditorPrefs.SetString(keyCurr, CurrentNotification = newNotification);
                isForceGet = false;
            }
        }

        public void AttendNotification()
        {
            EditorPrefs.SetBool(keyHas, HasNotification = false);
            if (!string.IsNullOrEmpty(CurrentNotification))
            {
                string[] split = CurrentNotification.Split('|');
                EditorWindow.GetWindowWithRect<NotificationWindow>(new Rect(100f, 100f, 570f, 256f), true, split[2]).Init(split);
                EditorPrefs.SetString(keyPrev, CurrentNotification);
                EditorPrefs.SetString(keyCurr, CurrentNotification = string.Empty);
            }
            else
            {
                repDlg();
#if TRIAL_VERSION
				var option = EditorUtility.DisplayDialog(GetString("Ticmov&aos\"wv|oig!") + tname, GetString("Ig\"zkp&aiof#") + tname + GetString(" tqfbpj+ qnfevc'cnlpmacu rwstjtsioe#mqu'ddtfhjvjeov#f|&rpfpb`lh` um#pmc'ftno$scushmm*") + "\n\n" + GetString("Ticmo%hu "), GetString("Uqeqeac"), GetString("M`{aa%jftdp"));
				if (option) NotificationWindow.OpenAssetStore("content/" + packId);
                usages -= 0x10;
                if (usages < minLim) usages = minLim;
                EditorPrefs.SetInt(keyLim, usages);
#else
                var option = EditorUtility.DisplayDialogComplex("Thanks for using " + tname, "If you find " + tname + " useful, please consider taking a minute to help us, and giving us a good review on the Unity Asset Store.\n\n" + "                        * * * * * / * * * * *\n\n" + "Good reviews mean more users, and the more time we can afford on improving this tool.", "Ok, review now", "Don't ask again", "Maybe later");
                if (option < 2)
                {
                    if (option == 0) NotificationWindow.OpenAssetStore("content/" + packId);
                    EditorPrefs.SetInt(keyUsg, -1 + minLim);
                }
#endif
                EditorPrefs.SetInt(keyUsg, 0);
            }
        }

        public void ShowPreviousNotification()
        {
            string[] split = EditorPrefs.GetString(keyPrev).Split('|');
            EditorWindow.GetWindowWithRect<NotificationWindow>(new Rect(100f, 100f, 570f, 256f), true, split[2]).Init(split);
        }

        public void AddUsage() { AddUsage(1); }

        public void AddUsage(int count)
        {
            int counter = 0;
            if (EditorPrefs.HasKey(keyUsg))
                counter = EditorPrefs.GetInt(keyUsg);
            if (DEBUG_MODE && counter == -1)
                EditorPrefs.SetInt(keyUsg, counter = 0);
            if (counter == -1) return;
            EditorPrefs.SetInt(keyUsg, counter += count);
            if (counter >= usages && !HasNotification)
                EditorPrefs.SetBool(keyHas, HasNotification = true);
        }

        string GetString(string s)
        {
            var sb = new StringBuilder();
            for (var n = 0; n < s.Length; n++)
            {
                var c = (char)((n & 7) ^ s[n]);
                if (c == '"' || c == '\\') sb.Append('\\');
                sb.Append(c);
            }
            s = sb.ToString();
            return s;
        }
    }

    public class NotificationWindow : EditorWindow
    {
        public string[] Values;

        WWW www;
        Texture2D ImageTexture;
        GUIStyle HeaderStyle;
        GUIStyle BodyStyle;
        GUIContent gc;

        public void Init(string[] values)
        {
            Values = values;
            www = new WWW(Values[3]);
            if (HeaderStyle == null)
            {
                HeaderStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                BodyStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                gc = new GUIContent();
            }
            HeaderStyle.richText = true;
            BodyStyle.richText = true;
        }

        public void Update()
        {
            if (www != null)
            {
                if (www.isDone)
                {
                    ImageTexture = www.texture;
                    ImageTexture.hideFlags = HideFlags.HideAndDontSave;
                    Repaint();
                    www = null;
                }
            }
        }

        public void OnGUI()
        {
            if (Values == null) return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(ImageTexture, GUILayout.Width(128));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Values[4], HeaderStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Values[5], BodyStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            float width = 0;
            if (Values.Length > 6 && !string.IsNullOrEmpty(Values[6]))
            {
                gc.text = Values[6];
                width = EditorStyles.whiteLabel.CalcSize(gc).x + 10; ;
                if (GUILayout.Button(gc, GUILayout.Width(width)))
                {
                    if (Values.Length > 7 && !string.IsNullOrEmpty(Values[7]))
                        if (Values[7].Substring(0, 8) == "content/")
                            OpenAssetStore(Values[7]);
                        else
                            Application.OpenURL(Values[7]);
                }
            }

            EditorGUILayout.Space();
            if (Values.Length > 8 && !string.IsNullOrEmpty(Values[8]))
            {
                gc.text = Values[8];
                width = EditorStyles.whiteLabel.CalcSize(gc).x + 10; ;
                if (GUILayout.Button(gc, GUILayout.Width(width)))
                {
                    if (Values.Length > 9 && !string.IsNullOrEmpty(Values[9]))
                    {
                        if (Values[9].Substring(0, 8) == "content/")
                            OpenAssetStore(Values[9]);
                        else
                            Application.OpenURL(Values[9]);
                    }
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.repaint)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                float size = rect.height + 28;
                if (minSize.y != size) minSize = maxSize = new Vector2(570f, size);
            }
        }

        public static void OpenAssetStore(string content)
        {
            typeof(EditorGUI).Assembly.GetType("UnityEditor.AssetStoreWindow").GetMethod("OpenURL", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).
                Invoke(null, new object[] { content });
        }
    }
}
