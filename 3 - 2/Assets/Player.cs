using UnityEngine;
using System;

public class Player {
    public float MaxHP { get; private set; }
    public float MaxMP { get; private set; }
    public float MoveSpeed { get; private set; }
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
    private float WaitNoHealingTime;
    private float WaitHPHealingSpeed;
    private float WaitMPHealingSpeed;
    private float PeaceStartTime;

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
        Controller = new PlayerStateController(this);
        SetPosition(new Vector3(0, 0));
        PeaceStartTime = Time.time;
    }
    public void Update() {
        if (Time.time > PeaceStartTime + WaitNoHealingTime) {
            HPChange(Time.deltaTime * WaitHPHealingSpeed);
            MPChange(Time.deltaTime * WaitMPHealingSpeed);
        }
        Controller.Update();
        HealthBoard.GetComponent<TextMesh>().text =
            "Player Lv." + Mathf.CeilToInt(Game.MonsterKilled/5) + Environment.NewLine
            + Mathf.FloorToInt(HP) + "/" + Mathf.FloorToInt(MaxHP) + Environment.NewLine
            + Mathf.FloorToInt(MP) + "/" + Mathf.FloorToInt(MaxMP);
    }

    public void SetHP(float hp) { HP = Math.Max(hp,MaxHP); }
    public void SetMana(float mp) { MP = Math.Max(mp,MaxMP); }
    public void SetPosition(Vector3 position) {
        Position = position;
        Entity.transform.position = Position;
        HealthBoard.transform.position = Position;
        Game.Camera.transform.position = new Vector3(Position.x,Position.y,-10);
    }
    public void HPChange(float delta) {
        HP = Mathf.Min(MaxHP, HP + delta);
        if (delta < 0) PeaceStartTime = Time.time;
    }
    public void MPChange(float delta) { MP = Mathf.Min(MaxMP, MP + delta); }

    public void Shoot(Monster monster) {
        monster.HPChange(-(GunDamage+Game.MonsterKilled/5));
        //BulletAmount--;
        Game.CreateLaser(Position, monster.Position, 0.2f);
    }
    public void Shoot(Vector3 position) {
        //BulletAmount--;
        Game.CreateLaser(Position, position, 0.2f);
    }
}