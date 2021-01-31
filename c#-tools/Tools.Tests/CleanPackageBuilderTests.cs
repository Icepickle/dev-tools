using System;
using System.IO;
using System.Linq;
using Cleaner;
using NLog.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Tools.Tests {
  [TestFixture]
  public class CleanPackageBuilderTests {
    [Test]
    public void ThrowsInvalidOperationWhenMalConfigured() {
      Assert.Throws<InvalidOperationException>( () => 
        new CleanPackageBuilder( string.Empty ).Build() );
      Assert.Throws<InvalidOperationException>( () =>
        new CleanPackageBuilder( TestContext.CurrentContext.TestDirectory ).Build() );
    }

    [Test]
    public void ReturnsFalseWhenErrorOccured() {
      var cleaner = new CleanPackageBuilder( Path.Combine( TestContext.CurrentContext.WorkDirectory, Randomizer.CreateRandomizer().GetString(10 )) )
        .Target( "bin", "debug" )
        .Build();
      Assert.IsFalse( cleaner.Execute() );
    }

    [Test]
    public void EnsureCleanerCleansSingleTarget() {
      var root = CreateDirectoryStructure();
      var cleaner = new CleanPackageBuilder( root )
        .Target( "bin" )
        .With( null )
        .Build();
      Assert.IsTrue( cleaner.Execute() );
      Assert.AreEqual( 0, Directory.GetDirectories( root, "bin", SearchOption.AllDirectories ).ToArray().Length );
      Assert.Greater( Directory.GetDirectories( root, "obj", SearchOption.AllDirectories ).ToArray().Length, 0 );
      Directory.Delete( root, true );
    }

    [Test]
    public void EnsureCleanerCleansAllTarget() {
      var root = CreateDirectoryStructure();
      var cleaner = new CleanPackageBuilder( root )
        .Target( "bin", "obj" )
        .Build();
      Assert.IsTrue( cleaner.Execute() );
      Assert.AreEqual( 0, Directory.GetDirectories( root, "bin", SearchOption.AllDirectories ).ToArray().Length );
      Assert.AreEqual( 0, Directory.GetDirectories( root, "obj", SearchOption.AllDirectories ).ToArray().Length );
      Directory.Delete( root, true );
    }

    [Test]
    public void EnsureCleanerRespectsFullNameExclude() {
      var root = CreateDirectoryStructure();
      var cleaner = new CleanPackageBuilder( root )
        .Target( "bin", "obj" )
        .Exclude( root )
        .Build();
      Assert.IsTrue( cleaner.Execute() );
      Assert.Greater( Directory.GetDirectories( root, "bin", SearchOption.AllDirectories ).ToArray().Length, 0 );
      Assert.Greater( Directory.GetDirectories( root, "obj", SearchOption.AllDirectories ).ToArray().Length, 0 );
      Directory.Delete( root, true );
    }

    [Test]
    public void EnsureCleanerLocalNameExclude() {
      var root = CreateDirectoryStructure();
      var cleaner = new CleanPackageBuilder( root )
        .Target( "bin", "obj" )
        .Exclude( Path.GetFileName( root ) )
        .Build();
      Assert.IsTrue( cleaner.Execute() );
      Assert.Greater( Directory.GetDirectories( root, "bin", SearchOption.AllDirectories ).ToArray().Length, 0 );
      Assert.Greater( Directory.GetDirectories( root, "obj", SearchOption.AllDirectories ).ToArray().Length, 0 );
      Directory.Delete( root, true );
    }

    private string CreateDirectoryStructure( uint depth = 2 ) {
      var randomizer = Randomizer.CreateRandomizer();

      var current = TestContext.CurrentContext.WorkDirectory;
      var target = string.Empty;

      do {
        current = Path.Combine( current, randomizer.GetString( 10 ) );        
        Directory.CreateDirectory( current );
        if ( string.IsNullOrWhiteSpace( target ) ) {
          target = current;
        }
      } while ( --depth > 0 );

      Directory.CreateDirectory( Path.Combine( current, "bin" ) );
      Directory.CreateDirectory( Path.Combine( current, "obj" ) );
      return target;
    }
  }
}