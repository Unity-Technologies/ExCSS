namespace ExCSS
{
    public sealed class AttrContainsSelector : AttrSelectorBase
    {
        public AttrContainsSelector(string attribute, string value) 
            : base(attribute, value, $"[{attribute}*={value.StylesheetString()}]")
        {
        }
    }
}