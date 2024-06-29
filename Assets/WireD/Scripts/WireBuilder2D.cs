#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(WireController2D))]
[ExecuteInEditMode]
public class WireBuilder2D : Editor
{
    Vector3 position;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WireController2D WireController2D = (WireController2D)target;
        if (GUILayout.Button("Set Start"))
        {
            WireController2D.AddStart();
        }
        if (GUILayout.Button("Add Segment"))
        {
            WireController2D.AddSegment();
        }
        if (GUILayout.Button("Set End"))
        {
            WireController2D.AddEnd();
        }
        //if (GUILayout.Button("Set Plug"))
        //{
        //    WireController2D.AddPlug();
        //}
        if (GUILayout.Button("Clear"))
        {
            WireController2D.Clear();
        }
        if (GUILayout.Button("Undo"))
        {
            WireController2D.Undo();
        }
        if (GUILayout.Button("Render Wire"))
        {
            WireController2D.RenderWireMesh();
        }
        if (GUILayout.Button("Finish no physics wire"))
        {
            WireController2D.FinishNoPhysicsWire();
        }
    }
    public void Update()
    {
        OnSceneGUI();
    }

    void OnSceneGUI()
    {
        Event e = Event.current;

        // check mouse down event
        if (e.type != EventType.MouseDown)
        {
            return;
        }

        // check right mouse button
        if (e.button != 1)
        {
            return;
        }

        //Debug.Log("Ray: " + Event.current.mousePosition);
        // create OnSceneGUI ray
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector2 pos = ray.origin;
        //Debug.Log("Ray: " + pos);
        // check hit
        //if (!Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    return;
        //}

        // Use ray cast hit here
        //Debug.Log("MousePos: " + pos);

        //position = hit.point;
        position = pos;
        Debug.Log("MousePos: " + pos);
        WireController2D WireController2D = (WireController2D)target;
        WireController2D.SetPosition(position);

        // tell event to no longer propergate
        e.Use();
    }
}
#endif