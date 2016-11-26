using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

class XML {
    public static Vector3 LoadVector3(XElement d) {
        return new Vector3(
            float.Parse(d.Element("X").Value),
            float.Parse(d.Element("Y").Value),
            float.Parse(d.Element("Z").Value)
        );
    }
    public static XElement SaveVector3(string Name, Vector3 d) {
        return
            new XElement(Name,
                new XElement("X", d.x),
                new XElement("Y", d.y),
                new XElement("Z", d.z)
            );
    }
    public static Color LoadColor(XElement d) {
        return new Color(
            float.Parse(d.Element("R").Value),
            float.Parse(d.Element("G").Value),
            float.Parse(d.Element("B").Value)
        );
    }
    public static XElement SaveColor(string Name, Color d) {
        return
            new XElement(Name,
                new XElement("R", d.r),
                new XElement("G", d.g),
                new XElement("B", d.b)
            );
    }
    public static XDocument GeneratePlaneXML(List<PlaneSettings> settings) {
        XDocument XML = new XDocument(new XElement("Planes"));
        XElement root = XML.Root;
        for (int i = 0; i < settings.Count; i++) {
            PlaneSettings s = settings[i];
            root.Add(
                new XElement("Plane",
                    new XElement("PoolSize", s._PoolSize),
                    new XElement("Name", s.Name),
                    new XElement("Shape", s.Shape),
                    SaveColor("Color", s.Color),
                    new XElement("HP", s.HP),
                    new XElement("Speed", s.Speed),
                    new XElement("Armor", s.Armor)
            ));
        }
        return XML;
    }
    public static List<PlaneSettings> LoadPlaneXML(XDocument XML) {
        XElement root = XML.Root;
        List<PlaneSettings> lis = new List<PlaneSettings>();
        foreach (var p in root.Elements("Plane")) {
            lis.Add(new PlaneSettings {
                _PoolSize = int.Parse(p.Element("PoolSize").Value),
                Name = p.Element("Name").Value,
                Shape = p.Element("Shape").Value,
                Color = LoadColor(p.Element("Color")),
                HP = float.Parse(p.Element("HP").Value),
                Speed = float.Parse(p.Element("Speed").Value),
                Armor = float.Parse(p.Element("Armor").Value),
            });
        }
        return lis;
    }
    public static List<PlaneSettings> GetBasicPlaneSettings() {
        string endl = System.Environment.NewLine;
        return new List<PlaneSettings> {
            new PlaneSettings {
                _PoolSize = 200,
                Name = "Small",
                Shape = "╟█╢",
                Color = new Color(255, 255, 255),
                HP = 25,
                Speed = 10,
                Armor = 0,
            },
            new PlaneSettings {
                _PoolSize = 100,
                Name = "Normal",
                Shape = "  ▵" + endl + "◥█◤" + endl + "  ▼",
                Color = new Color(255, 255, 255),
                HP = 50,
                Speed = 20,
                Armor = 0,
            },
            new PlaneSettings {
                _PoolSize = 30,
                Name = "Big",
                Shape = "  ▵  ▵" + endl + "◥███◤" + endl + "  ╚█╝" + endl + "    ▼",
                Color = new Color(255, 255, 255),
                HP = 150,
                Speed = 10,
                Armor = 0.5f,
            },
            new PlaneSettings {
                _PoolSize = 10,
                Name = "Super",
                Shape = "▵▵▵" + endl + "┠█┨" + endl + "┠█┨" + endl + "  ▼",
                Color = new Color(255, 0, 0),
                HP = 150,
                Speed = 30,
                Armor = 0,
            }
        };
    }
    public static List<Event> LoadEventXML(XDocument XML) {
        XElement root = XML.Root;
        List<Event> lis = new List<Event>();
        foreach(var e in root.Elements("Event"))
            lis.Add(Event.Dic[e.Attribute("Kind").Value](e));
        return lis;
    }
    public static XDocument GenerateEventXML(List<XElement> Eles) {
        XDocument XML = new XDocument(new XElement("Events"));
        XElement root = XML.Root;
        for (int i = 0; i < Eles.Count; i++) {
            root.Add(Eles[i]);
        }
        return XML;
    }
    public static List<XElement> GetEventEles() {
        return new List<XElement> {
            Event.RandomEnemyAppear.toXML(
                "Small",
                new Vector3(-80,100),
                new Vector3(160,0),
                2000,0,300,false,false
            ),
            Event.RandomEnemyAppear.toXML(
                "Normal",
                new Vector3(-80,100),
                new Vector3(160,0),
                200,0,300,false,false
            ),
            Event.RandomEnemyAppear.toXML(
                "Big",
                new Vector3(-80,100),
                new Vector3(160,0),
                50,0,300,false,false
            ),
            Event.SingleEnemyAppear.toXML(
                "Super",
                new Vector3(-50,100),
                20,0,false,false
            ),
            Event.SingleEnemyAppear.toXML(
                "Super",
                new Vector3(0,100),
                20,0,false,false
            ),
            Event.SingleEnemyAppear.toXML(
                "Super",
                new Vector3(50,100),
                20,0,false,false
            ),
        };
    }
}
