using System.CodeDom.Compiler;
using System.IO;

namespace System.CodeDom
{
    public static class CodeExpressionExtension
    {
        public static string ToLiteral(this string value)
        {
            CodeDomProvider cdp = CodeDomProvider.CreateProvider("CSharp");
            StringWriter sw = new StringWriter();
            cdp.GenerateCodeFromExpression(new CodePrimitiveExpression(value), sw, null);
            return sw.ToString();
        }
    }
}
