namespace CYM.Ver
{
    internal static class VersionConverter
    {
        public static Version FromString(string semVerString)
        {
            var strings = semVerString.Split(Version.IdentifiersSeparator, Version.PreReleasePrefix, Version.BuildPrefix);
            var preReleaseStart = semVerString.IndexOf(Version.PreReleasePrefix);
            var buildIndex = semVerString.IndexOf(Version.BuildPrefix);
            var preReleaseEnd = buildIndex >= 0 ? buildIndex : semVerString.Length;
            var preRelease = preReleaseStart >= 0
                ? semVerString.Substring(preReleaseStart + 1, preReleaseEnd - preReleaseStart - 1)
                : string.Empty;
            uint major = 0;
            if (strings.Length > 0) uint.TryParse(strings[0], out major);
            uint minor = 1;
            if (strings.Length > 1) uint.TryParse(strings[1], out minor);
            uint patch = 0;
            if (strings.Length > 2) uint.TryParse(strings[2], out patch);
            var semVer = new Version
            {
                major = major,
                minor = minor,
                patch = patch,
                preRelease = preRelease,
            };
            return semVer;
        }

        public static string ToString(Version semVer)
        {
            var preRelease =
                string.IsNullOrEmpty(semVer.preRelease)
                    ? string.Empty
                    : $"{Version.PreReleasePrefix}{semVer.preRelease}";
            return string.Format("{1}{0}{2}{0}{3}{4}",
                Version.IdentifiersSeparator,
                semVer.major,
                semVer.minor,
                semVer.patch,
                preRelease);
        }
    }
}