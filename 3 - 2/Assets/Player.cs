using UnityEngine;
using System;

public class Player {
    public float BasicMaxHP { get; private set; }
    public float BasicMaxMP { get; private set; }
    public float MoveSpeed { get; private set; }
    public float GunRange { get; private set; }
    public float GunInterval { get; private set; }
    public float GunBasicReloadInterval { get; private set; }
    public int GunBasicBulletAmount { get; private set; }
    public float GunBasicDamage { get; private set; }
    public float HealPrepareTime { get; private set; }
    public float HealMPCost { get; private set; }
    public float HealHPGain { get; private set; }

    public float HP { get; private set; }
    public float MP { get; private set; }
    public int BulletAmount { get; private set; }
    public Vector3 Position { get; private set; }

    public GameObject Entity, HealthBoard;
    private PlayerStateController Controller;
    private float MaxHP, MaxMP;
    public float GunReloadInterval;
    private float GunDamage;
    private int GunBulletAmount;
    private float WaitNoHealingTime;
    private float WaitHPHealingSpeed;
    private float WaitMPHealingSpeed;
    private float PeaceStartTime;
    private float ReloadStartTime;
    public float EXP;
    private int Level;

    public Player() {
        Level = 0;
        HP = BasicMaxHP = 100;
        MP = BasicMaxMP = 100;
        WaitNoHealingTime = 5;
        WaitHPHealingSpeed = 0.5f;
        WaitMPHealingSpeed = 0.25f;
        MoveSpeed = 15;
        GunBasicDamage = 50;
        GunRange = 75;
        GunInterval = 0.2f;
        BulletAmount = GunBasicBulletAmount = 15;
        GunBasicReloadInterval = 2;
        HealPrepareTime = 5;
        HealHPGain = 50;
        HealMPCost = 50;
        Entity = GameObject.Instantiate<GameObject>(Game.PlayerPrefab);
        Entity.transform.localScale = new Vector3(5, 5, 1);
        HealthBoard = GameObject.Instantiate<GameObject>(Game.HealthBoardPrefab);
        Controller = new PlayerStateController(this);
        PeaceStartTime = Time.time;
        SetPosition(Vector3.zero);
        RefreshLevel();
    }
    public void Restart() {
        RefreshLevel();
        HP = MaxHP;
        MP = MaxMP;
        BulletAmount = GunBulletAmount;
    }
    public void Update() {
        if (EXP != 0) RefreshLevel();
        if (BulletAmount == 0 && Time.time - ReloadStartTime > GunReloadInterval)
            Reload();
        if (Time.time > PeaceStartTime + WaitNoHealingTime) {
            HPChange(Time.deltaTime * WaitHPHealingSpeed);
            MPChange(Time.deltaTime * WaitMPHealingSpeed);
        }
        Controller.Update();
        HealthBoard.GetComponent<TextMesh>().text =
            "Player Lv." + Level + Environment.NewLine
            + Mathf.FloorToInt(HP) + "/" + Mathf.FloorToInt(MaxHP) + Environment.NewLine
            + Mathf.FloorToInt(MP) + "/" + Mathf.FloorToInt(MaxMP) + Environment.NewLine
            + BulletAmount + "/" + GunBulletAmount;
    }
    public void RefreshLevel() {
        Level = Mathf.FloorToInt(Mathf.Sqrt(EXP));
        GunDamage = GunBasicDamage + Level * 2;
        GunReloadInterval = GunBasicReloadInterval * 50 / (50 + Level);
        GunBulletAmount = GunBasicBulletAmount + Level;
        MaxHP = BasicMaxHP + Level * 10;
        MaxMP = BasicMaxMP + Level * 10;
    }
    public void Reload() { BulletAmount = GunBulletAmount; }
    public void SetHP(float hp) { HP = Math.Max(hp,MaxHP); }
    public void SetMana(float mp) { MP = Math.Max(mp,MaxMP); }
    public void SetPosition(Vector3 position) {
        Position = position;
        Entity.transform.position = Position;
        HealthBoard.transform.position = Position + new Vector3(0,5);
        Game.Camera.transform.position = new Vector3(Position.x,Position.y,-10);
    }
    public void HPChange(float delta) {
        HP = Mathf.Min(MaxHP, HP + delta);
        if (delta < 0) PeaceStartTime = Time.time;
    }
    public void MPChange(float delta) { MP = Mathf.Min(MaxMP, MP + delta); }
    public void Shoot(Monster monster) {
        monster.HPChange(-GunDamage);
        BulletAmount--;
        Game.CreateLaser(Position, monster.Position, 0.2f);
        if (BulletAmount == 0) ReloadStartTime = Time.time;
    }
    public void Shoot(Vector3 position) {
        BulletAmount--;
        Game.CreateLaser(Position, position, 0.2f);
        if (BulletAmount == 0) ReloadStartTime = Time.time;
    }
}