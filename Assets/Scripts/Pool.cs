using UnityEngine;
using System.Collections.Generic;

public class Pool : MonoBehaviour
{
    public GameObject prefab;

    public int startCount;
    public int maxCount;

    private Queue<GameObject> pool;
    private Queue<GameObject> active;

	void Start ()
    {
        pool = new Queue<GameObject>();
        active = new Queue<GameObject>();

        for (int i = 0; i < startCount; i++)
        {
            GameObject go = Instantiate(prefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
	}

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
            go.SetActive(true);
            active.Enqueue(go);

            return go;
        }
        else // The pool is empty
        {
            // If we can make more
            if(active.Count < maxCount)
            {
                GameObject go = Instantiate(prefab, transform);
                active.Enqueue(go);
                return go;
            }
            else // Grab the oldest active object
            {
                GameObject go = active.Dequeue();
                active.Enqueue(go);
                return go;
            }
        }
    }

    public void OnDestroy()
    {
        GameObject[] objects = new GameObject[active.Count + pool.Count];
        pool.ToArray().CopyTo(objects, 0);
        active.ToArray().CopyTo(objects, pool.Count);

        for (int i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i]);
        }
    }
}
