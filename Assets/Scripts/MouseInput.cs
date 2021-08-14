using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    
    [Range(1, 5)]
    public int branches = 1;
    public GameObject branch;
    private Camera _mainCamera;
    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                for(int i=0;i<branches;i++)
                {
                    var obj = Instantiate(branch, hit.point, Quaternion.identity);
                    obj.GetComponent<BranchGenerator>().Grow(hit.normal);
                }
            }
        }
        
    }
}
