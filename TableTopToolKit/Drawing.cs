using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TableTopToolKit
{
    public class Drawing : IXmlSerializable
    {
        private List<Shape> shapes;

        public IList<Shape> Shapes { get => shapes; }

        private Drawing() // for serialization only
        {
            shapes = new List<Shape>();
        }

        public Drawing(Shape shape)
        {
            shapes = new List<Shape>();
            shapes.Add(shape);
        }

        public void AddToDrawing(Shape shape)
        {
            shapes.Add(shape);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("Drawing");
            while (reader.Name == "Shape")
            {
                reader.ReadStartElement("Shape");
                Shape shape = XamlReader.Parse(reader.Value) as Shape;
                shapes.Add(shape);
                reader.Read();
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (Shape shape in Shapes)
            {
                writer.WriteElementString("Shape", XamlWriter.Save(shape));
            }
        }
    }
}