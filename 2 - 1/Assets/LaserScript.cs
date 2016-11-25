using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour {

    float FadeTime = 3f;
    float StartTime;

    LineRenderer l;

    void Start() {
        StartTime = Time.time;
        l = gameObject.GetComponent<LineRenderer>();
        Destroy(gameObject, FadeTime);
    }

    void Update() {
        float a = 1 - (Time.time - StartTime) / FadeTime;
        l.SetColors(new Color(255, 0, 0, a),new Color(255, 0, 0, a));
    }
}
