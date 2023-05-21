using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CYM.Ver
{
    /// <summary>
    /// A semantic version based on the <a href="https://semver.org/">Semantic Versioning 2.0.0</a> specification.
    /// </summary>
    [Serializable]
    public class Version : IComparable<Version>, IEquatable<Version>
    {
        public const char IdentifiersSeparator = '.';
        public const char PreReleasePrefix = '-';
        public const char BuildPrefix = '+';

        public static bool operator ==(Version left, Version right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Version left, Version right)
        {
            return !Equals(left, right);
        }

        public static bool operator >(Version left, Version right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(Version left, Version right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(Version left, Version right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(Version left, Version right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static implicit operator string(Version s)
        {
            return s.ToString();
        }

        public static implicit operator Version(string s)
        {
            return Parse(s);
        }

        public static Version Parse(string semVer)
        {
            return VersionConverter.FromString(semVer);
        }

        /// <summary>
        /// Major version X (X.y.z | X > 0) MUST be incremented if any backwards incompatible changes are introduced
        /// to the public API. It MAY include minor and patch level changes. Patch and minor version MUST be reset to 0
        /// when major version is incremented.
        /// <seealso cref="IncrementMajor"/>
        /// </summary>
        public uint major;

        /// <summary>
        /// Minor version Y (x.Y.z | x > 0) MUST be incremented if new, backwards compatible functionality is
        /// introduced to the public API. It MUST be incremented if any public API functionality is marked as
        /// deprecated. It MAY be incremented if substantial new functionality or improvements are introduced within
        /// the private code. It MAY include patch level changes. Patch version MUST be reset to 0 when minor version
        /// is incremented.
        /// <seealso cref="IncrementMinor"/>
        /// </summary>
        public uint minor;

        /// <summary>
        /// Patch version Z (x.y.Z | x > 0) MUST be incremented if only backwards compatible bug fixes are introduced.
        /// </summary>
        public uint patch;

        /// <summary>
        /// A pre-release version indicates that the version is unstable and might not satisfy the intended
        /// compatibility requirements as denoted by its associated normal version.
        /// </summary>
        /// <example>1.0.0-<b>alpha</b>, 1.0.0-<b>alpha.1</b>, 1.0.0-<b>0.3.7</b>, 1.0.0-<b>x.7.z.92</b></example>
        public string preRelease;

        [SerializeField] private string build = string.Empty;

        /// <summary>
        /// The base part of the version number (Major.Minor.Patch).
        /// </summary>
        /// <example>1.9.0</example>
        /// <returns>Major.Minor.Patch</returns>
        public string Core => $"{major}.{minor}.{patch}";

        /// <summary>
        /// An internal version number. This number is used only to determine whether one version is more recent than
        /// another, with higher numbers indicating more recent versions.
        /// <a href="https://developer.android.com/studio/publish/versioning"/>
        /// </summary>
        /// <returns><c>Major * 10000 + Minor * 100 + Patch</c></returns>
        public int AndroidBundleVersionCode
        {
            get
            {
                var clampedPatch = ClampAndroidBundleVersionCode(patch, "Patch");
                var clampedMinor = ClampAndroidBundleVersionCode(minor, "Minor");
                return (int) (major * 10000 + clampedMinor * 100 + clampedPatch);
            }
        }

        private static uint ClampAndroidBundleVersionCode(uint value, string name)
        {
            uint clamped;
            const uint max = 100;
            if (value >= max)
            {
                clamped = max - 1;
                Debug.LogWarning(name + " should be less than " + max);
            }
            else
            {
                clamped = value;
            }

            return clamped;
        }

        public Version()
        {
            minor = 1;
            preRelease = string.Empty;
        }

        /// <summary>
        /// Increment the major version, reset the patch and the minor version to 0.
        /// </summary>
        public void IncrementMajor()
        {
            major++;
            minor = patch = 0;
        }

        /// <summary>
        /// Increment the minor version, reset the patch version to 0.
        /// </summary>
        public void IncrementMinor()
        {
            minor++;
            patch = 0;
        }

        /// <summary>
        /// Increment the patch version.
        /// </summary>
        public void IncrementPatch()
        {
            patch++;
        }

        /// <summary>
        /// Check if this semantic version meets the <a href="https://semver.org/">Semantic Versioning 2.0.0</a>
        /// specification.
        /// </summary>
        /// <returns>The result of validation and automatically corrected version number.</returns>
        public VersionValidationResult Validate()
        {
            return new VersionValidator().Validate(this);
        }

        /// <summary>
        /// Creates a copy of this semantic version.
        /// </summary>
        public Version Clone()
        {
            return new Version
            {
                major = major,
                minor = minor,
                patch = patch,
                preRelease = preRelease,
            };
        }

        public int CompareTo(Version other)
        {
            return new VersionComparer().Compare(this, other);
        }

        public bool Equals(Version other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CompareTo(other) == 0;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Version) obj);
        }

        //public override int GetHashCode()
        //{
        //    throw new NotImplementedException();
        //}

        public override string ToString()
        {
            return VersionConverter.ToString(this);
        }
    }

    public static class VersionErrorMessage
    {
        public const string Empty = "Pre-release and build identifiers must not be empty";

        public const string Invalid =
            "Pre-release and build identifiers must comprise only ASCII alphanumerics and hyphen";

        public const string LeadingZero = "Numeric pre-release identifiers must not include leading zeroes";
    }

    /// <summary>
    /// Information returned about a checked semantic version.
    /// <seealso cref="Version.Validate"/>
    /// </summary>
    public class VersionValidationResult
    {
        /// <summary>
        /// Error messages. This collection is empty if the version is valid.
        /// </summary>
        public readonly ReadOnlyCollection<string> Errors;

        /// <summary>
        /// Automatically corrected semantic version.
        /// </summary>
        public readonly Version Corrected;

        /// <summary>
        /// Does the version meet the <a href="https://semver.org/">Semantic Versioning 2.0.0</a> specification?
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        internal VersionValidationResult(ReadOnlyCollection<string> errors, Version corrected)
        {
            Errors = errors;
            Corrected = corrected;
        }
    }
}