## Build and Host Alexa Custom Skills using .NET and Azure  

Take advantage of your C# and Azure knowledge, and build Alexa Custom Skills from the comfort of Visual Studio!  Our Azure4Alexa starter kit features the excellent [AlexaSkillsKit.NET package](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET) and demonstrates how you can query a web service and relay its content back to an Alexa user.  

You don’t need to use Amazon Web Services and AWS Lambdas to create and host your Alexa Custom Skill.  Get started today, millions of Echo, Tap, and Echo Dot users are waiting for you!


### Fundamentals

We strongly recommend you review the [Alexa Custom Skills Documentation](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/overviews/understanding-custom-skills) so you can understand how users interact with skills, and how Alexa will interact with your service.  No time?  Here’s a quick primer.


* Users begin a session with your skill by saying:
 
  * Alexa
  * An invocation name, which is the name you've given your skill: i.e. Azure
  * An intent, such as “is there a Good Service on the London Underground?  

 Combined, the invocation name and intent is known as an utterance.  You'll register a list of accepted utterances with Alexa so that it can match the user’s spoken words to the functionality provided by your skill.   While a single skill can only have one invocation name, it can have multiple intents.

* Optionally, utterances can make use of custom or pre-defined Alexa slots, which are variable values.  For instance, you could define an utterance in which the user states the name of a city: i.e. “Weatherly what’s the temperature in \{slot name\}?”, where “\{slot name\}” could be London, San Francisco, or another city.  Alexa will map the value of the spoken slot to a known value, which you can then use in your response to the user.  You can define your own slots or make of the pre-created ones from Alexa, which include well-known city names, date and times, and region names.  

Optionally, users can authenticate to your service.  Alexa provides an OAuth 2.0 framework to support authentication to your service, including automatic token refresh.  You enter the URLs to your sign in page, access token endpoint  



### Become an Alexa Developer

* Sign up as an Alexa Developer at [Amazon](https://developer.amazon.com/alexa-skills-kit)
* Review the [Alexa Custom Skills Documentation](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/overviews/understanding-custom-skills)
* Decide if you'll use an actual Alexa device (such as the Amazon Echo) or the Alexa Simulator at [EchoSim.io](https://echosim.io/)
* Install the Alexa Companion app for iOS or Android

### Setup a development environment

* Install Visual Studio 2015. Don't have it? Download the free Visual Studio Community Edition
* Get yourself on Azure, if you're not already! 
* Clone this GitHub repository 
* Open the Azure4Alexa solution in Visual Studio, right click over the Azure4Alexa project and publish it to Azure as a 


### Register your Alexa Custom Skill

* Foo
* Bar
* Etc

### 



