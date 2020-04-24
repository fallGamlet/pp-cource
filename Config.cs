using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration {
  public class Config {
    private Dictionary<string, string> dict;
    public Config (string filename) {
      dict = new Dictionary<string, string> ();
      File.ReadAllLines (filename)
        .Where (value => value?.Length > 0)
        .Select (value => value.Split ("=", 2))
        .Where (value => value.Count () == 2)
        .ToList ()
        .ForEach (value => dict.Add (value[0], value[1]));
    }

    public string getValue (string key) {
      return dict.GetValueOrDefault (key, "");
    }
  }
}