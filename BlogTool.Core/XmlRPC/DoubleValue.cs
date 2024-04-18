using SXL = System.Xml.Linq;

namespace BlogTool.Core.XmlRPC
{
    public class DoubleValue : Value
    {
        public readonly double Double;

        public DoubleValue(double d)
        {
            Double = d;
        }

        public static string TypeString => "double";

        protected override void AddToTypeEl(SXL.XElement parent)
        {
            parent.Value = Double.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DoubleValue XmlToValue(SXL.XElement parent)
        {
            DoubleValue bv = new DoubleValue(double.Parse(parent.Value));
            return bv;
        }

        public static implicit operator DoubleValue(double v)
        {
            return new DoubleValue(v);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is DoubleValue p))
            {
                return false;
            }

            return Double == p.Double;
        }

        public override int GetHashCode()
        {
            return Double.GetHashCode();
        }

        protected override string GetTypeString()
        {
            return TypeString;
        }
    }
}