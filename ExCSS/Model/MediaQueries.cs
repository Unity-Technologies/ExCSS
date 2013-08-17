using System;
using System.Collections;
using System.Collections.Generic;
using ExCSS.Model.Extensions;

namespace ExCSS.Model
{
    public sealed class MediaQueries : IEnumerable<string>
    {
        private readonly List<string> _media;
        private string _buffer;

        internal MediaQueries()
        {
            _buffer = string.Empty;
            _media = new List<string>();
        }

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= _media.Count)
                {
                    return null;
                }

                return _media[index];
            }
        }

        public int Length
        {
            get { return _media.Count; }
        }

        public string MediaText
        {
            get { return _buffer; }
            set
            {
                _buffer = string.Empty;
                _media.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    var entries = value.SplitCommas();

                    for (var i = 0; i < entries.Length; i++)
                    {
                        //if (!CheckSyntax(entries[i]))
                        //    throw new DOMException(ErrorCode.SyntaxError);
                    }

                    foreach (var t in entries)
                    {
                        AppendMedium(t);
                    }
                }
            }
        }

        public MediaQueries AppendMedium(string newMedium)
        {
            if (!CheckSyntax(newMedium))
            {
                //throw new DOMException(ErrorCode.SyntaxError);
            }

            if (!_media.Contains(newMedium))
            {
                _media.Add(newMedium);
                _buffer += (String.IsNullOrEmpty(_buffer) ? string.Empty : ",") + newMedium;
            }

            return this;
        }

        public MediaQueries DeleteMedium(string oldMedium)
        {
            if (!_media.Contains(oldMedium))
            {
                //throw new DOMException(ErrorCode.ItemNotFound);
            }

            _media.Remove(oldMedium);

            if (_buffer.StartsWith(oldMedium))
            {
                _buffer.Remove(0, oldMedium.Length + 1);
            }
            else
            {
                _buffer.Replace("," + oldMedium, string.Empty);
            }

            return this;
        }


        private static bool CheckSyntax(string medium)
        {
            return !string.IsNullOrEmpty(medium);
        }

        public IEnumerator<String> GetEnumerator()
        {
            return ((IEnumerable<string>) _media).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}