using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BordaAlgorithm.Utilities
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            string result = valueProviderResult.AttemptedValue;

            int n = result.Where(x => (x == ',')).Count();
            CultureInfo info = new CultureInfo("en-GB");
            if (n >= 1)
                result = result.Replace(",", string.Empty);

            if (valueProviderResult == null || string.IsNullOrEmpty(result))
                return null;
            else
                return Convert.ToDecimal(result, info);
        }
    }

    public class DoubleModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            string result = valueProviderResult.AttemptedValue;
            int n = result.Where(x => (x == ',')).Count();

            CultureInfo info = new CultureInfo("en-GB");
            if (n >= 1)
                result = result.Replace(",", string.Empty);

            if (valueProviderResult == null || string.IsNullOrEmpty(result))
                return null;
            else
                return Convert.ToDouble(result, info);
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}