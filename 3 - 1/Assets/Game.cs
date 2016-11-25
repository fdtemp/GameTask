using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

public class Game : MonoBehaviour {

    public static Vector3 ScreenOrigin = new Vector3(-100, -25);
    public static Vector3 ScreenSize = new Vector3(200, 150);
    public static GameObject PlanePrefab, TowerPrefab, LaserPrefab, EMPLockingPrefab;
    public static List<Plane> Planes;
    public static List<Event> Events;
    public static Tower Tower;
    public static Dictionary<string, PlaneFactory> Fac;

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
        XDocument px = XML.GeneratePlaneXML(XML.GetBasicPlaneSettings());
        XDocument ex = XML.GenerateEventXML(XML.GetEventEles());
        px.Save("Planes.xml");
        ex.Save("Events.xml");
        //load plane data
        List<PlaneSettings> lisp = XML.LoadPlaneXML(XDocument.Load("Planes.xml"));
        Fac = new Dictionary<string, PlaneFactory>();
        for (int i = 0; i < lisp.Count; i++)
            Fac.Add(lisp[i].Name, new PlaneFactory(lisp[i]));
        //load event data
        Event.Init();
        Events = XML.LoadEventXML(XDocument.Load("Events.xml"));
        for (int i = 0; i < Events.Count; i++)
            Events[i].State = Event.WAITING;
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

        for(int i = 0; i < Events.Count; i++) {
            Event e = Events[i];
            if (e.BeginTime < Time.time )
                switch (e.State) {
                case Event.WAITING:
                    e.Begin();
                    e.State = Event.STARTED;
                    break;
                case Event.STARTED:
                    if (Time.time < e.EndTime) {
                        e.Update();
                    } else {
                        e.End();
                        e.State = Event.FINISH;
                    }
                    break;
                }
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
        Tower.Entity.GetComponent<TowerScript>().enabled = false;
        foreach (var plane in Planes)
            plane.Entity.GetComponent<PlaneScript>().enabled = false;
    }
}

