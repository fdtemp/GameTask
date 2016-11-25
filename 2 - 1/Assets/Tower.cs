using UnityEngine;
using System.Collections;

public class Tower {

    public GameObject Entity;

    public Tower() {
        Entity = GameObject.Instantiate(Game.TowerPrefab) as GameObject;
    }
}
