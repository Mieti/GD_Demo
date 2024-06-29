using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor.Presets;
using UnityEditor;
#endif

public class WireController2D : MonoBehaviour
{
    // <summary>
    // Project setup:
    // Project Settings > Physics 2D > Default Solver Iterations: Set between 10 to 20
    // If you use URP update materials.
    // 
    // Create a new layer, example: "wire".
    // Go to Project Settings > Physics 2D > Layer Collision Matrix > disables collisions of the layer with itself.
    // Set the layer "wire" in the prefabs EndAnchor, segment, segmentNoPhysics, StartAnchor, WireBuilder. Change children as well. 
    // Do not change plug layer. 
    // 
    // If you use URP add layer "wire" to URP Renderer Data > Filtering.
    // 
    // Keep the gizmos active to be able to select position.
    // </summary>

    // <summary>
    // How to use:
    // Put the prefab WireBuilder in your scene.
    // Choose the starting position by right-clicking with the WireBuilder object selected in the hierarchy.
    // Press Set Start
    // Choose again the position by right-clicking.
    // Press Add Segment.
    // You can select position again and add more segments if you want.
    // Press Set End to finish the wire.
    // Select position and press Set Plug to add the plug if needed.
    // Press Clear if you want to delete the entire wire and start over from scratch.
    // Press undo to undo the previous segment creation.
    // Press Render Wire to update the mesh render of the wire in case you move segments individually from the editor.
    // 
    // Only if you are using the wire without physics and you don't want to modify the wire anymore
    // press Finish no physics wire, this removes the segments as they are not needed because the positions are stored in TubeRender.cs
    // it also removes references and some components. To improve performance.
    // </summary>

    #region TIPS
    [TextArea]
    [Tooltip("Don't remove Notes variable.")]
    public string Notes = "With the WireBuilder object selected use right-click to select position. Have active Gizmos.";
    [TextArea]
    [Tooltip("Don't remove Notes2 variable.")]
    public string Notes2 = "Wire render settings in TubeRender.cs on WireRender object.";
    #endregion

    [Header("SETTINGS")]
    [Tooltip("Disabling it removes the wire physics, for use as a prop (Only change after clearing).")]
    public bool usePhysics = true;
    [Tooltip("Distance between segments and position selected with the mouse. Lowering it allows more precision. Increase it when you want to set the end anchor point. Don't go below 0.01")]
    public float maxDistanceWithSelectedPos = 0.2f;
    [Tooltip("Separation between segments, lower it instance less segments. Don't go below 0.01")]
    public float segmentsSeparation = 0.2f;
    [Tooltip("Prevents infinite segments from being instantiated in case of an error in the code.")]
    public int limitMax = 200;
    private int limit = 0;
    [Tooltip("A higher value improves the stability of the physics.")]
    public float segmentsRadius = 1.5f;
    public float currentDistanceToStartAnchor;
    [Tooltip("Sets the maximum distance from the start anchor point to the end anchor point, based on the number of segments and the separation between them.")]
    public float maxDistanceToStarAnchor;

    [Header("SPAWNED SEGMENTS")]
    public List<Transform> segments;
    [HideInInspector]
    [Tooltip("You can delete these references when you are no longer modifying the wire.")]
    public List<int> undoSegments;
    private int undoCount = 0;

    [Header("REFERENCES")]
    public TubeRenderer2D ropeMesh;
    public Transform startAnchorTemp;
    public Transform firstSegment;
    public Transform endAnchorTemp;
    public Transform plugTemp;

    [Header("PREFABS")]
    public Transform startAnchorPoint;
    public Transform segment;
    public Transform segmentNoPhysics;
    public Transform endAnchorPoint;
    public Transform plugObjt;

    [Header("MOUSE POSS")]
    public Vector3 selectPosition;
    public Transform mousePossHelper;

#if UNITY_EDITOR
    [Header("PRESETS")]
    /// <summary>
    /// Preset of the ConfigurableJoint used by the segments.
    /// </summary>
    public Preset presetJoint;
#endif

