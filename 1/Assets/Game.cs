using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    public static Object PlanePrefab, TowerPrefab;

    private static Queue<PlaneObject> Planes;
    private static List<TowerObject> Towers;

    private static System.Random Rand;

    public static Factory fac;

    private float LastTime, StartTime;

    private static bool Stopped = false;

	// Use this for initialization
	void Start () {

        Rand = new System.Random();

        Planes = new Queue<PlaneObject>();
        Towers = new List<TowerObject>();
        PlanePrefab = Resources.Load("Plane", typeof(GameObject));
        TowerPrefab = Resources.Load("Tower", typeof(GameObject));

        fac = new Factory();

        for(int i = 0; i < 5; i++)
            Towers.Add(fac.CreateTower());

        StartTime = LastTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {

        if (Stopped) return;

        if (Time.time - StartTime > 40) TowerWin();

        while (Time.time - LastTime > 0.01 && !Stopped) {
            if (Planes.Count <= 500)
                Planes.Enqueue(fac.CreatePlane());
            LastTime += 0.01f;
        }
	}

    public static void TowerShooting() {
        if (Rand.Next(2) == 0 && Planes.Count != 0)
            Planes.Dequeue().IsShot();
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
            plane.Entity.GetComponent<PlaneScript>().enabled = false;
        foreach (var tower in Towers)
            tower.Entity.GetComponent<TowerScript>().enabled = false;
    }
}

