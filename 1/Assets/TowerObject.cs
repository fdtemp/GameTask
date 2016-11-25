using UnityEngine;
using System.Collections;

public class TowerObject {

    public GameObject Entity;

    public TowerObject() { Entity = GameObject.Instantiate(Game.TowerPrefab) as GameObject; }
}
