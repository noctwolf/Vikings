using System.CodeDom.Compiler;
using System.IO;

namespace System.CodeDom
{
    /// <summary>
    /// CodeExpression 扩展
    /// </summary>
    public static class CodeExpressionExtension
    {
        /// <summary>
        /// 转为文字，比如''转为'
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>转换后的文字</returns>
        public static string ToLiteral(this string value)
        {
            CodeDomProvider cdp = CodeDomProvider.CreateProvider("CSharp");
            StringWriter sw = new StringWriter();
            cdp.GenerateCodeFromExpression(new CodePrimitiveExpression(value), sw, null);
            return sw.ToString();
        }
    }
}