    //private Vector3 offset = new Vector3(0, 1.2f, 0);

#if UNITY_EDITOR
    private void Start()
    {
        mousePossHelper.gameObject.SetActive(false);

        if (endAnchorTemp == null)
        {
            GameObject mockEndAnchor = new GameObject("MockEndAnchor");
            endAnchorTemp = mockEndAnchor.transform;
        }
    }

    private void OnValidate()
    {
        ChangeRadius();
    }

    public void GetSegmentsDistance()
    {
        /// <summary>
        /// Instantiate the segments by checking the distance of the last instantiated segment from the selected position.
        /// </summary>

        // Get distance between lastSegment and selected position.
        int lastSegment = segments.Count - 1;
        float distance = Vector3.Distance(segments[lastSegment].position, selectPosition);

        // If the last segment has not reached the selected position another one is created.
        if (distance >= maxDistanceWithSelectedPos + segmentsSeparation && limit <= limitMax)
        {
            // Limit to prevent infinite loop
            limit++;
            if (usePhysics)
            {
                // Instantiate new segment.
                //Debug.Log("Transform position: " + transform.localPosition);
                //Debug.Log("New segment calculated position: " +( segments[lastSegment].position + (Vector3)segments[lastSegment].forward * segmentsSeparation));
                Transform newSegment = Instantiate(segment, segments[lastSegment].position + (Vector3)segments[lastSegment].forward * segmentsSeparation, segments[lastSegment].rotation, transform);
                newSegment.GetComponent<SpringJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
                newSegment.GetComponent<SpringJoint2D>().distance = segmentsSeparation;
                newSegment.GetComponent<DistanceJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
                newSegment.GetComponent<HingeJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
                //Debug.Log("New segment position" + newSegment.transform.position);
                segments.Add(newSegment);
            }
            else
            {
                // Instantiate new segment.
                Transform newSegment = Instantiate(segmentNoPhysics, segments[lastSegment].position + (Vector3)segments[lastSegment].forward * segmentsSeparation, segments[lastSegment].rotation, transform);
                segments.Add(newSegment);
            }
            #region Undo
            undoCount++;
            #endregion

            // The function is repeated until the selected position is reached.
            GetSegmentsDistance();
            return;
        }

        /// <summary>
        /// Sets the maximum distance from the start anchor point to the end anchor point, based on the number of segments and the separation between them.
        /// </summary>
        SetMaxDistance();
    }

