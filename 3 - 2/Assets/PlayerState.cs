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

public class PlayerStateController {

    private Player Player;
    private PlayerState[] States;
    public int Current;
    public PlayerStateController(Player player) {
        Player = player;
        States = new PlayerState[4] {
            new PlayerStates.Waiting(),
            new PlayerStates.Moving(),
            new PlayerStates.Firing(),
            new PlayerStates.Healing(),
        };
        for (int i = 0; i < 4; i++)
            States[i].Player = Player;
        Current = PlayerState.WAITING;
        States[Current].Regist();
    }
    public void Update() {
        int NewState = States[Current].TrySwitch();
        if (NewState != PlayerState.NONE) {
            States[Current].Unregist();
            States[NewState].Regist();
            Current = NewState;
        }
        States[Current].Update();
    }
}

namespace PlayerStates {
    public class Waiting : PlayerState {
        private float StartTime;
        private float LastHealTime;

        public override void Regist() {
            Debug.Log("Player is waiting.");
            StartTime = Time.time;
            LastHealTime = StartTime + Player.WaitNoHealingTime - Player.WaitHealingInterval;
        }
        public override int TrySwitch() {
            if (Input.GetMouseButtonDown(0))
                return FIRING;
            else if (Input.GetKeyDown(KeyCode.Q))
                return HEALING;
            else if (Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.S)
                || Input.GetKeyDown(KeyCode.D)
                || Input.GetKeyDown(KeyCode.W))
                return MOVING;
            return NONE;
        }
        public override void Update() {
            if (Time.time > StartTime + Player.WaitNoHealingTime)
                while (Time.time > LastHealTime + Player.WaitHealingInterval) {
                    Player.HPChange(Player.WaitHealingHPGain);
                    Player.MPChange(Player.WaitHealingMPGain);
                    LastHealTime += Player.WaitHealingInterval;
                }
        }
    }
    public class Moving : PlayerState {

        public override int TrySwitch() {
            if (Input.GetMouseButtonDown(0))
                return FIRING;
            else if (!Input.GetKey(KeyCode.A)
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
            float DeltaLength = Time.deltaTime * Player.MoveSpeed;
            Player.SetPosition(Vector3.MoveTowards(Player.Position, new Vector3(100 * xf, 100 * yf), DeltaLength));
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
            else if (Time.time - LastTime > Player.GunInterval * 2 || Player.BulletAmount == 0)
                return WAITING;
            return NONE;
        }
        public override void Regist() {
            Debug.Log("Player is Firing.");
            LastTime = Time.time;
        }
        public override void Update() {
            if (Player.BulletAmount > 0
                && Time.time - LastTime > Player.GunInterval
                && Input.GetMouseButton(0)) {
                Vector3
                    mousePos = new Vector3(
                        Game.ScreenOrigin.x + (Input.mousePosition.x / Screen.width) * Game.ScreenSize.x,
                        Game.ScreenOrigin.y + (Input.mousePosition.y / Screen.height) * Game.ScreenSize.y
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
                                        / Mathf.Sqrt(A * A + B * B) - m.Range;

                        if ((delta >= 0 && tdelta < delta)
                            || (delta < 0 && tdis < dis)) {

                            delta = tdelta;
                            dis = tdis;
                            target = m;
                        }
                    }
                }
                if (target != null) {
                    Player.Shot(target);
                    
                }LastTime += Player.GunInterval;
            }
        }
    }
    public class Healing : PlayerState {
        private float StartTime;

        public override int TrySwitch() {
            if (Time.time - StartTime > Player.HealPrepareTime || Input.GetKey(KeyCode.E))
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