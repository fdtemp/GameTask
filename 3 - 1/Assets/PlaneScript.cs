using UnityEngine;

public class PlaneScript : MonoBehaviour {

    public float Speed;

    void Update() {
        if (gameObject.transform.localPosition.y < 0)
            Game.PlaneWin();
        gameObject.transform.localPosition += new Vector3(0, -Speed * Time.deltaTime, 0);
    }
}
