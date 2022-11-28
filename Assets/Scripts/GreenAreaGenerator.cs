using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

// Spawn a little green area (one flower + some grass)
public class GreenAreaGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _flowerList;
    [SerializeField] private List<GameObject> _grassList;
    

    public float GrowingTime { get; set; } = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        int randomIndex = Random.Range(0, _flowerList.Count);
        var flower = Instantiate(_flowerList[randomIndex], transform.position, Quaternion.identity);
        flower.transform.localScale = new Vector3(1, 0, 1);
        StartCoroutine(ScaleUpObject(flower));
        int numGrass = Random.Range(1, 5);
        for (int i = 0; i < numGrass; i++)
        {
            var position = Random.insideUnitSphere;
            position.y = 0f;
            randomIndex = Random.Range(0, _grassList.Count);
            var grass = Instantiate(_grassList[randomIndex], transform.position + position, Quaternion.Euler(0,Random.Range(0,180),0));
            grass.transform.localScale = new Vector3(1, 0, 1);
            StartCoroutine(ScaleUpObject(grass));
        }
    }

    IEnumerator ScaleUpObject(GameObject element)
    {
        float elapsedTime = 0f;
        float startingValue = 0f;
        float endValue = 1f;
        float lerpTime = GrowingTime + Random.Range(0.5f, 1.5f);
        while (elapsedTime <= lerpTime)
        {
            float lerpedValue = Mathf.Lerp(startingValue, endValue, elapsedTime/lerpTime);
            elapsedTime += Time.deltaTime;
            element.transform.localScale = new Vector3(1,lerpedValue, 1);
            yield return null;
        }
        element.transform.localScale = Vector3.one;
    }
}
