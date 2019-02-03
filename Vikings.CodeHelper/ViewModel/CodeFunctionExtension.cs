using EnvDTE;
using Raize.CodeSiteLogging;
using System;
using System.Linq;

namespace Vikings.CodeHelper.ViewModel
{
    public static class CodeFunctionExtension
    {
        enum CSTextPart { Enter, Catch, Exit }

        static string CSText(this CodeFunction codeFunction, CSTextPart cstp)
        {
            const string sEnter = @"CodeSite.EnterMethod({0}""{1}{2}"");
try
{{
";
            const string sExit = @"}}
finally
{{
CodeSite.ExitMethod({0}""{1}{2}"");
}}
";
            const string sCatch = @"}}
catch (Exception ex) when (ex.SendCodeSite())
{{
throw;
";
            string sClass = string.Empty;
            if (codeFunction.IsShared)
            {
                CodeElement ce = codeFunction.Parent as CodeElement;
                if (ce.Kind == vsCMElement.vsCMElementClass || ce.Kind == vsCMElement.vsCMElementStruct)
                    sClass = string.Format(@"""{0}."" + ", ce.Name);
            }
            else
                sClass = "this, ";

            var sProperty = codeFunction.GetSetName();
            if (sProperty != null) sProperty = "/" + sProperty;

            switch (cstp)
            {
                case CSTextPart.Enter:
                    return string.Format(sEnter, sClass, codeFunction.Name, sProperty);
                case CSTextPart.Catch:
                    return string.Format(sCatch);
                case CSTextPart.Exit:
                    return string.Format(sExit, sClass, codeFunction.Name, sProperty);
                default:
                    return string.Empty;
            }
        }

        public static string CSEnterText(this CodeFunction value)
        {
            return value.CSText(CSTextPart.Enter);
        }

        public static string CSCatchText(this CodeFunction value)
        {
            return value.CSText(CSTextPart.Catch);
        }

        public static string CSExitText(this CodeFunction value)
        {
            return value.CSText(CSTextPart.Exit);
        }

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

        public static void AddCodeSite(this CodeFunction codeFunction)
        {
            if (codeFunction.HasExpressionBody())
            {//展开表达式主体为程序块主体，不做逆向处理
                CodeSite.Send("展开表达式主体为程序块主体", codeFunction.Name);
                var addReturn = !(codeFunction.Type.TypeKind == vsCMTypeRef.vsCMTypeRefVoid ||
                    codeFunction.Parent is CodeProperty codeProperty && codeFunction.EqualsOffset(codeProperty.Setter));
                if (codeFunction.HasBody())
                {
                    var epFind = codeFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                    var epEEnd = codeFunction.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                    epFind.FindPattern("=>", (int)vsFindOptions.vsFindOptionsBackwards);
                    epFind.Delete(2);
                    epFind.Insert("{get{");
                    if (addReturn) epFind.Insert("return ");
                    epEEnd.CharRight();
                    epEEnd.Insert("}}");
                }
                else
                {
                    var epFind = codeFunction.GetStartPoint().CreateEditPoint();
                    var epEEnd = codeFunction.GetEndPoint().CreateEditPoint();
                    epFind.FindPattern("=>");
                    epFind.Delete(2);
                    epFind.Insert("{");
                    if (addReturn) epFind.Insert("return ");
                    //epEEnd.CharRight();
                    epEEnd.Insert("}");
                }
            }
            if (codeFunction.ExistsCodeSite()) codeFunction.DeleteCodeSite();
            EditPoint epStart = codeFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
            EditPoint epEnd = codeFunction.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
            if (epStart.Line == epEnd.Line) epEnd.Insert(Environment.NewLine);
            epStart.Insert(codeFunction.CSEnterText());
            if (Properties.Settings.Default.IncludeCatch) epEnd.Insert(codeFunction.CSCatchText());
            epEnd.Insert(codeFunction.CSExitText());

            //格式化指定范围内的文本
            codeFunction.StartPoint.CreateEditPoint().SmartFormat(codeFunction.EndPoint.CreateEditPoint());
        }

        public static bool ExistsCodeSite(this CodeFunction codeFunction)
        {
            try
            {
                codeFunction.GetStartPoint(vsCMPart.vsCMPartBody);
            }
            catch//表达式主体
            {
                return false;
            }
            return codeFunction.DeleteCodeSite(false);
        }

        public static bool DeleteCodeSite(this CodeFunction value, bool bDelete = true)
        {
            if (bDelete && !value.ExistsCodeSite()) return true;
            EditPoint epStart = value.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
            EditPoint epEnd = value.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
            EditPoint epFind = null, epFindStart = null;

            bool Find(string text, bool start)
            {
                string[] textSplit;
                if (start)
                {
                    epFind = epStart.CreateEditPoint();
                    textSplit = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    epFind = epEnd.CreateEditPoint();
                    textSplit = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray();
                }
                int iLine = 0;
                foreach (var item in textSplit)
                {
                    int i = item.IndexOfAny(new char[] { '(', ' ' });
                    string s = i == -1 ? item : item.Substring(0, i + 1);

                    if ((start ? epFind.FindPattern(s) : epFind.FindPattern(s, (int)vsFindOptions.vsFindOptionsBackwards))
                        && (start ? epFind.LessThan(epEnd) : epFind.GreaterThan(epStart))
                        && (iLine == 0 || iLine == epFind.Line - (start ? 1 : -1)))
                    {
                        if (iLine == 0) epFindStart = epFind.CreateEditPoint();
                        iLine = epFind.Line;
                        if (start) epFind.EndOfLine();
                    }
                    else
                        return false;
                }
                return true;
            }

            void Delete()
            {
                if (epFind.GreaterThan(epFindStart))
                    epFind.LineDown(1);
                else
                    epFindStart.LineDown(1);
                epFind.StartOfLine();
                epFindStart.StartOfLine();
                epFindStart.Delete(epFind);
            }

            if (!Find(value.CSEnterText(), true)) return false;
            else if (bDelete) Delete();

            if (!Find(value.CSExitText(), false)) return false;
            else if (bDelete) Delete();
            else return true;

            if (Find(value.CSCatchText(), false) && bDelete) Delete();

            //格式化指定范围内的文本
            value.StartPoint.CreateEditPoint().SmartFormat(value.EndPoint);
            return true;
        }
    }
}
