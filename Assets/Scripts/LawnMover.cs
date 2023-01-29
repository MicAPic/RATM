using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LawnMover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var tempArray = new GameObject[transform.childCount];
        for(var i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = transform.GetChild(i).gameObject;
        }

        for (var i = tempArray.Length - 1; i >= 0; i -= 2)
        {
            DestroyImmediate(tempArray[i].gameObject);
        }
            
        
        Debug.Log("Grass Reduced!");
        DestroyImmediate(this);
    }

}
