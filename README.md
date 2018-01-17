## Create and Host Alexa Custom Skills for your Amazon Echo using .NET and Azure  

Use your C# and Azure knowledge to create Alexa Custom Skills for your Amazon Echo the comfort of Visual Studio!  Our Azure4Alexa Visual Studio project features the excellent [AlexaSkillsKit.NET](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET) package and demonstrates how you can accept a user's Alexa request, query a web service, and respond in both visual and spoken form.   

You don’t need Amazon Web Services and AWS Lambdas to create and host your Alexa Custom Skill.  Get started today, and reach millions of of Echo, Tap, and Echo Dot users!   

Special Note: We've created a forked version of the awesome AlexaSkillsKit.NET that implements Alexa's support for audio streaming. Our starter kit now includes and references this forked version, which is backwards compatible with the Outlook Mail and Calendar skills we've also bundled.

### Review the Basics

Review the [Alexa Custom Skills Documentation](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/overviews/understanding-custom-skills) so you can understand how users interact with skills, and how Alexa will interact with your service.  No time?  Here’s a quick primer.

#### Users
Echo, Tap, and Dot Echo users discover and enable your Alexa Custom Skill using the Alexa companion application on their iOS and Android devices.  You'll need to register at the Amazon Developer Console in order to develop and publish a Custom Skill.  Shy?  Don't worry, you can (privately) develop a skill without publishing it to the world.  

Users begin an Alexa session with your Custom Skill by saying:

* Alexa
* An Invocation Name, which is the name you've given your skill: i.e. "Azure"
* A phrase ("is there a good service on the Tube?") that's associated with an Intent. 

An intent corresponds to a feature of your skill.  An utterance is a combination of an intent and a phrase.  You'll register a list of accepted utterances in the Developer Console so Alexa can match the user’s spoken words to your skill's intents. Here's an example of a set of utterances for the sample service we've included in Azure4Alexa.  Multiple phrases can be provided for a single intent as to not enforce a rigid spoken syntax on users. TflStatusIntent is a skill that queries the Transport for London Web API to determine the status of the London Underground.

    TflStatusIntent is there a good service on the tube 
    TflStatusIntent is there a good service on the underground 
    TflStatusIntent are there any disruptions on the tube
    TflStatusIntent what's the tube status

And while a single skill can only have one invocation name, it can have multiple intents that provide the same, related, or un-related functionality.   

Optionally, intents can make use of custom or pre-defined Alexa slots, which are variable values.  You could define an intent in which the user states the name of a city in a slot: “what’s the temperature in \{slot name\}?”, where “\{slot name\}” could be London, San Francisco, or another city.  Alexa will map the value of the spoken slot to a known value, which you can then use in your response to the user.  You can define your own slot values (i.e. a list of transporation lines), or make use of the pre-created ones from Alexa, which include well-known cities, date and times, and region names.  

Optionally, users can authenticate to your service.  Alexa provides an OAuth 2.0  framework that also supports token refreshes(!).  You'll enter all of the usual OAuth 2.0 client configuration settings into the Amazon Developer Console: login URL,  client ID, application secret, scopes, etc.  The re-direct destination for your OAuth 2.0 flow will be an Alexa server, which will also be responsible for storing and refreshing your user's access token. When users add your Custom Skill using the companion app on their iOS or Android device, they'll be able to link their account to your skill: specifically, the companion app will launch an embedded browser that sends them to the login URL you identified.  After that, anytime a user invokes your skill, Alexa will send the user's access token to your service so you can make use of it in your code.  

#### Your Service

User speak to Alexa, which in turn, parses the words spoken and matches them to skills, intents and, optionally, slot values.   If the words spoken correspond to your Custom Skill, Alexa sends JSON POST to your service endpoint.  

You can gain a deep understanding of the interaction between Alexa and a Custom Skill endpoint here: https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/docs/handling-requests-sent-by-alexa

