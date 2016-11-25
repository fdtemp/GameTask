using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public abstract class Event {
    public float BeginTime;
    public float EndTime;
    abstract public void Init(XElement XMLData);
    virtual public void Begin() { }
    virtual public void Update() { }
    virtual public void End() { }

    public class SingleEnemyAppear : Event {
        private Vector3 Position;
        private string PlaneKind;
        private bool Disappear;
        private bool Unbeatable;

        private Plane Plane;

        public override void Init(XElement d) {
            PlaneKind = d.Element("PlaneKind").Value;
            Position = XML.LoadVector3(d.Element("Position"));

            BeginTime = (d.Element("BeginTime") == null) ? 0
                : float.Parse(d.Element("BeginTime").Value);
            EndTime = ((d.Element("Span") == null) ? 0
                : float.Parse(d.Element("Span").Value)) + BeginTime;
            Unbeatable = (d.Element("Unbeatable") == null) ? false
                : (int.Parse(d.Element("Unbeatable").Value) != 0);
            Disappear = (d.Element("Disappear") == null) ? false
                : (int.Parse(d.Element("Disappear").Value) != 0);
        }
        public override void Begin() {
            Plane = Game.Fac[Game.Dic[PlaneKind]].Create();
            Plane.Entity.transform.localPosition = Position;
            if (!Unbeatable) Game.Planes.Add(Plane);
        }
        public override void End() {
            if (Disappear) {
                if (Unbeatable) {
                    Plane.Factory.Recycle(Plane);
                } else {
                    Plane.RealDamage(99999);
                }
            }
        }
    }

    public class RandomEnemyAppear : Event {
        private string PlaneKind;
        private Vector3 Origin;
        private Vector3 Size;
        private float GenerateAmount;
        private bool Disappear;
        private bool Unbeatable;

        private List<Plane> Plane;
        private System.Random Seed;
        private PlaneFactory Fac;

        public override void Init(XElement d) {
            PlaneKind = d.Element("PlaneKind").Value;
            Origin = XML.LoadVector3(d.Element("Origin"));
            Size = XML.LoadVector3(d.Element("Size"));
            GenerateAmount = int.Parse(d.Element("GenerateAmount").Value);

            BeginTime = (d.Element("BeginTime") == null) ? 0
                : float.Parse(d.Element("BeginTime").Value);
            EndTime = ((d.Element("Span") == null) ? 0
                : float.Parse(d.Element("Span").Value)) + BeginTime;
            Unbeatable = (d.Element("Unbeatable") == null) ? false
                : (int.Parse(d.Element("Unbeatable").Value) != 0);
            Disappear = (d.Element("Disappear") == null) ? false
                : (int.Parse(d.Element("Disappear").Value) != 0);
        }
        private Vector3 GetRandomPosition() {
            return new Vector3(
                Origin.x + (float)Seed.NextDouble() * Size.x,
                Origin.y + (float)Seed.NextDouble() * Size.y,
                Origin.z + (float)Seed.NextDouble() * Size.z
            );
        }
        public override void Begin() {
            Plane = new List<Plane>();
            Fac = Game.Fac[Game.Dic[PlaneKind]];
            Seed = new System.Random();

            Plane.Add(Fac.Create());
            Plane[0].Entity.transform.localPosition = GetRandomPosition();
            if (!Unbeatable) Game.Planes.Add(Plane[0]);
        }
        public override void Update() {
            while (Time.time > BeginTime + ((EndTime - BeginTime) / GenerateAmount) * Plane.Count) {
                Plane p = Fac.Create();
                Plane.Add(p);
                p.Entity.transform.localPosition = GetRandomPosition();
                if (!Unbeatable) Game.Planes.Add(p);
            }
        }
        public override void End() {
            if (Disappear) {
                if (Unbeatable) {
                    for (int i = 0; i < Plane.Count; i++)
                        Fac.Recycle(Plane[i]);
                } else {
                    for (int i = 0; i < Plane.Count; i++)
                        Plane[i].RealDamage(99999);
                }
            }
        }
    }
}