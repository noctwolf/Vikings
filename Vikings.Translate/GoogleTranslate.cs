using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Vikings.Translate
{
    /// <summary>
    /// 谷歌翻译
    /// </summary>
    public static class GoogleTranslate
    {
        /// <summary>
        /// 翻译地址，默认是https://translate.google.cn/，可切换到https://translate.google.com/
        /// </summary>
        public static string BaseUrl { get; set; } = "https://translate.google.cn/";

        /// <summary>
        /// 翻译为简体中文
        /// </summary>
        /// <param name="text">待翻译文本</param>
        /// <returns>翻译后文本</returns>
        public static string ToZhHans(string text) => Translate(text, "auto", "zh-CN");

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="text">待翻译文本</param>
        /// <param name="fromLanguage">自动检测：auto</param>
        /// <param name="toLanguage">中文：zh-CN，英文：en</param>
        /// <returns>翻译后文本</returns>
        public static string Translate(string text, string fromLanguage, string toLanguage)
        {
            text = text.StripBidiControlCharacter();
            var cc = new CookieContainer();
            var tk = GetTK(text, GetTKK(BaseUrl, cc));
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

        /// <summary>
        /// 获取字符串的tk
        /// </summary>
        /// <param name="text">要计算的字符串</param>
        /// <param name="tkk">哈希参数</param>
        /// <returns>计算好的tk</returns>
        public static string GetTK(string text, string tkk)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(tkk)) throw new ArgumentNullException(nameof(tkk));
            var tkkSplit = tkk.Split('.');
            if (!(tkkSplit.Length == 2 && int.TryParse(tkkSplit[0], out _) && int.TryParse(tkkSplit[1], out _)))
                throw new ArgumentException(nameof(tkk));
            var list = new List<int>();
            for (int i = 0; i < text.Length; i++)
            {
                int charCode = text[i];
                if (charCode < 0x80) list.Add(charCode);
                else
                {
                    if (charCode < 0x800)
                        list.Add(charCode >> 6 | 0xC0);
                    else
                    {
                        if ((charCode & 0xFC00) == 0xD800 && i + 1 < text.Length && (text[i + 1] & 0xFC00) == 0xDC00)
                        {
                            charCode = 0x10000 + ((charCode & 0x3FF) << 10) + (text[++i] & 0x3FF);
                            list.Add(charCode >> 18 | 0xF0);
                            list.Add(charCode >> 12 & 0x3F | 0x80);
                        }
                        else
                            list.Add(charCode >> 12 | 0xE0);
                        list.Add(charCode >> 6 & 0x3F | 0x80);
                    }
                    list.Add(charCode & 0x3F | 0x80);
                }
            }
            int tkk0 = int.Parse(tkkSplit[0]);
            var r = tkk0;
            for (int i = 0; i < list.Count; i++)
            {
                r += list[i];
                r = HashTK(r, "+-a^+6");
            }
            r = HashTK(r, "+-3^+b+-f");
            r ^= int.Parse(tkkSplit[1]);
            var result = (uint)r % 1000000;
            return $"{result}.{result ^ tkk0}";
        }

        static int HashTK(int r, string expression)
        {
            for (int i = 0; i < expression.Length - 2; i += 3)
            {
                var bit = Convert.ToInt32(expression[i + 2].ToString(), 16);
                var offset = expression[i + 1] == '+' ? (int)((uint)r >> bit) : r << bit;
                r = expression[i] == '+' ? r + offset : r ^ offset;
            }
            return r;
        }
    }
}