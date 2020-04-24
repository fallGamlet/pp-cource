using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Wheathers {
  public class WheatherManager {
    private readonly RestClient client;

    public WheatherManager () {
      client = new RestClient ("https://api.met.no");
      client.UseNewtonsoftJson ();
    }

    public async Task<Wheather> getWheatherAsync (double latitude, double longitude) {
      try {
        var culture = new CultureInfo ("en-US", false);
        var latitudeStr = latitude.ToString ("0.00000", culture);
        var longitudeStr = longitude.ToString ("0.00000", culture);
        var request = new RestRequest ($"/weatherapi/locationforecast/1.9/.json?lat={latitudeStr}&lon={longitudeStr}", DataFormat.Json);
        // var response = client.Get(request);
        // Console.WriteLine(response);

        var resultJson = await client.GetAsync<WheatherResultJson> (request);

        if (resultJson == null) {
          return new Wheather (new List<TimeTemperature> ());
        } else {
          return MapWheather (resultJson);
        }
      } catch (Exception e) {
        Console.WriteLine (e);
        return new Wheather (new List<TimeTemperature> ());
      }
    }

    private Wheather MapWheather (WheatherResultJson json) {
      var items = json?.product?.time ?
        .Where (item => item.location?.temperature != null) ?
        .Select (MapTimeTemperature) ??
        new List<TimeTemperature> ();

      return new Wheather (items);
    }

    private TimeTemperature MapTimeTemperature (TimeJson json) {
      return new TimeTemperature (
        json.from?? "",
        json.to?? "",
        MapTemperature (json.location.temperature)
      );
    }

    private Temperature MapTemperature (TemperatureJson json) {
      return new Temperature (
        json.value?? "",
        json.unit?? ""
      );
    }
  }

  public class Wheather {
    public IEnumerable<TimeTemperature> Items { get; private set; }

    public Wheather (IEnumerable<TimeTemperature> items) {
      this.Items = items;
    }

    public override string ToString () {
      var builder = new StringBuilder ();
      Items.All (item => {
        builder.AppendLine (item.ToString ());
        return true;
      });
      return builder.ToString ();
    }
  }

  public class TimeTemperature {
    public string From { get; private set; }
    public string To { get; private set; }
    public Temperature Value { get; private set; }

    public TimeTemperature (string from, string to, Temperature temperature) {
      this.From = from;
      this.To = to;
      this.Value = temperature;
    }

    public override string ToString () {
      return $"(from: {From}, to: {To}, value: {Value})";
    }
  }

  public class Temperature {
    public string Value { get; private set; }
    public string Unit { get; private set; }

    public Temperature (string value, string unit) {
      this.Value = value;
      this.Unit = unit;
    }

    public override string ToString () {
      return $"(value: {Value}, unit: {Unit})";
    }
  }

  internal class WheatherResultJson {
    public ProductJson product;
  }
  internal class ProductJson {
    public List<TimeJson> time;
  }

  internal class TimeJson {
    public string from; // "2020-03-27T16:00:00Z"
    public string to; // "2020-03-27T16:00:00Z"
    public LocationJson location;
  }

  internal class LocationJson {
    public TemperatureJson temperature;
  }

  internal class TemperatureJson {
    public string id;
    public string value;
    public string unit;
  }
}