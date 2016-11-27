using UnityEngine;
using System.Collections;

public class PlayerStateController {
    public Player Player;
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