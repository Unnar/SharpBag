﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBag.Torrent
{
    /// <summary>
    /// A bencoding exception.
    /// </summary>
    public class BencodingException : FormatException
    {
        /// <summary>
        /// Creates a new BencodingException.
        /// </summary>
        public BencodingException() { }

        /// <summary>
        /// Creates a new BencodingException.
        /// </summary>
        /// <param name="message">The message.</param>
        public BencodingException(string message) : base(message) { }

        /// <summary>
        /// Creates a new BencodingException.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public BencodingException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// A class with extension methods for use with Bencoding.
    /// </summary>
    public static class BencodingExtensions
    {
        /// <summary>
        /// Decode the current instance.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <returns>The root elements of the decoded string.</returns>
        public static BElement[] BDecode(this string s)
        {
            return BencodeDecoder.Decode(s);
        }
    }

    /// <summary>
    /// A class used for decoding Bencoding.
    /// </summary>
    public class BencodeDecoder
    {
        /// <summary>
        /// The main constructor.
        /// </summary>
        /// <param name="s">The bencoded string to decode.</param>
        private BencodeDecoder(string s)
        {
            this.BencodedString = s;
        }

        /// <summary>
        /// Where the reader will start reading next.
        /// </summary>
        private int Index { get; set; }

        /// <summary>
        /// The bencoded string.
        /// </summary>
        private string BencodedString { get; set; }

        /// <summary>
        /// Decodes the specified string.
        /// </summary>
        /// <param name="s">The specified string.</param>
        /// <returns>An array of the root elements.</returns>
        public static BElement[] Decode(string s)
        {
            return new BencodeDecoder(s).Decode();
        }

        /// <summary>
        /// Decodes the string.
        /// </summary>
        /// <returns>An array of root elements.</returns>
        public BElement[] Decode()
        {
            this.Index = 0;

            try
            {
                if (this.BencodedString == null) return null;

                List<BElement> rootElements = new List<BElement>();
                while (this.BencodedString.Length > this.Index)
                {
                    rootElements.Add(this.ReadElement());
                }

                return rootElements.ToArray();
            }
            catch (BencodingException) { throw; }
            catch (Exception e) { throw this.Error(e); }
        }

        /// <summary>
        /// Reads and element.
        /// </summary>
        /// <returns>The element that was read.</returns>
        private BElement ReadElement()
        {
            switch (this.BencodedString[this.Index])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9': return this.ReadString();
                case 'i': return this.ReadInteger();
                case 'l': return this.ReadList();
                case 'd': return this.ReadDictionary();
                default: throw this.Error();
            }
        }

        /// <summary>
        /// Reads a dictionary.
        /// </summary>
        /// <returns>The dictionary that was read.</returns>
        private BDictionary ReadDictionary()
        {
            this.Index++;
            BDictionary dict = new BDictionary();

            try
            {
                while (this.BencodedString[this.Index] != 'e')
                {
                    BString K = this.ReadString();
                    BElement V = this.ReadElement();
                    dict.Add(K, V);
                }
            }
            catch (BencodingException) { throw; }
            catch (Exception e) { throw this.Error(e); }

            this.Index++;
            return dict;
        }

        /// <summary>
        /// Reads a list.
        /// </summary>
        /// <returns>The list that was read.</returns>
        private BList ReadList()
        {
            this.Index++;
            BList lst = new BList();

            try
            {
                while (this.BencodedString[this.Index] != 'e')
                {
                    lst.Add(this.ReadElement());
                }
            }
            catch (BencodingException) { throw; }
            catch (Exception e) { throw this.Error(e); }

            this.Index++;
            return lst;
        }

        /// <summary>
        /// Reads an integer.
        /// </summary>
        /// <returns>The integer that was read.</returns>
        private BInteger ReadInteger()
        {
            this.Index++;

            int end = this.BencodedString.IndexOf('e', this.Index);
            if (end == -1) throw this.Error();

            long integer;

            try
            {
                integer = Convert.ToInt64(this.BencodedString.Substring(this.Index, end - this.Index));
                this.Index = end + 1;
            }
            catch (Exception e) { throw Error(e); }

            return new BInteger(integer);
        }

        /// <summary>
        /// Reads a string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        private BString ReadString()
        {
            int length;
            int semicolon;

            try
            {
                semicolon = this.BencodedString.IndexOf(':', this.Index);
                if (semicolon == -1) throw this.Error();
                length = Convert.ToInt32(this.BencodedString.Substring(this.Index, semicolon - Index));
            }
            catch (Exception e) { throw this.Error(e); }

            this.Index = semicolon + 1;
            int tmpIndex = this.Index;
            this.Index += length;

            try
            {
                return new BString(this.BencodedString.Substring(tmpIndex, length));
            }
            catch (Exception e) { throw this.Error(e); }
        }

        /// <summary>
        /// Generates an error.
        /// </summary>
        /// <param name="e">The inner exception.</param>
        /// <returns>An exception that can then be thrown.</returns>
        private Exception Error(Exception e)
        {
            return new BencodingException("Bencoded string invalid.", e);
        }

        /// <summary>
        /// Generates an error.
        /// </summary>
        /// <returns>An exception that can then be thrown.</returns>
        private Exception Error()
        {
            return new BencodingException("Bencoded string invalid.");
        }
    }

    /// <summary>
    /// An interface for bencoded elements.
    /// </summary>
    public interface BElement
    {
        /// <summary>
        /// Generates the bencoded equivalent of the element.
        /// </summary>
        /// <returns>The bencoded equivalent of the element.</returns>
        string ToBencodedString();

        /// <summary>
        /// Generates the bencoded equivalent of the element.
        /// </summary>
        /// <param name="u">The StringBuilder to append to.</param>
        /// <returns>The bencoded equivalent of the element.</returns>
        StringBuilder ToBencodedString(StringBuilder u);
    }

    /// <summary>
    /// A bencode integer.
    /// </summary>
    public class BInteger : BElement, IComparable<BInteger>
    {
        /// <summary>
        /// Allows you to set an integer to a BInteger.
        /// </summary>
        /// <param name="i">The integer.</param>
        /// <returns>The BInteger.</returns>
        public static implicit operator BInteger(int i)
        {
            return new BInteger(i);
        }

        /// <summary>
        /// The value of the bencoded integer.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// The main constructor.
        /// </summary>
        /// <param name="value">The value of the bencoded integer.</param>
        public BInteger(long value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Generates the bencoded equivalent of the integer.
        /// </summary>
        /// <returns>The bencoded equivalent of the integer.</returns>
        public string ToBencodedString()
        {
            return this.ToBencodedString(new StringBuilder()).ToString();
        }

        /// <summary>
        /// Generates the bencoded equivalent of the integer.
        /// </summary>
        /// <returns>The bencoded equivalent of the integer.</returns>
        public StringBuilder ToBencodedString(StringBuilder u)
        {
			if (u == null) u = new StringBuilder();
            return u.Append('i').Append(Value.ToString()).Append('e');
        }

        /// <see cref="Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Int32.Equals(object)
        /// </summary>
        public override bool Equals(object obj)
        {
            try
            {
                return this.Value.Equals(((BInteger)obj).Value);
            }
            catch { return false; }
        }

        /// <see cref="Object.ToString()"/>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <see cref="IComparable.CompareTo(object)"/>
        public int CompareTo(BInteger other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }

    /// <summary>
    /// A bencode string.
    /// </summary>
    public class BString : BElement, IComparable<BString>
    {
        /// <summary>
        /// Allows you to set a string to a BString.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The BString.</returns>
        public static implicit operator BString(string s)
        {
            return new BString(s);
        }

        /// <summary>
        /// The value of the bencoded integer.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The main constructor.
        /// </summary>
        /// <param name="value"></param>
        public BString(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Generates the bencoded equivalent of the string.
        /// </summary>
        /// <returns>The bencoded equivalent of the string.</returns>
        public string ToBencodedString()
        {
            return this.ToBencodedString(new StringBuilder()).ToString();
        }

        /// <summary>
        /// Generates the bencoded equivalent of the string.
        /// </summary>
        /// <param name="u">The StringBuilder to append to.</param>
        /// <returns>The bencoded equivalent of the string.</returns>
        public StringBuilder ToBencodedString(StringBuilder u)
        {
			if (u == null) u = new StringBuilder();
            return u.Append(this.Value.Length).Append(':').Append(this.Value);
        }

        /// <see cref="Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// String.Equals(object)
        /// </summary>
        public override bool Equals(object obj)
        {
            try
            {
                return this.Value.Equals(((BString)obj).Value);
            }
            catch { return false; }
        }

        /// <see cref="Object.ToString()"/>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <see cref="IComparable.CompareTo(Object)"/>
        public int CompareTo(BString other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }

    /// <summary>
    /// A bencode list.
    /// </summary>
    public class BList : List<BElement>, BElement
    {
        /// <summary>
        /// Generates the bencoded equivalent of the list.
        /// </summary>
        /// <returns>The bencoded equivalent of the list.</returns>
        public string ToBencodedString()
        {
            return this.ToBencodedString(new StringBuilder()).ToString();
        }

        /// <summary>
        /// Generates the bencoded equivalent of the list.
        /// </summary>
        /// <param name="u">The StringBuilder to append to.</param>
        /// <returns>The bencoded equivalent of the list.</returns>
        public StringBuilder ToBencodedString(StringBuilder u)
        {
			if (u == null) u = new StringBuilder('l');
            else u.Append('l');

            foreach (BElement element in base.ToArray())
            {
                element.ToBencodedString(u);
            }

            return u.Append('e');
        }

        /// <summary>
        /// Adds the specified value to the list.
        /// </summary>
        /// <param name="value">The specified value.</param>
        public void Add(string value)
        {
            base.Add(new BString(value));
        }

        /// <summary>
        /// Adds the specified value to the list.
        /// </summary>
        /// <param name="value">The specified value.</param>
        public void Add(int value)
        {
            base.Add(new BInteger(value));
        }
    }

    /// <summary>
    /// A bencode dictionary.
    /// </summary>
    public class BDictionary : SortedDictionary<BString, BElement>, BElement
    {
        /// <summary>
        /// Generates the bencoded equivalent of the dictionary.
        /// </summary>
        /// <returns>The bencoded equivalent of the dictionary.</returns>
        public string ToBencodedString()
        {
            return this.ToBencodedString(new StringBuilder()).ToString();
        }

        /// <summary>
        /// Generates the bencoded equivalent of the dictionary.
        /// </summary>
        /// <param name="u">The StringBuilder to append to.</param>
        /// <returns>The bencoded equivalent of the dictionary.</returns>
        public StringBuilder ToBencodedString(StringBuilder u)
        {
			if (u == null) u = new StringBuilder('d');
            else u.Append('d');

            for (int i = 0; i < base.Count; i++)
            {
                base.Keys.ElementAt(i).ToBencodedString(u);
                base.Values.ElementAt(i).ToBencodedString(u);
            }

            return u.Append('e');
        }

        /// <summary>
        /// Adds the specified key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="value">The specified value.</param>
        public void Add(string key, BElement value)
        {
            base.Add(new BString(key), value);
        }

        /// <summary>
        /// Adds the specified key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="value">The specified value.</param>
        public void Add(string key, string value)
        {
            base.Add(new BString(key), new BString(value));
        }

        /// <summary>
        /// Adds the specified key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="value">The specified value.</param>
        public void Add(string key, int value)
        {
            base.Add(new BString(key), new BInteger(value));
        }

        /// <summary>
        /// Gets or sets the value assosiated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value assosiated with the specified key.</returns>
        public BElement this[string key]
        {
            get
            {
                return this[new BString(key)];
            }
            set
            {
                this[new BString(key)] = value;
            }
        }
    }
}