    public void AddStart()
    {
        if (startAnchorTemp == null)
        {
            #region unpack prefab
            //When the first segment is created, the prefab is unpacked, to avoid an error that causes references to be lost in play mode.
            if (PrefabUtility.IsPartOfAnyPrefab(this.gameObject))
                PrefabUtility.UnpackPrefabInstance(this.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            #endregion
            Debug.Log("starAnchor: " + selectPosition);

            startAnchorTemp = Instantiate(startAnchorPoint, selectPosition, Quaternion.identity, transform);
            //ropeMesh.ChangeRadius(segmentsRadius);
        }

        // If you do not use physics, the components are removed to the start anchor point to improve performance.
        if (!usePhysics)
        {
            DestroyImmediate(startAnchorTemp.GetComponent<SpringJoint2D>());
            DestroyImmediate(startAnchorTemp.GetComponent<DistanceJoint2D>());
            DestroyImmediate(startAnchorTemp.GetComponent<HingeJoint2D>());
            DestroyImmediate(startAnchorTemp.GetComponent<Collider2D>());
            DestroyImmediate(startAnchorTemp.GetComponent<Rigidbody2D>());
        }
    }

    public void AddSegment()
    {
        #region undo
        undoCount = 0;
        #endregion
        if (firstSegment == null)
        {
            //Debug.Log("First");
            if (usePhysics)
            {
                //Debug.Log("firstSegment: " + startAnchorTemp.position);
                firstSegment = Instantiate(segment, startAnchorTemp.position, startAnchorTemp.rotation, transform);
                firstSegment.GetComponent<SpringJoint2D>().connectedBody = startAnchorTemp.GetComponent<Rigidbody2D>();
                firstSegment.GetComponent<SpringJoint2D>().distance = segmentsSeparation;
                firstSegment.GetComponent<DistanceJoint2D>().connectedBody = startAnchorTemp.GetComponent<Rigidbody2D>();
                firstSegment.GetComponent<HingeJoint2D>().connectedBody = startAnchorTemp.GetComponent<Rigidbody2D>();
            }
            else
            {
                firstSegment = Instantiate(segmentNoPhysics, startAnchorTemp.position, startAnchorTemp.rotation, transform);
            }

            segments.Add(firstSegment);

            #region undo
            undoCount++;
            #endregion
        }

        // The last current segment is rotated in the direction of the selected position.
        int lastSegment = segments.Count - 1;
        //Debug.Log("Last pos: " + segments[lastSegment].position + ", current pos: " + selectPosition);
        segments[lastSegment].LookAt(selectPosition);
        //Debug.Log(" segments[lastSegment] position" + segments[lastSegment].position);
        //Debug.Log("Select Position: " + selectPosition);

        // Segment is added based on the distance to the selected position.
        GetSegmentsDistance();
        RenderWireMesh();

        #region undo
        undoSegments.Add(undoCount);
        #endregion
    }

    public void AddSegmentIncremental()
    {
        //    Debug.Log("Target " + endAnchorPoint.position + ", selected " + selectPosition);
        //    Debug.Log("Target TransformPoint" + transform.InverseTransformVector(endAnchorPoint.position));

        selectPosition = endAnchorTemp.position;
        AddSegment();

        //The last current segment is rotated in the direction of selected position.
        if (usePhysics)
        {
            int lastSegment = segments.Count - 1;
            endAnchorTemp.GetComponent<SpringJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
            endAnchorTemp.GetComponent<SpringJoint2D>().distance = segmentsSeparation;
            endAnchorTemp.GetComponent<HingeJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
            endAnchorTemp.GetComponent<DistanceJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
        }
        //segments[lastSegment].LookAt(targetPos);
        //if (usePhysics)
        //{
        //    //Instantiate new segment.
        //    Transform newSegment = Instantiate(segment, segments[lastSegment].position + (segments[lastSegment].forward * segmentsSeparation), segments[lastSegment].rotation, transform);
        //    newSegment.GetComponent<SpringJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
        //    newSegment.GetComponent<HingeJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
        //    newSegment.GetComponent<DistanceJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
        //    segments.Add(newSegment);
        //    endAnchorTemp.GetComponent<SpringJoint2D>().connectedBody = newSegment.GetComponent<Rigidbody2D>();
        //    endAnchorTemp.GetComponent<HingeJoint2D>().connectedBody = newSegment.GetComponent<Rigidbody2D>();
        //    endAnchorTemp.GetComponent<DistanceJoint2D>().connectedBody = newSegment.GetComponent<Rigidbody2D>();
        //    Debug.Log("Added segment");
        //}
        //else
        //{
        //    //Instantiate new segment.
        //    Transform newSegment = Instantiate(segmentNoPhysics, segments[lastSegment].position + (segments[lastSegment].forward * segmentsSeparation), segments[lastSegment].rotation, transform);
        //    segments.Add(newSegment);
        //}

        //RenderWireMesh();
    }

    /// <summary>
    /// Removes the last segment
    /// </summary>
    public void RemoveLastSegment()
    {
        // can be added here a check on minimun number of segments

        int lastSegmentIdx = segments.Count - 1;

        segments[lastSegmentIdx - 1].transform.position = endAnchorTemp.transform.position;

        if (usePhysics)
        {
            // connect end with previous seg
            endAnchorTemp.GetComponent<SpringJoint2D>().connectedBody = segments[lastSegmentIdx - 1].GetComponent<Rigidbody2D>();
            //endAnchorTemp.GetComponent<SpringJoint2D>().distance = segmentsSeparation;
            endAnchorTemp.GetComponent<DistanceJoint2D>().connectedBody = segments[lastSegmentIdx - 1].GetComponent<Rigidbody2D>();
            endAnchorTemp.GetComponent<HingeJoint2D>().connectedBody = segments[lastSegmentIdx - 1].GetComponent<Rigidbody2D>();
        }
        else
        {
            //Do nothing.
        }
        // destroy segment and remove from the list
        Destroy(segments[lastSegmentIdx].gameObject);
        segments.RemoveAt(lastSegmentIdx);
        RenderWireMesh();
    }
    /// <summary>
    /// Removes the last n segments
    /// </summary>
    public void RemoveLastNSegments(int n)
    {
        // can be added here a check on minimun number of segments
        int toRemove = Mathf.Min(n, segments.Count);
        int newlast = segments.Count - toRemove - 1;

        // move the player to the position
        endAnchorTemp.transform.position = segments[newlast].transform.position;
        if (usePhysics)
        {
            // connect player to the new last segment
            endAnchorTemp.GetComponent<SpringJoint2D>().connectedBody = segments[newlast].GetComponent<Rigidbody2D>();
            //endAnchorTemp.GetComponent<SpringJoint2D>().distance = segmentsSeparation;
            endAnchorTemp.GetComponent<DistanceJoint2D>().connectedBody = segments[newlast].GetComponent<Rigidbody2D>();
            endAnchorTemp.GetComponent<HingeJoint2D>().connectedBody = segments[newlast].GetComponent<Rigidbody2D>();
        }
        else
        {
            //Do nothing.
        }

        for (int i = 0; i < toRemove; i++)
        {
            Destroy(segments[segments.Count - 1].gameObject);
            segments.RemoveAt(segments.Count - 1);
        }
        RenderWireMesh();
    }
    /// <summary>
    /// Removes the segments included in radius
    /// </summary>
    public void RemoveSegmentsRadius(float radius)
    {
        int newlast;
        for (newlast = segments.Count - 1; newlast > 0; newlast--)
        {
            if (Vector3.Distance(endAnchorTemp.transform.position, segments[newlast].position) >= radius)
                break;
        }

        // move the player to the position
        endAnchorTemp.transform.position = segments[newlast].transform.position;
        if (usePhysics)
        {
            // connect player to the new last segment
            endAnchorTemp.GetComponent<DistanceJoint2D>().connectedBody = segments[newlast].GetComponent<Rigidbody2D>();
            endAnchorTemp.GetComponent<SpringJoint2D>().connectedBody = segments[newlast].GetComponent<Rigidbody2D>();
            endAnchorTemp.GetComponent<HingeJoint2D>().connectedBody = segments[newlast].GetComponent<Rigidbody2D>();
        }
        else
        {
            //Do nothing.
        }
        int toRemove = segments.Count - 1 - newlast;
        for (int i = 0; i < toRemove; i++)
        {
            Destroy(segments[segments.Count - 1].gameObject);
            segments.RemoveAt(segments.Count - 1);
        }
        RenderWireMesh();
    }
    /// <summary>
    /// Calculates the tension of the last n segments
    /// </summary>
    //public float RopeTension(int numSeg)
    //{
    //    float t = 0f;
    //    int size = segments.Count;
    //    int count = Mathf.Min(numSeg, size);
    //    if (count == 0) return 0;
    //    // can change from half rope to last n elements
    //    for (int i = size - count; i < size; i++)
    //    {
    //        SpringJoint2D j = segments[i].GetComponent<SpringJoint2D>();
    //        t += j.jointSpeed;
    //    }
    //    t /= count;
    //    return t;
    //}

    /// <summary>
    /// Calculates if distance between segs exceedes max
    /// </summary>

    public bool RopeDistance()
    {
        float epsilon = 0.1f;
        float maxDistance = segmentsSeparation + epsilon;
        float distance = 0f;
        for (int i = 0; i < segments.Count - 1; i++)
        {
            Vector3 segment1Pos = segments[i].position;
            Vector3 segment2Pos = segments[i + 1].position;

            distance += Vector3.Distance(segment1Pos, segment2Pos);
            //Debug.Log("Maxdistance: " + distance);
            //if (flag)
            //{
            //    Debug.Log("distance: " + distance);
            //}
            
        }
        float avgDistance = distance / (segments.Count + 1);
        Debug.Log("Max distance: " + avgDistance + ", segments: " + segments.Count);
        if (avgDistance > maxDistance)
        {
           return true; // Se la distanza tra i segmenti � maggiore della distanza massima, ritorna true
        }
        return false; // Se nessuna coppia di segmenti supera la distanza massima, ritorna false
    }

    public void AddEnd()
    {
        int lastSegment = segments.Count - 1;
        endAnchorTemp = Instantiate(endAnchorPoint, segments[lastSegment].position + (Vector3)segments[lastSegment].forward * .0005f, Quaternion.identity, transform);
        endAnchorTemp.GetComponent<SpringJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
        endAnchorTemp.GetComponent<SpringJoint2D>().distance = segmentsSeparation;
        endAnchorTemp.GetComponent<DistanceJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();
        endAnchorTemp.GetComponent<HingeJoint2D>().connectedBody = segments[lastSegment].GetComponent<Rigidbody2D>();

        if (!usePhysics)
        {
            DestroyImmediate(endAnchorTemp.GetComponent<SpringJoint2D>());
            DestroyImmediate(endAnchorTemp.GetComponent<DistanceJoint2D>());
            DestroyImmediate(endAnchorTemp.GetComponent<HingeJoint2D>());
            DestroyImmediate(endAnchorTemp.GetComponent<Collider2D>());
            DestroyImmediate(endAnchorTemp.GetComponent<Rigidbody2D>());
        }

        //endAnchorTemp.GetComponent<PlayerController>().SetWireController(this);
        // Update the camera target
        //CameraMovement cameraMovement = Camera.main.GetComponent<CameraMovement>();
        //if (cameraMovement != null)
        //{
        //    cameraMovement.SetTarget(endAnchorTemp);
        //}
    }

    //public void AddPlug()
    //{
    //    // Instantiate the plug in the selected position.
    //    plugTemp = Instantiate(plugObjt, selectPosition, plugObjt.transform.rotation, transform);

    //    // Set the tag to "Plug2L"
    //    plugTemp.tag = "Plug2L";

    //    // Set the size to x:7, y:7
    //    plugTemp.localScale = new Vector2(7, 7);

    //    PlugController plugScritp = plugTemp.GetComponent<PlugController>();

    //    plugScritp.endAnchor = endAnchorTemp;
    //    plugScritp.endAnchorRB = endAnchorTemp.GetComponent<Rigidbody>();
    //    plugScritp.wireController2D = this;
    //}

    public void SetMaxDistance()
    {
        maxDistanceToStarAnchor = segments.Count * segmentsSeparation;
    }

    ///<summary>
    ///Modifies the radius of the sphere colliders of all instantiated segments.
    ///Increasing the radius usually improves the stability of the physics but makes the collisions less accurate in relation to the mesh.
    /// </summary>

    private void ChangeRadius()
    {
        if (usePhysics)
        {
            foreach (Transform segment in segments)
            {
                segment.GetComponent<CircleCollider2D>().radius = segmentsRadius;
            }
            //ropeMesh.ChangeRadius(segmentsRadius);
            RenderWireMesh();
        }
    }

    #region Buttons
    public void Clear()
    {
        //Destroy the segments.
        for (int i = 1; i < segments.Count; i++)
        {
            //ropeMesh.ClearExistingSprites();
            DestroyImmediate(segments[i].gameObject);
        }

        //Destroy the start anchor point.
        if (firstSegment != null)
            DestroyImmediate(firstSegment.gameObject);

        //Destroy the start anchor point.
        if (startAnchorTemp != null)
            DestroyImmediate(startAnchorTemp.gameObject);

        //Destroy the end anchor point.
        if (endAnchorTemp != null)
            DestroyImmediate(endAnchorTemp.gameObject);

        //Destroy the plug.
        if (plugTemp != null)
            DestroyImmediate(plugTemp.gameObject);


        //Clears the lists.
        segments.Clear();
        #region undo
        //Clear undo list.
        undoSegments.Clear();
        undoCount = 0;
        #endregion

        //Render wire
        RenderWireMesh();
        ClearWireMesh();

        //Reset limit
        limit = 0;
    }

    public void Undo()
    {
        //Destroy the end anchor point.
        if (endAnchorTemp != null)
            DestroyImmediate(endAnchorTemp.gameObject);

        //Undo the last segment creation.
        for (int i = 1; i <= undoSegments[undoSegments.Count - 1]; i++)
        {
            DestroyImmediate(segments[segments.Count - 1].gameObject);
            segments.Remove(segments[segments.Count - 1]);
        }
        undoSegments.RemoveAt(undoSegments.Count - 1);

        //The wire rendering is cleaned.
        if (undoSegments.Count == 0)
            ClearWireMesh();
        //Wire rendering updated
        RenderWireMesh();
    }

    public void ClearWireMesh()
    {
        /// <summary>
        /// When the TubeRender.cs position array is cleared the mesh render is not updated properly.
        /// For it to work properly you have to add 2 momentary positions in the array, that's what this function is for. 
        /// </summary>

        Vector3[] temp = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero
        };
        //Debug.Log("ropeMesh: " + ropeMesh.ToSafeString());
        ropeMesh.SetPositions(temp);
    }

