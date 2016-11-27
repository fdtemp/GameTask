using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {
    public static Vector3 ScreenOrigin = new Vector3(-80,-60);
    public static Vector3 ScreenSize = new Vector3(160,120);

    public static GameObject PlayerPrefab, MonsterPrefab, HealthBoardPrefab;
    public static GameObject Camera;
    private static int ID = 0;
    public static List<Monster> Monsters;
    public static Player Player;

    public static int GetMonsterID() {
        return System.Threading.Interlocked.Increment(ref ID);
    }

	void Start () {
        PlayerPrefab = Resources.Load<GameObject>("Player");
        MonsterPrefab = Resources.Load<GameObject>("Monster");
        HealthBoardPrefab = Resources.Load<GameObject>("HealthBoard");
        Camera = GameObject.Find("Camera");
        Monsters = new List<Monster>();
        Monsters.Add(new Monster());
        Player = new Player();
	}
	void Update () {
        Player.Update();
        for (int i = 0; i < Monsters.Count; i++)
            Monsters[i].Update();
	}
}
