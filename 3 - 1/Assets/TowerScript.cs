using System;
using UnityEngine;

public class TowerScript : MonoBehaviour {

    private Weapon[] Weapons;

    void Start() {
        Weapons = new Weapon[] {
            new Weapon.Frost(4,0.5f,-0.5f,0),
            new Weapon.EMP(50, 5, 0),
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
