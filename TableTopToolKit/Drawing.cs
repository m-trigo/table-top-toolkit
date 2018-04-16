using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TableTopToolKit
{
    public class Drawing : IXmlSerializable
    {
        private List<UIElement> elements;

        public IList<UIElement> Elements { get => elements; }

        private Drawing() // for serialization only
        {
            elements = new List<UIElement>();
        }

        public Drawing(UIElement element)
        {
            elements = new List<UIElement>();
            elements.Add(element);
        }

        public void AddToDrawing(UIElement element)
        {
            elements.Add(element);
        }

        public void AddToDrawing(IEnumerable<UIElement> additions)
        {
            foreach (UIElement element in additions)
            {
                elements.Add(element);
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }
            reader.ReadStartElement("Drawing");
            while (reader.Name == "Shape")
            {
                reader.ReadStartElement("Shape");
                UIElement element = XamlReader.Parse(reader.Value) as UIElement;
                elements.Add(element);
                reader.Read();
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (UIElement element in Elements)
            {
                writer.WriteElementString("Shape", XamlWriter.Save(element));
            }
        }
    }
}