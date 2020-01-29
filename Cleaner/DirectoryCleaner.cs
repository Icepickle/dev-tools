using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace Cleaner {
  public class DirectoryCleaner : ICleaner {
    private readonly string _rootDirectory;
    private readonly ISet<string> _targets;
    private readonly ISet<string> _excludedTargets;
    private readonly ILogger _logger;

    private void Log( string message, LogLevel level = null, params object[] args ) {
      _logger?.Log( level ?? LogLevel.Info, message, args );
    }

    public DirectoryCleaner( string rootDirectory, IEnumerable<string> targets, IEnumerable<string> excludedTargets,
      ILogger logger ) {
      _rootDirectory = rootDirectory;
      _targets = new HashSet<string>( targets, StringComparer.InvariantCultureIgnoreCase );
      _excludedTargets = new HashSet<string>( excludedTargets, StringComparer.InvariantCultureIgnoreCase  );
      _logger = logger;
    }

    private void CleanDirectory( string directory ) {
      var localName = Path.GetFileName( directory );
      if ( _excludedTargets.Contains( directory ) || _excludedTargets.Contains( localName ) ) {
        Log( $"Skipping {directory}"  );
        return;
      }
      Log( $"Cleaning {directory}" );
      if ( _targets.Contains( directory ) || _targets.Contains( localName ) ) {
        Log( $"Deleting {directory}" );
        Directory.Delete( directory, true );
        return;
      }
      var directories = Directory.GetDirectories( directory );
      foreach ( var subDirectory in directories ) {
        CleanDirectory( subDirectory );
      }
    }
    
    public bool Execute() {
      try {
        CleanDirectory( _rootDirectory );
        return true;
      }
      catch ( Exception ex ) {
        Log( ex.ToString(), LogLevel.Error );
        return false;
      }
    }
  }
}