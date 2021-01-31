using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PackageConsolidation.Models;

namespace PackageConsolidation {
  public class ProjectUpdater {
    private readonly string _sourceDirectory;
    private readonly string _contribDirectory;
    private readonly string _contribUpstreamDirectory;

    public ProjectUpdater( string sourceDirectory, string contribDirectory, string upstreamDirectory ) {
      _sourceDirectory = sourceDirectory;
      _contribDirectory = contribDirectory;
      _contribUpstreamDirectory = upstreamDirectory;
    }

    private PackageSource CollectFromDirectory( string directoryName ) {
      var collector = new DirectoryPackageSource( directoryName );
      collector.Collect();
      return collector;
    }

    private PackageSource CollectFromPackageSource( string packageSourceName ) {
      var collector = new ConfigurationPackageSource( packageSourceName );
      collector.Collect();
      return collector;
    }

    private IEnumerable<PackageSource> RetrieveRequiredPackages( string sourceDirectory ) {
      var packageFiles = Directory.GetFiles( sourceDirectory, "packages.config", SearchOption.AllDirectories );
      foreach ( var packageFile in packageFiles ) {
        yield return CollectFromPackageSource( packageFile );
      }
    }

    public void Consolidate() {
      var currentContrib = CollectFromDirectory( _contribDirectory );
      var upgradeContrib = CollectFromDirectory( _contribUpstreamDirectory );
      var packages = RetrieveRequiredPackages( _sourceDirectory ).ToList();
    }
  }
}
