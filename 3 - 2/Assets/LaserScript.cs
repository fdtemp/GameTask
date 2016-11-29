using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour {
    public float AppearTime = 0;
    public ObjectPool<GameObject> Pool;
    private float StartTime;
    public LineRenderer l;

    void Start() {
        StartTime = Time.time;
        l = gameObject.GetComponent<LineRenderer>();
    }
    void Update() {
        if (Time.time - StartTime > AppearTime)
            Pool.Put(gameObject);
    }
}
