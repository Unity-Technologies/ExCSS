﻿using System.Collections.Generic;
using ExCSS.Model.Extensions;

namespace ExCSS.Model.Factories.StyleRuleFactories
{
    internal class StyleRuleFactory : RuleFactory
    {
        internal StyleRuleFactory(StyleSheetContext context) : base(context)
        {
        }

        public override void Parse(IEnumerator<Block> reader)
        {
            var style = new StyleRule(Context);
            var selector = new SelectorConstructor();

            Context.ActiveRules.Push(style);

            do
            {
                if (reader.Current.Type == GrammarSegment.CurlyBraceOpen)
                {
                    if (reader.SkipToNextNonWhitespace())
                    {
                        var tokens = reader.LimitToCurrentBlock();
                        tokens.GetEnumerator().AppendDeclarations(style.Style.List);
                    }

                    break;
                }

                selector.PickSelector(reader);
            }
            while (reader.MoveNext());

            style.Selector = selector.Result;
            Context.ActiveRules.Pop();

            Context.AppendStyleToActiveRule(style);
        }
    }
}