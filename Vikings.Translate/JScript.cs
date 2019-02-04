using Microsoft.JScript;
using System.CodeDom.Compiler;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Vikings.Translate
{
    static class JScript
    {
        static MethodInfo infoGetTK;

        static JScript()
        {
            Assembly assembly = new JScriptCodeProvider().CompileAssemblyFromSource(
                new CompilerParameters { GenerateInMemory = true }, Properties.Resources.gettkjs).CompiledAssembly;
            infoGetTK = assembly.GetType("JScript").GetMethod("tk");
        }

        public static string GetTK(string text, string tkk) => infoGetTK.Invoke(null, new object[] { text, tkk }).ToString();

        public static string TK(string a, string TKK)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(TKK)) throw new ArgumentNullException();
            var e = TKK.Split('.');
            if (!(e.Length == 2 && long.TryParse(e[0], out _) && long.TryParse(e[1], out _))) throw new ArgumentNullException();
            var h = long.Parse(e[0]);
            var g = new List<long>();
            for (int f = 0; f < a.Length; f++)
            {
                long c = a[f];
                if (c < 128) g.Add(c);
                else
                {
                    if (c < 2048)
                        g.Add(c >> 6 | 192);
                    else
                    {
                        if ((c & 64512) == 55296 && f + 1 < a.Length && (a[f + 1] & 64512) == 56320)
                        {
                            c = 65536 + ((c & 1023) << 10) + (a[++f] & 1023);
                            g.Add(c >> 18 | 240);
                            g.Add(c >> 12 & 63 | 128);
                        }
                        else
                        {
                            g.Add(c >> 12 | 224);
                        }
                        g.Add(c >> 6 & 63 | 128);
                    }
                    g.Add(c & 63 | 128);
                }
            }
            var r = h;
            for (int d = 0; d < g.Count; d++)
            {
                r += g[d];
                r = b(r, "+-a^+6");
            }
            r = b(r, "+-3^+b+-f");
            r ^= long.Parse(e[1]);
            if (r < 0) r = (r & 2147483647) + 2147483648;
            r %= 1000000;
            return r.ToString() + "." + (r ^ h);
        }

        private static long b(long a, string b)
        {
            for (int d = 0; d < b.Length - 2; d += 3)
            {
                var c = b[d + 2] >= 'a' ? b[d + 2] - 87 : long.Parse(b[d + 2].ToString());
                c = b[d + 1] == '+' ? (uint)a >> (int)c : a << (int)c;
                a = (int)(b[d] == '+' ? a + c & 4294967295 : a ^ c);
            }
            return (int)a;
        }
    }
}

