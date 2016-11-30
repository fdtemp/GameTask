using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {
    public static Vector3 ScreenOrigin = new Vector3(-80,-60);
    public static Vector3 ScreenSize = new Vector3(160,120);
    public float MonsterGenerateInterval = 6;

    private float LastGenerateTime;
    private System.Random Seed;

    private static int ID = 0;
    public static GameObject PlayerPrefab, MonsterPrefab, HealthBoardPrefab, LaserPrefab;
    public static GameObject Camera;
    public static List<Monster> Monsters;
    public static int MonsterKilled;
    public static Dictionary<string, ObjectPool<Monster>> MonsterPool;
    public static Player Player;
    public static bool Stopped;

    private static ObjectPool<GameObject> LaserPool;
    public static void CreateLaser(Vector3 a, Vector3 b, float t) {
        GameObject l;
        if (LaserPool.Count != 0) l = LaserPool.Get();
        else l = GameObject.Instantiate<GameObject>(Game.LaserPrefab);
        LineRenderer lr = l.GetComponent<LineRenderer>();
        lr.SetVertexCount(2);
        lr.SetWidth(1, 1);
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        LaserScript ls = l.GetComponent<LaserScript>();
        ls.AppearTime = t;
        ls.Pool = LaserPool;
    }
    public static Vector3 GetMousePosition() {
        return new Vector3(
            Game.Player.Position.x + Game.ScreenOrigin.x + (Input.mousePosition.x / Screen.width) * Game.ScreenSize.x,
            Game.Player.Position.y + Game.ScreenOrigin.y + (Input.mousePosition.y / Screen.height) * Game.ScreenSize.y
        );
    }
    public static int GetMonsterID() {
        return System.Threading.Interlocked.Increment(ref ID);
    }

	void Start () {
        PlayerPrefab = Resources.Load<GameObject>("Player");
        MonsterPrefab = Resources.Load<GameObject>("Monster");
        HealthBoardPrefab = Resources.Load<GameObject>("HealthBoard");
        LaserPrefab = Resources.Load<GameObject>("Laser");
        Camera = GameObject.Find("Camera");

        Seed = new System.Random();
        Monsters = new List<Monster>();
        MonsterPool = new Dictionary<string, ObjectPool<Monster>>();
        MonsterA.Init();
        MonsterB.Init();
        MonsterKilled = 0;
        LastGenerateTime = Time.time - MonsterGenerateInterval;
        Player = new Player();
        Player.SetPosition(new Vector3(0, 0));
        LaserPool = new ObjectPool<GameObject>(
            15,
            delegate () {
                return GameObject.Instantiate<GameObject>(LaserPrefab);
            },
            delegate (GameObject l) {
                l.SetActive(true);
                return true;
            },
            delegate (GameObject l) {
                l.SetActive(false);
                return true;
            },
            delegate (GameObject l) {
                GameObject.Destroy(l);
                return true;
            });
        Stopped = false;
	}
	void Update () {
        if (Stopped) return;
        while (Time.time - LastGenerateTime > MonsterGenerateInterval) {
            Monster m;
            for (int i = 0; i < 2; i++) {
                m = MonsterPool["MonsterA"].Get();
                m.SetRelativePosition(new Vector3(Seed.Next(-80, 80), Seed.Next(-60, 60)));
                Monsters.Add(m);
            }
            m = MonsterPool["MonsterB"].Get();
            m.SetRelativePosition(new Vector3(Seed.Next(-80, 80), Seed.Next(-60, 60)));
            Monsters.Add(m);
            LastGenerateTime += MonsterGenerateInterval;
        }
        Player.Update();
        for (int i = 0; i < Monsters.Count; i++)
            Monsters[i].Update();
        List<Monster> lis = new List<Monster>();
        for (int i = 0; i < Monsters.Count; i++) {
            Monster m = Monsters[i];
            if (m.HP > 1.01) {
                lis.Add(m);
            } else {
                MonsterPool[m._Settings.Name].Put(Monsters[i]);
                MonsterKilled++;
            }
        }
        Monsters = lis;
        if (Player.HP < 0) {
            Game.Player.Entity.GetComponent<PlayerScript>().Dead = true;
            Stopped = true;
        }
	}
}
