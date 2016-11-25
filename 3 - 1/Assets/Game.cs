using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

public class Game : MonoBehaviour {

    public static Vector3 ScreenOrigin = new Vector3(-100, -25);
    public static Vector3 ScreenSize = new Vector3(200, 150);
    public static GameObject PlanePrefab, TowerPrefab, LaserPrefab, EMPLockingPrefab;
    public static List<Plane> Planes;
    public static Tower Tower;
    public static Dictionary<string, int> Dic;
    public static PlaneFactory[] Fac;

    private float StartTime;
    private static bool Stopped = false;

	void Start () {
        PlanePrefab = Resources.Load<GameObject>("Plane");
        TowerPrefab = Resources.Load<GameObject>("Tower");
        LaserPrefab = Resources.Load<GameObject>("Laser");
        EMPLockingPrefab = Resources.Load<GameObject>("EMPLocking");
        Planes = new List<Plane>();
        Tower = new Tower();
        StartTime = Time.time;

        List<PlaneSettings> lis = XML.LoadPlaneXML(XDocument.Load("s.xml"));
        Dic = new Dictionary<string, int>();
        Fac = new PlaneFactory[lis.Count];
        for (int i = 0; i < lis.Count; i++) {
            Dic.Add(lis[i].Name, lis[i]._ID);
            Fac[i] = new PlaneFactory(lis[i]);
        }
    }
	void LateUpdate () {
        Planes.RemoveAll(delegate(Plane p) {
            if (p.HP < 0.01) {
                p.Factory.Recycle(p);
                return true;
            }
            return false;
        });
        if (Stopped) return;

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
        Tower.Entity.GetComponent<TowerScript>().enabled = false;
        foreach (var plane in Planes)
            plane.Entity.GetComponent<PlaneScript>().enabled = false;
    }
}

