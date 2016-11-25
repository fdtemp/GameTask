using UnityEngine;
using System.Collections;

public class PlaneObject {

    public GameObject Entity;

    public PlaneObject() { Entity = GameObject.Instantiate(Game.PlanePrefab) as GameObject; }

    public void IsShot() { Game.fac.RecyclePlane(this); }
}
