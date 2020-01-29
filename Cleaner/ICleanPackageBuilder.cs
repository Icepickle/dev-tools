using System.Diagnostics.CodeAnalysis;
using NLog;

namespace Cleaner {
  public interface ICleanPackageBuilder {
    ICleanPackageBuilder Target( [NotNull]params string[] directoryNames );
    ICleanPackageBuilder Exclude( [NotNull]params string[] toExclude );
    ICleanPackageBuilder With( [NotNull]ILogger logger );
    ICleaner Build();
  }
}