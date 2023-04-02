using System.Drawing;

namespace SemanticXamlPrint.Demo
{
    public static class StringAlign
    {
        public static StringFormat Center
        {
            get
            {
                return new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
            }
        }

        public static StringFormat Left
        {
            get
            {
                return new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near
                };
            }
        }

        public static StringFormat Right
        {
            get
            {
                return new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Far
                };
            }
        }
    }
}
