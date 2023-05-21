using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CYM.Ver
{
    internal class VersionValidator
    {
        private List<string> _errors;
        private Version _corrected;

        public VersionValidationResult Validate(Version semVer)
        {
            _errors = new List<string>();
            _corrected = semVer.Clone();
            ValidatePreRelease(semVer);
            return new VersionValidationResult(_errors.AsReadOnly(), _corrected.Clone());
        }

        private void ValidatePreRelease(Version semVer)
        {
            var identifiers = ValidateIdentifiers(semVer.preRelease);
            identifiers = ValidateLeadingZeroes(identifiers);
            var joined = JoinIdentifiers(identifiers);
            _corrected.preRelease = joined;
        }

        private string[] ValidateIdentifiers(string identifiers)
        {
            if (string.IsNullOrEmpty(identifiers)) return new string[0];
            var separators = new[] {Version.IdentifiersSeparator};
            var strings = identifiers.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (strings.Length < identifiers.Count(c => c == Version.IdentifiersSeparator) + 1)
            {
                _errors.Add(VersionErrorMessage.Empty);
            }

            for (var i = 0; i < strings.Length; i++)
            {
                var raw = strings[i];
                var corrected = Regex.Replace(raw, "[^0-9A-Za-z-]", "-");
                if (string.Equals(raw, corrected)) continue;
                _errors.Add(VersionErrorMessage.Invalid);
                strings[i] = corrected;
            }

            return strings;
        }

        private string[] ValidateLeadingZeroes(IList<string> identifiers)
        {
            var length = identifiers.Count;
            var corrected = new string[length];
            for (var i = 0; i < length; i++)
            {
                var identifier = identifiers[i];
                var isNumeric = int.TryParse(identifier, out var numericIdentifier);
                if (isNumeric && identifier.StartsWith("0") && identifier.Length > 1)
                {
                    corrected[i] = numericIdentifier.ToString();
                    _errors.Add(VersionErrorMessage.LeadingZero);
                }
                else
                {
                    corrected[i] = identifier;
                }
            }

            return corrected;
        }

        private static string JoinIdentifiers(string[] identifiers)
        {
            return string.Join(Version.IdentifiersSeparator.ToString(), identifiers);
        }
    }
}