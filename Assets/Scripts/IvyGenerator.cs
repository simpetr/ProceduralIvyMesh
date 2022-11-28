using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class IvyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _branch;
    [SerializeField] private GameObject _greenArea;
    public int NumBranches { get; set; } = 1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartIvySpreading(int numBranches = 1)
    {
        for (int i = 0; i <numBranches;i++)
            Instantiate(_branch, transform.position, Quaternion.identity).GetComponent<BranchGenerator>().
                Grow(Vector3.up);
        Instantiate(_greenArea, transform.position, quaternion.identity);

    }
}