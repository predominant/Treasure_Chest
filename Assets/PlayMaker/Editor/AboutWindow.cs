// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

/* NOTE: Wrapper no longer needed in Unity 4.x
 * BUT: changing it breaks saved layouts
 * SO: wrap in namespace instead (supported in 4.x)
 */

// EditorWindow classes can't be called from a dll in Unity 3.5
// so create a thin wrapper class as a workaround

using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    internal class AboutWindow : BaseEditorWindow
    {
        // Temporary implementation of About Window for version 1.7.8
        // Normally implemented in AboutPlayMaker

        private static bool heightHasBeenSet;

        public override void Initialize()
        {
            InitWindowTitle();

            // initial fixed size
            minSize = new Vector2(264, 292);
            maxSize = new Vector2(264, 292);

            // updated to fit contents in OnGUI
            heightHasBeenSet = false;
        }

        public void InitWindowTitle()
        {
            title = Strings.AboutPlaymaker_Title;
        }

        public override void DoGUI()
        {
            FsmEditorStyles.Init();

            GUILayout.BeginVertical();

            FsmEditorGUILayout.PlaymakerHeader(this);

            GUILayout.Space(10);
            GUILayout.Label("© Hutong Games LLC. All Rights Reserved.", EditorStyles.miniLabel);

            GUILayout.Label("Version 1.7.8.3");
            if (VersionInfo.PlayMakerVersionInfo != "")
            {
                EditorGUILayout.HelpBox(VersionInfo.PlayMakerVersionInfo, MessageType.None);
            }

            EditorGUILayout.HelpBox(string.Format(Strings.AboutPlaymaker_Special_Thanks,
                "Erin Ko, Kemal Amarasingham, Bruce Blumberg, Steve Gargolinski, Lee Hepler, Bart Simon, " +
                "Lucas Meijer, Joachim Ante, Jaydee Alley, James Murchison, XiaoHang Zheng, Andrzej Łukasik, " +
                "Vanessa Wesley, Marek Ledvina, Bob Berkebile, Jean Fabre, MaDDoX, gamesonytablet, " +
                "Marc 'Dreamora' Schaerer, Eugenio 'Ryo567' Martínez, Steven 'Nightreaver' Barthen, " +
                "Damiangto, VisionaiR3D, 黄峻, Nilton Felicio, Andre Dantas Lima, " +
                "Ramprasad Madhavan, and the PlayMaker Community!\r\n"),
                MessageType.None);

            if (GUILayout.Button(Strings.AboutPlaymaker_Release_Notes))
            {
                EditorCommands.OpenWikiPage(WikiPages.ReleaseNotes);
            }

            if (GUILayout.Button(Strings.AboutPlaymaker_Hutong_Games_Link))
            {
                Application.OpenURL("http://www.hutonggames.com/");
            }

            GUILayout.Space(5);

            GUILayout.EndVertical();

            if (!heightHasBeenSet && Event.current.type == EventType.repaint)
            {
                SetWindowHeightToFitContents();
            }
        }

        private void SetWindowHeightToFitContents()
        {
            var height = GUILayoutUtility.GetLastRect().height + 10f;

            position.Set(position.x, position.y, 264, height);

            minSize = new Vector2(264, height);
            maxSize = new Vector2(264, height + 1);

            heightHasBeenSet = true;
        }
    }
}
