using UnityEngine;
using System;

public class Monster {
    public float MaxHP { get; private set; }
    public float MaxMP { get; private set; }
    public float HPHealingSpeed { get; private set; }
    public float MPHealingSpeed { get; private set; }
    public float BodyRange { get; private set; }
    public float MoveSpeed { get; private set; }
    public float AlertRange { get; private set; }
    public float AttackDamage { get; private set; }
    public float AttackRange { get; private set; }
    public float AttackInterval { get; private set; }
    public float ThreatDecreaseSpeed { get; private set; }
    public float EscapingStartHPLimit { get; private set; }
    public float EscapingEndHPLimit { get; private set; }
    public float EscapingExtraHPHealingSpeed { get; private set; }
    public float EscapingExtraMPHealingSpeed { get; private set; }
    public float AngryThreatLimit { get; private set; }

    public int ID { get; private set; }
    public float Threat { get; private set; }
    public float HP { get; private set; }
    public float MP { get; private set; }
    public Vector3 Position { get; private set; }

    private GameObject Entity, HealthBoard;
    private MonsterStateController Controller;

    public Monster() {
        MaxHP = HP = 200;
        MaxMP = MP = 50;
        HPHealingSpeed = 5;
        MPHealingSpeed = 0.5f;
        BodyRange = 5;
        MoveSpeed = 10;
        AlertRange = 50;
        AttackDamage = 10;
        AttackRange = 10;
        AttackInterval = 2;
        ThreatDecreaseSpeed = 1;
        EscapingStartHPLimit = 80;
        EscapingEndHPLimit = 120;
        EscapingExtraHPHealingSpeed = 10;
        EscapingExtraMPHealingSpeed = 1;
        AngryThreatLimit = 150;
        Threat = 0;
        ID = Game.GetMonsterID();
        Entity = GameObject.Instantiate<GameObject>(Game.MonsterPrefab);
        HealthBoard = GameObject.Instantiate<GameObject>(Game.HealthBoardPrefab);
        HealthBoard.transform.parent = Entity.transform;
        Controller = new MonsterStateController(this);
        SetPosition(new Vector3(50, 50));
    }
    public void Update() {
        Controller.Update();
    }

    public void SetHP(float hp) { HP = Math.Max(hp, MaxHP); }
    public void SetMana(float mp) { MP = Math.Max(mp, MaxMP); }
    public void SetPosition(Vector3 position) {
        Position = position;
        Entity.transform.position = Position;
        HealthBoard.GetComponent<TextMesh>().text = Mathf.FloorToInt(HP) + "/" + Mathf.FloorToInt(MaxHP);
    }
    public void HPChange(float delta) { HP = Mathf.Min(MaxHP, Mathf.Max(HP + delta, 0)); }
    public void MPChange(float delta) { MP = Mathf.Min(MaxMP, Mathf.Max(MP + delta, 0)); }
    public void ThreatChange(float delta) { Threat = Mathf.Max(0, Threat + delta); }

    public void Attack() {
        Game.Player.HPChange(-AttackDamage);
    }
}