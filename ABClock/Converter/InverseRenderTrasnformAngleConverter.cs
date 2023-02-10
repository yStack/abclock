using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace ABClock.Converter
{
    public class InverseRenderTrasnformAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var trans = value as RotateTransform;
            if(trans != null)
            {
                var inverseTrans = new RotateTransform();
                inverseTrans.CenterX = trans.CenterX;
                inverseTrans.CenterY = trans.CenterY;
                inverseTrans.Angle = -1 * trans.Angle;
                return inverseTrans;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
