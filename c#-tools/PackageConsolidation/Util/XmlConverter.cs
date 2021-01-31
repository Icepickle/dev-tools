using System.IO;
using System.Xml.Serialization;

namespace PackageConsolidation.Util {
  public static class XmlConverter {
    public static T ReadAs<T>( string filename ) {
      using var fs = new FileStream( filename, FileMode.Open, FileAccess.Read );
      var xs = new XmlSerializer( typeof(T) );
      return (T) xs.Deserialize( fs );
    }

    public static void Write( string filename, object content ) {
      using var fs = new FileStream( filename, FileMode.OpenOrCreate, FileAccess.Write );
      var type = content.GetType();
      var xs = new XmlSerializer( type, "" );
      xs.Serialize(fs, content );
    }
  }
}
