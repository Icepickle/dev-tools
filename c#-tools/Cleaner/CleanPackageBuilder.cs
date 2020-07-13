using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Cleaner {
  public class CleanPackageBuilder : ICleanPackageBuilder {
    private readonly string _rootDirectory;
    private readonly ISet<string> _target = new HashSet<string>();
    private readonly ISet<string> _excludes = new HashSet<string>();
    private ILogger _logger = LogManager.GetCurrentClassLogger();
    
    public CleanPackageBuilder( string rootDirectory ) {
      _rootDirectory = rootDirectory;
    }
    
    public ICleanPackageBuilder Target( params string[] directoryNames ) {
      foreach ( var directory in directoryNames ) {
        _target.Add( directory );
      }

      return this;
    }

    public ICleanPackageBuilder Exclude( params string[] toExclude ) {
      foreach ( var exclude in toExclude ) {
        _excludes.Add( exclude );
      }

      return this;
    }

    public ICleanPackageBuilder With( ILogger logger ) {
      _logger = logger;
      return this;
    }

    public ICleaner Build() {
      if ( string.IsNullOrWhiteSpace( _rootDirectory ) ) {
        throw new InvalidOperationException( $"{_rootDirectory} isn't a valid target directory" );
      }

      if ( !_target.Any() ) {
        throw new InvalidOperationException( $"No targets set to clean {_rootDirectory}" );
      }
      return new DirectoryCleaner( _rootDirectory, _target, _excludes, _logger );
    }
  }
}