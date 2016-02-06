using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.IO.Xml
{
    internal class MeterXmlReader
    {
        private readonly SceneBindingXmlReader _bindingReader;

        public MeterXmlReader(SceneBindingXmlReader bindingReader)
        {
            _bindingReader = bindingReader;
        }

        public MeterInfo LoadMeter(XElement meterNode, string basePath)
        {
            MeterInfo meter = new MeterInfo();

            meter.Name = meterNode.TryAttribute<string>("name") ?? "";

            meter.Position = new PointF(meterNode.GetAttribute<float>("x"), meterNode.GetAttribute<float>("y"));

            XAttribute imageAttr = meterNode.RequireAttribute("image");
            meter.TickImage = FilePath.FromRelative(imageAttr.Value, basePath);

            XAttribute backAttr = meterNode.Attribute("background");
            if (backAttr != null)
            {
                meter.Background = FilePath.FromRelative(backAttr.Value, basePath);
            }

            bool horiz = false;
            XAttribute dirAttr = meterNode.Attribute("orientation");
            if (dirAttr != null)
            {
                horiz = (dirAttr.Value == "horizontal");
            }
            meter.Orient = horiz ? MegaMan.Common.MeterInfo.Orientation.Horizontal : MegaMan.Common.MeterInfo.Orientation.Vertical;

            int x = meterNode.TryAttribute<int>("tickX");
            int y = meterNode.TryAttribute<int>("tickY");

            meter.TickOffset = new Point(x, y);

            XElement soundNode = meterNode.Element("Sound");
            if (soundNode != null) meter.Sound = IncludeFileXmlReader.LoadSound(soundNode, basePath);

            XElement bindingNode = meterNode.Element("Binding");
            if (bindingNode != null) meter.Binding = _bindingReader.Load(bindingNode);

            return meter;
        }
    }
}
