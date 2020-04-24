using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Telegram {
  public class Bot {
    private const string Value = "wheather";
    private RestClient client;
    private bool isRunning = false;
    private long lastUpdate = 0;

    public Bot (string token, long lastUpdate = 0) {
      this.lastUpdate = lastUpdate;
      var baseUrl = $"https://api.telegram.org/bot{token}/";
      client = new RestClient (baseUrl);
      client.UseNewtonsoftJson ();
    }

    public async Task Run () {
      isRunning = true;
      Console.WriteLine ("Bot start work");
      var botUser = await GetMe ();
      Console.WriteLine ("-- Bot info:");
      Print (botUser);
      Console.WriteLine ("--");

      while (isRunning) {
        await MakeProcessIteration ();
        Thread.Sleep (1000);
      }
      Console.WriteLine ("Bot stop work");
    }

    private async Task<Boolean> MakeProcessIteration () {
      try {
        var updates = await GetUpdates (lastUpdate);
        lastUpdate = GetLastUpdateId (updates, lastUpdate) + 1;
        ProcessUpdates (updates);
        return true;
      } catch (Exception e) {
        Console.WriteLine ("!!! Failure: ");
        Console.WriteLine (e);
        Console.WriteLine ("!!! end failure");
        return false;
      }
    }

    public void Stop () {
      isRunning = false;
    }

    private async Task<List<UpdateJson>> GetUpdates (long offset) {
      var argOffset = offset > 0 ? $"&offset={offset}" : "";
      var path = $"getUpdates?limit=10{argOffset}";
      var request = new RestRequest (path, DataFormat.Json);
      request.AddHeader ("content-type", "application/json");
      var response = await client.GetAsync<ResponseJson<List<UpdateJson>>> (request);
      return response.result ?? new List<UpdateJson> ();
    }

    private long GetLastUpdateId (List<UpdateJson> items, long defaultValue) {
      if (items == null || items.Count == 0) {
        return defaultValue;
      } else {
        return items.Last ()?.update_id ?? defaultValue;
      }
    }

    private void ProcessUpdates (List<UpdateJson> items) {
      items?.ForEach (item => ProcessUpdates (item));
    }

    private void ProcessUpdates (UpdateJson item) {
      var owner = item.message?.from?.first_name ?? "<ufo>";
      var msg = item.message.text;
      var text = $"& {owner} \n=> {msg}\n";
      Console.WriteLine (text);

      ArgSendMessageJson args;
      var isWeatherRequest = msg?.ToLower ()?.Contains ("weather") ?? false;
      if (isWeatherRequest) args = getMessageForRequestLocation ();
      else args = getMessageForEcho ();
      args.chat_id = item.message.chat.id.ToString ();
      SendMessage (args);
    }

    private ArgSendMessageJson getMessageForRequestLocation () {
      var buttonLocation = new KeyboardButtonJson ();
      buttonLocation.text = "Send location";
      buttonLocation.request_location = true;

      var args = new ArgSendMessageJson ();
      args.text = "Please, send your location for take Weather";
      args.reply_markup = new ReplyMarkupJson ();
      args.reply_markup.keyboard = new List<List<KeyboardButtonJson>> () {
        new List<KeyboardButtonJson> () { buttonLocation }
      };
      return args;
    }

    private ArgSendMessageJson getMessageForEcho () {
      var args = new ArgSendMessageJson ();
      args.text = "message received";
      return args;
    }

    private void Print<T> (T value) {
      var text = JsonConvert.SerializeObject (value);
      Console.WriteLine (text);
    }

    private async Task<UserJson> GetMe () {
      var request = new RestRequest ("getMe", DataFormat.Json);
      var response = await client.GetAsync<ResponseJson<UserJson>> (request);
      return response.result ?? new UserJson ();
    }

    private async Task<MessageJson> SendMessage (ArgSendMessageJson data) {
      var request = new RestRequest ("sendMessage", DataFormat.Json);
      request.AddJsonBody (data);
      var response = await client.PostAsync<ResponseJson<MessageJson>> (request);
      return response.result ?? new MessageJson ();
    }

  }

  class ResponseJson<T> {
    public bool ok;
    public T result;
  }

  class UpdateJson {
    public long update_id;
    public MessageJson message;
    public MessageJson edited_message;
    public MessageJson channel_post;
    public MessageJson edited_channel_post;
    public InlineQueryJson inline_query;
  }

  class MessageJson {
    public long message_id;
    public UserJson from;
    public long date;
    public ChatJson chat;
    public UserJson forward_from;
    public ChatJson forward_from_chat;
    public string text;
  }

  class InlineQueryJson {
    public string id;
    public UserJson from;
    public LocationJson location;
    public string query;
    public string offset;
  }

  class UserJson {
    public long id = 0;
    public bool is_bot = false;
    public string username = "";
    public string first_name = "";
    public string last_name = "";
  }

  class ChatJson {
    public long id;
    public string type;
    public string title;
  }

  class LocationJson {
    public float longitude;
    public float latitude;
  }

  class ArgSendMessageJson {
    public string chat_id = "";
    public string text = "";
    public ReplyMarkupJson reply_markup;
  }

  class ReplyMarkupJson {
    public List<List<KeyboardButtonJson>> keyboard;
  }

  class KeyboardButtonJson {
    public string text;
    public bool? request_location = false;
  }
}