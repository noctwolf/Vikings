using Microsoft.JScript;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Vikings.Translate
{
    public static class JScript
    {
        static MethodInfo infoGetTK;

        static JScript()
        {
            Assembly assembly = new JScriptCodeProvider().CompileAssemblyFromSource(
                new CompilerParameters { GenerateInMemory = true }, Properties.Resources.gettkjs).CompiledAssembly;
            infoGetTK = assembly.GetType("JScript").GetMethod("tk");
        }

        public static string GetTK(string text, string tkk) => infoGetTK.Invoke(null, new object[] { text, tkk }).ToString();
    }
}

