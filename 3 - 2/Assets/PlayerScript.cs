using UnityEngine;

class PlayerScript : MonoBehaviour {
    private float Interval = 0.5f;

    public bool Dead = false;
    private bool Show = true;
    private float LastTime = 0;
    private float LastRate = 1;
    void Update() {
        gameObject.transform.rotation = Quaternion.FromToRotation(
            new Vector3(0, 1),
            Game.GetMousePosition() - Game.Player.Position
        );
        if (Dead) {
            if (Time.time - LastTime > Interval) {
                LastTime = Time.time;
                Show = !Show;
            } else {
                float Rate;
                if (Show) {
                    Rate = Mathf.Lerp(0, 1, LastRate + Time.deltaTime * (1 / Interval));
                } else {
                    Rate = Mathf.Lerp(0, 1, LastRate - Time.deltaTime * (1 / Interval));
                }
                gameObject.transform.localScale = new Vector3(5 * Rate, 5 * Rate, 1);
                LastRate = Rate;
            }
        }
    }
}