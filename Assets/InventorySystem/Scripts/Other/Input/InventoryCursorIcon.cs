using System;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public struct InventoryCursorIcon
    {
        public Texture2D texture;
        public Vector2 hotspot;
        public CursorMode cursorMode;

        public InventoryCursorIcon(Texture2D texture, Vector2 hotspot, CursorMode cursorMode = CursorMode.Auto)
        {
            this.texture = texture;
            this.hotspot = hotspot;
            this.cursorMode = cursorMode;
        }


        public void Enable()
        {
            Cursor.SetCursor(texture, hotspot, cursorMode);    
        }
    }
}
