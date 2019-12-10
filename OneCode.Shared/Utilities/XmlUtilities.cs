using System;
using System.Collections.Generic;

#nullable enable

namespace OneCode.Shared.Utilities
{
    public static class XmlUtilities
    {
        public const string SpecialPrefix = "<![CDATA[";
        public const string SpecialPostfix = "]]>";

        public static string GetVersionText(string modifiersText)
        {
            return modifiersText.Extract(SpecialPrefix + "Version: ", SpecialPostfix) ?? "1.0.0.0";
        }

        public static List<string> GetDependencies(string modifiersText)
        {
            return modifiersText.ExtractAll(SpecialPrefix + "Dependency: ", SpecialPostfix);
        }

        public static Version GetVersion(string modifiersText)
        {
            return Version.TryParse(GetVersionText(modifiersText), out var result)
                ? result
                : Version.Parse("1.0.0.0");
        }
    }
}
