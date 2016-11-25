using UnityEngine;

interface IFactory {
    PlaneObject CreatePlane();
    TowerObject CreateTower();
}
public class Factory : IFactory {

    private ObjectCollector<PlaneObject> PlaneCollector;

    public Factory() {
        PlaneCollector = new ObjectCollector<PlaneObject>(
            delegate(PlaneObject Object) {
                Object.Entity.SetActive(true);
                return true;
            },
            delegate (PlaneObject Object) {
                Object.Entity.SetActive(false);
                return true;
            }
        );
    }

    public PlaneObject CreatePlane() {

        if (!PlaneCollector.Empty())
            return PlaneCollector.Reuse();

        PlaneObject t = new PlaneObject();
        t.Entity.transform.localPosition = new Vector3(0, 100);
        return t;
    }
    public void RecyclePlane(PlaneObject Plane) {
        PlaneCollector.Collect(Plane);
    }
    public TowerObject CreateTower() {

        TowerObject t = new TowerObject();
        t.Entity.transform.localPosition = new Vector3(0, 0);
        return t;
    }
}