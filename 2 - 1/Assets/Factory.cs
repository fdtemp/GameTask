

interface IFactory {
    Plane CreatePlane(int ID);
    Tower CreateTower();
}

public class Factory : IFactory {

    private ObjectPool<SmallPlane> SmallPlanePool;
    private ObjectPool<BigPlane> BigPlanePool;
    private System.Random Rand;

    public Factory() {
        Rand = new System.Random();
        SmallPlanePool = new ObjectPool<SmallPlane>(
            10,
            delegate(SmallPlane p) {
                p.Entity.SetActive(true);
                p.Reset();
                return true;
            },
            delegate (SmallPlane p) {
                p.Entity.SetActive(false);
                return true;
            }
        );
        BigPlanePool = new ObjectPool<BigPlane>(
            10,
            delegate (BigPlane p) {
                p.Entity.SetActive(true);
                p.Reset();
                return true;
            },
            delegate (BigPlane p) {
                p.Entity.SetActive(false);
                return true;
            }
        );
    }

    public Plane CreatePlane(int ID) {
        Plane t;
        switch (ID) {
        case Plane.SMALL:
            t = SmallPlanePool.Get();
            break;
        case Plane.BIG:
            t = BigPlanePool.Get();
            break;
        default:
            throw new System.Exception("Factory : ID " + ID + " is not a kind of plane.");
        };
        t.Entity.transform.localPosition = new UnityEngine.Vector3(Rand.Next(-75,75), 100);
        return t;
    }

    public void RecyclePlane(Plane Plane) {
        switch (Plane.Kind) {
        case Plane.SMALL:
            SmallPlanePool.Put((SmallPlane)Plane);
            break;
        case Plane.BIG:
            BigPlanePool.Put((BigPlane)Plane);
            break;
        default:
            throw new System.Exception("Factory : Plane kind " + Plane.Kind + " is not exist.");
        };
    }

    public Tower CreateTower() {
        Tower t = new Tower();
        t.Entity.transform.localPosition = new UnityEngine.Vector3(0, 0);
        return t;
    }
}