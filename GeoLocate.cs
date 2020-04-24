using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace GeoLocate {
  public class GeoLocation {
    private readonly RestClient client;
    private readonly RestRequest request;
    public GeoLocation () {
      client = new RestClient ("https://location.services.mozilla.com");
      client.UseNewtonsoftJson ();
      request = new RestRequest ("/v1/geolocate?key=test", DataFormat.Json);
    }

    public async Task<Location> GetAsync () {
      var json = await client.GetAsync<GeoPointJson> (request);

      if (json == null) json = new GeoPointJson ();
      if (json.location == null) json.location = new LocationJson ();

      return new Location (
        json.location.lat,
        json.location.lng,
        json.accuracy
      );
    }
  }

  public class Location {
    public double Lat { get; private set; }
    public double Lng { get; private set; }
    public double Accuracy { get; private set; }

    public Location (double lat, double lng, double accuracy) {
      this.Lat = lat;
      this.Lng = lng;
      this.Accuracy = accuracy;
    }

    override public string ToString () {
      return $"{{lat: {Lat}, lng: {Lng}, accuracy: {Accuracy}}}";
    }
  }

  internal class GeoPointJson {
    public LocationJson location;
    public double accuracy;
  }
  internal class LocationJson {
    public double lat;
    public double lng;
  }

}