using UnityEngine;

[ExecuteInEditMode]
public class GrassSpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject grassPrefab;
    [SerializeField] 
    private float spawnHeight = 0.0f;
    [SerializeField] 
    private float step = 1f;
    [SerializeField] 
    private float spawnThreshold = 0.75f;
    
    void Start()
    {
        var width = GetComponent<Renderer>().bounds.size.x;
        var height = GetComponent<Renderer>().bounds.size.z;

        var position = transform.position;
        Vector3 bottomLeft = position;

        bottomLeft.x -= width;
        bottomLeft.z -= height;

        for (var x = bottomLeft.x; x < position.x; x += step)
        {
            for (var z = bottomLeft.z; z < position.z; z += step)
            {
                if (Mathf.PerlinNoise(x, z) > spawnThreshold)
                {
                    var grass = Instantiate(grassPrefab);
                    grass.transform.position = new Vector3(x, spawnHeight, z);
                    grass.transform.parent = gameObject.transform;
                    /*grass.transform.localScale = 0.1f * Vector3.one;*/
                }
            }
        }
        
        Debug.Log("Grass Spawned!");
        DestroyImmediate(this);
    }
}
