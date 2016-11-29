using UnityEngine;

abstract public class PlayerState {
    public const int NONE = -1;
    public const int WAITING = 0;
    public const int MOVING = 1;
    public const int FIRING = 2;
    public const int HEALING = 3;
    public Player Player;
    abstract public int TrySwitch();
    virtual public void Regist() { }
    virtual public void Update() { }
    virtual public void Unregist() { }
}

namespace PlayerStates {
    public class Waiting : PlayerState {
        public override int TrySwitch() {
            if (Input.GetMouseButtonDown(0))
                return FIRING;
            if (Input.GetKeyDown(KeyCode.Q))
                return HEALING;
            if (Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.S)
                || Input.GetKeyDown(KeyCode.D)
                || Input.GetKeyDown(KeyCode.W))
                return MOVING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Player is waiting.");
        }
    }
    public class Moving : PlayerState {
        public override int TrySwitch() {
            if (Input.GetMouseButtonDown(0))
                return FIRING;
            if (!Input.GetKey(KeyCode.A)
                && !Input.GetKey(KeyCode.S)
                && !Input.GetKey(KeyCode.D)
                && !Input.GetKey(KeyCode.W))
                return WAITING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Player is Moving.");
        }
        public override void Update() {
            int xf = 0, yf = 0;
            if (Input.GetKey(KeyCode.A)) xf -= 1;
            if (Input.GetKey(KeyCode.S)) yf -= 1;
            if (Input.GetKey(KeyCode.D)) xf += 1;
            if (Input.GetKey(KeyCode.W)) yf += 1;
            Player.SetPosition(Vector3.MoveTowards(
                Player.Position,
                new Vector3(Player.Position.x + Player.MoveSpeed * xf, Player.Position.y + Player.MoveSpeed * yf),
                Time.deltaTime * Player.MoveSpeed)
            );
        }
    }
    public class Firing : PlayerState {
        private float LastTime;

        public override int TrySwitch() {
            if (Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.S)
                || Input.GetKeyDown(KeyCode.D)
                || Input.GetKeyDown(KeyCode.W))
                return MOVING;
            if (Time.time - LastTime > Player.GunInterval * 2 || Player.BulletAmount == 0)
                return WAITING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Player is Firing.");
			LastTime = Time.time - Player.GunInterval;
        }
        public override void Update() {
            if (Player.BulletAmount > 0
                && Time.time - LastTime > Player.GunInterval
                && Input.GetMouseButton(0)) {
                Vector3
                    mousePos = new Vector3(
					    Game.Player.Position.x + Game.ScreenOrigin.x + (Input.mousePosition.x / Screen.width) * Game.ScreenSize.x,
					    Game.Player.Position.y + Game.ScreenOrigin.y + (Input.mousePosition.y / Screen.height) * Game.ScreenSize.y
                    ),
                    endPos = Vector3.MoveTowards(Player.Position, mousePos, Player.GunRange);
                float
                    a = Player.Position.x,
                    b = Player.Position.y,
                    c = endPos.x,
                    d = endPos.y,
                    A = d - b,
                    B = a - c,
                    C = b * c - a * d; 

                Monster target = null;
                float dis = Player.GunRange, delta = 10;
                for (int i = 0; i < Game.Monsters.Count; i++) {
                    Monster m = Game.Monsters[i];
                    float tdis = (Player.Position - m.Position).magnitude;

                    if (Mathf.Min(a,c)-5 <= m.Position.x
                        && m.Position.x <= Mathf.Max(a, c)+5
                        && Mathf.Min(b, d)-5 <= m.Position.y
                        && m.Position.y <= Mathf.Max(b, d)+5) {
                        float tdelta = Mathf.Abs(A * m.Position.x + B * m.Position.y + C)
                                        / Mathf.Sqrt(A * A + B * B) - m._Settings.BodyRange;

                        if ((delta >= 0 && tdelta < delta)
                            || (delta < 0 && tdis < dis)) {

                            delta = tdelta;
                            dis = tdis;
                            target = m;
                        }
                    }
                }
                if (target == null || delta > 0)
                    Player.Shoot(endPos);
                else
                    Player.Shoot(target);
                LastTime += Player.GunInterval;
            }
        }
    }
    public class Healing : PlayerState {
        private float StartTime;

        public override int TrySwitch() {
            if (Time.time - StartTime > Player.HealPrepareTime
                || Input.GetKey(KeyCode.E)
                || Player.MP < 1)
                return WAITING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Player is Healing.");
            StartTime = Time.time;
        }
        public override void Update() {
            Player.MPChange(-Player.HealMPCost * Time.deltaTime / Player.HealPrepareTime);
        }
        public override void Unregist() {
            if (Time.time - StartTime > Player.HealPrepareTime)
                Player.HPChange(Player.HealHPGain);
        }
    }
}