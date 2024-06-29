using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class TubeRenderer2D : MonoBehaviour
{
    [SerializeField] Vector3[] _positions;
    [SerializeField] int _sides;
    [SerializeField] float _radiusOne;
    [SerializeField] float _radiusTwo;
    [SerializeField] bool _useWorldSpace = true;
    [SerializeField] private Camera _camera;

    //private Vector3[] _vertices;
    private SpriteRenderer _spriteRenderer;
    private LineRenderer _lineRenderer;
    private float _radius = 0.05f;
    //private Mesh _mesh;
    //private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    public Material material
    {
        get { return _spriteRenderer.material; }
        set { _spriteRenderer.material = value; }
    }

    void Awake()
    {
        //_camera = GetComponent<Camera>();
        //if (_camera == null)
        //{
        //    Debug.LogError("No camera set");
        //}
        //_meshCollider = GetComponent<MeshCollider>();
        //if (_meshCollider == null)
        //{
        //    _meshCollider = gameObject.AddComponent<MeshCollider>();
        //}
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            SetRadius();

        }
        //_mesh = new Mesh();
    }
    private void OnEnable()
    {
        _lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        GenerateSprite();
    }

    private void OnValidate()
    {
        _sides = Mathf.Max(3, _sides);
    }

    public void SetPositions(Vector3[] positions)
    {
        _positions = positions;
        GenerateSprite();
    }

    public void ChangeRadius(float radius)
    {
        _radius = radius;
        _radius = radius;
    }

    public void SetRadius()
    {
        _lineRenderer.startWidth = _radius;
        _lineRenderer.endWidth = _radius;
    }

    private void GenerateSprite()
    {
        if (_positions == null || _positions.Length <= 1)
        {
            return;
        }

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = _positions.Length;
        //List<Vector2> all_pos_a = new List<Vector2>();
        //List<Vector2> all_pos_b = new List<Vector2>();
        //transform.TransformPoints(_positions);
        //Vector2 last_pos = Vector2.zero;
        for (int i = 0; i < _positions.Length; i++)
        {
            var pos2D = transform.TransformPoint(_positions[i]);
            //var pos2D =_positions[i];
            _lineRenderer.SetPosition(i, pos2D);
            //if (i != 0)
            //{
            //    var distance = Vector2.Distance(pos2D, last_pos);
            //    all_pos_a.Add(new Vector2(distance, 1));
            //    all_pos_b.Add(new Vector2(distance, -1));
            //}

        }
        //if (_meshCollider != null)
        //{
        //    var _mesh = new Mesh();
        //    //Debug.Log("Camera: " + Camera.current);
        //    _lineRenderer.BakeMesh(_mesh, _camera, true);
        //    //_lineRenderer.BakeMesh(_mesh, true);
        //    //Debug.Log("Mesh after: " + _mesh);
        //    //_meshFilter.mesh = _mesh;
        //    _meshCollider.sharedMesh = _mesh; // Use sharedMesh for efficiency
        //}
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

}
