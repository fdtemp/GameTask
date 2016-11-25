

public class Tower {
    public UnityEngine.GameObject Entity;
    public Tower() {
        Entity = UnityEngine.GameObject.Instantiate<UnityEngine.GameObject>(Game.TowerPrefab);
    }
}
