This is a modified version of the ASP.NET Core 1.0 RC2 Web Application created by Visual Studio with "Individual Accounts".

The changes made are:

* Moved Account and Manage controller resources into a "Users" Area
* Split Account into Controllers that follow REST (Index, Details, Create, Edit, Delete only)
* Moved ViewModels out of Models folder and into "Users" Area
* Added mapping of "Users" route to Startup.cs

Notes: ASP.NET Core really needs a built-in way to do flash/notice messages like rails! Having to define a parameter and "Message" enums for every action that can show a message is silly.
