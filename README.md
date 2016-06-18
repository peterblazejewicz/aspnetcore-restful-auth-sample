This is a modified version of the ASP.NET Core 1.0 RC2 Web Application created by Visual Studio with "Individual Accounts".

The changes made are:

* Moved Account and Manage controller resources into a "Users" Area
* Created a BaseController that controllers can inherit from. This should be out of the box as it is in rails 5 to allow folks to extend all controllers without refactoring.
* Split Account into Controllers that follow REST (Index, Details, Create, Edit, Delete only)
* Moved ViewModels out of Models folder and into "Users" Area
* Added mapping of "Users" route to Startup.cs

Notes: ASP.NET Core really needs a built-in way to do flash/notice messages like rails! Having to define a parameter and "Message" enums for every action that can show a message is silly.

An out of the box aspnet project should still have good structure that follows REST to encourage this as the app grows. See the rails devise project's [views](https://github.com/plataformatec/devise/tree/master/app/views/devise) and [controllers](https://github.com/plataformatec/devise/tree/master/app/controllers/devise) as examples of why I'm recommending this approach.