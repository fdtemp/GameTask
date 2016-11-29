using UnityEngine;
using System.Collections;

public class MonsterStateController {
    private Monster Monster;
    private MonsterState[] States;
    public int Current;

    public void Reset() {
        if (Current != MonsterState.WAITING) {
            States[Current].Unregist();
            States[MonsterState.WAITING].Regist();
            Current = MonsterState.WAITING;
        }
    }

    public MonsterStateController(Monster monster) {
        Monster = monster;
        States = new MonsterState[4] {
            new MonsterStates.Waiting(),
            new MonsterStates.Moving(),
            new MonsterStates.Attacking(),
            new MonsterStates.Escaping(),
        };
        for (int i = 0; i < 4; i++)
            States[i].Monster = Monster;
        Current = MonsterState.WAITING;
        States[Current].Regist();
    }
    public void Update() {
        int NewState = States[Current].TrySwitch();
        if (NewState != MonsterState.NONE) {
            States[Current].Unregist();
            States[NewState].Regist();
            Current = NewState;
        }
        States[Current].Update();
    }
}