    public void FinishNoPhysicsWire()
    {
        /// <summary>
        /// Only when it is wire without physics.
        /// Only when you no longer want to modify the position of the segments.
        /// Eliminates segments and segment references to improve performance.
        /// </summary>>
        if (!usePhysics)
        {
            foreach (Transform segment in segments)
            {
                DestroyImmediate(segment.gameObject);
            }
            segments.Clear();
            undoSegments.Clear();
        }
        else
        {
            Debug.LogWarning("only use in no-physics wires and when you don't want to modify them anymore.");
        }
    }
    #endregion

    #region Selected Position
    public void SetPosition(Vector3 position)
    {
        //The position of the mouse is saved based on the raycast hit.
        selectPosition = position;
        //Debug.Log("Position: " + selectPosition);
        AddClickPosHelper();
    }
    public void AddClickPosHelper()
    {
        //The helper is placed in the position to see graphically where it is.
        mousePossHelper.transform.position = selectPosition;
    }
    #endregion

#endif

    private void Update()
    {
        if (usePhysics)
        {
            //MoveCollider();
            RenderWireMesh();
            DistanceBetweenStartAndEnd();
        }
    }

    public void DistanceBetweenStartAndEnd()
    {
        currentDistanceToStartAnchor = Vector3.Distance(endAnchorTemp.position, startAnchorTemp.position);

        if (currentDistanceToStartAnchor > maxDistanceToStarAnchor)
        {
            /// <summary>
            /// Call a function when the distance between the start anchor point and the End anchor point exceeds the maximum.
            /// Example: do not let the wire rope move any further.
            /// </summary>>
        }
    }
    public void RenderWireMesh()
    {
        /// <summary>
        /// For more wire render settings see TubeRender.cs.
        /// </summary>

        //Render the wire.
        List<Vector3> tempPos = new List<Vector3>();
        foreach (Transform pos in segments)
        {
            tempPos.Add(pos.localPosition);
        }
        if (endAnchorTemp != null)
        {
            var pos = endAnchorTemp.position;
            tempPos.Add(pos);
        }
        //Debug.Log("endAnchorTemp: " + endAnchorTemp.position);
        //Debug.Log("ropeMesh: " + ropeMesh.ToSafeString());
        //Debug.Log("ropeMesh: " + ropeMesh.ToString());

        ropeMesh.SetPositions(tempPos.ToArray());
    }
}