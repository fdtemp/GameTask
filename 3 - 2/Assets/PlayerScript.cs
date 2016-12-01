using UnityEngine;

class PlayerScript : MonoBehaviour {
    private float Interval = 5;

    public bool Dead = false;
    private float Rate = 1;
    void Update() {
        gameObject.transform.rotation = Quaternion.FromToRotation(
            new Vector3(0, 1),
            Game.GetMousePosition() - Game.Player.Position
        );
        if (Dead) {
            Rate = Mathf.Lerp(0, 1, Rate - Time.deltaTime * (1 / Interval));
            gameObject.transform.localScale = new Vector3(5 * Rate, 5 * Rate, 1);
        }
        if (Rate == 0) {
            Game.Restart();
            Dead = false;
            Rate = 1;
            gameObject.transform.localScale = new Vector3(5 * Rate, 5 * Rate, 1);
        }
    }
}