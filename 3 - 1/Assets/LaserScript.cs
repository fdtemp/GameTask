using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour {
    float FadeTime = 1;
    public float StartTime;
    LineRenderer l;

    void Start() {
        StartTime = Time.time;
        l = gameObject.GetComponent<LineRenderer>();
    }
    void Update() {
        float a = 1 - (Time.time - StartTime) / FadeTime;
        l.SetColors(new Color(255, 0, 0, a),new Color(255, 0, 0, a));
        if (Time.time - StartTime > FadeTime) Weapon.Laser.Pool.Put(gameObject);
    }
}
