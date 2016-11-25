using UnityEngine;
using System.Collections;

public class TowerScript : MonoBehaviour {

    const float Interval = 0.01666f;

    float LastTime;

    void Start() {
        LastTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {

        while (Time.time - LastTime > Interval) {
            Game.TowerShooting();
            LastTime += Interval;
        }
    }
}
