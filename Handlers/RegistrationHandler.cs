// Dessa "using"-rader berättar för C# vilka delar vi använder
using SnackToSixPack.Classes;
using Spectre.Console; // För färg och snygga texter
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SnackToSixPack.Models;

namespace SnackToSixPack.Handlers
{
    public class  RegistrationHandler
    {
        public static async Task RegistrationForm()
        {
            using (var db = new AppDbContext())
            {
                AnsiConsole.Clear();
                MenuHandler.ShowTitle();
                AnsiConsole.MarkupLine("[blue]----- Register user -----[/]");

                string username = "";
                bool runRegister = true;

                while (runRegister)
                {
                    username = AnsiConsole.Ask<string>("[bold]Username:[/]");

                    if (string.IsNullOrEmpty(username))
                    {
                        AnsiConsole.MarkupLine("[red]Username cannot be empty. Try again.[/]");
                        continue;
                    }
                    
                    bool usernameExists = db.Users.Any(u => u.UserName.ToLower() == username.ToLower());

                    if (usernameExists)
                    {
                        AnsiConsole.MarkupLine("[red]Username already exists. Try again.[/]");
                        continue;
                    }
                    
                    runRegister = false;
                }

                string password = "";
                bool rightPasword = false;
                while (!rightPasword)
                {
                    var enterPasword = new TextPrompt<string>("[bold]Password: [/]")
                        .PromptStyle("green")
                        .Secret();
                    
                    string passwordPrompt = AnsiConsole.Prompt(enterPasword);

                    rightPasword = PasswordValidator.IsPassWordStrong(passwordPrompt);
                    if (rightPasword)
                    {
                        password = passwordPrompt;
                    }
                }

                string email = "";
                bool correctEmail = false;
                while (!correctEmail)
                {
                    email = AnsiConsole.Prompt(new TextPrompt<string>("[bold]Email: [/]")
                        .PromptStyle("green")
                        .Validate(email =>
                            email.Contains("@")
                                ? ValidationResult.Success()
                                : ValidationResult.Error("Invalid email address"))
                    );
                
                    bool emailExists = db.Users.Any(u => u.Email.ToLower() == email.ToLower());
                    if (emailExists)
                    {
                        AnsiConsole.MarkupLine("[red]Email already exist. Try again.[/]");
                        continue;
                    }

                    correctEmail = true;
                }

                var newUser = new User
                {
                    UserName = username,
                    Password = password,
                    Email = email
                };
                
                // .Add "Queue this entity to be inserted later", staged the insert
                db.Add(newUser);
                // Accually save to the database, executes the insert
                db.SaveChanges();
                
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[green]Successfully registered successfully![/]");
                AnsiConsole.Status()
                    .Start("Please wait...\n Redirecting", ctx => System.Threading.Thread.Sleep(2000));
                
                AuthForms.ShowLogInForm();
            }
        }
    
        public static async Task DeleteCurrentUser()
        {
            var user = Session.CurrentUser;

            List<User> users = JSONFileHanldler.Load<List<User>>(
            Path.Combine($"Data", "Users.json")
            );

            AnsiConsole.MarkupLine("[red]Are your sure you want to delete the account?[/]");
            var confirmChoice = new SelectionPrompt<string>();
                confirmChoice.AddChoice("Yes");
                confirmChoice.AddChoice("No");

            var choiceConfirmed = AnsiConsole.Prompt<string>(confirmChoice);
            if (choiceConfirmed == "Yes")
            {
                int removed = users.RemoveAll(u => u.Id == user.Id);
                JSONFileHanldler.Save("Data/Users.json", users);

                // 4. Ta bort användarmapp
                string userFolder = "Data/Users/" + user.Id;
                if (Directory.Exists(userFolder))
                {
                    Directory.Delete(userFolder, true); 
                }

                // 5. Logga ut
                Session.SetCurrentUser(null);

                AnsiConsole.MarkupLine("[green]Your account has been deleted successfully.[/]");
                AnsiConsole.MarkupLine("[yellow]Returning to main menu...[/]");

                Thread.Sleep(3000);
                await MenuHandler.ShowMainMenu();

            }
            if (choiceConfirmed == "No")
            {
                return;
            }
        }
    }
}