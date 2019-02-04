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
            if (!(e.Length == 2 && int.TryParse(e[0], out _) && int.TryParse(e[1], out _))) throw new ArgumentNullException();
            var g = new List<int>();
            for (int f = 0; f < a.Length; f++)
            {
                int c = a[f];
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
                            g.Add(c >> 12 | 224);
                        g.Add(c >> 6 & 63 | 128);
                    }
                    g.Add(c & 63 | 128);
                }
            }
            var r = int.Parse(e[0]);
            for (int d = 0; d < g.Count; d++)
            {
                r += g[d];
                r = B(r, "+-a^+6");
            }
            r = B(r, "+-3^+b+-f");
            r ^= int.Parse(e[1]);
            var v = (uint)r % 1000000;
            return v.ToString() + "." + (v ^ int.Parse(e[0]));
        }

        private static int B(int a, string expression)
        {
            for (int i = 0; i < expression.Length - 2; i += 3)
            {
                var bit = System.Convert.ToInt32(expression[i + 2].ToString(), 16);
                var offset = expression[i + 1] == '+' ? (int)((uint)a >> bit) : a << bit;
                a = expression[i] == '+' ? a + offset : a ^ offset;
            }
            return a;
        }
    }
}

