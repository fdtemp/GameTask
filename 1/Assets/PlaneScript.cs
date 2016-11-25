using UnityEngine;
using System.Collections;

public class PlaneScript : MonoBehaviour {

	void Update () {
        transform.localPosition += new Vector3(0, -10 * Time.deltaTime, 0);
        if (transform.localPosition.y < 0) Game.PlaneWin();
	}
}
