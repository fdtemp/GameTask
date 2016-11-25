using System;
using UnityEngine;

abstract public class Factory<T> {
    abstract public T Create();
    abstract public void Recycle(T Product);
}

public class PlaneFactory : Factory<Plane> {
    private ObjectPool<Plane> Pool;
    private PlaneSettings Settings;
    public PlaneFactory(PlaneSettings _Settings) {
        Settings = _Settings;
        Pool = new ObjectPool<Plane>(
            Settings._PoolSize,
            Plane.Reset,
            Plane.Collect,
            Plane.Destroy
        );
    }
    public override Plane Create() {
        if (Pool.Count != 0) return Pool.Get();
        return new Plane(this, Settings);
    }
    public override void Recycle(Plane Plane) {
        Pool.Put(Plane);
    }
}