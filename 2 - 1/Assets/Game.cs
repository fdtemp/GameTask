using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    private const float SmallPlaneInterval = 0.333333f;
    private const float BigPlaneInterval = 2f;

    public static Object SmallPlanePrefab, BigPlanePrefab, TowerPrefab, LaserPrefab;

    public static List<Plane> Planes;
    public static Tower Tower;

    public static Factory fac;

    private float SmallPlaneLastTime, BigPlaneLastTime, StartTime;

    private static bool Stopped = false;

	// Use this for initialization
	void Start () {
        Planes = new List<Plane>();
        SmallPlanePrefab = Resources.Load("SmallPlane", typeof(GameObject));
        BigPlanePrefab = Resources.Load("BigPlane", typeof(GameObject));
        TowerPrefab = Resources.Load("Tower", typeof(GameObject));
        LaserPrefab = Resources.Load("Laser", typeof(GameObject));

        fac = new Factory();
        Tower = fac.CreateTower();
        StartTime = SmallPlaneLastTime = BigPlaneLastTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        Planes.RemoveAll(delegate (Plane p) {
            if (p.HP < 0.01) {
                fac.RecyclePlane(p);
                return true;
            }
            return false;
        });
        if (Stopped) return;
        if (Time.time - SmallPlaneLastTime > SmallPlaneInterval) {
            Planes.Add(fac.CreatePlane(Plane.SMALL));
            SmallPlaneLastTime = Time.time;
        }
        if (Time.time - BigPlaneLastTime > BigPlaneInterval) {
            Planes.Add(fac.CreatePlane(Plane.BIG));
            BigPlaneLastTime = Time.time;
        }
        if (Time.time - StartTime > 40)
            TowerWin();
	}

    public static void PlaneWin() {
        GameOver();
        GameObject.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Plane Win";
    }
    public static void TowerWin() {
        GameOver();
        GameObject.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Tower Win";
    }

    private static void GameOver() {
        Stopped = true;
        foreach (var plane in Planes)
            if (plane.Kind == Plane.SMALL)
                plane.Entity.GetComponent<SmallPlaneScript>().enabled = false;
            else
                plane.Entity.GetComponent<BigPlaneScript>().enabled = false;
        Tower.Entity.GetComponent<TowerScript>().enabled = false;
    }
}

