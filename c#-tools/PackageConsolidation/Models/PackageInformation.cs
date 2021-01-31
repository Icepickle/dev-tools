using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PackageConsolidation.Models {
  [XmlRoot("packages", Namespace = "")]
  public class PackageInformation {
    [XmlElement("package", Namespace = "")]
    public List<PackageItem> Packages { get; } = new List<PackageItem>();
  }

  public class PackageItem {
    [XmlAttribute("id")]
    public string Name { get; set; }
    [XmlAttribute("version")]
    public string Version { get; set; }
    [XmlAttribute("targetFramework")]
    public string TargetFramework { get; set; }
  }
}
