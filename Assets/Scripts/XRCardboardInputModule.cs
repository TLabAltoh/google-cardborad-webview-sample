/**
 * Portions of this file are taken from the following repositories: 
 * Repository Name: cardboard-xr-plugin
 * Repository URL: https://github.com/joaoborks/cardboard-xr-plugin
 * Author: https://github.com/joaoborks
 * Lisence: Apache License 2.0
 * Source File: Samples~/hellocardboard-unity/Scripts/XRCardboard/XRCardboardInputModule.cs
 * Source Commit ID: d9f0aa2eb03eb544069ea7278048994e548e6d03
 * Modifications: Fire both pointer click and pointer up/down
 */

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
#if !UNITY_EDITOR
using UnityEngine.XR;
#endif

public class XRCardboardInputModule : PointerInputModule
{
    [SerializeField]
    XRCardboardInputSettings settings = default;
    [SerializeField]
    bool interactWithTouch = false;

    PointerEventData pointerEventData;
    GameObject currentTarget;
    float currentTargetClickTime = float.MaxValue;
    bool hovering;

    public override void Process()
    {
        HandleLook();
        HandleSelection();
    }

    void HandleLook()
    {
        if (pointerEventData == null)
            pointerEventData = new PointerEventData(eventSystem);
#if UNITY_EDITOR
        pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
#else
        pointerEventData.position = new Vector2(XRSettings.eyeTextureWidth / 2, XRSettings.eyeTextureHeight / 2);
#endif
        pointerEventData.delta = Vector2.zero;
        var raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, raycastResults);
        raycastResults = raycastResults.OrderBy(r => !r.module.GetComponent<GraphicRaycaster>()).ToList();
        pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);
        pointerEventData.pointerPressRaycast = pointerEventData.pointerCurrentRaycast;
        ProcessMove(pointerEventData);
    }

    bool GetInputDown() => interactWithTouch ? Input.GetMouseButtonDown(0) : Input.GetButtonDown(settings.ClickInput);

    bool GetInput() => interactWithTouch ? Input.GetMouseButton(0) : Input.GetButton(settings.ClickInput);

    bool GetInputUp() => interactWithTouch ? Input.GetMouseButtonUp(0) : Input.GetButtonUp(settings.ClickInput);

    void HandleSelection()
    {
        GameObject handler = ExecuteEvents.GetEventHandler<IEventSystemHandler>(pointerEventData.pointerEnter);
        if (handler == null)
        {
            currentTarget = null;
            StopHovering();
            return;
        }

        if (currentTarget != handler)
        {
            var gazeTime = settings.GazeTime;
            currentTarget = handler;
            currentTargetClickTime = Time.realtimeSinceStartup + gazeTime;
            if (hovering)
                StopHovering();
            hovering = true;
            ExecuteEvents.ExecuteHierarchy(currentTarget, pointerEventData, ExecuteEvents.pointerEnterHandler);
        }

        if ((settings.GazeEnabled ? Time.realtimeSinceStartup > currentTargetClickTime : false) || GetInputDown())
        {
            currentTargetClickTime = float.MaxValue;
            ExecuteEvents.ExecuteHierarchy(currentTarget, pointerEventData, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.ExecuteHierarchy(currentTarget, pointerEventData, ExecuteEvents.pointerDownHandler);
        }
        else if (GetInput())
            ExecuteEvents.ExecuteHierarchy(currentTarget, pointerEventData, ExecuteEvents.dragHandler);
        else if (GetInputUp())
            ExecuteEvents.ExecuteHierarchy(currentTarget, pointerEventData, ExecuteEvents.pointerUpHandler);
    }

    void StopHovering()
    {
        if (!hovering)
            return;
        hovering = false;
        ExecuteEvents.ExecuteHierarchy(currentTarget, pointerEventData, ExecuteEvents.pointerExitHandler);
    }
}