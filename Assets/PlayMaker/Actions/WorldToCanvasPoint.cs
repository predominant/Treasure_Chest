// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUI)]
    [Tooltip("Transforms position from world space into screen space. NOTE: Uses the MainCamera!")]
    public class WorldToCanvasPoint : FsmStateAction
    {
        public FsmGameObject fsmWorldObject;

        public Transform worldObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("World position to transform into screen coordinates if no object provided.")]
        public FsmVector3 fsmWorldPosition;

        public Vector3 worldPosition;

        [UIHint(UIHint.Script)]
        public RectTransform canvasTransform;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the canvas position in a Vector3 Variable. Z will equal zero.")]
        public FsmVector3 storeScreenPoint;

        public override void Reset()
        {
            fsmWorldObject = null;
            worldObject = null;
            fsmWorldPosition = null;
            worldPosition = Vector3.zero;
            storeScreenPoint = null;
        }

        public override void OnEnter()
        {
            DoWorldToCanvasPoint();
            Finish();
        }

        public override void OnUpdate()
        {
            DoWorldToCanvasPoint();
        }

        void DoWorldToCanvasPoint()
        {
            if (Camera.main == null)
            {
                LogError("No MainCamera defined!");
                Finish();
                return;
            }

            Vector3 position = Vector3.zero;

            if (!fsmWorldObject.IsNone)
                position = fsmWorldObject.Value.transform.position;
            else if (null != worldObject)
                position = worldObject.position;
            else if (!fsmWorldPosition.IsNone)
                position = fsmWorldPosition.Value;
            else
                position = worldPosition;

            position = Camera.main.WorldToScreenPoint(position);

            position.x *= canvasTransform.rect.width;
            position.y *= canvasTransform.rect.height;

            storeScreenPoint.Value = new Vector3(position.x, position.y, 0f);
        }


    }
}