The awesome [AlexaSkillsSet.NET](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET) package provides a .NET-based implementation of the Java-based SpeechServlet described by Amazon and session management facilities so you can build [Conversational Alexa Apps](https://freebusy.io/blog/building-conversational-alexa-apps-for-amazon-echo).  

We've included AlexaSkillsSet.NET in our Alexa4Azure Visual Studio project, which you can use as a scaffolding for your own Custom Skill.  Included in our project is a Custom Skill that requests the current status of the London Underground (a.k.a. "The Tube") using the Transport for London Web API.   When you're ready, you can even comment out the code within our Custom Skill and subsitute your own.  

Now, onto configuration and deployment!


### Become an Alexa Developer

* Sign up as an Alexa Developer at [Amazon](https://developer.amazon.com/alexa-skills-kit)
* Review the [Alexa Custom Skills Documentation](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/overviews/understanding-custom-skills)
* Decide if you'll use an actual Alexa device (such as the Amazon Echo) or the Alexa Simulator at [EchoSim.io](https://echosim.io/)
* Install the Alexa Companion app for iOS or Android
* Setup your Alexa device or Simluator, and Companion app using your Alexa Developer account credentials.  Why?  Your Custom Skill is only visible to you until you decide to begin the Amazon Custom Skill approval process, and make it available to all Alexa users.   

### Setup a Visual Studio and Azure Environment

* Install Visual Studio 2015. Don't have it? Download the free [Visual Studio Community Edition](https://www.visualstudio.com/)
* Get yourself on Azure, if you're not already. [Free Credits await!](https://azure.microsoft.com)
* Clone this GitHub repository 
* Open the Azure4Alexa solution in Visual Studio, right click over the Azure4Alexa project and publish it to Azure as a new App Service.
* In your web browser, go to the HTTPS URL you deployed the project to.  Azure automatically provides you both an HTTPS and HTTP endpoint for your project, located at the same hostname (i.e. https://your-project.azurewebsites.net and http://your-project.azurewebsites.net)
* If your deployment succeeded, you'll see a static html page that describes this project!

### Register your Alexa Custom Skill

* Go to the Alexa section of the Amazon Developer Console at https://developer.amazon.com/edw/home.html
* Choose Alexa Skills Kit -> Get Started
* Choose Add a New Skill (upper right hand portion of the page)
* On the Skill Information Screen (left hand navigation)
    * Write down the "Application Id".  This is a unique value generated by Amazon that you will need to put into your Visual Studio project.
    * Skill Type - Custom Interaction Model
    * Name - Anything you want
    * Invocation Name - What users will say to activate your skill.  One word works best (i.e. "Azure")
    * Select Next
* On the Interaction Model Screen 
    * Intent Schema - copy and paste the content of the file called AlexaIntentSchema.json located in this Visual Studio project path: Azure4Alexa -> Alexa -> Registration 
    * Sample Utterances - copy and paste the content of the file called AlexaSampleUtterances.txt located in this Visual Studio project path: Azure4Alexa -> Alexa -> Registration 
    * Select Next
* On the Configuration Screen 
    * Services Endpoint Type is HTTPS
    * HTTPS URL is https://your-project.azurewebsites.net/api/alexa (substitute your-project-azurewebsites.net with your hostname).
    * Account Linking is No
    * Select Next
* On the SSL Certificate Screen 
    * Select "My development endpoint is a sub-domain ..." unless you have separately setup one of the two other options
    * Select Next
* On the Test Screen 
    * Service Simulator -> Enter Utterance - Type in the phrase "is there a good service on the Tube?"
    * Click the "Ask <invocation name>" button to run your query.  
    * You'll see a reply in the Service Response window, that states that the Application ID in your project is incorrect.  This is expected!  You'll need to now go back to Visual Studio and update the variable AppId in Web.config to include the "Application Id" generated by Amazon, which we mentioned further up in these instrucitons.
    
### Update Your Project and Test

* In Visual Studio, open Web.config which you'll find in the root folder "Azure4Alexa".
* Update the value of AppId to be the "Application Id" assigned to your skill by Alexa
* Re-deploy to Azure
* Go back to the Test Screen for your Custom Skill.  Re-enter and re-run "is there a good service on the Tube?" in the Service Simulator and you should see the latest status of the London Underground!  
* Go to the Alexa companion app on your iOS or Android device, and navigate to Skills.  Then go to Your Skills, tap your new Custom Skill, and then enable your Skill.
* Talk to your Alexa.  Say "Alexa, ask \<Invocation Name\> is there a good service on the Tube?".  You should get a verbal reply and a visual card in the companion app! 

### Explaining the Code

Azure4Alexa is a Web API project.  The main controller is implemented in Alexa -> AlexaSpeechletAsync.cs and is an override of SpeechletAsync included in [AlexaSkillsSet.NET](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET)

* OnSessionStartedAsync is run when a user invokes your custom skill
* OnSessionEndedAsync is run when a user ends their session with your custom skill
* OnLaunchAsync is run when a user invokes your custom skill without an intent
* OnIntentAsync is run when a user invokes your custom skill with a specific intent

As a single skill may have multiple intents, it's up to you to parse the incoming value provided to you (intentName) within OnIntentAsync and map it to a corresponding function.

* Each of the four methods above return a SpeechletResponse object, which is defined in AlexaSkillsSet.NET 
* Within AlexaUtils.cs, we've defined SimpleIntentResponse class, which you can pass to BuildSpeechletResponse.  The latter will return a SpeechletResponse object.
* At minimum, an instance of SimpleIntentResponse requires one and only one variable to be set: cardText.  This is the value of the string seen in the Alexa companion app and will be spoken back to the user unless you assign a string to ssmlString.

Example: GetOnLaunchAsyncResult is called by OnLaunchAsync, and therefore run whenver a user invokes your custom skill without an intent (i.e. Alexa, ask \<Invocation Name\> for help).  It demonstrates how to use SpeechletResponse, BuildSpeechletResponse, and SimpleIntentResponse together in the simplest manner. 

    private SpeechletResponse GetOnLaunchAsyncResult(Session session)
        {
            // called by OnLaunchAsync - when the user invokes your skill without an intent
            // called by OnIntentAsync if you forget to map an intent to an action

            return AlexaUtils.BuildSpeechletResponse(new AlexaUtils.SimpleIntentResponse() { cardText = "Thanks for giving us a try"  }, true);
        }

The true parameter passed to BuildSpeechletResponse is a boolean that determines if the user's session should end or continue.  We've set it to true in this example.  

Within OnIntentAsync you'll notice we look for the value of intentRequest to match TflStatusIntent in a case statement.  Remember, TflStatusIntent is the Intent Name we've identified in our utterance file.  Thus, Alexa provides us this intent value when a match between what the user has spoken and what we've registered has occurred, and it's now up to us in Tfl.Status.GetResults to create a reply!

### Further Topics: Images, Static HTML, and OAuth

#### Images
The response cards shown to users in the Alexa companion app can include images.  

Alexa requires that all images be served from a CORS-enabled host.  Therefore, we've implemented a CORS-enabled controller called AlexaImagesController.cs that serves images found in the ~/Images/ subfolder of the Azure4Alexa project.  When you create a reply using an instance of the SimpleIntentResponse class mentioned before, you can optionally define both a largeImage and smallImage.  Alexa will decide which one to show the user based on their device resolution.  For example:

    return new AlexaUtils.SimpleIntentResponse()
            {
                cardText = "Hello",
                ssmlString = "Is it me you're looking for?",
                largeImage = "msftHiRes.png",
                smallImage = "msftLowRes.png",
            };

#### Static HTML

Azure4Alexa is a Web API project that does not include the usual MVC components.  If you'd like to provide a web landing page for your Custom Skill, or are required to provide a Privacy Policy or Terms of Service to pass Amazon's Certification Process, you can simply replace the static HTML documents we've placed into the root of the Azure4Alexa project - or create new ones of your own.

If you REALLY REALLY want to use MVC, you can certainly add those components to the project. They'll peacefully co-exist with the Web API controllers.

#### OAuth

Feeling confident?  You can connect your Custom Skill to Microsoft Outlook using the Outlook REST API, Alexa's Account Linking framework, and Oauth 2.0. [Learn more](./Azure4Alexa/Outlook/README.md)


## Questions and comments

We'd love to get your feedback about this sample. You can send your questions and suggestions to us in the Issues section of this repository.

## Additional resources
* [AlexaSkillsKit.NET - .NET library to write Alexa skills that's interface-compatible with Amazon's AlexaSkillsKit for Java and matches that functionality](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET)
* [Getting started with Alexa App development for Amazon Echo using .NET on Windows](https://freebusy.io/blog/getting-started-with-alexa-app-development-for-amazon-echo-using-dot-net)
* [Getting Started with the Alexa Skills Kit](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/getting-started-guide)
* [Transport for London Unified API](https://api-portal.tfl.gov.uk/docs)

## Copyright

Copyright (c) 2016 Tam Huynh. All rights reserved. 


### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
