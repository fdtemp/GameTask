using UnityEngine;
using System.Collections;

public class TowerScript : MonoBehaviour {

    private const float BombInterval = 2f;
    private const float LaserInterval = 0.5f;

    float BombLastTime;
    float LaserLastTime;

    void Start() {
        LaserLastTime = BombLastTime = Time.time;
    }

	void Update () {
        if (Time.time - LaserLastTime > LaserInterval) {
            Plane p = null;
            float d = 999999;
            foreach(var plane in Game.Planes) {//find the nearest
                float td = plane.Entity.transform.localPosition.magnitude;
                if (td < d) {
                    d = td;
                    p = plane;
                }
            }
            if (p != null) {
                p.Damage(50);

                LineRenderer laser = (Instantiate(Game.LaserPrefab) as GameObject).GetComponent<LineRenderer>();
                Vector3 pos = p.Entity.transform.localPosition;
                laser.SetVertexCount(2);
                laser.SetPosition(0, new Vector3(0, -3, 0));
                laser.SetPosition(1, pos);
                laser = null;
            }
            LaserLastTime = Time.time;
        }
        if (Time.time - BombLastTime > BombInterval) {
            foreach(var plane in Game.Planes)
                plane.RealDamage(50);
            BombLastTime = Time.time;
        }
    }
}
