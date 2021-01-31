using PackageConsolidation.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace PackageConsolidation.Models {
  public class PackageVersion {
    public string Name { get;set; }
    public Version Version { get; set; }
    public string TargetFramework { get; set; }

    public static PackageVersion From( [NotNull]PackageItem item ) {
      if ( string.IsNullOrWhiteSpace( item.Version ) ) {
        item.Version = "0.0.0.0";
      }

      if ( item.Version.Contains( "-" ) ) {
        item.Version = item.Version.Split( '-' )[0];
      }
      try {
        return new PackageVersion {
          Name = item.Name,
          Version = new Version( item.Version ),
          TargetFramework = item.TargetFramework ?? "net472"
        };
      }
      catch ( FormatException fe ) {
        Console.WriteLine( $"Couldn't parse {item.Version}");
      }

      return null;
    }

    public static PackageVersion From( [NotNull] AssemblyName name, [NotNull] string filename, string defaultFramework = "net472" ) {
      var targetFramework = defaultFramework;
      try {
        if ( Assembly.LoadFile( filename )
            .GetCustomAttribute( typeof(TargetFrameworkAttribute) ) is TargetFrameworkAttribute targetFrameworkAttribute
        ) {
          targetFramework = $"net{new FrameworkName( targetFrameworkAttribute.FrameworkName ).Version}";
        }
      }
      catch ( FileNotFoundException fnex ) {
        return null;
      }
      catch ( TypeLoadException tle ) {
        return null;
      }

      return new PackageVersion {
        Name = name.Name,
        Version = name.Version,
        TargetFramework = targetFramework
      };
    }

    public static PackageItem As( PackageVersion version ) {
      return new PackageItem {
        Name = version.Name,
        Version = version.Version.ToString(),
        TargetFramework = version.TargetFramework
      };
    }
  }

  public enum PackageSourceType {
    File,
    Directory
  }

  public abstract class PackageSource {
    public abstract PackageSourceType SourceType { get; }

    public IList<PackageVersion> Packages { get; } = new List<PackageVersion>();

    public abstract void Collect();

    public virtual bool Update( IEnumerable<PackageVersion> packagesToUpdate ) {
      return false;
    }
  }

  public class ConfigurationPackageSource : PackageSource {
    private readonly string _fileName;

    public ConfigurationPackageSource( string fileName ) {
      _fileName = fileName;
    }

    public override PackageSourceType SourceType => PackageSourceType.File;

    public override void Collect() {
      Packages.Clear();
      var package = XmlConverter.ReadAs<PackageInformation>( _fileName );
      foreach ( var item in package.Packages ) {
        Packages.Add( PackageVersion.From( item ));
      }
    }

    public override bool Update( IEnumerable<PackageVersion> packagesToUpdate ) {
      var didUpdate = false;
      var packageInformation = new PackageInformation();
      var updateDict = packagesToUpdate.ToDictionary( p => p.Name, p => p, StringComparer.OrdinalIgnoreCase );
      foreach ( var item in Packages ) {
        if ( updateDict.TryGetValue( item.Name, out var updatedPackageVersion ) ) {
          packageInformation.Packages.Add( PackageVersion.As( updatedPackageVersion ) );
          didUpdate = true;
          continue;
        }
        packageInformation.Packages.Add( PackageVersion.As( item ) );
      }

      if ( didUpdate ) {
        XmlConverter.Write( _fileName, packageInformation );
        return true;
      }

      return false;
    }
  }

  public class DirectoryPackageSource : PackageSource {
    private readonly string _directory;

    public DirectoryPackageSource( string directory ) {
      _directory = directory;
    }

    public override PackageSourceType SourceType => PackageSourceType.Directory;

    public override void Collect() {
      Packages.Clear();
      var files = Directory.GetFiles( _directory, "*.dll", SearchOption.AllDirectories );
      foreach ( var file in files ) {
        try {
          var assemblyName = AssemblyName.GetAssemblyName( file );
          var packageVersion = PackageVersion.From( assemblyName, file );
          if ( packageVersion == null ) {
            continue;
          }
          Packages.Add( packageVersion );
        }
        catch ( BadImageFormatException bex ) {
          Console.WriteLine( $"{file} couldn't be loaded as it has the wrong image format");
        }
      }
    }
  }
}
