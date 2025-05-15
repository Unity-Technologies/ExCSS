namespace ExCSS
{
    internal struct ParserOptions
    {
        public bool IncludeUnknownRules { get; set; }
        public bool IncludeUnknownDeclarations { get; set; }
        public bool AllowInvalidSelectors { get; set; }
        public bool AllowInvalidValues { get; set; }
        public bool AllowInvalidConstraints { get; set; }
        public bool PreserveComments { get; set; }
        public bool PreserveDuplicateProperties { get; set; }

        /// <summary>
        /// Added by Unity. We want to preserve short hand properties. (Karl)
        /// </summary>
        public bool ExpandShorthandProperties { get; set; }
    }
}