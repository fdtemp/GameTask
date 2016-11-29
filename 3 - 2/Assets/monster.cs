using UnityEngine;
using System;

public class MonsterSettings {
    public string Name;
    public float MaxHP;
    public float MaxMP;
    public float BodyRange;
    public float MoveSpeed;
    public float AlertRange;
    public float AttackRange;
    public float AttackInterval;
    public float ThreatDecreaseSpeed;
    public float EscapingStartHPLimit;
    public float EscapingEndHPLimit;
    public float AngryThreatLimit;
}

abstract public class Monster {
    public MonsterSettings _Settings { get; protected set; }
    public int ID { get; protected set; }
    public float Threat { get; protected set; }
    public float HP { get; protected set; }
    public float MP { get; protected set; }
    public Vector3 Position { get; protected set; }
    public GameObject Entity, HealthBoard;
    public MonsterStateController Controller;
    public void SetID(int id) { ID = id; }
    public void SetThreat(float threat) { Threat = threat; }
    public void SetHP(float hp) { HP = Math.Max(hp, _Settings.MaxHP); }
    public void SetMP(float mp) { MP = Math.Max(mp, _Settings.MaxMP); }
    public void SetRelativePosition(Vector3 position) {
        SetPosition(Game.Player.Position + position);
    }
    public void SetPosition(Vector3 position) {
        Position = position;
        Entity.transform.position = Position;
        HealthBoard.transform.position = Position;
    }
    public void HPChange(float delta) { HP = Mathf.Min(_Settings.MaxHP, HP + delta); }
    public void MPChange(float delta) { MP = Mathf.Min(_Settings.MaxMP, MP + delta); }
    public void ThreatChange(float delta) { Threat = Mathf.Max(0, Threat + delta); }

    abstract public void SetHealthBoard(string str);
    abstract public void Attack();

    virtual public void Update() {
        Controller.Update();
        HealthBoard.GetComponent<TextMesh>().text =
            _Settings.Name + Environment.NewLine
            + Mathf.FloorToInt(HP) + "/" + Mathf.FloorToInt(_Settings.MaxHP) + Environment.NewLine
            + Mathf.FloorToInt(MP) + "/" + Mathf.FloorToInt(_Settings.MaxMP);
    }
}

public class MonsterA : Monster {

    private TextMesh HBText;
    private float HPHealingSpeed = 5;
    private float MPHealingSpeed = 0.5f;

    public static void Init() {
        Game.MonsterPool.Add("MonsterA", new ObjectPool<Monster>(
            100,
            delegate () { return new MonsterA(); },
            delegate (Monster m) {
                m.Entity.SetActive(true);
                m.HealthBoard.SetActive(true);

                m.Controller.Reset();
                m.SetID(Game.GetMonsterID());
                m.SetThreat(0);
                m.SetHP(m._Settings.MaxHP);
                m.SetMP(m._Settings.MaxMP);
                return true;
            },
            delegate (Monster m) {
                m.Entity.SetActive(false);
                m.HealthBoard.SetActive(false);
                return true;
            },
            delegate (Monster m) {
                GameObject.Destroy(m.Entity);
                GameObject.Destroy(m.HealthBoard);
                return true;
            }
        ));
    }

    public MonsterA() {
        _Settings = new MonsterSettings {
            Name = "MonsterA",
            MaxHP =200,
            MaxMP =50,
            BodyRange = 3,
            MoveSpeed = 10,
            AlertRange = 50,
            AttackRange = 10,
            AttackInterval = 1,
            ThreatDecreaseSpeed = -1,
            EscapingStartHPLimit = 40,
            EscapingEndHPLimit = 160,
            AngryThreatLimit = 150,
        };
        Entity = GameObject.Instantiate<GameObject>(Game.MonsterPrefab);
        HealthBoard = GameObject.Instantiate<GameObject>(Game.HealthBoardPrefab);
        HBText = HealthBoard.GetComponent<TextMesh>();
        Controller = new MonsterStateController(this);
        SetID(Game.GetMonsterID());
        SetThreat(0);
        SetHP(_Settings.MaxHP);
        SetMP(_Settings.MaxMP);
    }

    public override void Update() {
        HPChange(Time.deltaTime * HPHealingSpeed);
        MPChange(Time.deltaTime * MPHealingSpeed);
        base.Update();
    }
    public override void SetHealthBoard(string str) { HBText.text = str; }
    public override void Attack() { Game.Player.HPChange(-2); }
    
}