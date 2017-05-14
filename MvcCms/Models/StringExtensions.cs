﻿using System.Text.RegularExpressions;

namespace MvcCms.Models
{
    public static class StringExtensions
    {
        public static string MakeUrlFriendly(this string value)
        {
            value = value.ToLowerInvariant().Replace(" ", "-");
            value = Regex.Replace(value, @"[^0-9a-z-]", string.Empty);
            return value;
        }
    }
}