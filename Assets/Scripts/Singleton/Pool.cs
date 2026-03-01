using UnityEngine;
using System.Collections.Generic;

public class Pool : MonoBehaviour
{
    public static Pool instances;

    [SerializeField] private List<poolObject> poolLists;
    private Dictionary<string, Queue<GameObject>> poolDicts;

    private void Awake()
    {
        poolDicts = new Dictionary<string, Queue<GameObject>>();

        instances = this;
        foreach (poolObject po in poolLists)
        {
            Queue<GameObject> nq = new Queue<GameObject>();
            for (int i = 0; i < po.count; i++)
            {
                GameObject np = Instantiate(po.obj);
                np.transform.SetParent(this.transform);
                np.SetActive(false);
                nq.Enqueue(np);
                np.name = po.tag.ToLower();
            }
            poolDicts.Add(po.tag.ToLower(), nq);
        }
    }

    public GameObject CreateObject(string tag, Vector3 position, Vector3 rotation)
    {
        if (!poolDicts.ContainsKey(tag.ToLower()))
            return null;

        GameObject rp = poolDicts[tag.ToLower()].Dequeue();
        rp.transform.position = position;
        rp.transform.localEulerAngles = rotation;
        rp.SetActive(true);

        return rp;
    }

    public void DestroyObject(GameObject prefab)
    {
        if (!poolDicts.ContainsKey(prefab.name.ToLower()))
            return;

        if (prefab.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            rb.linearVelocity = Vector2.zero;

        // prefab.gameObject.transform.position = Vector3.zero;
        prefab.gameObject.SetActive(false);
        prefab.transform.SetParent(this.transform);

        poolDicts[prefab.name.ToLower()].Enqueue(prefab);
    }
}

[System.Serializable]
public class poolObject
{
    public string tag;
    public GameObject obj;
    public int count;
}
