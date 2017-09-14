using System;
using ReactiveUI;
using Xamarin.Forms;
using Splat;

namespace FormsTutor.BindingTypeConverters
{
    public class HtmlBindingTypeConverter : IBindingTypeConverter, IEnableLogger
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return fromType == typeof(string) && toType == typeof(WebViewSource) ? 100 : 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            try
            {
                result = new HtmlWebViewSource { Html = (string)from };
                return true;
            }
            catch (Exception error)
            {
                this.Log().ErrorException("Problem converting to HtmlWebViewSource", error);
                result = null;
                return false;
            }
        }
    }
}
