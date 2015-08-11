using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace WebTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CsvDownload(WebTest.Models.SendCodeViewModel model)
        {
            List<WebTest.Models.SendCodeViewModel> list = new List<Models.SendCodeViewModel>
            {
                   new WebTest.Models.SendCodeViewModel{
                   SelectedProvider = "test0704",
                   ReturnUrl = "http://blog.shibayan.jp/entry/20110731/1312107909",
                   Providers = null,
                   RememberMe = true,
                   }            
            };
            list.Add(model);
            var text = GetCsvString(list, "	");
            var data = Encoding.UTF8.GetBytes(text);
            return File(data, "text/csv", "test.tsv");
        }

        public string GetCsvString<T>(List<T> list, string separater, bool header = true)
        {
            var sb = new StringBuilder();
            IList<System.Reflection.PropertyInfo> propertyInfos = typeof(T).GetProperties();
            if (header)
            {
                //add header line.
                foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfos)
                {
                    sb.Append(propertyInfo.Name).Append(separater);
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }
            foreach (T obj in list)
            {
                foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfos)
                {
                    sb.Append(MakeValueCsvFriendly(propertyInfo.GetValue(obj, null))).Append(separater);
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            System.Reflection.FieldInfo[] fi = typeof(T).GetFields();

            return sb.ToString();
        }
        private string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is Nullable && ((System.Data.SqlTypes.INullable)value).IsNull) return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();

            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            return output;

        }
    }
}