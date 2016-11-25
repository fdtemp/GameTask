using UnityEngine;
using System.Collections;

public class BigPlaneScript : MonoBehaviour {

    private float Speed = 10;

    void Update() {
        transform.localPosition += new Vector3(0, -Speed * Time.deltaTime, 0);
        if (transform.localPosition.y < 0) Game.PlaneWin();
    }
}
