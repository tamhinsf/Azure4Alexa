## Connect Alexa to Microsoft Groove Music

* “Alexa, ask Groove to play the album Gold by Abba"
* "Alexa, ask Groove to play the song Dancing Queen by Abba"
* "Alexa, ask Groove to play playlist 1"

Let's connect your Echo, Dot Echo, and Tap to Microsoft Groove and your OneDrive Music!  We'll make combined use of the Groove REST API, Alexa's Account Linking framework, and Microsoft Account. When you're done, you'll be able to ask Alexa to play OneDrive hosted songs, albums, and playlists from your Microsoft Groove Music account.

Special Note: We've created a forked version of the awesome [AlexaSkillsKit.NET](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET)  that implements Alexa's support for audio streaming.  Our starter kit now includes and references this forked version, which is backwards compatible with the Outlook Mail and Calendar skills we've also bundled.  

##### Supported Groove Music Sources

OneDrive hosted songs, albums, and playlists.  Specifically, music from your OneDrive account.

Groove Music Pass streams (songs and albums) don't work at the moment.  We're working on it!

### Alexa, Authentication, and Skills Linking

Here's a recap of what we mentioned in the README.md in the root of this project.

Alexa provides an OAuth 2.0-based Account Linking framework that also supports token refreshes(!). You'll enter all of the usual OAuth 2.0 client configuration settings into the Amazon Developer Console: login URL, client ID, application secret, scopes, etc. The re-direct destination for your OAuth 2.0 flow will be an Alexa server, which will also be responsible for storing and refreshing your user's access token. When users add your Custom Skill using the companion app on their iOS or Android device, they'll be able to link their account to your skill: specifically, the companion app will launch an embedded browser that sends them to the login URL you identified. After that, anytime a user invokes your skill, Alexa will send the user's access token to your service so you can make use of it in your code.

Microsoft enables you to create applications that connect to Groove Music using OAuth 2.0 and the Groove Music REST API.   

To help you understand how you can use all these components together, we've included this Microsoft Groove Music skill in the Azure4Alexa project.  All you have to do is configure it, and re-deploy your project to Azure!  

### Prerequisites

Before you can configure and deploy the Microsoft Groove Music Custom Skill, you’ll need to successfully complete the steps described in the [README.md](../README.md) file stored in the root of this project.  

This means you have already:

- Registered as an Alexa Custom Skills developer
- Deployed the Azure4Alexa project to an Azure environment
- Registered the sample Custom Skill included with Azure4Alexa
- Updated and re-deployed Azure4Alexa with the Alexa-provided Application Id
- Tested Azure4Alexa and validated that you can ask Alexa if there is a Good Service on the Tube

If yes, it’s now time to get started!

### Register with Microsoft

You'll need to sign up as a Microsoft developer and register an application with Microsoft.  The application credentials you receive and generate will then be used by your Custom Skill to access a user's inbox.

* Sign in to the Microsoft Application Registration Portal.  Complete the process to become a registered developer, if you haven't already.
  * https://apps.dev.microsoft.com.  
* Follow the directions on this page to setup an application that uses the Groove API
  * https://docs.microsoft.com/groove/getting-started 
  * Make sure you complete these sections
    * Subscribe to the Groove API on Microsoft Developer Center
    * Register and associate a Microsoft Account application
      * Create an application
        * Copy and paste the Application ID and Application / Client Secret provided and generated for you.  You will need this in order to setup your Alexa Custom Skill to connect to the Groove API.
      * Allow this application to access Groove Music API
  * You don't have to complete the steps in the section "Getting an authentication token" and the sections below it

### Update your Alexa Custom Skills settings

##### Enable Account Linking

Now, you’ll update your existing Custom Skill to support Account Linking.  

* Open a new browser tab or window.
* Sign in to the Amazon Developer Console 
  * https://developer.amazon.com/edw/home.html
* Alexa -> Alexa Skills Kit -> Get Started
  * Edit the Custom Skill you created when you deployed the Azure4Alexa project
* Go to Skills Information in the left hand navigation
  * Global Fields -> Audio Player
  * Change to Yes
  * Click Next to save this change
* Go to Configuration in the left hand navigation
* Account Linking
  * Change to Yes
* Authorization URL
     * https://login.live.com/oauth20_authorize.srf 
     * Alexa sends the user to this page when they try to link your skill to their Microsoft account.
      * Alexa also includes the OAuth 2.0 query string parameters (i.e. client_id) when loading this page.
      * In the future, we'll include a landing page you can use to describe your skill before forwarding the user to the actual Authorization URL above
* Client Id
  * Paste the Application Id that was assigned to your application on the Microsoft Application Registration Portal.
* Domain List
  * You'll need to enter all of the hostnames that are used during the Groove OAuth web authentication flow. Why?  The Alexa companion application will not let the user navigate to a page not identified in this list.
  * At minimum, you will need these hostnames:
    * login.live.com
    * account.live.com
* Scope
   * MicrosoftMediaServices.GrooveApiAccess
   * offline_access
