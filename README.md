# Vikings
- Vikings是一个.NET工具包，可在nuget直接下载编译过的包。其主要目的是为.NET框架增加易用的扩展
- Vikings is a .NET coding utility package, binary package avaliable on nuget for Visual Studio. Its main purpose is add some awosome extension to .NET Framework.

# Extensions list
- .NET WinForm和WPF的扩展，将LINQ引入界面控件查询。其主要目的在于简化代码并提高代码可读性。
- .NET WinForm and WPF extension, introducing LINQ to UI controls retrieval. Its main purpose is code reducing and readability improving.
	- Usage: (some winform object).All() return a IEnumable object contained all UI controls in the given win form Control object
	-		 (some WPF form object).All() return a IEnumable object contained all UI controls in the given WPF form object
	-		 (some WPF form object).Child() return a IEnumable object contained all child UI controls in the given WPF form object
	- 		 So, you can use LINQ functions like Any() Where() to retrieval controls
	
- Google 翻译调用，简化代码，一行搞定
- Google translation, very simple, only one line
	- Usage: - Vikings.Translate.Translate("some text to be translate","original language code,like en-US, support 'auto'","target language code, like zh-CN")
