namespace CYM.DLC
{
    public partial class DLCItem
    {
        public string[] GetAllCSharp() => GetAllFilies(SysConst.Dir_CSharp, "*.csharp");
    }
}