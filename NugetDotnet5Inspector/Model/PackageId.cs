using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Synopsys.Integration.Nuget.Dotnet3.Model
{
    public class PackageId
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Framework { get; set; }

        public PackageId(string name, string version, string framework) : this(name, version)
        {
            Framework = framework;
        }

        public PackageId(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public override int GetHashCode()
        {
            int prime = 37;
            int result = 1;
            result = result * prime + ((Name == null) ? 0 : Name.GetHashCode());
            result = result * prime + ((Version == null) ? 0 : Version.GetHashCode());
            result = result * prime + ((Framework == null) ? 0 : Framework.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                PackageId other = (PackageId)obj;
                if (Name == null)
                {
                    if (other.Name != null)
                    {
                        return false;
                    }
                }
                else if (!Name.Equals(other.Name))
                {
                    return false;
                }

                if (Version == null)
                {
                    if (other.Version != null)
                    {
                        return false;
                    }
                }
                else if (!Version.Equals(other.Version))
                {
                    return false;
                }

                if (Framework == null)
                {
                    if (other.Framework != null)
                    {
                        return false;
                    }
                }
                else if (!Framework.Equals(other.Framework))
                {
                    return false;
                }
                return true;
            }
        }


    }
}
