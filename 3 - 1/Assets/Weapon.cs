﻿using UnityEngine;
using System;
using System.Collections.Generic;

abstract class Weapon {
    public abstract bool CheckState();
    public abstract bool LockTarget(List<Plane> Planes);
    public abstract void Fire();

    private static Vector3 ScreenPosotionTranslate(Vector3 ScreenPosition) {
        return new Vector3(
            Game.ScreenOrigin.x + (ScreenPosition.x / Screen.width) * Game.ScreenSize.x,
            Game.ScreenOrigin.y + (ScreenPosition.y / Screen.height) * Game.ScreenSize.y
        );
    }
    private static Plane GetTheNearestPlane(Vector3 CenterPosition, List<Plane> Planes, float MaxRange) {
        Plane p = null;
        float d = MaxRange;
        foreach (var plane in Planes) {//find the nearest
            float td = (plane.Entity.transform.localPosition - CenterPosition).magnitude;
            if (td < d) {
                d = td;
                p = plane;
            }
        }
        return p;
    }

    public class Laser : Weapon {
        private float Damage;
        private Vector3 Origin;
        private float Interval;
        private float LastTime;
        private Plane Target;
        public static ObjectPool<GameObject> Pool;
        public Laser(float _Damage, Vector3 _Origin, float _Interval) {
            Damage = _Damage;
            Origin = _Origin;
            Interval = _Interval;
            LastTime = Time.time - Interval;
            Pool = new ObjectPool<GameObject>(
                (int)(3f / Interval) + 1,
                delegate (GameObject l) {
                    l.SetActive(true);
                    return true;
                },
                delegate (GameObject l) {
                    l.SetActive(false);
                    return true;
                },
                delegate (GameObject l) {
                    UnityEngine.Object.Destroy(l);
                    return true;
                }
            );
        }
        public override bool CheckState() {
            if (Time.time - LastTime > Interval
                && Input.GetMouseButtonDown(0)) return true;
            return false;
        }
        public override bool LockTarget(List<Plane> Planes) {
            LastTime = Time.time;
            Vector3 CenterPos = ScreenPosotionTranslate(Input.mousePosition);
            Target = GetTheNearestPlane(CenterPos, Planes, 15f);
            if (Target == null) return false;
            return true;
        }
        public override void Fire() {
            Target.Damage(Damage);
            GameObject l;
            if (Pool.Count != 0) l = Pool.Get();
            else l = GameObject.Instantiate<GameObject>(Game.LaserPrefab);
            LineRenderer lr = l.GetComponent<LineRenderer>();
            Vector3 pos = Target.Entity.transform.localPosition;
            lr.SetVertexCount(2);
            lr.SetPosition(0, Origin);
            lr.SetPosition(1, pos);
        }
    }
    public class EMP : Weapon {
        private const int WAITING = 0;
        private const int FINDING_TARGET = 1;

        private float Damage;
        private float MaxTargetFindingTime;
        private float Interval;
        private float LastTime;
        private List<Plane> Target;
        private int State;
        private float StartTime;
        private float StateTime;
        private ObjectPool<GameObject> Pool;
        private List<GameObject> lis;
        public EMP(float _Damage, float _MaxTargetFindingTime, float _Interval) {
            Damage = _Damage;
            MaxTargetFindingTime = _MaxTargetFindingTime;
            Interval = _Interval;
            LastTime = Time.time - Interval;
            Target = new List<Plane>();
            State = WAITING;
            Pool = new ObjectPool<GameObject>(
                500,
                delegate (GameObject obj) {
                    obj.SetActive(true);
                    return true;
                },
                delegate (GameObject obj) {
                    obj.SetActive(false);
                    return true;
                },
                delegate (GameObject obj) {
                    GameObject.Destroy(obj);
                    return true;
                }
            );
            lis = new List<GameObject>();
        }
        public override bool CheckState() {
            if (Time.time - LastTime > Interval
                && ((State == WAITING && Input.GetKeyDown(KeyCode.Z))
                    || (State == FINDING_TARGET && (Input.GetKeyUp(KeyCode.Z) || Time.time > StateTime))))
                return true;
            return false;
        }
        public override bool LockTarget(List<Plane> Planes) {
            if (State == WAITING) {
                State = FINDING_TARGET;
                StartTime = Time.time;
                StateTime = Time.time - 0.1f;
            }
            if (Input.GetKeyUp(KeyCode.Z) || StateTime - StartTime > MaxTargetFindingTime) {
                for (int i = 0; i < lis.Count; i++)
                    Pool.Put(lis[i]);
                lis.Clear();
                return true;
            }
            StateTime += 0.1f;
            Vector3 CenterPos = ScreenPosotionTranslate(Input.mousePosition);
            foreach(var plane in Planes)
                if ((plane.Entity.transform.localPosition-CenterPos).magnitude < 45f) {
                    bool exist = false;
                    for(int i= 0; i < Target.Count; i++)
                        if (plane == Target[i]) {
                            exist = true;
                            break;
                        }
                    if (!exist) {
                        Target.Add(plane);
                        GameObject t;
                        if (Pool.Count != 0) t = Pool.Get();
                        else t = GameObject.Instantiate<GameObject>(Game.EMPLockingPrefab);
                        lis.Add(t);
                    }
                }
            for (int i = 0; i < lis.Count; i++)
                lis[i].transform.localPosition = Target[i].Entity.transform.localPosition;
            return false;
        }
        public override void Fire() {
            State = WAITING;
            LastTime = Time.time;
            for (int i = 0; i < Target.Count; i++)
                Target[i].RealDamage(Damage);
            Target.Clear();
        }
    }
    public class Frost : Weapon {
        private const int WAITING = 0;
        private const int FIRING = 1;

        private float FrozenTime;
        private float TransTime;
        private float Rate;
        private float Interval;
        private float LastTime;
        private List<Plane> Target;
        private int State;
        private float StartTime;
        private float StateTime;
        public Frost(float _FrozenTime, float _TransTime, float _Rate, float _Interval) {
            FrozenTime = _FrozenTime;
            TransTime = _TransTime;
            Rate = _Rate;
            Interval = _Interval;
            LastTime = Time.time - Interval;
            State = WAITING;
        }
        public override bool CheckState() {
            if (Time.time - LastTime > Interval
                && ((State == WAITING && Input.GetKeyDown(KeyCode.X))
                    || (State == FIRING && Time.time > StateTime)))
                return true;
            return false;
        }
        public override bool LockTarget(List<Plane> Planes) {
            Target = Planes;
            return true;
        }
        public override void Fire() {
            if (State == WAITING) {
                State = FIRING;
                StartTime = Time.time;
                StateTime = Time.time - 0.05f;
            }
            StateTime += 0.05f;
            float delta = StateTime - StartTime, rate;
            if (delta < TransTime) {
                rate = (delta / TransTime) * Rate;
            } else if (delta < FrozenTime + TransTime) {
                rate = Rate;
            } else if (delta < FrozenTime + 2 * TransTime){
                rate = ((FrozenTime + 2 * TransTime - delta) / TransTime) * Rate;
            } else {
                rate = 0;
                State = WAITING;
            }
            Target.ForEach(delegate (Plane p) {
                p.ChangeSpeed(rate);
            });
        }
    }
}
