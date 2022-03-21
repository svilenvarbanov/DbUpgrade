using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbUpgrade.Models
{
    public class DbVersionModel : IComparable<DbVersionModel>, IEqualityComparer<DbVersionModel>, IEquatable<DbVersionModel>
    {
        private int Major { get; }
        private int Minor { get; }
        private int Patch { get; }
        private int Build { get; }
        public DbVersionModel(string version)
        {
            var verArr = version.Split(".");
            if (verArr.Length != 4)
            {
                throw new InvalidCastException($"Version {version} is not a valid semantic version string.");
            }

            try
            {
                Major = int.Parse(verArr[0]);
                Minor = int.Parse(verArr[1]);
                Patch = int.Parse(verArr[2]);
                Build = int.Parse(verArr[3]);
            }
            catch
            {
                throw new InvalidCastException($"Version {version} is not a valid semantic version string.");
            }
        }

        #region Overrides of Object

        /// <inheritdoc />

        public static bool operator <(DbVersionModel first, DbVersionModel second)
        {
            return first.GetHashCode() < second.GetHashCode();
        }

        public static bool operator > (DbVersionModel first, DbVersionModel second )
        {
            return first.GetHashCode() > second.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}.{Build}";
        }

        public int CompareTo(DbVersionModel other)
        {
            return GetHashCode() - other.GetHashCode();
        }

        public bool Equals(DbVersionModel x, DbVersionModel y)
        {
            return x?.GetHashCode() == y?.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var verStr = $"{Major}{Minor}{Patch}{Build}";

            return int.Parse(verStr);
        }

        public int GetHashCode([DisallowNull] DbVersionModel obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(DbVersionModel other)
        {
            return GetHashCode() == other?.GetHashCode();
        }
        #endregion
    }
}
