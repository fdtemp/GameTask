using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {
    public static Vector3 ScreenOrigin;
    public static Vector3 ScreenSize;

    public static List<Monster> Monsters;
    public static Player Player;
    private PlayerStateController PlayerStateController;

	void Start () {
        Monsters = new List<Monster>();
        Player = new Player();
        PlayerStateController = new PlayerStateController(Player);
	}
	
	// Update is called once per frame
	void Update () {
        PlayerStateController.Update();
	}
}
