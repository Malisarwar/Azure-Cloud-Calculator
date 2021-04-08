using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureCalculator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(float result = 0)
        {
            return View(result);
        }

        public ActionResult Result(float num1, float num2, string op)
        {
            float result = 0;

            switch (op)
            {
                case "add":
                    result = num1 + num2;
                    break;
                case "subtract":
                    result = num1 - num2;
                    break;
                case "multiply":
                    result = num1 * num2;
                    break;
                case "divide":
                    result = num1 / num2;
                    break;
                default:
                    result = num1 + num2;
                    break;
            }

            LogIntoFile(GetIPAddress(), op, num1, num2, result);

            return View("Index", result);
        }

        public ActionResult ShowLog()
        {
            var content = new List<string>();
            try
            {
                String mainFilePath = Server.MapPath("~/calcultorops.txt");

                var fileLines = System.IO.File.ReadAllLines(mainFilePath);
                foreach (var line in fileLines)
                {
                    content.Add(line);
                }
            }
            catch { }
            return View(content);
        }

        public ActionResult ShowMyLog()
        {
            var content = new List<string>();
            try
            {
                var myIp = GetIPAddress();

                String ipFilePath = Server.MapPath("~/" + GetIPAddress() + ".txt");

                var fileLines = System.IO.File.ReadAllLines(ipFilePath);
                foreach (var line in fileLines)
                {
                    if (line.Contains(myIp))
                        content.Add(line);
                }
            }
            catch { }

            ViewBag.IsMyLog = true;
            return View("ShowLog", content);
        }

        [HttpGet]
        public ActionResult Reload()
        {
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult Clear()
        {
            try
            {
                var myIp = GetIPAddress();

                String ipFilePath = Server.MapPath("~/" + GetIPAddress() + ".txt");

                System.IO.File.Delete(ipFilePath);
            }
            catch { }

            return RedirectToAction("ShowMyLog");
        }

        private void LogIntoFile(string ip, string op, float num1, float num2, float result)
        {
            try
            {
                String mainFilePath = Server.MapPath("~/calcultorops.txt");
                String ipFilePath = Server.MapPath("~/" + GetIPAddress() + ".txt");

                string content = "\nIP: " + ip + "\t\tTimestamp: " + DateTime.Now + "\t\tOperation: " + op + "\t\tNum1: " + num1 + "\t\tNum2: " + num2 + "\t\tResult: " + result;
                System.IO.File.AppendAllText(mainFilePath, content);
                System.IO.File.AppendAllText(ipFilePath, content);
            }
            catch (Exception ex) { }
        }

        public FileResult Download()
        {
            String ipFilePath = Server.MapPath("~/" + GetIPAddress() + ".txt");
            byte[] fileBytes = System.IO.File.ReadAllBytes(ipFilePath);
            string fileName = "MyLog.txt";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        private string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string ipWithoutPort = string.Empty;

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    ipWithoutPort = addresses[0].Split(':').FirstOrDefault() ?? "localhost";
                    return ipWithoutPort;
                }
            }

            ipWithoutPort = context.Request.ServerVariables["REMOTE_ADDR"].Split(':').FirstOrDefault() ?? "localhost";
            return ipWithoutPort;
        }
    }
}