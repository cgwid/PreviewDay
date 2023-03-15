# PreviewDay

[![Hack Together: Microsoft Graph and .NET](https://img.shields.io/badge/Microsoft%20-Hack--Together-orange?style=for-the-badge&logo=microsoft)](https://github.com/microsoft/hack-together)

v1

Created for Hack Together: Microsoft Graph and .NET

At the moment, this is a simple ASP.NET Core MVC application that uses Microsoft Graph to get a users ToDo Lists and Calendar Events
with the idea of being able to plan out the day. 

For the ToDo lists, it gets the tasks: 
1) That are not yet completed
2) Displays the subtasks for Tasks that have them
3) Displays the notes for the Task
4) Orders by Importance
5) Hightlights the tasks that have been marked important in red

For the Calendar:
1) It gets the events for that day
2) Orders from Earliest to Latest
2) Displays the Subject, organizer, start time, and end time within the timezone set in Outlook


# Set up

1) Within Azure AD, create a new app registration. 
2) Select Single Tenant
3) Redirect URI - select "web" and value - https://localhost:7004
4) Go to Authentication section and add another Redirect URI https://localhost:7004/signin-oidc
5) Within Auth section, go to section "Front channel logout" add https://localhost:7004/signout-oidc
6) Within Auth section, "Implicit grant and hybrid flows" -> select ID tokens
7) Optional: Go to API permissions, Add permission, Microsoft Graph, Delegated, add the required permissions 
user.read tasks.read tasks.readwrite mailboxsettings.read calendars.read.
The permissions are already set within appsettings.json -> Microsoft:Scopes
8) Go to Certificates and Secrets section, add a secret, make sure to copy it right away since you can't view it after you leave the section
9) In overview section, copy ClientId and TenantId
10) Add TenantId, ClientId, and ClientSecret to appsettings.json or alternatively use "dotnet user-secrets" or environment variables.

# To Do

1) Refactor/Organize code
2) At the moment, it only gets To Do and Calendar data --> would like to add functionality to be able to create/edit
3) Improve UI
4) Add more features/components such as email, teams, OneDrive.



