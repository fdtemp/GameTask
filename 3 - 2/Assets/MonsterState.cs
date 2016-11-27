using System;
using UnityEngine;

abstract public class MonsterState {
    public const int NONE = -1;
    public const int WAITING = 0;
    public const int MOVING = 1;
    public const int ATTACKING = 2;
    public const int ESCAPING = 3;
    public Monster Monster;
    abstract public int TrySwitch();
    virtual public void Regist() { }
    virtual public void Update() { }
    virtual public void Unregist() { }
}
namespace MonsterStates {
    public class Waiting : MonsterState {
        public override int TrySwitch() {
            if (Monster.HP < Monster.EscapingStartHPLimit
                && Monster.Threat < Monster.AngryThreatLimit)
                return ESCAPING;
            if ((Game.Player.Position - Monster.Position).magnitude < Monster.AlertRange
                || Monster.Threat > 0.1)
                return MOVING;
            if ((Game.Player.Position - Monster.Position).magnitude < Monster.AttackRange)
                return ATTACKING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Monster " + Monster.ID + " is waiting.");
        }
        public override void Update() {
            Monster.HPChange(Time.deltaTime * Monster.HPHealingSpeed);
            Monster.MPChange(Time.deltaTime * Monster.MPHealingSpeed);
        }
    }
    public class Moving : MonsterState {
        public override int TrySwitch() {
            if (Monster.HP < Monster.EscapingStartHPLimit
                && Monster.Threat < Monster.AngryThreatLimit)
                return ESCAPING;
            if ((Game.Player.Position - Monster.Position).magnitude > Monster.AlertRange * 1.5f
                && Monster.Threat < 0.1)
                return WAITING;
            if ((Game.Player.Position - Monster.Position).magnitude < Monster.AttackRange)
                return ATTACKING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Monster " + Monster.ID + " is moving.");
        }
        public override void Update() {
            Monster.SetPosition(Vector3.MoveTowards(
                Monster.Position,
                Game.Player.Position,
                Time.deltaTime * Monster.MoveSpeed)
            );
            Monster.ThreatChange(-Time.deltaTime * Monster.ThreatDecreaseSpeed);
        }
    }
    public class Attacking : MonsterState {
        private float LastTime;
        public override int TrySwitch() {
            if (Monster.HP < Monster.EscapingStartHPLimit
                && Monster.Threat < Monster.AngryThreatLimit)
                return ESCAPING;
            if ((
                    (Game.Player.Position - Monster.Position).magnitude < Monster.AlertRange * 2
                    && (Game.Player.Position - Monster.Position).magnitude > Monster.AttackRange
                ) || Monster.Threat > 0.1)
                return MOVING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Monster " + Monster.ID + " is attacking.");
            LastTime = Time.time;
        }
        public override void Update() {
            while (Time.time - LastTime > Monster.AttackInterval) {
                Monster.Attack();
                LastTime += Monster.AttackInterval;
            }
        }
    }
    public class Escaping : MonsterState {
        public override int TrySwitch() {
            if (Monster.HP > Monster.EscapingEndHPLimit)
                return WAITING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Monster " + Monster.ID + " is escaping.");
        }
        public override void Update() {
            Monster.SetPosition(Vector3.MoveTowards(
                Monster.Position,
                Monster.Position + 100 * (Monster.Position - Game.Player.Position),
                Time.deltaTime * Monster.MoveSpeed)
            );
            Monster.ThreatChange(-Time.deltaTime * Monster.ThreatDecreaseSpeed);
            Monster.HPChange(Time.deltaTime * (Monster.HPHealingSpeed + Monster.EscapingExtraHPHealingSpeed));
            Monster.MPChange(Time.deltaTime * (Monster.MPHealingSpeed + Monster.EscapingExtraMPHealingSpeed));
        }
    }
}