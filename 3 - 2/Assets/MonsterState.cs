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
            if (Monster.HP < Monster._Settings.EscapingStartHPLimit
                && Monster.Threat < Monster._Settings.AngryThreatLimit)
                return ESCAPING;
            if ((Game.Player.Position - Monster.Position).magnitude < Monster._Settings.AlertRange
                || Monster.Threat > 0.1)
                return MOVING;
            if ((Game.Player.Position - Monster.Position).magnitude < Monster._Settings.AttackRange)
                return ATTACKING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Monster " + Monster.ID + " is waiting.");
        }
    }
    public class Moving : MonsterState {
        public override int TrySwitch() {
            if (Monster.HP < Monster._Settings.EscapingStartHPLimit
                && Monster.Threat < Monster._Settings.AngryThreatLimit)
                return ESCAPING;
            if ((Game.Player.Position - Monster.Position).magnitude > Monster._Settings.AlertRange * 1.5f
                && Monster.Threat < 0.1)
                return WAITING;
            if ((Game.Player.Position - Monster.Position).magnitude < Monster._Settings.AttackRange)
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
                Time.deltaTime * Monster._Settings.MoveSpeed)
            );
            Monster.ThreatChange(-Time.deltaTime * Monster._Settings.ThreatDecreaseSpeed);
        }
    }
    public class Attacking : MonsterState {
        private float LastTime;
        public override int TrySwitch() {
            if (Monster.HP < Monster._Settings.EscapingStartHPLimit
                && Monster.Threat < Monster._Settings.AngryThreatLimit)
                return ESCAPING;
            if ((Game.Player.Position - Monster.Position).magnitude > Monster._Settings.AttackRange
                && ((Game.Player.Position - Monster.Position).magnitude < Monster._Settings.AlertRange * 2
                    || Monster.Threat > 0.1f))
                return MOVING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Monster " + Monster.ID + " is attacking.");
            LastTime = Time.time;
        }
        public override void Update() {
            while (Time.time - LastTime > Monster._Settings.AttackInterval) {
                Monster.Attack();
                LastTime += Monster._Settings.AttackInterval;
            }
        }
    }
    public class Escaping : MonsterState {
        public override int TrySwitch() {
            if (Monster.HP > Monster._Settings.EscapingEndHPLimit)
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
                Time.deltaTime * Monster._Settings.MoveSpeed)
            );
        }
    }
}