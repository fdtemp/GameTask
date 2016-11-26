using UnityEngine;
using System;

public class Player {
    public float MaxHP { get; private set; }
    public float MaxMP { get; private set; }
    public float WaitNoHealingTime { get; private set; }
    public float WaitHealingInterval { get; private set; }
    public float WaitHealingHPGain { get; private set; }
    public float WaitHealingMPGain { get; private set; }
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

    public Player() {
        HP = MaxHP = 100;
        MP = MaxMP = 100;
        WaitNoHealingTime = 3;
        WaitHealingInterval = 1;
        WaitHealingHPGain = 1;
        WaitHealingMPGain = 0.5f;
        MoveSpeed = 5;
        GunDamage = 50;
        GunRange = 50;
        GunInterval = 0.5f;
        BulletAmount = GunBulletAmount = 20;
        GunReloadInterval = 2;
        HealPrepareTime = 5;
        HealHPGain = 50;
        HealMPCost = 50;
        Position = new Vector3(0, 0);
    }

    public void SetHP(float hp) { HP = Math.Max(hp,MaxHP); }
    public void SetMana(float mp) { MP = Math.Max(mp,MaxMP); }
    public void SetPosition(Vector3 position) { Position = position; }
    public void HPChange(float delta) { HP = Mathf.Lerp(0, MaxHP, HP + delta); }
    public void MPChange(float delta) { MP = Mathf.Lerp(0, MaxMP, MP + delta); }

    public void Shot(Monster monster) {

    }
}