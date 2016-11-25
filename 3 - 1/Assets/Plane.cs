using UnityEngine;

public class PlaneSettings {
    public int _ID;
    public int _PoolSize;
    public string Name;
    public string Shape;
    public Color Color;
    public float HP;
    public float Speed;
    public float Armor;
}
public class Plane {

    public PlaneFactory Factory;
    
    public float HP { get; private set; }
    public float Speed { get; private set; }
    public float Armor { get; private set; }
    public GameObject Entity;
    private PlaneSettings Settings;

    public Plane(PlaneFactory _Factory, PlaneSettings _Settings) {
        Factory = _Factory;
        Settings = _Settings;
        HP = Settings.HP;
        Speed = Settings.Speed;
        Armor = Settings.Armor;
        Entity = GameObject.Instantiate<GameObject>(Game.PlanePrefab);
        Entity.GetComponent<PlaneScript>().Speed = Speed;
        Entity.GetComponent<TextMesh>().text = Settings.Shape;
        Entity.GetComponent<TextMesh>().color = Settings.Color;
    }

    public void RealDamage(float Damage) { HP -= Damage; }
    public void Damage(float Damage) { RealDamage(Damage * (1 - Armor)); }
    public void ChangeSpeed(float DeltaRate) {
        Speed = Settings.Speed * (1 + DeltaRate);
        Entity.GetComponent<PlaneScript>().Speed = Speed;
    }

    public static bool Reset(Plane Plane) {
        Plane.HP = Plane.Settings.HP;
        Plane.Speed = Plane.Settings.Speed;
        Plane.Armor = Plane.Settings.Armor;
        Plane.Entity.SetActive(true);
        return true;
    }
    public static bool Collect(Plane Plane) {
        Plane.Entity.SetActive(false);
        return true;
    }
    public static bool Destroy(Plane Plane) {
        UnityEngine.Object.Destroy(Plane.Entity);
        return true;
    }
}
