using System;
using System.Collections.Generic;
using System.Text;

namespace ExCSS
{
    internal abstract class LexerBase : IDisposable
    {
        private readonly Stack<ushort> _columns;

        protected LexerBase(TextSource source)
        {
            StringBuffer = Pool.NewStringBuilder();
            _columns = new Stack<ushort>();
            Source = source;
            Current = Symbols.Null;
            Column = 0;
            Line = 1;
        }

        public string FlushBuffer()
        {
            var content = StringBuffer.ToString();
            StringBuffer.Clear();
            return content;
        }

        public void Dispose()
        {
            var isDisposed = StringBuffer == null;
            if (!isDisposed)
            {
                var disposable = Source as IDisposable;
                disposable?.Dispose();
                StringBuffer.Clear().ToPool();
                StringBuffer = null;
            }
        }

        public TextPosition GetCurrentPosition()
        {
            return new(Line, Column, Position);
        }

        /// <summary>
        /// Captures the lexer position (source index, line, column and current character) so
        /// <see cref="RewindTo"/> can restore it exactly.
        /// </summary>
        public LexerState SaveState()
        {
            return new LexerState(Source.Index, Line, Column, Current, _columns.Count);
        }

        /// <summary>
        /// Repositions the lexer to a state captured by <see cref="SaveState"/> so the next token is
        /// re-lexed from there. Unlike setting <see cref="InsertionPoint"/> (whose <c>BackNative</c> loop
        /// is not a faithful inverse across <c>\r\n</c> normalization), this restores the character
        /// stream and the line/column tracking exactly. Used by <see cref="StylesheetComposer"/> to rewind
        /// a CSS-Nesting classification look-ahead so a declaration re-lexes cleanly in value mode.
        /// </summary>
        public void RewindTo(LexerState state)
        {
            Source.Index = state.SourceIndex;
            Line = state.Line;
            Column = state.Column;
            Current = state.Current;
            while (_columns.Count > state.ColumnDepth)
                _columns.Pop();
        }

        protected char SkipSpaces()
        {
            var c = GetNext();
            while (c.IsSpaceCharacter())
                c = GetNext();
            return c;
        }

        protected char GetNext()
        {
            Advance();
            return Current;
        }

        protected char GetPrevious()
        {
            Back();
            return Current;
        }

        protected void Advance()
        {
            if (Current != Symbols.EndOfFile) AdvanceNative();
        }

        protected void Advance(int distance)
        {
            while (distance-- > 0 && Current != Symbols.EndOfFile) AdvanceNative();
        }

        protected void Back()
        {
            if (InsertionPoint > 0) BackNative();
        }

        protected void Back(int distance)
        {
            while (distance-- > 0 && InsertionPoint > 0) BackNative();
        }

        private void AdvanceNative()
        {
            if (Current == Symbols.LineFeed)
            {
                _columns.Push(Column);
                Column = 1;
                Line++;
            }
            else
            {
                Column++;
            }

            Current = NormalizeForward(Source.ReadCharacter());
        }

        private void BackNative()
        {
            Source.Index -= 1;
            if (Source.Index == 0)
            {
                Column = 0;
                Current = Symbols.Null;
                return;
            }

            var c = NormalizeBackward(Source[Source.Index - 1]);
            if (c == Symbols.LineFeed)
            {
                Column = _columns.Count != 0 ? _columns.Pop() : (ushort) 1;
                Line--;
                Current = c;
            }
            else if (c != Symbols.Null)
            {
                Current = c;
                Column--;
            }
        }

        private char NormalizeForward(char symbol)
        {
            if (symbol != Symbols.CarriageReturn) return symbol;
            if (Source.ReadCharacter() != Symbols.LineFeed) Source.Index--;
            return Symbols.LineFeed;
        }

        private char NormalizeBackward(char symbol)
        {
            if (symbol != Symbols.CarriageReturn) return symbol;
            if (Source.Index < Source.Length && Source[Source.Index] == Symbols.LineFeed)
            {
                BackNative();
                return Symbols.Null;
            }

            return Symbols.LineFeed;
        }

        protected StringBuilder StringBuffer { get; private set; }

        public TextSource Source { get; }

        public ushort Line { get; private set; }
        public ushort Column { get; private set; }
        public int Position => Source.Index;
        protected char Current { get; private set; }

        public readonly struct LexerState
        {
            public LexerState(int sourceIndex, ushort line, ushort column, char current, int columnDepth)
            {
                SourceIndex = sourceIndex;
                Line = line;
                Column = column;
                Current = current;
                ColumnDepth = columnDepth;
            }

            public int SourceIndex { get; }
            public ushort Line { get; }
            public ushort Column { get; }
            public char Current { get; }
            public int ColumnDepth { get; }
        }

        public int InsertionPoint
        {
            get => Source.Index;
            protected set
            {
                var delta = Source.Index - value;
                while (delta > 0)
                {
                    BackNative();
                    delta--;
                }

                while (delta < 0)
                {
                    AdvanceNative();
                    delta++;
                }
            }
        }
    }
}