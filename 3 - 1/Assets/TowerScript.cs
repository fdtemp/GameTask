using System;
using UnityEngine;

public class TowerScript : MonoBehaviour {

    private Weapon[] Weapons;

    void Start() {
        Weapons = new Weapon[] {
            new Weapon.Frost(KeyCode.X, 5, 0.5f, -0.5f, 14),
            new Weapon.EMP(KeyCode.C, 50, 3, 0),
            new Weapon.EMP(KeyCode.Z, 50, 3, 7),
            new Weapon.Laser(50, new Vector3(0,-3), 0.1f),
        };
    }
	void Update () {
        for (int i = 0; i < Weapons.Length; i++) {
            Weapon weapon = Weapons[i];
            while (weapon.CheckState())
                if (weapon.LockTarget(Game.Planes))
                    weapon.Fire();
        }
    }
}
