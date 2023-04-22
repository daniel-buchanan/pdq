﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using pdq.common.Utilities.Reflection;
using pdq.db.common;
using pdq.npgsql.Builders;
using pdq.state.Utilities;

namespace pdq.npgsql
{
    public class NpgsqlValueParser : ValueParser
    {
        private readonly List<Tuple<string, string>> replacements;

        public NpgsqlValueParser(IReflectionHelper reflectionHelper)
            : base(reflectionHelper)
        {
            Replacements = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("'", "''"),
                new Tuple<string, string>("%%", ""),
                new Tuple<string, string>("--", "")
            };
        }

        protected override List<Tuple<string, string>> Replacements { get; }

        /// <inheritdoc/>
        public override bool ValueNeedsQuoting(Type type)
        {
            var underlyingType = this.reflectionHelper.GetUnderlyingType(type);

            if (underlyingType == typeof(bool)) return true;
            else if (underlyingType == typeof(byte[])) return true;
            else if (underlyingType == typeof(DateTime)) return true;
            else if (underlyingType == typeof(int)) return false;
            else if (underlyingType == typeof(double) ||
                        underlyingType == typeof(Single) ||
                        underlyingType == typeof(float) ||
                        underlyingType == typeof(decimal))
                return true;
            else if (underlyingType == typeof(string)) return true;
            else return true;
        }

        protected override bool BooleanFromString(string input)
            => input == "1";

        protected override string BooleanToString(bool input)
            => input ? "1" : "0";

        protected override byte[] BytesFromString(string input)
        {
            if (!(input.StartsWith(@"\x") || input.StartsWith(@"\\x")))
                return default(byte[]);

            if (input.StartsWith(@"\\x")) input = input.Substring(3);
            else if (input.StartsWith(@"\x")) input = input.Substring(2);

            int NumberChars = input.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);
            return bytes;
        }

        protected override string BytesToString(byte[] input)
        {
            var valueString = BitConverter.ToString(input).Replace("-", "");
            return $@"\x{valueString}";
        }
    }
}

