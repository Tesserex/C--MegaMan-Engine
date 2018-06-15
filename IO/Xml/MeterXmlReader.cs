using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml
{
    internal class MeterXmlReader
    {
        private readonly SceneBindingXmlReader bindingReader;

        public MeterXmlReader(SceneBindingXmlReader bindingReader)
        {
            this.bindingReader = bindingReader;
        }

        public MeterInfo LoadMeter(XElement meterNode, string basePath, IDataSource dataSource)
        {
            var meter = new MeterInfo();

            meter.Name = meterNode.TryAttribute<string>("name") ?? "";

            meter.Position = new PointF(meterNode.GetAttribute<float>("x"), meterNode.GetAttribute<float>("y"));

            var imageAttr = meterNode.RequireAttribute("image");
            meter.TickImage = FilePath.FromRelative(imageAttr.Value, basePath);
            meter.TickImageData = dataSource.GetBytesFromFilePath(meter.TickImage);

            var backAttr = meterNode.Attribute("background");
            if (backAttr != null)
            {
                meter.Background = FilePath.FromRelative(backAttr.Value, basePath);
                meter.BackgroundData = dataSource.GetBytesFromFilePath(meter.Background);
            }

            var horiz = false;
            var dirAttr = meterNode.Attribute("orientation");
            if (dirAttr != null)
            {
                horiz = (dirAttr.Value == "horizontal");
            }
            meter.Orient = horiz ? MeterInfo.Orientation.Horizontal : MeterInfo.Orientation.Vertical;

            var x = meterNode.TryAttribute<int>("tickX");
            var y = meterNode.TryAttribute<int>("tickY");

            meter.TickOffset = new Point(x, y);

            var soundNode = meterNode.Element("Sound");
            if (soundNode != null) meter.Sound = IncludeFileXmlReader.LoadSound(soundNode, basePath);

            var bindingNode = meterNode.Element("Binding");
            if (bindingNode != null) meter.Binding = bindingReader.Load(bindingNode);

            return meter;
        }
    }
}
