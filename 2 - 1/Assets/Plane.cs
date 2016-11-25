using UnityEngine;
using System.Collections;
using System;

abstract public class Plane {

    public const int BIG = 0;
    public const int SMALL = 1;
    public float HP { get; protected set; }
    public int Kind { get; protected set; }
    public GameObject Entity;

    public void RealDamage(float Damage) { HP -= Damage; }
    abstract public void Damage(float Damage);
    abstract public void Reset();
}

public class SmallPlane : Plane {

    public SmallPlane() {
        Kind = SMALL;
        HP = 50;
        Entity = GameObject.Instantiate(Game.SmallPlanePrefab) as GameObject;
    }
    public override void Damage(float Damage) { RealDamage(Damage); }
    public override void Reset() { HP = 50; }
}

public class BigPlane : Plane {

    public BigPlane() {
        Kind = BIG;
        HP = 100;
        Entity = GameObject.Instantiate(Game.BigPlanePrefab) as GameObject;
    }
    public override void Damage(float Damage) { RealDamage(Damage / 2); }
    public override void Reset() { HP = 100; }
}
