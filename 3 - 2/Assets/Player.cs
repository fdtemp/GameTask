using UnityEngine;
using System;

public class Player {
    public float MaxHP { get; private set; }
    public float MaxMP { get; private set; }
    public float MoveSpeed { get; private set; }
    public float WaitNoHealingTime { get; private set; }
    public float WaitHPHealingSpeed { get; private set; }
    public float WaitMPHealingSpeed { get; private set; }
    public float GunDamage { get; private set; }
    public float GunRange { get; private set; }
    public float GunInterval { get; private set; }
    public int GunBulletAmount { get; private set; }
    public float GunReloadInterval { get; private set; }
    public float HealPrepareTime { get; private set; }
    public float HealMPCost { get; private set; }
    public float HealHPGain { get; private set; }

    public float HP { get; private set; }
    public float MP { get; private set; }
    public int BulletAmount { get; private set; }
    public Vector3 Position { get; private set; }

    private GameObject Entity, HealthBoard;
    private PlayerStateController Controller;

    public Player() {
        HP = MaxHP = 100;
        MP = MaxMP = 100;
        WaitNoHealingTime = 3;
        WaitHPHealingSpeed = 1;
        WaitMPHealingSpeed = 0.5f;
        MoveSpeed = 15;
        GunDamage = 10;
        GunRange = 50;
        GunInterval = 0.1f;
        BulletAmount = GunBulletAmount = 30;
        GunReloadInterval = 2;
        HealPrepareTime = 5;
        HealHPGain = 50;
        HealMPCost = 50;
        Entity = GameObject.Instantiate<GameObject>(Game.PlayerPrefab);
        HealthBoard = GameObject.Instantiate<GameObject>(Game.HealthBoardPrefab);
        HealthBoard.transform.parent = Entity.transform;
        Controller = new PlayerStateController(this);
        SetPosition(new Vector3(0, 0));
    }
    public void Update() {
        Controller.Update();
        HealthBoard.GetComponent<TextMesh>().text = Mathf.FloorToInt(HP) + "/" + Mathf.FloorToInt(MaxHP);
    }

    public void SetHP(float hp) { HP = Math.Max(hp,MaxHP); }
    public void SetMana(float mp) { MP = Math.Max(mp,MaxMP); }
    public void SetPosition(Vector3 position) {
        Position = position;
        Entity.transform.position = Position;
        Game.Camera.transform.position = new Vector3(Position.x,Position.y,-10);
    }
    public void HPChange(float delta) { HP = Mathf.Min(MaxHP, Mathf.Max(HP + delta, 0)); }
    public void MPChange(float delta) { MP = Mathf.Min(MaxMP, Mathf.Max(MP + delta, 0)); }

    public void Shoot(Monster monster) {
        monster.HPChange(-GunDamage);
        BulletAmount--;
    }
}