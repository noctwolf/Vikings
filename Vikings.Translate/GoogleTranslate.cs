using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Vikings.Translate
{
    public static class GoogleTranslate
    {
        public static string BaseUrl { get; set; } = "https://translate.google.cn/";

        /// <summary>
        /// 翻译为简体中文 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToZhHans(string text) => Translate(text, "auto", "zh-CN");

        /// <summary>
        /// 谷歌翻译
        /// </summary>
        /// <param name="text">待翻译文本</param>
        /// <param name="fromLanguage">自动检测：auto</param>
        /// <param name="toLanguage">中文：zh-CN，英文：en</param>
        /// <returns>翻译后文本</returns>
        public static string Translate(string text, string fromLanguage, string toLanguage)
        {
            //.NET 的uri会屏蔽特殊字符，UrlEncode 后是 %e2%80%8e，导致计算的tk不一致
            text = string.Join("", text.ToCharArray().Where(f => !"\u200B\u200C\u200D\u200E\u200F\uFEFF".Contains(f)));
            var cc = new CookieContainer();
            var tk = JScript.GetTK(text, GetTKK(BaseUrl, cc));
            string googleTransUrl = BaseUrl + "translate_a/single?client=webapp&sl=" + fromLanguage + "&tl=" + toLanguage + "&hl=en&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&ie=UTF-8&oe=UTF-8&otf=1&ssel=0&tsel=0&kc=1&tk=" + tk + "&q=" + WebUtility.UrlEncode(text);
            var ResultHtml = GetResultHtml(googleTransUrl, cc, BaseUrl);
            dynamic TempResult = Newtonsoft.Json.JsonConvert.DeserializeObject(ResultHtml);
            string result = "";
            for (int i = 0; i < TempResult[0].Count; i++) result += Convert.ToString(TempResult[0][i][0]);
            return result;
        }

        static string GetTKK(string GoogleTransBaseUrl, CookieContainer cc)
        {
            var BaseResultHtml = GetResultHtml(GoogleTransBaseUrl, cc, "");
            //TKK变化，json格式。2018-11-30，dxg//tkk:'428764.2089508696',
            var re = new Regex(@"(?<=tkk:')(.*?)(?=',)");
            return re.Match(BaseResultHtml).ToString();
        }

        static string GetResultHtml(string url, CookieContainer cc, string refer)
        {
            var html = "";
            var webRequest = WebRequest.Create(url) as HttpWebRequest;
            //webRequest.Proxy = new WebProxy("127.0.0.1:8086");
            webRequest.Method = "GET";
            webRequest.CookieContainer = cc;
            webRequest.Referer = refer;
            webRequest.Timeout = 20000;
            webRequest.Headers.Add("X-Requested-With:XMLHttpRequest");
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
            using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using (var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                {
                    html = reader.ReadToEnd();
                    reader.Close();
                    webResponse.Close();
                    return html;
                }
            }
        }
    }
}