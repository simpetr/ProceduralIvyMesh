using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class BranchGenerator : MonoBehaviour
{
    [SerializeField] public bool DebugLine = false;
    [Range(.1f, 1f),SerializeField] private float _nodeDistance = .1f;
    [Range(1, 20),SerializeField] private int _nodesNum = 3;
    [Range(.01f, 0.05f),SerializeField] private float _branchWidth = 0.01f;
    [SerializeField] private Material _material;
    [SerializeField] private GameObject _greenArea;
    
    private List<Vector3> _points;
    private List<Vector3> _normals;
    private Vector3 _branchDirection;
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private int _call = 0;

    
    // Start is called before the first frame update
    private void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = _material;
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new Mesh();
            
        _points = new List<Vector3>();
        _normals = new List<Vector3>();

        DebugUtility.logger = DebugLine;
    }

    public void SetBranchParameters( Tuple<int, int> minMaxNodes)
    {
        _nodesNum = Random.Range(minMaxNodes.Item1, minMaxNodes.Item2);
    }

    public void Grow(Vector3 normal)
    {
       Vector3 startPosition = transform.position;
       _normals.Add(normal);
       _points.Add(startPosition);
       
        Debug.Log(startPosition);
        DebugUtility.Sphere(startPosition);
        DebugUtility.DrawRay(startPosition, normal, Color.blue);
        
        Vector3 surfaceParallel = Vector3.ProjectOnPlane(Random.insideUnitSphere, normal).normalized;
        surfaceParallel *= _nodeDistance;
        _branchDirection = surfaceParallel;
        //Because in this version i'm limiting the obstacles only to the plane we won't need to use the raycast
        //the first time because it will never hit anything. Here I'm "simulating" a raycast fired from the 
        //startposition +normal point in a surfacePrallel direction. I used the simulated hit result
        //as starting point of the backray
        DebugUtility.DrawRay(startPosition+normal, surfaceParallel, Color.blue);
        BackRay(startPosition+normal+surfaceParallel,-normal,_nodesNum);
        
        //BuildMesh();
        StartCoroutine(SlowlyBuildMesh());
        
        //TODO Fix Coordinate problem
         transform.position = Vector3.zero;
    }
    
    private void BackRay(Vector3 startingPoint, Vector3 direction, int maxCall)
    {
        RaycastHit hit;
        if (Physics.Raycast(startingPoint, direction, out hit))
        {
            OnHit(hit);
            DebugUtility.DrawRay(startingPoint, direction, Color.red);
            if (maxCall == 0)
                return;
            //start from the previous point in this version, but in a no plane limtied version i would do a ray up relative to the normal
            //and continue on the previous direction
            LoopRay(startingPoint,-direction,maxCall-1);
        }
        //in this version it is impossible that it does not hit
    }

    private void LoopRay(Vector3 startingPoint, Vector3 direction, int maxCall)
    {
        RaycastHit hit;
        Vector3 surfaceParallel = Vector3.ProjectOnPlane(Random.insideUnitSphere, direction).normalized;
        //limit new direction angle to avoid overlapping mesh or loops
        var newDirection = Quaternion.Euler(0, Random.Range(-60, 60), 0) * _branchDirection;
        _branchDirection = newDirection;
        
        if (Physics.Raycast(startingPoint, newDirection, out hit))
        {
            //in this version it won't hit anything it's going to be usefull when 
            //we will also have other obstacles, not only the plane.
        }
        
        DebugUtility.DrawRay(startingPoint, newDirection, Color.blue);
        BackRay(startingPoint+newDirection,-direction,maxCall);
    }
    
    private void OnHit(RaycastHit hit)
    {
        
        _points.Add(hit.point);
        _normals.Add(hit.normal);
        DebugUtility.Sphere(hit.point);
       
    }
    
    IEnumerator SlowlyBuildMesh()
    {
        Vector3[] verts = new Vector3[_points.Count * 2];
        int[] tris = new int[2 * (_points.Count - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < _points.Count; i++)
        {
            Vector3 forward = Vector3.zero;
            if (i < _points.Count - 1)
            {
                forward += _points[i + 1] - _points[i];
            }

            if (i > 0)
            {
                forward += _points[i] - _points[i - 1];
            }
            
            forward.Normalize();
            Vector3 left = new Vector3(-forward.z, 0.1f, forward.x);
            Vector3 pointA = _points[i] + left * _branchWidth;
            Vector3 pointB = _points[i] - left * _branchWidth;
            DebugUtility.Sphere(pointA,Color.cyan);
            DebugUtility.Sphere(pointB,Color.cyan);
            verts[vertIndex] = pointA;
            verts[vertIndex + 1] = pointB;

            if (i < _points.Count - 1)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
                tris[triIndex + 2] = vertIndex + 1;
                
                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = vertIndex + 2;
                tris[triIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            triIndex += 6;

            if (i > 1)
            {
                
                Vector3[] temp = verts.Skip(0).Take(i*2).ToArray();
                int[] temp2= tris.Skip(0).Take(2 * (i - 1) * 3).ToArray();
      
                _mesh.vertices = temp;
                _mesh.triangles = temp2;
                _meshFilter.mesh = _mesh;
                Instantiate(_greenArea, _points[i], Quaternion.identity);
                
                yield return new WaitForSeconds(1);
            }

        }
        
        _mesh.vertices = verts;
        _mesh.triangles = tris;
        _meshFilter.mesh = _mesh;
        
    }
    
    // //Create the mesh based on the points previously found.
    // private void BuildMesh()
    // {
    //     
    //     Vector3[] verts = new Vector3[_points.Count * 2];
    //     int[] tris = new int[2 * (_points.Count - 1) * 3];
    //     int vertIndex = 0;
    //     int triIndex = 0;
    //
    //     for (int i = 0; i < _points.Count; i++)
    //     {
    //         Vector3 forward = Vector3.zero;
    //         if (i < _points.Count - 1)
    //         {
    //             forward += _points[i + 1] - _points[i];
    //         }
    //
    //         if (i > 0)
    //         {
    //             forward += _points[i] - _points[i - 1];
    //         }
    //         
    //         forward.Normalize();
    //         Vector3 left = new Vector3(-forward.z, 0.1f, forward.x);
    //         Vector3 pointA = _points[i] + left * _branchWidth;
    //         Vector3 pointB = _points[i] - left * _branchWidth;
    //         DebugUtility.Sphere(pointA,Color.cyan);
    //         DebugUtility.Sphere(pointB,Color.cyan);
    //         verts[vertIndex] = pointA;
    //         verts[vertIndex + 1] = pointB;
    //
    //         if (i < _points.Count - 1)
    //         {
    //             tris[triIndex] = vertIndex;
    //             tris[triIndex + 1] = vertIndex + 2;
    //             tris[triIndex + 2] = vertIndex + 1;
    //             
    //             tris[triIndex + 3] = vertIndex + 1;
    //             tris[triIndex + 4] = vertIndex + 2;
    //             tris[triIndex + 5] = vertIndex + 3;
    //         }
    //
    //         vertIndex += 2;
    //         triIndex += 6;
    //         
    //     }
    //     
    //     _mesh.vertices = verts;
    //     _mesh.triangles = tris;
    //     _meshFilter.mesh = _mesh;
    // }
}
