using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PackageConsolidation;

namespace Tools.Tests {
  [TestFixture]
  public class PackageConsolidationTests {
    [Test]
    public void CollectingPackagesFromExistingSources_Reads_AllSourcesProperly() {
      var projectUpdater = new ProjectUpdater( @"C:\dev\viessmann", @"C:\dev\viessmann\contrib", @"C:\Program Files (x86)\Configit Quote 11.0\Services\Configit.Quote.Web\bin" );
      projectUpdater.Consolidate();
    }
  }
}
