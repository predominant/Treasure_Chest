using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem.Editors
{
    public interface IInventorySystemEditorCrud
    {
        bool requiresDatabase { get; set; }

        void Focus();
        void Draw();
    }
}
