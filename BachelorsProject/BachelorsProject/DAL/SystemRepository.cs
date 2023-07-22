using BachelorsProject.Models;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Security.Claims;
//using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Security.Policy;
using Castle.Core.Internal;
using System.Net.Mail;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using Microsoft.AspNetCore.Razor.Language;
using NuGet.Protocol;
using System.Text.Json;
using System.Drawing;
using System.Net.WebSockets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using QandA = BachelorsProject.DAL.QandA;

namespace BachelorsProject.DAL
{
    public class SystemRepository : ISystemRepository
    {

        private readonly SystemContext _db;
        private ILogger<SystemRepository> _log;
        private readonly IConfiguration _configuration;
        private static IWebHostEnvironment _webHostEnvironment;


        public SystemRepository(SystemContext db, ILogger<SystemRepository> log, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _log = log;
            _webHostEnvironment = webHostEnvironment;
        }


        public SystemRepository(SystemContext db, ILogger<SystemRepository> log, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _db = db;
            _log = log;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        //repeating service invcoation

        public void PeriodicTimerOnceAMinute()
        {
            
            //ceheck status of all printers in the DB and sets the status
            CheckStatusOfAllPrinters();

            //selects a print from the queue, selects an empty printer and sends the print
            SelectPrintAndSend();
            Console.WriteLine(DateTime.Now + " Once a minute");

           
            

            //SelectPrintAndSend();
            //CancelQueuePrint("test", "testUser");
           // StreamFromPrinter("Tore");
        }

        public void PeriodicTimerOnceADay()
        {
            /*
            DeleteUnusedUsers();
            DeleteOldUserFiles();
            DeleteOldLogs();
            Console.WriteLine(DateTime.Now + " Once a day" );
            */
        }


        private void DeleteOldLogs()
        {
            try
            {
                string pathToLogs = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)))
                {
                    pathToLogs = Path.Combine(_webHostEnvironment.ContentRootPath, "Logs");
                    DirectoryInfo logsDirectory = new DirectoryInfo(pathToLogs);
                    FileInfo[] allLogs = logsDirectory.GetFiles("*.txt");

                    for (int i = 0; i < allLogs.Length; i++)
                    {
                        FileInfo log = allLogs[i];
                        string logPath = log.FullName;
                        var timeOfLog = System.IO.File.GetLastWriteTime(logPath);


                        if (timeOfLog < DateTime.Now.AddDays(-7))
                        {
                            System.IO.File.Delete(logPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
            }
        }

        public async Task<bool> GetImageFromPrinter() //WORKS  //TESTING
        {

            var url = "http://100.127.254.230:8080/?action=snapshot";

            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var image = Image.FromStream(stream);

                // Set the file path to save the image
                //var filePath = @"C:\path\to\image.jpg";
                var filePath = @"C:\Users\Denis\Desktop\Bachelors Project\GitHub\Bachelors-Project\BachelorsProject\BachelorsProject\ClientApp\src\image.jpg";

                // Save the image to a file
                image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            return true;
        }
        //}
        //public async Task<bool> StreamFromPrinter(string inPrinterName)
        //{
        //    try
        //    {
        //        var findPrinter = _db.ListOfPrinters.FirstOrDefault(x => x.PrinterName == inPrinterName);
        //        if (findPrinter != null)
        //        {
        //            //url to stream from
        //            string sourceOfStream = $"http://{findPrinter.IP}:8080/?action=stream";

        //            // string sourceUrl = "http://100.127.254.230:8080/?action=stream";
        //            //string destinationUrl = "http://localhost:8081/";

        //            string streamTo = "http://localhost:8081/";

        //            HttpListener streamListener = new HttpListener();
        //            streamListener.Prefixes.Add(streamTo);
        //            streamListener.Start();


        //            while (true)
        //            {
        //                HttpListenerContext listenerContext = streamListener.GetContext();
        //                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://100.127.254.195:8080/?action=stream");
        //                webRequest.Method = listenerContext.Request.HttpMethod;

        //                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
        //                {
        //                    listenerContext.Response.ContentType = webResponse.ContentType;

        //                    using (Stream stream = webResponse.GetResponseStream())
        //                    {
        //                        byte[] buffer = new byte[4096];
        //                        int bytesRead;

        //                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        //                        {
        //                            listenerContext.Response.OutputStream.Write(buffer, 0, bytesRead);
        //                        }
        //                    }
        //                }
        //                listenerContext.Response.Close();
        //            }
        //        }
        //        else return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogInformation(ex.Message);
        //        return false;
        //    }
        //}

        public async Task<bool> StreamFromPrinter(string inPrinterName)
        {
            try
            {
                var findPrinter = await _db.ListOfPrinters.FirstOrDefaultAsync(x => x.PrinterName == inPrinterName);
                var allPrinters = await _db.ListOfPrinters.ToListAsync();
                var finxdIndexOfPrinter = allPrinters.FindIndex(x => x.PrinterName == inPrinterName);

                string streamTo = "";
                if (findPrinter != null)
                {
                    //url to stream from
                    string streamFrom = $"http://{findPrinter.IP}:8080/?action=stream";

                    // string sourceUrl = "http://100.127.254.230:8080/?action=stream";
                    //string destinationUrl = "http://localhost:8081/";
                    //string streamTo = "http://localhost:8081/";

                    if (finxdIndexOfPrinter < 10)
                    {
                        streamTo = $"http://localhost:808{finxdIndexOfPrinter + 3}/";
                        //streamTo = "http://localhost:8081";
                    }
                    //+1 to correspond to the index in 
                    else
                    {
                        streamTo = $"http://localhost:80{finxdIndexOfPrinter + 1}/";
                    }

                    HttpListener streamListener = new HttpListener();
                    streamListener.Prefixes.Add(streamTo);
                    streamListener.Start();

                    while (true)
                    {
                        HttpListenerContext listenerContext = streamListener.GetContext();
                        Task.Run(() =>
                        {
                            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(streamFrom);
                            webRequest.Method = listenerContext.Request.HttpMethod;

                            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                            {
                                listenerContext.Response.ContentType = webResponse.ContentType;

                                using (Stream stream = webResponse.GetResponseStream())
                                {
                                    byte[] buffer = new byte[4096];
                                    int readBytes;

                                    while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        listenerContext.Response.OutputStream.Write(buffer, 0, readBytes);
                                    }
                                }
                            }
                            listenerContext.Response.Close();
                        });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }



                    //            HttpListener streamListener = new HttpListener();
                    //            streamListener.Prefixes.Add(streamTo);
                    //            streamListener.Start();


                    //            //https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-7.0
                    //            //https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-6.0

                    //            while (true)
                    //            {
                    //                HttpListenerContext listenerContext = streamListener.GetContext();
                    //                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(streamFrom);
                    //                webRequest.Method = listenerContext.Request.HttpMethod;

                    //                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                    //                {
                    //                    listenerContext.Response.ContentType = webResponse.ContentType;

                    //                    using (Stream stream = webResponse.GetResponseStream())
                    //                    {
                    //                        byte[] buffer = new byte[4096];
                    //                        int readBytes;

                    //                        while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                    //                        {
                    //                            listenerContext.Response.OutputStream.Write(buffer, 0, readBytes);
                    //                        }
                    //                    }
                    //                }
                    //                listenerContext.Response.Close();
                    //            }
                    //        }
                    //        else return false;

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        _log.LogInformation(ex.Message);
                    //        return false;
                    //    }
                    //}


        private async void DeleteUnusedUsers()
        {
            try
            {
                List<Users> allUsers = await _db.Users.Select(f => new Users
                {
                    Username = f.Username,
                    CreatedOn = f.CreatedOn
                }).ToListAsync();

                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (allUsers[i].CreatedOn >= allUsers[i].CreatedOn.AddYears(3))
                    {
                        _db.Users.Remove(allUsers[i]);

                        await _db.SaveChangesAsync();
                        _log.LogInformation($"{allUsers[i]} was deleted from the system");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
            }
        }

        private async void DeleteOldUserFiles()
        {
            try
            {
                List<UploadedFilesInfo> allFiles = await _db.UploadedFilesInfo.Select(f => new UploadedFilesInfo
                {
                    UserName = f.UserName,
                    FullFilePath = f.FullFilePath,
                    FileName = f.FileName,
                    PlasticWeight = f.PlasticWeight,
                    PrintingTime = f.PrintingTime,
                    Deleted = f.Deleted,
                    CreatedOn = f.CreatedOn
                }).ToListAsync();

                for (int i = 0; i < allFiles.Count; i++)
                {
                    if (allFiles[i].CreatedOn >= allFiles[i].CreatedOn.AddDays(30))
                    {
                        string absolutePath = "";
                        string UserName = allFiles[i].UserName;
                        string fileName = allFiles[i].FileName;

                        string logsPath = "";

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            absolutePath = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads" + "\\" + UserName, Path.GetFileName(fileName).ToString());
                            System.IO.File.Delete(allFiles[i].FullFilePath); //path to delet file

                            allFiles[i].Deleted = true;
                            _db.UploadedFilesInfo.Update(allFiles[i]);
                            await _db.SaveChangesAsync();

                            //delete logs
                            logsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Logs");
                        }

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            absolutePath = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads" + "/" + UserName, Path.GetFileName(fileName).ToString());
                            System.IO.File.Delete(allFiles[i].FullFilePath); //path to delet file

                            allFiles[i].Deleted = true;
                            _db.UploadedFilesInfo.Update(allFiles[i]);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
            }
        }


        private string CreateJWCToken(Users inUser, Admins inAdmin)
        {

            
            List<Claim> claims = new List<Claim>();

            //claims for User
            if (inAdmin == null)
            {
                //UserName = ExtractUserName(inUser);
                claims.Add(new Claim(ClaimTypes.NameIdentifier, inUser.Username));
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            else if (inAdmin == null && inUser == null)
            {
                throw new Exception("No object was sent to the function");
            }
            //claims for Admin
            if (inAdmin != null && inUser == null)
            {
                //UserName = ExtractUserName(inUser);
                claims.Add(new Claim(ClaimTypes.NameIdentifier, inAdmin.UserName));
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            //get secret key from appsettings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value));
            //credientials
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //define payload of the JWT
            //the token experis in 1 day and takes signin credentials
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private async Task<bool> SendLogInEmail(string inUserName, string inPassword)
        {
            try
            {
                string verificationLink = $"http://localhost:44490/Register?username={inUserName}&password={inPassword}";

                string ver = $"http://localhost:5265/User/LogIn?inUserName={inUserName}&inPassword={inPassword}";
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("denix565@gmail.com"),
                    Subject = "Test",
                    Body = $"<a href=\"{verificationLink}\">Click here to verify and login</a> ",
                    IsBodyHtml = true
                };
                mailMessage.To.Add("s354340@oslomet.no");

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("denix565@gmail.com", "gkakhlxxzftslhyr"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {

                _log.LogInformation(ex.Message);
                return false;
            }
        }

        private async Task<bool> SendPrintingEmail(string inUserName, string inPrintName)
        {
            try
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("denix565@gmail.com"),
                    Subject = "Your print has started",
                    Body = $"Hello, we want to let you know, that you print has started. Please monitor at the website panel ",
                    IsBodyHtml = true
                };
                mailMessage.To.Add("denix565@gmail.com");

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("denix565@gmail.com", "gkakhlxxzftslhyr"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {

                _log.LogInformation(ex.Message);
                return false;
            }
        }




        public async Task<(bool, string)> LogIn(string inUserName, string inPassword)
        {
            try
            {
                if (inUserName.IsNullOrEmpty() || inUserName == "null" || inUserName =="")
                {
                    return (false, "");
                }

                //First check admin DB
                var foundAdmins = await _db.Admins.FirstOrDefaultAsync(f => f.UserName == inUserName);
                if (foundAdmins != null)
                {
                    ////Manually set admin, cannot expire
                    //if (foundAdmins.UserName == "FirstAdmin")
                    //{
                    //    var jwt = CreateJWCToken(null, foundAdmins);
                    //    return (true, jwt);
                    //}
                    //Password is invalid, send a new password to email
                    if (foundAdmins.PasswordValid == false)
                    {
                        string randomPassword = GenerateRandomPassword();
                        //Send login email to user
                        if (await SendLogInEmail(inUserName, randomPassword))
                        {
                            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

                            foundAdmins.PasswordValid = true;
                            foundAdmins.PasswordCreatedOn = centralEuTime;

                            //Hash and Salt password
                            var newAdmin = new Admins
                            {
                                UserName = inUserName,
                                Email = inUserName + "@oslomet.no"
                            };

                            //method taken  https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1.hashpassword?source=recommendations&view=aspnetcore-7.0
                            PasswordHasher<Admins> passwordHasher = new PasswordHasher<Admins>();
                            string hashedAndSaltedPassword = passwordHasher.HashPassword(newAdmin, randomPassword);
                            foundAdmins.Password = hashedAndSaltedPassword;
                            await _db.SaveChangesAsync();

                            return (true, "Password expired, new email has been sent");
                        }
                    }
                    if (foundAdmins != null && foundAdmins.PasswordValid == true)
                    {
                        PasswordHasher<Admins> passwordHasher = new PasswordHasher<Admins>();

                        var newAdmin = new Admins
                        {
                            UserName = inUserName,
                            Email = foundAdmins.Email
                        };

                        var result = passwordHasher.VerifyHashedPassword(newAdmin, foundAdmins.Password, inPassword);
                        if (PasswordVerificationResult.Success == result)
                        {
                            var jwt = CreateJWCToken(null, foundAdmins);
                            foundAdmins.PasswordValid = false;
                            await _db.SaveChangesAsync();

                            return (true, jwt);
                        }
                    }
                }
                //Check for User in DB

                var foundUser = await _db.Users.FirstOrDefaultAsync(f => f.Username == inUserName);

                //Create user if not found

                if (foundUser == null)
                {
                    CreateUser(inUserName);
                }

                //if password is expired, create a new one and send it to the users email
                if (foundUser != null && foundUser.PasswordValid == false)
                {
                    string randomPassword = GenerateRandomPassword();

                    //Send login email to user
                    if (await SendLogInEmail(inUserName, randomPassword))
                    {
                        TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

                        //generate user
                        var newUser = new Users
                        {
                            Email = inUserName + "@oslomet.no",
                            Username = inUserName,
                        };

                        //Hashs and salt random password
                        //method taken  https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1.hashpassword?source=recommendations&view=aspnetcore-7.0
                        PasswordHasher<Users> passwordHasher = new PasswordHasher<Users>();
                        string hashedAndSaltedPassword = passwordHasher.HashPassword(newUser, randomPassword);

                        foundUser.PasswordValid = true;
                        foundUser.PasswordCreatedOn = centralEuTime;
                        foundUser.Password = hashedAndSaltedPassword;

                        await _db.SaveChangesAsync();
                        return (true, "Password expired, new email has been sent");
                    }
                }

                //found users password is checked for validity and a JWT is created and sent to the frontend
                if (foundUser != null)
                {
                    PasswordHasher<Users> passwordHasher = new PasswordHasher<Users>();

                    Users userForHashChecking = new Users
                    {
                        Email = foundUser.Email,
                        Username = inPassword
                    };

                    var result = passwordHasher.VerifyHashedPassword(userForHashChecking, foundUser.Password, inPassword);
                    if (PasswordVerificationResult.Success == result)
                    {
                        var userForJWT = new Users
                        {
                            Email = foundUser.Email,
                            Username = inUserName,
                        };
                        var jwt = CreateJWCToken(userForJWT, null);

                        TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

                        //save password as used
                        foundUser.PasswordCreatedOn = centralEuTime;
                        foundUser.PasswordValid = false;
                        await _db.SaveChangesAsync();

                        return (true, jwt);
                    }
                    if(PasswordVerificationResult.Failed == result)
                    {
                        string randomPassword = GenerateRandomPassword();
                        if (await SendLogInEmail(inUserName, randomPassword))
                        {
                            //generate user
                            var newUser = new Users
                            {
                                Email = inUserName + "@oslomet.no",
                                Username = inUserName,
                            };

                            //Hashs and salt random password
                            string hashedAndSaltedPasswordUser = passwordHasher.HashPassword(newUser, randomPassword);

                            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

                            foundUser.Password = hashedAndSaltedPasswordUser;
                            foundUser.CreatedOn = centralEuTime;
                            foundUser.PasswordValid = true;
                            foundUser.PasswordCreatedOn = centralEuTime;
                            await _db.SaveChangesAsync();
                        }
                    }

                }
                return (false, "");
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return (false, "");
            }
        }




        //if (inUserName.IsNullOrEmpty() || inPassword.IsNullOrEmpty())
        //{
        //    return (false, "");
        //}



        //if (_db.Users != null)
        //{
        //    var foundUser = await _db.Users.FirstOrDefaultAsync(f => f.Username == inUserName);
        //    //Find all the shares belonging to this particular user

        //    //if password is expired, create a new one and send it to the users email
        //    if (foundUser != null && foundUser.PasswordValid == false)
        //    {
        //        string randomPassword = GenerateRandomPassword();

        //        //Send login email to user
        //        if (await SendLogInEmail(inUserName, randomPassword))
        //        {
        //            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        //            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

        //            foundUser.PasswordValid = true;
        //            foundUser.PasswordCreatedOn = centralEuTime;
        //            foundUser.Password = randomPassword;

        //            await _db.SaveChangesAsync();
        //            return (true, "Password expired, new email has been sent");
        //        }
        //    }

        //    //found users password is checked for validity and a JWT is created and sent to the frontend
        //    if (foundUser != null)
        //    {
        //        PasswordHasher<Users> passwordHasher = new PasswordHasher<Users>();

        //        Users userForHashChecking = new Users
        //        {
        //            Email = foundUser.Email,
        //            Username = inPassword
        //        };

        //        //PasswordHasher passwordHasher = new PasswordHasher<User>;
        //        var result = passwordHasher.VerifyHashedPassword(userForHashChecking, foundUser.Password, inPassword);
        //        if (PasswordVerificationResult.Success == result)
        //        {
        //            var userForJWT = new Users
        //            {
        //                Email = foundUser.Email,
        //                Username = inUserName,
        //            };
        //            var jwt = CreateJWCToken(userForJWT, null);

        //            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        //            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

        //            //save password as used
        //            foundUser.PasswordCreatedOn = centralEuTime;
        //            foundUser.PasswordValid = false;
        //            await _db.SaveChangesAsync();

        //            return (true, jwt);
        //        }
        //    }
        //    else
        //    //Check for Admin
        //    {
        //        var foundAdmins = await _db.Admins.FirstOrDefaultAsync(f => f.UserName == inUserName);
        //        //Find all the shares belonging to this particular user

        //        //Manually set admin, cannot expire
        //        if (foundAdmins.UserName == "FirstAdmin")
        //        {
        //            var jwt = CreateJWCToken(null, foundAdmins);
        //            return (true, jwt);
        //        }
        //        //Password is invalid, send a new password to email
        //        if (foundAdmins.PasswordValid == false)
        //        {
        //            string randomPassword = GenerateRandomPassword();
        //            //Send login email to user
        //            if (await SendLogInEmail(inUserName, randomPassword))
        //            {
        //                TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        //                DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

        //                foundAdmins.PasswordValid = true;
        //                foundAdmins.PasswordCreatedOn = centralEuTime;
        //                foundAdmins.Password = randomPassword;
        //                await _db.SaveChangesAsync();
        //                return (true, "Password expired, new email has been sent");
        //            }
        //        }
        //        if (foundAdmins != null && foundAdmins.PasswordValid == true)
        //        {
        //            PasswordHasher<Admins> passwordHasher = new PasswordHasher<Admins>();

        //            var newAdmin = new Admins
        //            {
        //                UserName = inUserName,
        //                Email = foundAdmins.Email
        //            };

        //            var result = passwordHasher.VerifyHashedPassword(newAdmin, foundAdmins.Password, inPassword);
        //            if (PasswordVerificationResult.Success == result)
        //            {
        //                var jwt = CreateJWCToken(null, foundAdmins);
        //                foundAdmins.PasswordValid = false;
        //                await _db.SaveChangesAsync();

        //                return (true, jwt);
        //            }
        //        }
        //    }
        //}


        public async Task<bool> CreateUser(string inUsername)
        {
            try
            {
                if (_db.Users != null)
                {
                    //string UserName = ExtractUserName(inUser.Email);
                    if (inUsername == "")
                    {
                        throw new ArgumentNullException("Username is empty");
                    }

                    //Find the user name in the datab, whole user object is taken from DB
                    var foundUser = await _db.Users.FirstOrDefaultAsync(x => x.Username == inUsername);

                    //if user is  not found, start creating the user
                    if (foundUser == null)
                    {
                        //generate random password

                        string randomPassword = GenerateRandomPassword();

                        //generate user
                        var newUser = new Users
                        {
                            Email = inUsername+"@oslomet.no",
                            Username = inUsername,
                        };

                        //Hashs and salt random password
                        //method taken  https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1.hashpassword?source=recommendations&view=aspnetcore-7.0
                        PasswordHasher<Users> passwordHasher = new PasswordHasher<Users>();
                        string hashedAndSaltedPassword = passwordHasher.HashPassword(newUser, randomPassword);

                        TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

                        newUser.Password = hashedAndSaltedPassword;
                        newUser.CreatedOn = centralEuTime;
                        newUser.PasswordValid = true;
                        newUser.PasswordCreatedOn = centralEuTime;
                        newUser.Role = Role.User;
                        //generate newUser and save the information to the DB
                        _db.Users.Add(newUser);
                        await _db.SaveChangesAsync();

                        //Send login email to user
                        if (await SendLogInEmail(inUsername, randomPassword))
                        {

                            return true;
                        }
                        else return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        private string GenerateRandomPassword()
        {
            try
            {
                //The password will be 30 characters long and is created randomly from 66 charactres
                string allowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@()";
                string[] specialCharacters = { "!", "@", "(", ")" };
                int passwordLenght = 30;
                string password = "";
                var random = new Random();

                for (int i = 0; i < passwordLenght; i++)
                {
                    password += allowedCharacters[random.Next(allowedCharacters.Length)];
                }

                bool containsChar = specialCharacters.Any(password.Contains);
                if (!containsChar)
                {
                    GenerateRandomPassword();
                }
                else return password;

                throw new Exception();

            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return GenerateRandomPassword();
            }
        }

        //Extracts userName from the email
        private string ExtractUserName(string inUserEmail)
        {
            try
            {
                int findCharIndex = inUserEmail.IndexOf('@');
                var UserName = inUserEmail.Substring(0, findCharIndex);

                return UserName;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                throw new Exception("Error in user name");
            }
        }

        private bool VerifyFile(IFormFile inFile)
        {
            try
            {
                string[] allowedExtensions = new[] { ".gcode" };
                int maxFileSizeBytes = 268435456; //265 mb

                //check if file is within size limit
                if (inFile.Length > maxFileSizeBytes)
                {
                    return false;
                }
                //cheche if file contains ending gcode
                string fileExtension = Path.GetExtension(inFile.FileName.ToLower());

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        //extracts volume of plastic used and estimated time of print
        public async Task<(int, double)> GetVolumeAndTime(Files inFile)
        {
            string printingTime = "";
            if (inFile.FilePath == null)
            {
                throw new Exception("Path can't be empty");
            }
            using (StreamReader reader = new StreamReader(inFile.FilePath))
            {
                double volume = 0;
                int seconds = 0;
                bool flag1 = false;
                bool flag2 = false;

                string text = reader.ReadToEnd();
                string[] lines = text.Split(Environment.NewLine);

                foreach (string line in lines)
                {
                    if (line.Contains("VOLUME_USED"))
                    {
                        string splitVolume = line.Split(':')[1].Trim();
                        volume = Convert.ToDouble(splitVolume);
                        flag1 = true;
                    }
                    if (line.Contains("PRINT.TIME"))
                    {
                        string splitTime = line.Split(':')[1].Trim();
                       // TimeSpan printingTimeSpan = TimeSpan.Parse(splitTime);
                       seconds = Convert.ToInt32(splitTime);

                        //in case there is some error in the G code, the time to print will be displayed as 0
                        if (seconds < 0 || volume <= 0)
                        {
                            throw new Exception("Error in the Gcode file");
                        }
                        flag2 = true;
                    }
                    if (flag1 && flag2)
                    {
                        return (seconds, volume);
                    }
                }
                return (seconds, volume);
            }
        }


        public async Task<(bool, double, double)> Upload(IFormFile inFile, string userName)
        {
            try
            {
                if (userName == null || userName =="" || userName =="null")
                {
                    return (false, 0, 0);
                }
                string aboslutePaht = "";
                bool streamFlag = false;

                if (!VerifyFile(inFile))
                {
                    return (false,0,0);
                }
                else
                {
                    //CHECK IF THE USER EXISTS (has to be added later )!!!!!

                    //create folder if directory dosent exists
                    //tring path = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp\\InitialUpload");
                    //string path = "C:\\Users\\Denis\\Desktop\\AbsoluteUploadPath";

                    //Create main folder on the server -- this should mybe be generated once at startup insted of each time
                    string path = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads");


                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //if the manin directory exists then create user folder within it
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        if (Directory.Exists(path))
                        {   //if the user directory dosent exist, then create it and upload the file to it
                            if (!Directory.Exists(path + "/" + userName))
                            {
                                Directory.CreateDirectory(path + "/" + userName);
                            }
                            aboslutePaht = Path.Combine(path + "/" + userName, Path.GetFileName(inFile.FileName)).ToString();
                        }
                    }

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (Directory.Exists(path))
                        {   //if the user directory dosent exist, then create it and upload the file to it
                            if (!Directory.Exists(path + "\\" + userName))
                            {
                                Directory.CreateDirectory(path + "\\" + userName);
                            }
                            aboslutePaht = Path.Combine(path + "\\" + userName, Path.GetFileName(inFile.FileName)).ToString();
                        }
                    }

                    /*
                    await using FileStream fs = new(path, FileMode.Create);
                    await inFile.CopyToAsync(fs);
                 */
                    //var path2 = Path.GetTempFileName();
                    //aboslutePaht = Path.Combine(path + "\\"+ userName, Path.GetFileName(inFile.FileName)).ToString();
                    // This is how the path has to look for the streamer not to give you a Access denied error, the file name has to be included in the path
                    //var testPaht = "C:\\Users\\Denis\\Desktop\\Bachelors Project\\GitHub\\Bachelors-Project\\BachelorsProject\\BachelorsProject\\uploads\\UM3E_pi-HDMI-CASE.gcode";



                    //take file from stream and save it                   
                    using (var fileStream = new FileStream(aboslutePaht, FileMode.Create))
                    {
                        await inFile.CopyToAsync(fileStream);
                        streamFlag = true;
                    }

                    Files files = new Files
                    {
                        FilePath = aboslutePaht
                    };


                    if (streamFlag)
                    {
                        //get volume and time estimate from file
                        (int, double) getVolumeAndTime = await GetVolumeAndTime(files);

                        //Convert from PlasticWeight (g/mm^3) to g 
                        //densityPLA = 1.24 g/cm^3
                        // g = volume (g/mm^3) * densityPLA(g/cm^3) * 0.001
                        double PlasticWeight = getVolumeAndTime.Item2 * 1.24 * 0.001;

                        UploadedFilesInfo newUploadedFilesInfo = new UploadedFilesInfo
                        {
                            UserName = userName,
                            FullFilePath = aboslutePaht,
                            FileName = Path.GetFileName(inFile.FileName),
                            PrintingTime = getVolumeAndTime.Item1,
                            PlasticWeight = PlasticWeight,
                            Deleted = false
                        };

                        await _db.UploadedFilesInfo.AddAsync(newUploadedFilesInfo);
                        await _db.SaveChangesAsync();

                        await AddFileToPrintingQueue(userName, Path.GetFileName(inFile.FileName));

                        return (true, PlasticWeight, getVolumeAndTime.Item1);
                    }
                    return (false, 0, 0);
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return (false, 0, 0);
            }
        }

        public async Task<bool> CancelQueuePrint(string inFileName, string inUserName)
        {
            try
            {
                var printToRemove =await _db.PrintingQueues.FirstOrDefaultAsync(x=> x.UserName == inUserName && x.FileName == inFileName);
                if(printToRemove != null)
                {
                    _db.PrintingQueues.Remove(printToRemove);
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<List<PrintingQueue>> GetPrintingQueue()
        {
            try
            {
                var GetPrintingQueue = await _db.PrintingQueues.ToListAsync();
                if (GetPrintingQueue != null)
                {
                    return GetPrintingQueue;
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }

        public async Task<List<PrintingQueue>> GetPrintingQueueForUser(string inUserName)
        {
            try
            {
                var userPrintingQueue = await _db.PrintingQueues.Where(x => x.UserName == inUserName).ToListAsync();
                if (GetPrintingQueueForUser != null)
                {
                    return userPrintingQueue;
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }

        public async Task<(double,double)> ReturnPrintTimeAndPlastic(string inFileName, string inUserName)
        {
            try
            {
                var information = await _db.UploadedFilesInfo.FirstOrDefaultAsync(x => x.UserName == inUserName && x.FileName == inFileName);
                if(information != null)
                {
                    return (information.PrintingTime, information.PlasticWeight);
                }
                return (0, 0); 
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return (0, 0);
            }
        }




        public async Task<bool> DeleteFile(User inUser, string fileName)
        {
            string absoluteTime = "";
            string UserName = ExtractUserName(inUser.Email);


            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                absoluteTime = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads" + "\\" + UserName, Path.GetFileName(fileName)).ToString();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                absoluteTime = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads" + "/" + UserName, Path.GetFileName(fileName)).ToString();
            }

            //define path to file

            // aboslutePaht = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads" + "\\" + inUser.Username, Path.GetFileName(fileName)).ToString();
            //Find file path in DBf
            if (_db.UploadedFilesInfo != null) //check if the DB was constructed
            {
                var fileInDB = await _db.UploadedFilesInfo.FirstOrDefaultAsync(x => x.FileName == fileName + ".gcode");

                //in case the object returned from DB is null
                if (fileInDB == null)
                {
                    return false;
                }
                //check if the file name is rgistered to the claimed user
                if (fileInDB.UserName == UserName)
                {
                    System.IO.File.Delete(fileInDB.FullFilePath); //path to delet file
                    await _db.SaveChangesAsync(fileInDB.Deleted = true); ////the delete indicates if the file was deleted, true yes, false no

                    return true;
                }
                else return false;
            }
            return false;
        }


        public async Task<bool> GetFileInformation(Files inFile)
        {
            //double volume = 0;
            //double printingTime = 0;
            //bool flag1 = false;
            //bool flag2 = false;

            if (inFile.FileName == null || inFile.FilePath == null)
            {
                throw new ArgumentNullException("File information is empty");
            }

            if (Path.GetExtension(inFile.FilePath) == ".gcode")
            {
            }
            else
            {
                throw new ArgumentException("File is not a gcode file");
            }

            try
            {
                (int, double) getVolumeAndTime = await GetVolumeAndTime(inFile);

                if (getVolumeAndTime.Item1 == 0 || getVolumeAndTime.Item2 == 0)
                {
                    throw new ArgumentNullException("The file is empty");
                }
                else return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred while reading the gcode file. Exception: " + ex.Message);
                _log.LogInformation("FileID: " + inFile.Id + "\nFileName: " + inFile.FileName + "\nFilePath: " + inFile.FilePath
                    + "\nError: " + ex);
                return false;
            }
        }
        //DO NOT DELETE
        /*
        public async Task<string> PrinterCameraFeed(User inUser)
            //Take in UserID, check if he is printing sometign,
            //if yes search DB for the printer he is using and fetch the ip and return the stream string
        {
            string resultString = "";
            var findUser= new Users();
            var findPrinter = new PrinterList();
            try
            {
                if(inUser.Username == null)
                {
                    throw new ArgumentNullException("User is empty");
                }
                else
                {
                    findUser = await _db.Users.FirstOrDefaultAsync(x => x.Username == inUser.Username);
                    findPrinter = await _db.PrinterLists.FirstOrDefaultAsync(x => x.PrinterUsedBy == findUser.Username);

                    //finds all printers used by the username, decide if want to keep
                    findPrinter = await _db.PrinterLists.FindAsync(inUser.Username);

                    if (findUser != null && findPrinter != null)
                    {
                        resultString = findPrinter.PrinterIP + ":" + findPrinter.PrinterPort + "/?action=stream";
                    }
                }
                return resultString;
            }
            catch (Exception ex)
            catch (Exception ex)
            {
                
                _log.LogInformation("Printer: " + findPrinter.Id + "\nFileName: " + findPrinter.PrinterName + "\nFilePath: " + findPrinter.PrinterIP
                    + "\nError: " + ex);
                return ("Printer: " + findPrinter.Id + "\nFileName: " + findPrinter.PrinterName + "\nFilePath: " + findPrinter.PrinterIP
                    + "\nError: " + ex);
            }


        }*/
        public async Task<bool> CheckStatusOfAllPrinters()  //finish fun
        {
            try
            {
                var allPrinters = await _db.ListOfPrinters.ToListAsync();

                for (int i = 0; i <= allPrinters.Count; i++)
                {
                    string url = $"http://{allPrinters[i].IP}";
                    string clinetAddress = $"http://{allPrinters[i].IP}/api/v1/printer/status";

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(clinetAddress);
                 
                        string status = await response.Content.ReadAsStringAsync();
                        string cleandedStatus = status.Replace("\"", "");
                        ListOfPrinters chagedPrinter = allPrinters[i];
                        chagedPrinter.PrinterStatus = cleandedStatus;

                        _db.ListOfPrinters.Update(chagedPrinter);
                        await _db.SaveChangesAsync();

                        //if (response.IsSuccessStatusCode) //header has to be 200 to check the status
                        //{
                        //    string status = await response.Content.ReadAsStringAsync();
                        //    if (status == "idle" || status == "printing" || status == "error" || status == "maintenance" || status == "booting")
                        //    {
                        //        ListOfPrinters chagedPrinter = allPrinters[i];
                        //        chagedPrinter.PrinterStatus = status;

                        //        _db.ListOfPrinters.Update(chagedPrinter);
                        //        await _db.SaveChangesAsync();
                        //    }
                        //    //return true; //remove if not lopp breaks
                        //} 
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }


        //adds a users print task to the Queue
        public async Task<string> AddFileToPrintingQueue(string inUserName, string inFileName) //ADD to printer que
        {
            //string searchAvaliablePrinter = "Available";
            try
            {
                if (inUserName != null || inFileName != null)
                {
                    var userResponse = await _db.Users.FirstOrDefaultAsync(x => x.Username == inUserName);
                    var fileResponse = await _db.UploadedFilesInfo.FirstOrDefaultAsync(x => x.FileName == inFileName);


                    if (userResponse.Username == null)
                    {
                        throw new ArgumentNullException("User" + inUserName + "  could not be found in DB");
                    }
                    if (fileResponse.FileName == null)
                    {
                        throw new ArgumentNullException("File name could not be found in DB for user: " + inUserName);
                    }

                    //add the selected Print with info to the queue
                    PrintingQueue print = new PrintingQueue
                    {
                        UserName = fileResponse.UserName,
                        FileName = fileResponse.FileName,
                        PrintTime = fileResponse.PrintingTime.ToString(),
                        PlasticWeight = fileResponse.PlasticWeight
                    };

                    await _db.PrintingQueues.AddAsync(print); //added await check if it works
                    await _db.SaveChangesAsync();

                    //to BE REMOVED FOR PRODUCTION
                    SelectPrintAndSend();


                    /*
                    if (userResponse != null || fileResponse != null) {
                        //select all printers from the list
                        List<ListOfPrinters> avaliablePrinters = await _db.ListOfPrinters.Select(f => new ListOfPrinters
                        {
                            PrinterName = f.PrinterName,
                            PrinterIP = f.PrinterIP,
                            PrinterPort = f.PrinterPort,
                            Avaliable = f.Avaliable,
                            PrinterUsedBy = f.PrinterUsedBy,

                        }).ToListAsync();
                       
                        //find all avalible printers
                        List<ListOfPrinters> printsPerPrinter = avaliablePrinters.FindAll(f => f.Avaliable == searchAvaliablePrinter);
                        //determine size of list
                        var lenghtOfList = printsPerPrinter.Count;

                        if(lenghtOfList > 1)
                        {
                            //randomly select the a printer, prvent overuse of 1 printer       
                            Random random = new Random();

                            int randomPrinter = random.Next(0, printsPerPrinter.Count);
                            var selectedPrinter = printsPerPrinter[randomPrinter];

                            //Send to print returns true if it managed to post the print to the selected printer
                            if(await SendToPrinter(inUserName, inFileName, selectedPrinter.PrinterName))
                            {
                                return "Your print was sucessufly sned to the printer";
                            }
                        }
                    }*/
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return "Error in the printer Queue";
            }
            return "General Printer queue error";
        }


        private async void SelectPrintAndSend() //should be checked every 5 min //mybe the admin should unlock each printer after print
        {
            try
            {
                //Prevents prints to be sent on weekends
                TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);
                
                //prevents prints beeing posted on weekdays
                if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday || (DateTime.Today.Day == 17 && DateTime.Today.Month == 5)) {
                    return;
                }
                if (centralEuTime.TimeOfDay >= new TimeSpan(17,0,0) || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    return;
                }
                //Select all printers from DB
                /* List<ListOfPrinters> allPrinters = await _db.ListOfPrinters.Select(f => new ListOfPrinters
                 {
                     PrinterName = f.PrinterName,
                     PrinterStatus = f.PrinterStatus,
                     PrinterAdminLock = f.PrinterAdminLock
                 }).ToListAsync();*/


                //Find all avalible printers on the list
                List<ListOfPrinters> listOfAvailablePrinters = await _db.ListOfPrinters.Where(x => x.PrinterStatus.Contains("idle")&& x.PrinterAdminLock == false).ToListAsync();

                //if there are no avalialbe printers, break the loop
                if (!listOfAvailablePrinters.IsNullOrEmpty())
                {
                    //Select task from the task queueu
                    //List<PrintingQueue> allQueue = await _db.PrintingQueues.Select(f => new PrintingQueue
                    //{
                    //    UserName = f.UserName,
                    //    FileName = f.FileName,
                    //    PrintTime = f.PrintTime
                    //}).ToListAsync();

                    var allQueue = await _db.PrintingQueues.ToListAsync();
                    if(allQueue.IsNullOrEmpty()) {
                        return;
                    }

                    //check that the queue is not empty
                    if (!allQueue.IsNullOrEmpty())
                    {
                        //Returns the first  element of the list form the queue
                        PrintingQueue selectedPrint = allQueue.First();

                        //When there are more printers on the list than one
                        if (listOfAvailablePrinters.Count > 1)
                        {
                            //randomly select the a printer, prvent overuse of 1 printer       
                            Random random = new Random();

                            int randomPrinter = random.Next(0, listOfAvailablePrinters.Count);
                            ListOfPrinters randomlySelectedPrinter = listOfAvailablePrinters[randomPrinter];

                            //Send to print returns true if it managed to post the print to the selected printer
                            if (await SendPrintingJob(selectedPrint.UserName, selectedPrint.FileName, randomlySelectedPrinter.PrinterName))
                            //if(true)
                            {
                                //lock the printer until admin unlocks it
                                //listOfAvailablePrinters[randomPrinter].PrinterAdminLock = true;
                                DateTime centralEuTimeToSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);
                                //print has been sent, move data to printingHistory
                                PrintingHistory newPrintingHistory = new PrintingHistory
                                {
                                    UserName = selectedPrint.UserName,
                                    FileName = selectedPrint.FileName,
                                    PrinterName = randomlySelectedPrinter.PrinterName,
                                    PlasticWeight = selectedPrint.PlasticWeight,
                                    StartedPrintingAt = centralEuTimeToSave,
                                    FinishedPrintingAt = centralEuTimeToSave.AddSeconds(Double.Parse(selectedPrint.PrintTime))
                                };

                                var printingHistory = await _db.PrintingHistory.AddAsync(newPrintingHistory);

                                //remove print job from the queue, as it has been sent to printer
                                //allQueue.Remove(selectedPrint);
                                _db.PrintingQueues.Remove(selectedPrint);
                                await _db.SaveChangesAsync();

                                //send email to user, print has started
                                await SendPrintingEmail(selectedPrint.UserName, selectedPrint.FileName);

                                //log information about the print
                                _log.LogInformation($"Print -{selectedPrint.UserName} - has been sent to printer {randomlySelectedPrinter.PrinterName}");

                            }
                        }
                        else
                        {
                            //Send UserName, FileName to be printed and the selected printer name to the printing function
                            if (await SendPrintingJob(selectedPrint.UserName, selectedPrint.FileName, listOfAvailablePrinters[0].PrinterName))
                            //if with true for testing REMOVE LATER
                            //if(true)
                            {
                                DateTime centralEuTimeToSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);
                                PrintingHistory newPrintingHistory = new PrintingHistory
                                {
                                    UserName = selectedPrint.UserName,
                                    FileName = selectedPrint.FileName,
                                    PrinterName = listOfAvailablePrinters[0].PrinterName,
                                    PlasticWeight = selectedPrint.PlasticWeight,
                                    StartedPrintingAt = centralEuTimeToSave,
                                    FinishedPrintingAt = centralEuTimeToSave.AddSeconds(Double.Parse(selectedPrint.PrintTime))
                                };

                                var printingHistory = await _db.PrintingHistory.AddAsync(newPrintingHistory);

                               _db.PrintingQueues.Remove(selectedPrint);

                                await _db.SaveChangesAsync();
                                //send email to user, print has started
                                await SendPrintingEmail(selectedPrint.UserName, selectedPrint.FileName);

                                //listOfAvailablePrinters[0].PrinterAdminLock = true;
                                _log.LogInformation("Print -{selectedPrint.UserName} - has been sent to printer {listOfAvailablePrinters[0].PrinterName}");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
            }
        }

        public async Task<bool> SendPrintingJob(string inUserName, string inFileName, string inPrinterName)
        {
            try
            {
                if (_db.Users != null && _db.UploadedFilesInfo != null)
                {
                    Users userResponse = await _db.Users.FirstOrDefaultAsync(x => x.Username == inUserName);
                    UploadedFilesInfo fileInfo = await _db.UploadedFilesInfo.FirstOrDefaultAsync(x => x.FileName == inFileName);
                    var selectedPrinter = await _db.ListOfPrinters.FirstOrDefaultAsync(x => x.PrinterName == inPrinterName);




                    if (userResponse.Username == null || fileInfo.FileName == null) //check if it is null here or ""
                    {
                        throw new ArgumentNullException("User name could not be found in DB");
                    }
                    else
                    {
                        //select printing job

                        //select avalible 3D printer
                        //string url =  $"http://100.127.254.230/api/v1/print_job"; //move to DB 
                        //string apiKey = "bce85abf98d196883c467e6e130e6603";  //move to DB (moved)
                        //string apiSecret = "6a0daf0a3067d3a2e5fb7752d03cf64970b45176d2ea19889eb9e1415c9bea25"; //move to db (moved)

                        string url = $"http://{selectedPrinter.IP}/api/v1/print_job"; //move to DB 
                        string jobName = fileInfo.FileName;
                        string filePath = fileInfo.FullFilePath;

                        using var handler = new HttpClientHandler
                        {
                            Credentials = new NetworkCredential(selectedPrinter.ApiKey, selectedPrinter.ApiSecret)
                        };

                        //using is used to destroy the variables content after use, prevent memory leeks
                        using HttpClient client = new HttpClient(handler);

                        using var fileStream = new FileStream(filePath, FileMode.Open);
                        using var content = new MultipartFormDataContent();
                        using var fileContent = new StreamContent(fileStream);

                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                        content.Add(fileContent, "file", jobName);

                        var response = await client.PostAsync(url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            return true;
                        }
                        // return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation("An error occurred while sending the print job. Exception: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> CancelOngoingPrint(string inUserName, string inFileName)
        {
            string inCommand = "cancel";
            var printerIp = "";
            try
            {
                if (inCommand.ToLower() == "cancel")
                {
                    //Select all printers from DB
                    List<PrintingQueue> totalQueue = await _db.PrintingQueues.Select(f => new PrintingQueue
                    {
                        UserName = f.UserName,
                        FileName = f.FileName
                    }).ToListAsync();

                    if (!totalQueue.IsNullOrEmpty())
                    {
                        PrintingQueue printingJob = totalQueue.FirstOrDefault(f => f.UserName == inUserName && f.FileName == inFileName);
                        if (printingJob != null)
                        {

                            totalQueue.Remove(printingJob);
                            await _db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        List<PrintingHistory> printHistory = await _db.PrintingHistory.Select(f => new PrintingHistory
                        {
                            UserName = f.UserName,
                            FileName = f.FileName,
                            PrinterName = f.PrinterName
                        }).ToListAsync();

                        if (!printHistory.IsNullOrEmpty())
                        {
                            var printerInformation = printHistory.FirstOrDefault(x => x.UserName == inUserName && x.FileName.Contains(inFileName));
                            //take out the printer name and IP
                            if (!printHistory.IsNullOrEmpty())
                            {
                                var foundPrinter = await _db.ListOfPrinters.FirstOrDefaultAsync(x => x.PrinterName == printerInformation.PrinterName);
                                printerIp = foundPrinter.IP;
                            }
                        }

                        using (var httpClient = new HttpClient())
                        {
                            //using (var webRequest = new HttpRequestMessage(new HttpMethod("PUT"), "http://100.127.254.230/api/v1/print_job/state"))

                            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), $"http://{printerIp}/api/v1/print_job/state")) //move to DB, check which user uses which printer
                            {
                                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                                request.Content = new StringContent("abort");
                                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                                HttpResponseMessage response = await httpClient.SendAsync(request);
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }

        }

        //////////////////ADMIN /////////////////////
        // print history per user:

        //unlock function for printer
        public async Task<bool> UnlockAndLockPrinter(string PrinterName, string inCommand)
        {
            try
            {
                var foundPrinter = await _db.ListOfPrinters.FirstOrDefaultAsync(x => x.PrinterName == PrinterName);

                if (foundPrinter != null && inCommand != "")
                {
                    if (inCommand == "lock")
                    {
                        foundPrinter.PrinterAdminLock = true;
                        await _db.SaveChangesAsync();
                        return true;
                    }
                    else if (inCommand == "unlock")
                    {
                        foundPrinter.PrinterAdminLock = false;
                        await _db.SaveChangesAsync();
                        return true;
                    }
                    return false;
                    
                }
                else return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<List<string>> GetAllUsers()
        {
            try
            {
                var allUsers = await _db.Users.ToListAsync();
                if (allUsers.IsNullOrEmpty())
                {
                    return null;
                }
                else return allUsers.Select(x => x.Username).ToList();
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }   
        }

        public async Task<List<string>> GetAllAdmins ()
        {
            try
            {
                var allAdmins = await _db.Admins.ToListAsync();
                if (allAdmins.IsNullOrEmpty())
                {
                    return null;
                }
                else return allAdmins.Select(x => x.UserName).ToList();
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }



        public async Task<bool> CreateAdmin(Admin inAdmin)
        {
            try
            {
                if (_db.Users != null)
                {
                    string UserName = ExtractUserName(inAdmin.Email);

                    //Find the user name in the datab, whole user object is taken from DB
                    var foundUser = await _db.Admins.FirstOrDefaultAsync(x => x.UserName == UserName);

                    //if user is  not found, start creating the user
                    if (foundUser == null)
                    {
                        string extractedUserName = ExtractUserName(inAdmin.Email);
                        //generate random password
                        string randomPassword = GenerateRandomPassword();
                        if (randomPassword.IsNullOrEmpty())
                        {
                            throw new Exception("Problem with password generator");
                        }

                        //generate user
                        var newAdmin = new Admins
                        {
                            UserName = extractedUserName,
                            Email = inAdmin.Email
                        };

                        //Hashs and salt random password
                        //method taken  https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1.hashpassword?source=recommendations&view=aspnetcore-7.0
                        PasswordHasher<Admins> passwordHasher = new PasswordHasher<Admins>();
                        string hashedAndSaltedPassword = passwordHasher.HashPassword(newAdmin, randomPassword);

                        TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

                        newAdmin.Password = hashedAndSaltedPassword;
                        newAdmin.PasswordCreatedOn = centralEuTime;
                        newAdmin.CreatedOn = centralEuTime;
                        newAdmin.PasswordValid = true;
                        newAdmin.Role = Role.Admin;

                        //generate newUser and save the information to the DB

                        //Send login email to user
                        if (await SendLogInEmail(inAdmin.UserName, randomPassword))
                        {
                            await _db.Admins.AddAsync(newAdmin);
                            await _db.SaveChangesAsync();
                            return true;
                        }
                        else return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<List<PrintingHistory>> PrintingHistoryForUser(string inUserName)  //change to entity framework
        {
            try
            {
                List<PrintingHistory> allPrintingHistory = await _db.PrintingHistory.ToListAsync();
                List<PrintingHistory> historyForUser = allPrintingHistory.FindAll(f => f.UserName == inUserName);

                return historyForUser;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }


        // DeleteUser by ADMIN.

        public async Task<bool> DeleteUser(string inUserName)
        {
            try
            {
                if (!inUserName.IsNullOrEmpty())
                {
                    //var userToDelete = await _db.Users.FirstOrDefaultAsync(x => x.Username == inUserName);
                    var userToDelete = await _db.Users.FirstOrDefaultAsync(x => x.Username == inUserName);

                    if (userToDelete == null)
                    {
                        return false;
                    }
                    else
                    {
                        _db.Users.Remove(userToDelete);
                        await _db.SaveChangesAsync();

                        return true;
                    }
                } return false;

            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAdmin(string inUserName)
        {
            try
            {
                if(!inUserName.IsNullOrEmpty())
                {
                    var userToDelete = await _db.Admins.FirstOrDefaultAsync(x => x.UserName == inUserName);

                    if (userToDelete == null)
                    {
                        return false;
                    }
                    else
                    {
                        _db.Admins.Remove(userToDelete);
                        await _db.SaveChangesAsync();

                        return true;
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        ////DELET
        //public async Task<bool> ChangeUserInformation(User inUser)
        //{
        //    try
        //    {
        //        string UserName = ExtractUserName(inUser.Email);

        //        var userInfo = await _db.Users.FirstOrDefaultAsync(x => x.Username == UserName);
        //        if (userInfo != null)
        //        {
        //            await _db.SaveChangesAsync();
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogInformation(ex.Message);
        //        return false;
        //    }
        //}





        //////////////////ADMIN /////////////////////
        //Moh
        // Create a new printer
        public async Task<bool> CreatePrinter(Printer CreatePrinter)
        {
            try
            {
                if(CreatePrinter.PrinterName == "" || CreatePrinter.PrinterName == "null")
                {
                    return false;
                }
                var printerInformation = await _db.ListOfPrinters.FirstOrDefaultAsync(x => x.PrinterName == CreatePrinter.PrinterName);
                if(printerInformation == null)
                {
                    ListOfPrinters newPrinter = new ListOfPrinters();
                    newPrinter.PrinterName = CreatePrinter.PrinterName;
                    newPrinter.IP = CreatePrinter.IP;
                    newPrinter.ApiSecret = CreatePrinter.ApiSecret;
                    newPrinter.ApiKey = CreatePrinter.ApiKey;
                    newPrinter.PrinterAdminLock = false;

                    _db.ListOfPrinters.Add(newPrinter);

                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }


        public async Task<List<ListOfPrinters>> GetAllPrinters()
        {
            try
            {
                var allPrinters = await _db.ListOfPrinters.ToListAsync();
                if (allPrinters != null)
                {
                    return allPrinters;
                }
                else return null;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdatePrinter(Printer inPrinter)
        {
            try
            {
                var printerIn = await _db.ListOfPrinters.FirstOrDefaultAsync(x => x.PrinterName == inPrinter.PrinterName);
                if (printerIn != null)
                {
                    //printerIn.PrinterName = inPrinter.PrinterName;
                    printerIn.IP = inPrinter.IP;
                    printerIn.ApiKey = inPrinter.ApiKey;
                    printerIn.ApiSecret = inPrinter.ApiSecret;

                    await _db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeletePrinter(string inPrinterName)
        {
            try
            {
                var printer = await _db.ListOfPrinters.FirstOrDefaultAsync(x => x.PrinterName == inPrinterName);
                if (printer == null)
                {
                    throw new ArgumentNullException("Printer couldnt be found");
                }

                _db.ListOfPrinters.Remove(printer);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<List<PrintingHistory>> ReturnPrintsPerPrinter(string inPrinterName)
        {
            try
            {
                var allPrintedFilePerPrinter = await _db.PrintingHistory.Where(x => x.PrinterName.Equals(inPrinterName)).ToListAsync();
                if (!allPrintedFilePerPrinter.IsNullOrEmpty())
                {
                    return allPrintedFilePerPrinter;
                }
                return null;

            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }

        //DELETEEEEEEEEEEEEEEEEE
        public async Task<double> ReturnPlasticUsedForPrinter(string inPrinterName, string inCommand)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(inPrinterName))
                {
                    if (inCommand.ToLower() == "month")
                    {
                        double monthlyPlasticUseGrams = 0;
                        List<PrintingHistory> data = await _db.PrintingHistory.Where(x => x.StartedPrintingAt.Month == DateTime.Now.Month && x.PrinterName == inPrinterName).ToListAsync();
                        for (int i = 0; i < data.Count ; i++)
                        {
                            monthlyPlasticUseGrams += data[i].PlasticWeight;
                        }
                        return monthlyPlasticUseGrams;
                    }
                    else if (inCommand.ToLower() == "year")
                    {
                        double yearlyPlasticUse = 0;
                        List<PrintingHistory> data = await _db.PrintingHistory.Where(x => x.StartedPrintingAt.Year == DateTime.Now.Year && x.PrinterName == inPrinterName).ToListAsync();
                        for (int i = 0; i < data.Count; i++)
                        {
                            yearlyPlasticUse += data[i].PlasticWeight;
                        }
                        return yearlyPlasticUse;
                    }
                    else return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return 0;
            }
        }

        //FOR AMDIN to SET Q&A
        public async Task<bool> MakeNewQAndA(QandA inQandA)
        {
            try
            {
                if(inQandA != null)
                {
                    QandA newQandA = new QandA
                    {
                        Question = inQandA.Question,
                        Answers = inQandA.Answers
                    };

                    await _db.QandA.AddAsync(newQandA);
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
       
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }
        public async Task<List<QandA>> GetQAndA()
        {
            try
            {
                var result = await _db.QandA.ToListAsync();
                if (!result.IsNullOrEmpty())
                {
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }

        public async Task<bool> EditQAndA(QandA inQandA)
        {
            try
            {
                if(inQandA != null)
                {
                    var findObject = await _db.QandA.FirstOrDefaultAsync(x => x.Id == inQandA.Id);
                    if (findObject != null)
                    {
                        findObject.Question = inQandA.Question;
                        findObject.Answers = inQandA.Answers;

                        await _db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteQAndA(int id)
        {
            try
            {
                var findObject = await _db.QandA.FirstOrDefaultAsync(x => x.Id == id);
                if (findObject != null)
                {
                    _db.QandA.Remove(findObject);

                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }


    }
}






                /*
                List<PrintFile> returnAllPrintsPerPrinter = await _db.PrintFiles.
                    Where(f => f.PrinterName == inPrinterName).Select(f => new
                    {
                        PrinterName = f.PrinterName,
                        PrintTime = f.PrintTime,
                        Username = f.Owner,
                        FileName = f.FileName
                    }).ToListAsync(); */

                /*var files = await _db.PrintFiles
                    .Where(f => f.PrinterName == inPrinterName)
                    .Select(f => new
                    {
                        PrinterName = f.PrinterName,
                        PrintTime = f.PrintTime,
                        Username = f.Owner,
                        FileName = f.FileName
                    })
                    .ToListAsync();*/
  










