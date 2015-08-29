using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public partial class InventoryUtility
    {
        /// <summary>
        /// Plays an audio clip, only use this for the UI, it is not pooled so performance isn't superb.
        /// TODO: Pool this
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        public static void AudioPlayOneShot(AudioClip clip, float volume = 1.0f)
        {
            var obj = new GameObject("TEMP_AUDIO_SOURCE_UI");
            var source = obj.AddComponent<AudioSource>();

            source.PlayOneShot(clip, volume);
            Object.Destroy(obj, clip.length + 0.1f);
        }

        public static FieldInfo FindFieldInherited(System.Type startType, string fieldName)
        {
            if (startType == typeof(UnityEngine.MonoBehaviour) || startType == null)
                return null;

            // Copied fields can be restricted with BindingFlags
            var field = startType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field;

            // Keep going untill we hit UnityEngine.MonoBehaviour type.
            return FindFieldInherited(startType.BaseType, fieldName);
        }


        public static void ResetTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}
