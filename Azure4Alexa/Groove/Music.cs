using AlexaSkillsKit.Speechlet;
using Azure4Alexa.Alexa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Azure4Alexa.Groove
{
    public class Music
    {
        // When making un-authenticated API calls to Groove Music you must provide
        // an app token. The function GetOrRefreshAppToken handles getting and
        // refreshing the token based on your app's appClientId and
        // appClientSecret settings, which are provided to you after you register
        // your application at the Application Registration Portal located at
        // https://apps.dev.microsoft.com

        // Currently, our skill does not make un-authenticated API calls: all calls
        // are made in the context of an end-user.  When providing an end-user
        // token to the Groove Music API, you are not required to provide
        // an app token.  

        // Therefore, the code below is currently un-used, but you may find it useful 
        // as you extend this starter kit to your own needs 

        // global app settings
        //private static string appTokenUri = "https://login.live.com/accesstoken.srf";
        //private static string appScope = "app.music.xboxlive.com";
        //private static string appGrantType = "client_credentials";

        // these are your app specific settings 
        // in the real-world, you wouldn't put these settings into code
        // but are here for this simple-to-learn example
        //private static string appClientId = "";
        //private static string appClientSecret = "";

        // we store appToken in escaped and unescaped form here
        // along with the expiration in UTC

        // appToken is reusable across requests from different users

        //private static string appToken = "";
        //private static string appTokenUnescaped = "";

        //public static DateTimeOffset appTokenExpiration;

        // Get an app-wide token that can be used for un-authenticated calls
        // to the Groove Music API

        //public static async void GetOrRefreshAppToken(HttpClient httpClient)
        //{
        //    if (appToken == "" || appTokenExpiration < DateTimeOffset.UtcNow)
        //    {
        //        string httpResultString = "";

        //        // Connect to app token API Endpoint

        //        httpClient.DefaultRequestHeaders.Clear();
        //        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

        //        var postData = new List<KeyValuePair<string, string>>();
        //        postData.Add(new KeyValuePair<string, string>("grant_type", appGrantType));
        //        postData.Add(new KeyValuePair<string, string>("client_id", appClientId));
        //        postData.Add(new KeyValuePair<string, string>("client_secret", appClientSecret));
        //        postData.Add(new KeyValuePair<string, string>("scope", appScope));
        //        HttpContent content = new FormUrlEncodedContent(postData);


        //        var httpResponseMessage = await httpClient.PostAsync(appTokenUri, content);
        //        if (httpResponseMessage.IsSuccessStatusCode)
        //        {
        //            httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
        //            dynamic resultObject = JObject.Parse(httpResultString);
        //            appTokenUnescaped = resultObject.access_token;
        //            double appTokenExpiresIn = resultObject.expires_in;
        //            appTokenExpiration = DateTimeOffset.UtcNow.AddSeconds(appTokenExpiresIn);
        //            appToken = HttpUtility.UrlEncode(appTokenUnescaped);
        //        }

        //        httpResponseMessage.Dispose();

        //    }

        //}

        // URL prefix for the Microsoft Groove Music API endpoint
        private static string msgBaseUri = "https://api.media.microsoft.com/1/content/";
        private static string msgSearchUri = msgBaseUri + "music/search?q=";

        // receive the request from AlexaSpeechletAsync, parse the slot (parameters) provided by the end user,
        // and route it to the correct function to find and enqueue user's song or album.

        // note that playlists are handled in a different function

        public static async Task<SpeechletResponse>
            PlayGrooveMusic(Session session, HttpClient httpClient, IntentRequest intentRequest)
        {
            AlexaUtils.SimpleIntentResponse simpleIntentResponse = new AlexaUtils.SimpleIntentResponse();

            AlexaSkillsKit.Slu.Slot objectByArtistName;
            AlexaSkillsKit.Slu.Slot objectName;
            AlexaSkillsKit.Slu.Slot objectType;

            intentRequest.Intent.Slots.TryGetValue("object.byArtist.name", out objectByArtistName);
            intentRequest.Intent.Slots.TryGetValue("object.name", out objectName);
            intentRequest.Intent.Slots.TryGetValue("object.type", out objectType);

            if (objectType.Value.ToString() == "song")
            {
                simpleIntentResponse = await SearchGrooveSong(objectName.Value, objectByArtistName.Value, session, httpClient);
            }
            else if (objectType.Value.ToString() == "album")
            {
                simpleIntentResponse = await SearchGrooveAlbum(objectName.Value, objectByArtistName.Value, session, httpClient);
            }

            return AlexaUtils.BuildSpeechletResponse(simpleIntentResponse, true);

        }

        // find the song the user has requested and generate the intent response needed to begin playing it. 

        public static async Task<AlexaUtils.SimpleIntentResponse> SearchGrooveSong(string songName, string artistName, Session session, HttpClient httpClient)
        {
            string msgSongId = "";
            string msgStreamUri = "";

            // first, search for the song based on songName and artistName

            try
            {
                var requestUri = msgSearchUri + Uri.EscapeUriString(songName + "+" + artistName) + "&" +
                    "filters=tracks+artists&" + "source=collection";
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", session.User.AccessToken.ToString());
                var httpResponseMessage = await httpClient.GetAsync(requestUri);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
                    dynamic resultObject = JObject.Parse(httpResultString);
                    msgSongId = resultObject.Tracks.Items[0].Id; // assume the first result is the correct one

                }
            }
            catch
            {
                return null;
            }

            // now, we get the stream URL for the song based on its ID (msgId)

            msgStreamUri = await GetStreamUrl(msgSongId, httpClient, session);

            var stringToRead = "You asked Groove to play the song " + songName;

            if (!string.IsNullOrEmpty(artistName))
            {
                stringToRead += " by " + artistName;
            }

            AlexaUtils.SimpleIntentResponse simpleIntentResponse = new AlexaUtils.SimpleIntentResponse()
            {
                cardText = stringToRead,
                ssmlString = AlexaUtils.AddSpeakTagsAndClean(stringToRead),
                largeImage = "groove.png",
                smallImage = "groove.png",
                musicUrl = msgStreamUri,
                msgId = msgSongId,
            };

            return simpleIntentResponse;

        }

        // find the album the user has requested and generate the intent response needed to begin playing it. 

        public static async Task<AlexaUtils.SimpleIntentResponse> SearchGrooveAlbum(string albumName, string artistName, Session session, HttpClient httpClient)
        {
            string msgAlbumId = "";
            string msgStreamUri = "";

            // first, search for the album based on albumName and artistName
            // return the value into msgAlbumId

            try
            {
                var requestUri = msgSearchUri + albumName + " " + artistName + "&" +
                    "filters=albums+artists&" + "source=collection";
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", session.User.AccessToken.ToString());
                var httpResponseMessage = await httpClient.GetAsync(requestUri);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
                    dynamic resultObject = JObject.Parse(httpResultString);
                    msgAlbumId = resultObject.Albums.Items[0].Id;  // assume the first album returned (Items[0]) is the correct one
                    msgStreamUri = await GetSongUrlByAlbumIdAndIndex(msgAlbumId, 0, httpClient, session, "album");  // get the stream URL for the first track so we can queue it up 
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

            var stringToRead = "You asked Groove to play the album " + albumName;

            if (!string.IsNullOrEmpty(artistName))
            {
                stringToRead += " by " + artistName;
            }

            AlexaUtils.SimpleIntentResponse simpleIntentResponse = new AlexaUtils.SimpleIntentResponse()
            {
                cardText = stringToRead,
                ssmlString = AlexaUtils.AddSpeakTagsAndClean(stringToRead),
                largeImage = "groove.png",
                smallImage = "groove.png",
                musicUrl = msgStreamUri,

                // When playing an audio stream, you provide Alexa an audio token
                // you generate.  Alexa provides the token back to you in subsequent
                // audio requests: i.e. enqueue the next song to play.

                // The format of the token we generate is:
                // <MusicId>?<Track> where <MusicId> is the unique identifier provided by
                // the Groove API for the individual song, album, or playlist the user wants to hear, and
                // <Track> is the index of the song in the album or playlist.  If there is no <Track>
                // value, then the user has asked to only play a single song

                msgId = msgAlbumId + "?" + "0",
            };
            return simpleIntentResponse;
        }


        // Playlists have names in Groove Music. 
        // In this first version, users will have to refer to the playlist they want by number: 0, 1, 2, 3, etc.
        // For example: "Alexa, ask Groove to play playlist 1".
        // The number corresponds to the playlists' position in the Groove UI which also corresponds to the playlist's
        // position when we make an API call to get all playlsits. 

        // We're using Alexa's pre-built slot for numbers to translate spoken numbers to their text equivalent

        // Parse the number the user has spoken, call the Groove API to get the playlist that corresponds to the index (number)
        // and do the work needed to start playing the playlist from the first song.

        public static async Task<SpeechletResponse> PlayGroovePlaylist(Session session, HttpClient httpClient, IntentRequest intentRequest)
        {
            AlexaSkillsKit.Slu.Slot playlistNumber;

            intentRequest.Intent.Slots.TryGetValue("PlaylistNumber", out playlistNumber);

            string msgStreamUri = "";
            string msgPlaylistId = "";
            string playlistName = "";
            int msgPlaylistIndex = int.Parse(playlistNumber.Value);
            try
            {
                var requestUri = msgBaseUri + "/music/collection/playlists/browse?" + "source=collection";
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", session.User.AccessToken.ToString());
                var httpResponseMessage = await httpClient.GetAsync(requestUri);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
                    Debug.WriteLine("response is " + httpResultString);
                    dynamic resultObject = JObject.Parse(httpResultString);

                    msgPlaylistId = resultObject.Playlists.Items[msgPlaylistIndex].Id;
                    playlistName = resultObject.Playlists.Items[msgPlaylistIndex].Name;
                    msgStreamUri = await GetSongUrlByAlbumIdAndIndex(msgPlaylistId, 0, httpClient, session, "playlist");
                }

            }
            catch
            {
                return null;
            }

            var stringToRead = "You asked Groove to play playlist " + playlistNumber.Value +
                ", also known as " + playlistName;


            AlexaUtils.SimpleIntentResponse simpleIntentResponse = new AlexaUtils.SimpleIntentResponse()
            {
                cardText = stringToRead,
                ssmlString = AlexaUtils.AddSpeakTagsAndClean(stringToRead),
                largeImage = "groove.png",
                smallImage = "groove.png",
                musicUrl = msgStreamUri,

                // When playing an audio stream, you provide Alexa an audio token
                // you generate.  Alexa provides the token back to you in subsequent
                // audio requests: i.e. enqueue the next song to play.

                // The format of the token we generate is:
                // <MusicId>?<Track> where <MusicId> is the unique identifier provided by
                // the Groove API for the individual song, album, or playlist the user wants to hear, and
                // <Track> is the index of the song in the album or playlist.  If there is no <Track>
                // value, then the user has asked to only play a single song

                msgId = msgPlaylistId + "?" + "0",
            };

            return AlexaUtils.BuildSpeechletResponse(simpleIntentResponse, true);
        }

        // pass the ID of an album or playlist and the index of the song whose stream URL is wanted
        // then return the stream URL

        public static async Task<string> GetSongUrlByAlbumIdAndIndex(string msgAlbumId, int msgSongIndex, HttpClient httpClient, Session session, string browseType)
        {
            string msgStreamUri = "";
            try
            {
                var requestUri = msgBaseUri + msgAlbumId + "/collection/" + browseType + "/tracks/browse";
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", session.User.AccessToken.ToString());
                var httpResponseMessage = await httpClient.GetAsync(requestUri);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
                    dynamic resultObject = JObject.Parse(httpResultString);

                    // get the number of tracks in the album, we might have a single or only one track 
                    // from the whole thing

                    int msgTrackCount;
                    string nuts = httpResultString;
                    if (browseType == "album")
                    {
                        msgTrackCount = resultObject.Albums.Items[0].Tracks.TotalItemCount;
                    }
                    else
                    {
                        msgTrackCount = resultObject.Playlists.Items[0].Tracks.TotalItemCount;
                    }

                    // if we're requesting an index that's higher than the total number of tracks
                    // return an empty string.  we're all the way through the album

                    if (msgSongIndex >= msgTrackCount)
                    {
                        return "";
                    }
                    else
                    {
                        // get the stream URL for the track at the indicated msgSongIndex so we can queue it up or make it the first in the queue
                        string trackId = "";
                        if (browseType == "album")
                        {
                            trackId = resultObject.Albums.Items[0].Tracks.Items[msgSongIndex].Id;
                        }
                        else
                        {
                            trackId = resultObject.Playlists.Items[0].Tracks.Items[msgSongIndex].Id;
                        }
                        msgStreamUri = await GetStreamUrl(trackId, httpClient, session);
                    }
                }
            }
            catch
            {
                return null;
            }

            return msgStreamUri;
        }

        // given the ID of a song, return its stream URL

        public static async Task<string> GetStreamUrl(string msgSongId, HttpClient httpClient, Session session)
        {
            // now, we get the stream URL for the track/song identified by msgId
            // stream URLs expire, so you'll want to get the URL just beore you play/enqueue it

            string msgStreamUri = "";

            try
            {
                var requestUri = msgBaseUri + msgSongId + "/stream?" +
                    "clientInstanceId=someRandomPerClientStringsomeRandomPerClientString";
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", session.User.AccessToken.ToString());
                var httpResponseMessage = await httpClient.GetAsync(requestUri);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
                    dynamic resultObject = JObject.Parse(httpResultString);
                    string resultUri = resultObject.Url;
                    if (resultUri.StartsWith("http://"))
                    {
                        msgStreamUri = resultUri.Replace("http://", "https://");
                    }
                    else
                    {
                        msgStreamUri = resultUri;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

            return msgStreamUri;
        }

        // add the next track in the album or playlist to the play queue
        // after Alexa tells us the current song is almost finished
        // in reality this occurs quickly after the current song has started!

        public static async Task<SpeechletResponse>
            EnqueueGrooveMusic(Context context, HttpClient httpClient, String action)
        {
            Session session = new Session()
            {
                User = new User()
                {
                    AccessToken = context.System.User.AccessToken
                }
            };

            // When playing an audio stream, you provide Alexa an audio token
            // you generate.  Alexa provides the token back to you in subsequent
            // audio requests: i.e. enqueue the next song to play.

            // The format of the token we generate is:
            // <MusicId>?<Track> where <MusicId> is the unique identifier provided by
            // the Groove API for the individual song, album, or playlist the user wants to hear, and
            // <Track> is the index of the song in the album or playlist.  If there is no <Track>
            // value, then the user has asked to only play a single song

            string[] items = context.AudioPlayer.Token.Split('?');

            if (items.Length != 2)
            {
                return null;
            }

            // if there are two items in the items[] array then an album or playlist
            // is being played.  find out if it's a playlist or album, get the stream url,
            // then enqueue it, and increment the index 

            string msgAlbumId = items[0];
            int msgAlbumIndex = int.Parse(items[1]);
            msgAlbumIndex++;

            string msgStreamUri = "";

            if (items[0].Contains("playlist"))
            {
                msgStreamUri = await GetSongUrlByAlbumIdAndIndex(msgAlbumId, msgAlbumIndex,
                   httpClient, session, "playlist");
            }
            else // it's an album that's playing
            {
                msgStreamUri = await GetSongUrlByAlbumIdAndIndex(msgAlbumId, msgAlbumIndex,
                   httpClient, session, "album");
            }

            if (msgStreamUri == "")
            {
                return null;
            }

            AlexaUtils.SimpleIntentResponse simpleIntentResponse = new AlexaUtils.SimpleIntentResponse()
            {
                musicUrl = msgStreamUri,
                msgId = msgAlbumId + "?" + msgAlbumIndex,
                musicAction = action
            };

            return AlexaUtils.BuildSpeechletResponse(simpleIntentResponse, true);
        }


        // todo: get album image on-demand based on music ID

        //public static async Task<string> GetImageUrl(string id, HttpClient httpClient)
        //{

        //}


    }
}