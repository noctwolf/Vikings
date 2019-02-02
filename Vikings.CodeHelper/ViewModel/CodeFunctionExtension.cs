using EnvDTE;
using Raize.CodeSiteLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vikings.CodeHelper.ViewModel
{
    public static class CodeFunctionExtension
    {
        public static string GetSetName(this CodeFunction codeFunction)
        {
            if (codeFunction.Parent is CodeProperty codeProperty)
            {
                if (codeFunction.EqualsOffset(codeProperty.Getter)) return "get";
                else if (codeFunction.EqualsOffset(codeProperty.Setter)) return "set";
                else throw new Exception("不是get也不是set");
            }
            return null;
        }

        public static bool EqualsOffset(this CodeFunction codeFunction, CodeFunction other)
        {
            if (other == null) return false;
            if (codeFunction.HasBody())
                return codeFunction.GetStartPoint(vsCMPart.vsCMPartBody).AbsoluteCharOffset == other.GetStartPoint(vsCMPart.vsCMPartBody).AbsoluteCharOffset;
            else
                return codeFunction.StartPoint.AbsoluteCharOffset == other.StartPoint.AbsoluteCharOffset;
        }

        public static bool HasExpressionBody(this CodeFunction codeFunction)
        {
            try
            {
                if (codeFunction.HasBody())//public string 只读表达式属性 => "只读表达式属性";
                    try { codeFunction.GetStartPoint(); } catch { return true; }
                else
                {
                    EditPoint editPointFind = codeFunction.StartPoint.CreateEditPoint();
                    if (editPointFind.FindPattern("=>") && editPointFind.LessThan(codeFunction.EndPoint)) return true;
                }
            }
            catch (Exception ex)
            {
                CodeSite.SendException(codeFunction.Name, ex);
            }
            return false;
        }

        public static bool HasBody(this CodeFunction codeFunction)
        {
            try
            {
                codeFunction.GetStartPoint(vsCMPart.vsCMPartBody);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