* Redirect URL
  *  Copy the NA URL (starting with https://)
     *  We've tested these instructions with the NA URL.  You can certainly configure your environment using the EU URL.
  *  Go to the browser window/tab where you're logged into the Microsoft Application Registration Portal. On the Microsoft website for the application you're registering:
     *  Platforms -> Add Platform -> Web
     *  Allow Implicit Flow - Checked
     *  Redirect URIs - paste the NA URL from the Amazon Developer Console
     *  At the bottom of the Microsoft Portal, click Save
* Go back to the Amazon Developer Console 
* Authorization Grant Type
  * Auth Code Grant (not Implicit Grant)
  * Access Token URI 
    * https://login.live.com/oauth20_authorize.srf
  * Client Secret
    * Paste the Application Secret value you wrote down from the Microsoft Application Registration Portal.  If you forgot to do this, you can immediately generate a new one on the Microsoft site.  
  * Client Authentication Scheme
    * Credentials in request body
* Privacy Policy URL
  * You are NOT required to provide one at this time, but you will need one in order to Go Live.
* Click Save

##### Update Alexa Intent Schema and Utterances

You'll now update the Intent Schema and Utterances for your Custom Skill to include the ability to access Groove.  Let's do it while you're still logged into the Amazon Developer Console.

* In the Amazon Developer Console, go to Your Custom Skill -> Interaction Model
* Intent Schema
  * Paste the content of the file AlexaIntentSchema.json from the Azure4Alexa -> Groove -> Registration folder.  
    * You may notice the new intents we've defined
      * AMAZON.ChooseAction\<object@MusicCreativeWork\> - this is a pre-built intent that parses a user's request to play a specific song or album.
      * PlaylistPlay - this intent parses a user's request to play a playlist. 
      * AMAZON.ResumeIntent and AMAZON.Pause - these intents handle the pause and resume of an audio track.  We've not built the code to support these features yet, but are required for certification.  Look for support in a future release.
     * Don't worry, we've included TflStatusIntent in this file as well.
* Sample Utterances
  * Paste the content of the file AlexaSampleUtterances.txt from the Azure4Alexa -> Groove -> Registration folder.
    * Again, don't worry - we've included all of the utterances for TflStatusIntent in this file as well.  

### Update and Re-Deploy Azure4Alexa

We've bundled the code needed to access Microsoft Groove in the Azure4Alexa project, which includes a forked version of the AlexaSkillsKit library.  However, you need to uncomment the code that calls it.  

In the file called AlexaSpeechletAsync.cs in Azure4Alexa -> Alexa, these  lines:

    //case ("AMAZON.ChooseAction<object@MusicCreativeWork>"):
    //    return await Groove.Music.PlayGrooveMusic(session, httpClient, intentRequest);
                
    //case ("PlaylistPlay"):
    //    return await Groove.Music.PlayGroovePlaylist(session, httpClient, intentRequest);

Now, the block of code that matches inbound intents from Amazon to functions in your code is looking for the Groove Music functionality we just added Intent Schema and Utterances for!

Note there are two additional functions at the bottom of AlexaSpeechletAsync.cs that support the playing of Groove Music streams you don't need to uncomment.  They handle the ability to play the multiple songs that are part of an album and playlist, and the ability for you to go to the next song in either.  We'll extend their functionality in the future to support operations like "repeat" and "back".

        public override async Task<SpeechletResponse> OnAudioPlayerAsync(AudioPlayerRequest audioPlayerRequest, Context context)
            
        public override async Task<SpeechletResponse> OnAudioIntentAsync(AudioIntentRequest audioIntentRequest, Context context)

Finally, re-publish your code to Azure.  Right click over Azure4Alexa and Publish.


### Link Your Account

Make sure you have some music in your OneDrive account and that you can play it from the Groove Music player for web
* https://music.microsoft.com/

Start the Alexa companion app on your iOS or Android device.

* Menu -> Skills -> Your Skills
* Scroll to Name of Your Skill
* Disable Skill
* Enable
* Login to Microsoft.  
* If you successfully authenticate, you'll see a web page where Alexa asks you to close the current window - "Your \<Skill Name\> Skill was successfully linked."

Now, ask Alexa to play your music!
 * "Alexa, ask \<My Skill Name\>, to play the song \<Song Name\> by \<Artist\>
 * "Alexa, ask \<My Skill Name\>, to play the album \<Album Name\> by \<Artist\>

You can also ask Alexa to play your playlists.  For now, you'll need to refer to your playlists by number instead of their actual name.   "Playlist Number" starts at zero, and corresponds to the top most playlist you see in the Groove Music website.  The next playlist down is one, and so on:

 * "Alexa, ask \<My Skill Name\>, to play playlist \<Playlist Number\>"

Don't like what's currently playing?

 * "Alexa, next"

### Next Steps

Apply your newly-gained knowledge and create new Azure-hosted Alexa skills that make use of Account Linking and web services.  Microsoft Graph and many, many other services await!  

## Questions and comments

We'd love to get your feedback about this sample. You can send your questions and suggestions to us in the Issues section of this repository.

## Additional resources
* [Linking an Alexa User with a User in Your System](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/docs/linking-an-alexa-user-with-a-user-in-your-system)
* [AudioPlayer Interface Reference](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/docs/custom-audioplayer-interface-reference)
* [Groove for Developers](https://music.microsoft.com/Developer)
* [Groove Music API REST Reference](https://docs.microsoft.com/en-us/groove/groove-service-rest-reference/overview)


## Copyright

Copyright (c) 2017 Tam Huynh. All rights reserved. 


### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
