## Connect Alexa to Microsoft Outlook

“Alexa, ask My Skill how many unread Outlook emails I have”

Let's connect your Echo, Dot Echo, and Tap to Microsoft Outlook!  We'll make combined use of the Outlook REST API, Alexa's Account Linking framework, and Microsoft's Converged Authentication Endpoint.  When you're done, you'll be able to ask Alexa how many unread emails you have and hear the subject and sender of the first five!

##### Supported Outlook Inboxes 

Personal Outlook Accounts (i.e. hotmail.com, outlook.com) - This skill works with personal inboxes that have been upgraded to the new Outlook user interface.  Not yet upgraded?  Just create a new Outlook.com account so you can try things out.

Exchange Online and Office 365 - This skill will work with most school and work accounts that use Azure Active Directory for authentication. If your organization has customized the login experience by introducing additional screens (i.e. a redirect to https://your-company.sts.domain.com), you'll need to enter the hostname from each of these additional webpages into the Amazon Developer Console.  More on this later.

### Alexa, Authentication, and Skills Linking

Here's a recap of what we mentioned in the README.md in the root of this project.

Alexa provides an OAuth 2.0-based Account Linking framework that also supports token refreshes(!). You'll enter all of the usual OAuth 2.0 client configuration settings into the Amazon Developer Console: login URL, client ID, application secret, scopes, etc. The re-direct destination for your OAuth 2.0 flow will be an Alexa server, which will also be responsible for storing and refreshing your user's access token. When users add your Custom Skill using the companion app on their iOS or Android device, they'll be able to link their account to your skill: specifically, the companion app will launch an embedded browser that sends them to the login URL you identified. After that, anytime a user invokes your skill, Alexa will send the user's access token to your service so you can make use of it in your code.

Microsoft enables you to create applications that connect to Outlook.com, Office 365, and Exchange Online inboxes using OAuth 2.0 and the Outlook REST API.   

To help you understand how you can use all these components together, we've included an Outlook Custom Skill in the Azure4Alexa project.  All you have to do is configure it, and re-deploy your project to Azure!  

### Prerequisites

Before you can configure and deploy the Microsoft Outlook Custom Skill, you’ll need to successfully complete the steps described in the [README.md](../README.md) file stored in the root of this project.  

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
* Click "Go to app list"
  * If you just signed-up as a developer, you may be re-directed to a web page that doesn't have this button.  If so, go back to the Portal home page: https://apps.dev.microsoft.com/ then click "Go to app list"
* My Applications -> Add an app (right hand side of page).  
  * New Application Registration -> Create Application.
  * Name your app anything you want. It doesn't have to match anything you've entered into the Amazon Developer Console or your code.      
* You’ll then be taken to page called \<Your App’s Name\> Registration
  * Application Id
    * This value appears beneath your app's name.  Take note: you'll later enter this into the Amazon Developer Console, as the Account Linking -> Client ID.
  * Application Secrets -> Generate New Password.  
    * Copy and store the value shown to you.  It will be shown to you once, and only once.  
    * No worries if you lose it, however.  You can immediately generate a new one, and have multiple secrets for your application.  
  * Microsoft Graph Permissions
    * If you see this section, don’t change any of the default values.
  * Profile
    * Logo, Terms of Service, and Privacy Statement can be left empty for now.  However, you'll need to complete these fields if you Go Live.
  * Advanced Options 
    * Leave “Live SDK support” checked
  * Click Save
    * However, do NOT close your browser window.  You'll need to keep this window open to copy and paste values to and from the Amazon Developer Console.

### Update your Alexa Custom Skills settings

##### Enable Account Linking

Now, you’ll update your existing Custom Skill to support Account Linking.  

* Open a new browser tab or window.
* Sign in to the Amazon Developer Console 
  * https://developer.amazon.com/edw/home.html
* Alexa -> Alexa Skills Kit -> Get Started
  * Edit the Custom Skill you created when you deployed the Azure4Alexa project
* Go to Configuration in the left hand navigation
* Account Linking
  * Change to Yes
* Authorization URL
     * https://your-project.azurewebsites.net/outlook.htm (substitute your-project-azurewebsites.net with your hostname).
     * Alexa sends the user to this page when they try to link your skill to their Outlook account.
      * Alexa also includes the OAuth 2.0 query string parameters (i.e. client_id) when loading this page.
      * outlook.htm parses these query string parameters, and uses them to generate a login link to the actual Microsoft sign-in page (i.e. login.microsoftonline.com)
      * You can use this page to further describe what your skill does, as well as provide links to your privacy policy and terms of service.  Some or all of these may be required to gain certification from Amazon.
* Client Id
  * Paste the Application Id that was assigned to your application on the Microsoft Application Registration Portal.
* Domain List
  * You'll need to enter all of the hostnames that are used during the Outlook.com OAuth web authentication flow. Why?  The Alexa companion application will not let the user navigate to a page not identified in this list.
  * At minimum, you will need these hostnames:
    * login.microsoftonline.com
    * login.windows.net
    * login.live.com
    * account.live.com
  *  If your organization has customized the login experience by introducing additional screens (i.e. a redirect to https://your-company.sts.domain.com), you'll now need to enter the hostname from each of these additional webpages into the Amazon Developer Console.  
    * Tip: Open a new browser window in private mode, sign in to your Office 365 or Exchange Online account, and record the URLs that are part of the experience. If there's a hostname that's not included in the list above, add it.
    * Technical Note: If your organization is using Active Directory Federation Services (ADFS), you'll typically encounter these additional login pages. 
* Scope
   * https://outlook.office.com/mail.read
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
    * https://login.microsoftonline.com/common/oauth2/v2.0/token
  * Client Secret
    * Paste the Application Secret value you wrote down from the Microsoft Application Registration Portal.  If you forgot to do this, you can immediately generate a new one on the Microsoft site.  
  * Client Authentication Scheme
    * Credentials in request body
* Privacy Policy URL
  * You are NOT required to provide one at this time, but you will need one in order to Go Live.
* Click Save

##### Update Alexa Intent Schema and Utterances

You'll now update the Intent Schema and Utterances for your Custom Skill to include the ability to access Outlook.  Let's do it while you're still logged into the Amazon Developer Console.

* In the Amazon Developer Console, go to Your Custom Skill -> Interaction Model
* Intent Schema
  * Paste the content of the file AlexaIntentSchema.json from the Azure4Alexa -> Outlook -> Registration folder.  
    * You may notice the new intent we've defined - OutlookUnreadIntent
     * Don't worry, we've included TflStatusIntent in this file as well.
* Sample Utterances
  * Paste the content of the file AlexaSampleUtterances.txt from the Azure4Alexa -> Outlook -> Registration folder.
    * Again, don't worry - we've included all of the utterances for TflStatusIntent in this file as well.  

### Update and Re-Deploy Azure4Alexa

We've bundled the code needed to access Outlook in the Azure4Alexa project.  However, you need to uncomment the code that calls it.  

In the file called AlexaSpeechletAsync.cs in Azure4Alexa -> Alexa, uncomment these two lines:

    //case ("OutlookUnreadIntent"):
    //    return await Outlook.Mail.GetUnreadEmailCount(session, httpClient);

Now, the block of code that matches inbound intents from Amazon to functions in your code is looking for the OutlookUnreadIntent we just added Intent Schema and Utterances for!

Finally, re-publish your code to Azure.  Right click over Azure4Alexa and Publish.


### Link Your Account

Start the Alexa companion app on your iOS or Android device.

* Menu -> Skills -> Your Skills
* Scroll to Name of Your Skill
* Disable Skill
* Enable
* Login to Outlook.  
 * Respond to any two-factor authentication requests that you may receive
* If you successfully authenticate, you'll see a web page where Alexa asks you to close the current window - "Your \<Skill Name\> Skill was successfully linked."

Finally!  Ask Alexa, "Alexa, ask \<My Skill Name\>, how many unread Outlook emails do I have"

### Next Steps

Apply your newly-gained knowledge and create new Azure-hosted Alexa skills that make use of Account Linking and web services.  Microsoft Graph and many, many other services await!  

## Questions and comments

We'd love to get your feedback about this sample. You can send your questions and suggestions to us in the Issues section of this repository.

## Additional resources
* [Linking an Alexa User with a User in Your System](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/docs/linking-an-alexa-user-with-a-user-in-your-system)
* [Outlook REST API Overview](https://dev.outlook.com/restapi)
* [Outlook Mail REST API reference](https://msdn.microsoft.com/office/office365/APi/mail-rest-operations)
* [Sign-in Microsoft Account & Azure AD users in a single app](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview/)

## Copyright

Copyright (c) 2016 Tam Huynh. All rights reserved. 


### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
