using UnityEngine;

class PlayerScript : MonoBehaviour {
    void Update() {
        gameObject.transform.rotation = Quaternion.FromToRotation(
            new Vector3(0, 1),
            Game.GetMousePosition() - Game.Player.Position
        );
    }
}