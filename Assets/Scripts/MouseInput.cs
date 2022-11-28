using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    
    [Range(1, 5)]
    [SerializeField] private int _branches = 1;
    [SerializeField] private GameObject _ivy;
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
               Instantiate(_ivy, hit.point, Quaternion.identity).GetComponent<IvyGenerator>()
                   .StartIvySpreading(_branches);
            }
        }
        
    }
}
