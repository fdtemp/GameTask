using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public abstract class Event {
    public const int WAITING = 0;
    public const int STARTED = 1;
    public const int FINISH = 2;
    public int State;
    public float BeginTime;
    public float EndTime;
    virtual public void Begin() { }
    virtual public void Update() { }
    virtual public void End() { }

    public delegate Event InitFunc(XElement d);
    public static Dictionary<string, InitFunc> Dic = new Dictionary<string, InitFunc>();

    public static void Init() {
        Dic.Clear();
        Dic.Add("SingleEnemyAppear", SingleEnemyAppear.Init);
        Dic.Add("RandomEnemyAppear", RandomEnemyAppear.Init);
    }

    public class SingleEnemyAppear : Event {
        private Vector3 Position;
        private string PlaneKind;
        private bool Disappear;
        private bool Unbeatable;

        private Plane Plane;

        public static Event Init(XElement d) {
            SingleEnemyAppear e = new SingleEnemyAppear();

            e.PlaneKind = d.Element("PlaneKind").Value;
            e.Position = XML.LoadVector3(d.Element("Position"));

            e.BeginTime = (d.Element("BeginTime") == null) ? 0
                : float.Parse(d.Element("BeginTime").Value);
            e.EndTime = ((d.Element("Span") == null) ? 0
                : float.Parse(d.Element("Span").Value)) + e.BeginTime;
            e.Unbeatable = (d.Element("Unbeatable") == null) ? false
                : bool.Parse(d.Element("Unbeatable").Value);
            e.Disappear = (d.Element("Disappear") == null) ? false
                : bool.Parse(d.Element("Disappear").Value);

            return e;
        }
        public static XElement toXML(string PlaneKind, Vector3 Position, float BeginTime, float Span, bool Unbeatable, bool Disappear) {
            XElement x = new XElement("Event", new XAttribute("Kind", "SingleEnemyAppear"));
            x.Add(new XElement("PlaneKind", PlaneKind));
            x.Add(XML.SaveVector3("Position",Position));
            x.Add(new XElement("BeginTime", BeginTime));
            x.Add(new XElement("Span", Span));
            x.Add(new XElement("Unbeatable", Unbeatable));
            x.Add(new XElement("Disappear", Disappear));
            return x;
        }
        public override void Begin() {
            Plane = Game.Fac[PlaneKind].Create();
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

        public static Event Init(XElement d) {
            RandomEnemyAppear e = new RandomEnemyAppear();

            e.PlaneKind = d.Element("PlaneKind").Value;
            e.Origin = XML.LoadVector3(d.Element("Origin"));
            e.Size = XML.LoadVector3(d.Element("Size"));
            e.GenerateAmount = int.Parse(d.Element("GenerateAmount").Value);

            e.BeginTime = (d.Element("BeginTime") == null) ? 0
                : float.Parse(d.Element("BeginTime").Value);
            e.EndTime = ((d.Element("Span") == null) ? 0
                : float.Parse(d.Element("Span").Value)) + e.BeginTime;
            e.Unbeatable = (d.Element("Unbeatable") == null) ? false
                : bool.Parse(d.Element("Unbeatable").Value);
            e.Disappear = (d.Element("Disappear") == null) ? false
                : bool.Parse(d.Element("Disappear").Value);

            return e;
        }
        public static XElement toXML(string PlaneKind, Vector3 Origin, Vector3 Size, float GenerateAmount, float BeginTime, float Span, bool Unbeatable, bool Disappear) {
            XElement x = new XElement("Event", new XAttribute("Kind", "RandomEnemyAppear"));
            x.Add(new XElement("PlaneKind", PlaneKind));
            x.Add(XML.SaveVector3("Origin", Origin));
            x.Add(XML.SaveVector3("Size", Size));
            x.Add(new XElement("GenerateAmount", GenerateAmount));
            x.Add(new XElement("BeginTime", BeginTime));
            x.Add(new XElement("Span", Span));
            x.Add(new XElement("Unbeatable", Unbeatable));
            x.Add(new XElement("Disappear", Disappear));
            return x;
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
            Fac = Game.Fac[PlaneKind];
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