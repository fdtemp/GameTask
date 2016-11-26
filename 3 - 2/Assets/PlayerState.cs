using UnityEngine;

public interface PlayerState {
    void Regist(Player player);
    PlayerState TrySwift();
    void Update();
    void Unregist();
}

public class PlayerStateController {
    private Player Player;
    public PlayerState CurrentState;
    public PlayerStateController(Player player) {
        Player = player;
        CurrentState = new PlayerStates.Waiting();
        CurrentState.Regist(Player);
    }
    public void Update() {
        CurrentState = CurrentState.TrySwift();
        CurrentState.Update();
    }
}

namespace PlayerStates {
    public class Waiting : PlayerState {
        private Player Player;
        private float StartTime, LastHealTime;
        public Moving moving;
        public Healing healing;
        public Firing firing;
        public Waiting() {
            if (moving == null) {
                moving = new Moving();
                moving.waiting = this;
                moving._f();
            }
            if (healing == null) {
                healing = new Healing();
                healing.waiting = this;
                healing._f();
            }
            if (firing == null) {
                firing = new Firing();
                firing.waiting = this;
                firing._f();
            }
        }

        public void Regist(Player player) {
            Debug.Log("Player is waiting.");
            Player = player;
            StartTime = Time.time;
            LastHealTime = StartTime + Player.WaitNoHealingTime - Player.WaitHealingInterval;
        }
        public PlayerState TrySwift() {
            PlayerState t = null;
            if (Input.GetMouseButtonDown(0))
                t = firing;
            else if (Input.GetKeyDown(KeyCode.Q))
                t = healing;
            else if (Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.S)
                || Input.GetKeyDown(KeyCode.D)
                || Input.GetKeyDown(KeyCode.W))
                t = moving;
            if (t == null) {
                return this;
            } else {
                Unregist();
                t.Regist(Player);
                return t;
            }
        }
        public void Update() {
            if (Time.time > StartTime + Player.WaitNoHealingTime)
                while (Time.time > LastHealTime + Player.WaitHealingInterval) {
                    Player.HPChange(Player.WaitHealingHPGain);
                    Player.MPChange(Player.WaitHealingMPGain);
                    LastHealTime += Player.WaitHealingInterval;
                }
        }
        public void Unregist() { }
    }
    public class Moving : PlayerState {
        private Player Player;
        public Waiting waiting;
        public Firing firing;
        public void _f() {
            if (firing == null) {
                firing = new Firing();
                firing.moving = this;
                firing._f();
            }
        }

        public void Regist(Player player) {
            Debug.Log("Player is Moving.");
            Player = player;
        }
        public PlayerState TrySwift() {
            PlayerState t = null;
            if (Input.GetMouseButtonDown(0))
                t = firing;
            else if (!Input.GetKey(KeyCode.A)
                && !Input.GetKey(KeyCode.S)
                && !Input.GetKey(KeyCode.D)
                && !Input.GetKey(KeyCode.W))
                t = waiting;
            if (t == null) {
                return this;
            } else {
                Unregist();
                t.Regist(Player);
                return t;
            }
        }
        public void Update() {
            int xf = 0, yf = 0;
            if (Input.GetKey(KeyCode.A)) xf -= 1;
            if (Input.GetKey(KeyCode.S)) yf -= 1;
            if (Input.GetKey(KeyCode.D)) xf += 1;
            if (Input.GetKey(KeyCode.W)) yf += 1;
            float DeltaLength = Time.deltaTime * Player.MoveSpeed;
            Player.SetPosition(Vector3.MoveTowards(Player.Position, new Vector3(100 * xf, 100 * yf), DeltaLength));
        }
        public void Unregist() { }
    }
    public class Firing : PlayerState {
        private Player Player;
        private float LastTime;
        public Waiting waiting;
        public Moving moving;
        public void _f() {
            if (moving == null) {
                moving = new Moving();
                moving.firing = this;
                moving._f();
            }
        }

        public void Regist(Player player) {
            Debug.Log("Player is Firing.");
            Player = player;
            LastTime = Time.time;
        }
        public PlayerState TrySwift() {
            PlayerState t = null;
            if (Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.S)
                || Input.GetKeyDown(KeyCode.D)
                || Input.GetKeyDown(KeyCode.W))
                t = moving;
            else if (Time.time - LastTime > Player.GunInterval * 2 || Player.BulletAmount == 0)
                t = waiting;
            if (t == null) {
                return this;
            } else {
                Unregist();
                t.Regist(Player);
                return t;
            }
        }
        public void Update() {
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
                    LastTime += Player.GunInterval;
                }
            }
        }
        public void Unregist() { }
    }
    public class Healing : PlayerState {
        private Player Player;
        private float StartTime;
        public Waiting waiting;
        public void _f() { }

        public void Regist(Player player) {
            Debug.Log("Player is Healing.");
            Player = player;
            StartTime = Time.time;
        }
        public PlayerState TrySwift() {
            PlayerState t = null;
            if (Time.time - StartTime > Player.HealPrepareTime || Input.GetKey(KeyCode.E))
                t = waiting;
            if (t == null) {
                return this;
            } else {
                Unregist();
                t.Regist(Player);
                return t;
            }
        }
        public void Update() {
            Player.MPChange(Player.HealMPCost * Time.deltaTime / Player.HealPrepareTime);
        }
        public void Unregist() {
            if (Time.time - StartTime > Player.HealPrepareTime)
                Player.HPChange(Player.HealHPGain);
        }
    }
}