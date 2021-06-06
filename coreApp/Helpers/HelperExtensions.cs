using System.Linq.Expressions;
using System.Web.Mvc;

namespace System.Web.Helpers
{
    public static class HelperExtensions
    {
        public static MvcHtmlString Checkbox_WithLabel(this HtmlHelper helper, string name, string value, string label, bool isChecked = false, bool disabled = false, bool customValues = true, bool noHiddenField = false, bool inline = false)
        {
            string hiddenField = "<input type=\"hidden\" name=\"{1}\" value=\"{2}\" />";
            string template = "<div class=\"checkbox {0}\"" + (inline ? " style=\"display:inline-block\"" : "") + "><label>" + (noHiddenField ? "" : hiddenField) + "<input type=\"checkbox\" name=\"{1}\" value=\"{2}\" {0} {4} {5}>{3}</label></div>";
            string _disabled = disabled ? "disabled" : "";
            string _checked = isChecked ? "checked" : "";
            string _customValues = customValues ? "custom-values" : "";

            string html = string.Format(template, _disabled, name, value, label, _checked, _customValues);

            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString CheckboxFor_WithLabel<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string namePrefix = "", bool disabled = false)
        {
            ModelMetadata m = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            var propName = m.PropertyName;
            var dispName = m.DisplayName;
            bool value = (bool)m.Model;

            string template = "<div class=\"checkbox {0}\"><label><input type=\"hidden\" name=\"{1}\" value=\"{2}\" /><input type=\"checkbox\" name=\"{1}\" value=\"{2}\" {0} {4}>{3}</label></div>";
            string _disabled = disabled ? "disabled" : "";
            string _checked = value ? "checked" : "";
            string _name = (string.IsNullOrEmpty(namePrefix) ? "" : namePrefix + ".") + propName;
            string _value = value.ToString().ToLower();

            string html = string.Format(template, _disabled, _name, _value, dispName, _checked);

            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString DisplayBooleanFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string noText = "No", string yesText = "Yes")
        {
            ModelMetadata m = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            var propName = m.PropertyName;
            var dispName = m.DisplayName;
            bool value = Convert.ToBoolean(m.Model);

            string html = noText;

            if (value)
            {
                html = yesText;
            }
            
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString DisplayMultiLineFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, int ellipsisLength = 0, bool encloseInQuotes = false)
        {
            ModelMetadata m = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            var propName = m.PropertyName;
            var dispName = m.DisplayName;
            string value = (string)m.Model;

            bool appliedEllipsis = false;
            string html = doEllipsis(value, ellipsisLength, out appliedEllipsis);

            if (encloseInQuotes)
            {
                html = "\"" + html + "\"";
            }

            if (!string.IsNullOrEmpty(html))
            {
                html = "<div>" + html.Replace("\n", "<br />") + "</div>";
            }
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString DisplayMultiLine(this HtmlHelper helper, string expression, int ellipsisLength = 0)
        {
            bool appliedEllipsis = false;
            string html = doEllipsis(expression, ellipsisLength, out appliedEllipsis);

            if (!string.IsNullOrEmpty(html))
            {
                html = "<div>" + html.Replace("\n", "<br />") + "</div>";
            }
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString DisplayFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, int ellipsisLength)
        {
            ModelMetadata m = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            var propName = m.PropertyName;
            var dispName = m.DisplayName;
            string value = (string)m.Model;

            bool appliedEllipsis = false;
            string html = doEllipsis(value, ellipsisLength, out appliedEllipsis);

            if (appliedEllipsis)
            {
                return MvcHtmlString.Create(html);
            }

            return System.Web.Mvc.Html.DisplayExtensions.DisplayFor(helper, expression);
        }

        public static MvcHtmlString Display(this HtmlHelper helper, string expression, int ellipsisLength)
        {
            bool appliedEllipsis = false;
            string html = doEllipsis(expression, ellipsisLength, out appliedEllipsis);

            if (appliedEllipsis)
            {
                return MvcHtmlString.Create(html);
            }

            return System.Web.Mvc.Html.DisplayExtensions.Display(helper, expression);
        }

        static string doEllipsis(string expression, int ellipsisLength, out bool appliedEllipsis)
        {
            string ret = expression;
            appliedEllipsis = false;
            if (!string.IsNullOrEmpty(expression))
            {
                if (ellipsisLength > 0 && expression.Length > ellipsisLength)
                {
                    ret = expression.Substring(0, ellipsisLength) + "...";
                    appliedEllipsis = true;
                }
            }
            return ret;
        }

        public static MvcHtmlString ToDateTimeString<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata m = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            var propName = m.PropertyName;
            var dispName = m.DisplayName;
            DateTime value = (DateTime)m.Model;

            string html = "";
            if (value != null)
            {
                html = value.ToShortDateString() + "<br />" + value.ToShortTimeString();
            }


            return MvcHtmlString.Create(html);
        }
    }
}