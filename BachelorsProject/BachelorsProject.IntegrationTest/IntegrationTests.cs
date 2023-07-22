using BachelorsProject.Controllers;
using BachelorsProject.DAL;
using BachelorsProject.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;
using NuGet.Common;
using System.Drawing;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using WebApiIntegrationTesting.IntegrationTests.ControllerTests;
using System.Xml.Linq;
using PrintingHistory = BachelorsProject.DAL.PrintingHistory;
using Newtonsoft.Json.Linq;
using QandA = BachelorsProject.DAL.QandA;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BachelorsProject.IntegrationTest
{
    public class IntegrationTests : IDisposable
    {

        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private readonly ILoggerFactory _loggerFactory;

        public IntegrationTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _loggerFactory = LoggerFactory.Create(builder => // Create an instance of ILoggerFactory
            {
                builder.AddConsole();
            });


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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Abdubi2b43123dasdws34d"));
            //credientials
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //define payload of the JWT
            //the token experis in 1 day and takes signin credentials
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        //////////////////////////////////////////////////////ADMIN INTEGRATION TESTS////////////////////////////////////////////////////
        private string adminJWT()
        {
            Admins adminForJWT = new Admins
            {
                Email = "AdminToken@oslomet.no",
                UserName = "AdminToken"

            };
            var token = CreateJWCToken(null, adminForJWT);
            return token;
        }


        //CreateAdmin
        [Fact]
        public async Task CreateAdmin_ValidJWT()
        {
            string message = "Admin was successfully created.";
            var token = adminJWT();

            Admin inNewAdmin = new Admin
            {
                Email = "AdminIntegrationTest@oslomet.no"
            };

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.CreateAdmin(It.Is<Admin>(b => b.Email == inNewAdmin.Email))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(inNewAdmin);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/CreateAdmin", httpContent);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task CreateAdmin_ValidJWT_InvalidData()
        {
            string message = "Admin could not be created.";
            var token = adminJWT();

            Admin inNewAdmin = new Admin
            {
            };

            Task<bool> task = Task.FromResult(false);

            _factory.IsystemRepositoryMock.Setup(r => r.CreateAdmin(It.Is<Admin>(b => b.Email == "AdminIntegrationTest@oslomet.no"))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(inNewAdmin);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/CreateAdmin", httpContent);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task CreateAdmin_InvalidJWT()
        {
            var token = "invalidToken";

            Admin inNewAdmin = new Admin
            {
                Email = "AdminIntegrationTest@oslomet.no"
            };


            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(inNewAdmin);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/CreateAdmin", httpContent);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Login Admin

        [Fact]
        public async Task LoginAdmin_GenerateAndReturnJWT_LogIn()
        {
            string inUserName = "Admin";
            string inPassword = "PasswordAdmin";
            var token = adminJWT();
    
            _factory.IsystemRepositoryMock.Setup(r => r.LogIn(inUserName, inPassword)).ReturnsAsync((true, token));

            var response = await _client.PostAsync($"/User/LogIn?inUserName={inUserName}&inPassword={inPassword}", null);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(token, responseObject["message"].ToString());
        }
        [Fact]
        public async Task LoginAdmin_ExpiredPassword()
        {
            string message = "Password expired, new email has been sent";
            string inUserName = "Admin";
            string inPassword = "PasswordAdmin";

            _factory.IsystemRepositoryMock.Setup(r => r.LogIn(inUserName, inPassword)).ReturnsAsync((true, message));

            var response = await _client.PostAsync($"/User/LogIn?inUserName={inUserName}&inPassword={inPassword}", null);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task Admin_WrongPasswordAndUser()
        {
            string message = "Username or password is wrong, a new email has been sent.";
            string inUserName = "Admin";
            string inPassword = "";

            _factory.IsystemRepositoryMock.Setup(r => r.LogIn(inUserName, inPassword)).ReturnsAsync((false, ""));

            var response = await _client.PostAsync($"/User/LogIn?inUserName={inUserName}&inPassword={inPassword}", null);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }

        //DeleteUser
        [Fact]
        public async Task DeleteUser_ValidJWT()
        {

            string message = "User successfully deleted.";
            var token = adminJWT();
            string inUserName = "IntegrationTest";

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.DeleteUser(inUserName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeleteUser?userName={inUserName}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task DeleteUser_InvalidJWT()
        {
            var token = "Invalid Jwt";
            string inUserName = "IntegrationTest";

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.DeleteUser(inUserName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeleteUser?userName={inUserName}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        //DeleteAdmin
        [Fact]
        public async Task DeleteAdmin_ValidJWT()
        {

            string message = "Admin was successfully deleted.";
            var token = adminJWT();
            string inAdminName = "AdminTest";

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.DeleteAdmin(inAdminName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeleteAdmin?inAdminName={inAdminName}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task DeleteAdmin_InvalidJWT()
        {
            var token = "Invalid Jwt";
            string inAdminName = "AdminTest";

            Task<bool> task = Task.FromResult(false);

            _factory.IsystemRepositoryMock.Setup(r => r.DeleteAdmin(inAdminName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeleteAdmin?inAdminName={inAdminName}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        ////Change User Information
        //[Fact]
        //public async Task ChangeUserInformation_ValidJWT()
        //{
        //    string message = "User information updated successfully.";
        //    User updateUser = new User
        //    {
        //        FirstName = "IntegrationTestChanged",
        //        LastName = "Test",
        //        Email = "IntegrationTest@oslomet.no"
        //    };
        //    var token = adminJWT();

        //    Task<bool> task = Task.FromResult(true);
        //    _factory.IsystemRepositoryMock.Setup(r => r.ChangeUserInformation(It.Is<User>(b => b.FirstName == updateUser.FirstName && b.LastName == updateUser.LastName && b.Email == updateUser.Email))).Returns(task);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(updateUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PutAsync("/Admin/ChangeUserInformation", httpContent);
        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.Equal(message, responseObject["message"].ToString());
        //}
        //[Fact]
        //public async Task ChangeUserInformation_InvalidJWT()
        //{
        //    User updateUser = new User
        //    {
        //        FirstName = "IntegrationTestChanged",
        //        LastName = "Test",
        //        Email = "IntegrationTest@oslomet.no"
        //    };
        //    var token = "Invalid Jwt";

        //    Task<bool> task = Task.FromResult(true);
        //    _factory.IsystemRepositoryMock.Setup(r => r.ChangeUserInformation(It.Is<User>(b => b.FirstName == "IntegrationTestChanged" && b.LastName == "Test" && b.Email == "IntegrationTest@oslomet.no"))).Returns(task);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(updateUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PutAsync("/Admin/ChangeUserInformation", httpContent);
        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        //}

        //Return prits per printer
        [Fact]
        public async Task ReturnPrintsPerPrinter_ValidJWT()
        {
            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

            PrintingHistory newPrintingHistory = new PrintingHistory { UserName = "TestUser1", FileName = "test", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 100 };
            PrintingHistory newPrintingHistory1 = new PrintingHistory { UserName = "Tester1", FileName = "test2", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 200 };
            PrintingHistory newPrintingHistory2 = new PrintingHistory { UserName = "s495782", FileName = "test3", StartedPrintingAt = centralEuTime.AddHours(-3), FinishedPrintingAt = centralEuTime.AddHours(-2), PrinterName = "Cleopatra", PlasticWeight = 300 };
            PrintingHistory newPrintingHistory3 = new PrintingHistory { UserName = "s495742", FileName = "test4", StartedPrintingAt = centralEuTime.AddHours(-2), FinishedPrintingAt = centralEuTime.AddHours(-1), PrinterName = "Cleopatra", PlasticWeight = 300 };
            var PrintingHistory = new List<PrintingHistory>
            {
                newPrintingHistory, newPrintingHistory1, newPrintingHistory2, newPrintingHistory3
            };

            var token = adminJWT();

            _factory.IsystemRepositoryMock.Setup(r => r.ReturnPrintsPerPrinter(It.IsAny<string>())).ReturnsAsync((string inPrinterName) => PrintingHistory.Where(x => x.PrinterName == "Cleopatra").ToList());

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(PrintingHistory);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/ReturnPrintsPerPrinter", httpContent);

            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingHistory>>((await response.Content.ReadAsStringAsync()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Collection(responseData,
                x =>
                {
                    Assert.Equal("TestUser1", x.UserName);
                    Assert.Equal("test", x.FileName);
                    Assert.Equal("Cleopatra", x.PrinterName);
                },
                x =>
                {
                    Assert.Equal("Tester1", x.UserName);
                    Assert.Equal("test2", x.FileName);
                    Assert.Equal("Cleopatra", x.PrinterName);
                },
                x =>
                {
                    Assert.Equal("s495782", x.UserName);
                    Assert.Equal("test3", x.FileName);
                    Assert.Equal("Cleopatra", x.PrinterName);
                },
                 x =>
                 {
                     Assert.Equal("s495742", x.UserName);
                     Assert.Equal("test4", x.FileName);
                     Assert.Equal("Cleopatra", x.PrinterName);
                 });
        }
        [Fact]
        public async Task ReturnPrintsPerPrinter_InvalidJWT()
        {
            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

            //printingHistory
            PrintingHistory newPrintingHistory = new PrintingHistory { UserName = "TestUser1", FileName = "test", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 100 };
            PrintingHistory newPrintingHistory1 = new PrintingHistory { UserName = "Tester1", FileName = "test2", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 200 };
            PrintingHistory newPrintingHistory2 = new PrintingHistory { UserName = "s495782", FileName = "test3", StartedPrintingAt = centralEuTime.AddHours(-3), FinishedPrintingAt = centralEuTime.AddHours(-2), PrinterName = "Cleopatra", PlasticWeight = 300 };
            PrintingHistory newPrintingHistory3 = new PrintingHistory { UserName = "s495742", FileName = "test4", StartedPrintingAt = centralEuTime.AddHours(-2), FinishedPrintingAt = centralEuTime.AddHours(-1), PrinterName = "Cleopatra", PlasticWeight = 300 };
            var PrintingHistory = new List<PrintingHistory>
            {
                newPrintingHistory, newPrintingHistory1, newPrintingHistory2, newPrintingHistory3
            };

            var token = "Invalid Jwt";

            _factory.IsystemRepositoryMock.Setup(r => r.ReturnPrintsPerPrinter(It.IsAny<string>())).ReturnsAsync((string inPrinterName) => PrintingHistory.Where(x => x.PrinterName == "Cleopatra").ToList());


            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(PrintingHistory);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/ReturnPrintsPerPrinter", httpContent);

            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingHistory>>((await response.Content.ReadAsStringAsync()));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Null(responseData);
        }

        // Create Printer
        [Fact]
        public async Task CreatePrinter_validJWT()
        {
            string message = "Printer has been successfully created.";
            ListOfPrinters newPrinter = new ListOfPrinters
            {
                PrinterName = "TestPrinter",
                IP = "139.50.30.10",
                ApiKey = "TestAPI",
                ApiSecret = "TestSecret",
                PrinterAdminLock = false,
            };
            var token = adminJWT();
            Task<bool> task = Task.FromResult(true);

            // Update the setup to match the actual input
            _factory.IsystemRepositoryMock.Setup(r => r.CreatePrinter(It.Is<Printer>(b => b.PrinterName == newPrinter.PrinterName && b.IP == newPrinter.IP && b.ApiKey == newPrinter.ApiKey && b.ApiSecret == newPrinter.ApiSecret))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newPrinter);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/CreatePrinter", httpContent);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Change to HttpStatusCode.OK since we expect success
            Assert.Equal(message, responseObject["message"].ToString());



            ////
            ///
            //_factory.IsystemRepositoryMock.Setup(r => r.CreatePrinter(newPrinter)).ReturnsAsync(true);
            //_factory.IsystemRepositoryMock.Setup(r => r.CreateAdmin(It.Is<Admin>(b => b.FirstName == "AdminIntegrationTest" && b.LastName == "AdminIntegrationTest" && b.Email == "AdminIntegrationTest@oslomet.no"))).Returns(task);

            //_factory.IsystemRepositoryMock.Setup(r => r.CreatePrinter(It.Is<ListOfPrinters>(b => b.PrinterName == "AdminInt2egrationTest" && b.IP == "139.50.30.10" && b.ApiKey == "TestAPI" && b.ApiSecret == "TestSecret" && b.PrinterAdminLock == false && b.PrinterStatus == "idle"))).ReturnsAsync(true);
            ////Task<bool> task = Task.FromResult(true);
            ////_factory.IsystemRepositoryMock.Setup(r => r.CreatePrinter(It.Is<ListOfPrinters>(b => b.PrinterName == "AdminIntegrationTest" && b.IP == "139.50.30.10" && b.ApiKey == "TestAPI" && b.ApiSecret == "TestSecret" && b.PrinterAdminLock == false))).Returns(task);

            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var jsonContent = JsonConvert.SerializeObject(newPrinter);
            //var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            ////var response = await _client.PostAsync("/Admin/CreatePrinter", httpContent);
            //var response = await _client.PostAsync("/Admin/CreatePrinter", JsonContent.Create(newPrinter));
            //var responseJson = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task CreatePrinter_InvalidJWT()
        {

            Printer newPrinter = new Printer
            {
                PrinterName = "TestPrinter",
                IP = "139.50.30.10",
                ApiKey = "TestAPI",
                ApiSecret = "TestSecret"
            };

            var token = "Invalid JWT";
            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.CreatePrinter(It.Is<Printer>(b => b.PrinterName == "AdminIntegrationTest" && b.IP == "139.50.30.10" && b.ApiKey == "TestAPI" && b.ApiSecret == "TestSecret"))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newPrinter);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/CreatePrinter", httpContent);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        //Delete Printer
        [Fact]
        public async Task DeletePrinter_ValidJWT()
        {
            string message = "Printer has been successfully deleted.";
            var token = adminJWT();
            string inPrinter = "TestPrinter";

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.DeletePrinter(inPrinter)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeletePrinter?inPrinterName={inPrinter}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task DeletePrinter_InvalidJWT()
        {
            var token = "Invalid Jwt";
            string inPrinter = "TestPrinter";

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.DeletePrinter(inPrinter)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeletePrinter?inPrinterName={inPrinter}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Update Printer
        [Fact]
        public async Task UpdatePrinter_ValidJWT()
        {
            string message = "Printer has been successfully updated.";
            Printer newPrinter = new Printer
            {
                IP = "139.50.30.20",
                ApiKey = "ApiTest",
                ApiSecret = "ApiSecret",
            };
            var token = adminJWT();

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.UpdatePrinter(It.Is<Printer>(b => b.IP == newPrinter.IP && b.ApiKey == newPrinter.ApiKey && b.ApiSecret == newPrinter.ApiSecret))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newPrinter);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/Admin/UpdatePrinter", httpContent);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task UpdatePrinter_InvalidJWT()
        {
            Printer newPrinter = new Printer
            {
                IP = "139.50.30.20",
                ApiKey = "ApiTest",
                ApiSecret = "ApiSecret",
            };
            var token = "Invalid Jwt";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.UpdatePrinter(It.Is<Printer>(b => b.IP == newPrinter.IP && b.ApiKey == newPrinter.ApiKey && b.ApiSecret == newPrinter.ApiSecret))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newPrinter);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/Admin/UpdatePrinter", httpContent);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Get All users
        [Fact]
        public async Task GetAllUsers_ValidJWT()
        {
            var token = adminJWT();

            User newUser0 = new User {Username = "NewUser0"};
            User newUser1 = new User {Username = "NewUser1"};

            var userList = new List<User>
            {
                newUser0, newUser1
            };

            var userLis2t = userList.Select(x => x.Username).ToList();
            Task<List<string>> task = Task.FromResult(userLis2t);

            _factory.IsystemRepositoryMock.Setup(r => r.GetAllUsers()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        

            var response = await _client.GetAsync("/Admin/GetAllUsers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<IEnumerable<string>>((await response.Content.ReadAsStringAsync()));
            Assert.Collection(responseData,
                x =>
                {
                    Assert.Equal("NewUser0", x);
                },
                x =>
                {
                    Assert.Equal("NewUser1", x);
                });
        }

        [Fact]
        public async Task GetAllUsers_ValidJWT_InvalidDBData()
        {
            var token = adminJWT();
            var userList = new List<User>
            {
            };

            var userList2 = userList.Select(x => x.Username).ToList();
            Task<List<string>> task = Task.FromResult(userList2);

            _factory.IsystemRepositoryMock.Setup(r => r.GetAllUsers()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetAllUsers");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_InvalidJWT()
        {
            var token = "Invalid Jwt";

            User newUser0 = new User { Username = "NewUser0" };
            User newUser1 = new User { Username = "NewUser1" };

            var userList = new List<User>
            {
                newUser0, newUser1
            };

            var userList2 = userList.Select(x => x.Username).ToList();
            Task<List<string>> task = Task.FromResult(userList2);

            _factory.IsystemRepositoryMock.Setup(r => r.GetAllUsers()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.GetAsync("/Admin/GetAllUsers");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<IEnumerable<string>>((await response.Content.ReadAsStringAsync()));
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Get all admins
        [Fact]
        public async Task GetAllAdmins_ValidJWT()
        {
            var token = adminJWT();

            Admin newAdmin0 = new Admin { UserName = "newAdmin0" };
            Admin newAdmin1 = new Admin { UserName = "newAdmin1" };

            var userList = new List<Admin>
            {
                newAdmin0, newAdmin1
            };

            var adminList = userList.Select(x => x.UserName).ToList();
            Task<List<string>> task = Task.FromResult(adminList);

            _factory.IsystemRepositoryMock.Setup(r => r.GetAllAdmins()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.GetAsync("/Admin/GetAllAdmins");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<IEnumerable<string>>((await response.Content.ReadAsStringAsync()));
            Assert.Collection(responseData,
                x =>
                {
                    Assert.Equal("newAdmin0", x);
                },
                x =>
                {
                    Assert.Equal("newAdmin1", x);
                });
        }

        [Fact]
        public async Task GetAllAdmins_InvalidJWT()
        {
            var token = "Invalid JWT";

            Admin newAdmin0 = new Admin { UserName = "newAdmin0" };
            Admin newAdmin1 = new Admin { UserName = "newAdmin1" };

            var userList = new List<Admin>
            {
                newAdmin0, newAdmin1
            };

            var adminList = userList.Select(x => x.UserName).ToList();
            Task<List<string>> task = Task.FromResult(adminList);

            _factory.IsystemRepositoryMock.Setup(r => r.GetAllAdmins()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetAllAdmins");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<IEnumerable<string>>(( await response.Content.ReadAsStringAsync()));
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }


    
        //Get All Printers
        [Fact]
        public async Task GetAllPrinters_ValidJWT()
        {
            var token = adminJWT();

            ListOfPrinters aPrinter = new ListOfPrinters
            {
                Id = 1,
                PrinterName = "Cleopatra",
                ApiKey = "API_IntegrationTest",
                ApiSecret = "API_IntegrationTest",
                PrinterAdminLock = false
            };
            List<ListOfPrinters> allPrinters = new List<ListOfPrinters>();
            allPrinters.Add(aPrinter);
            Task<List<ListOfPrinters>> task = Task.FromResult(allPrinters);


            _factory.IsystemRepositoryMock.Setup(r => r.GetAllPrinters()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetAllPrinters");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task GetAllPrinters_Invalid_ValidJWT()
        {

            string token = "Invalid token";

            ListOfPrinters aPrinter = new ListOfPrinters
            {
                Id = 1,
                PrinterName = "Cleopatra",
                ApiKey = "API_IntegrationTest",
                ApiSecret = "API_IntegrationTest",
                PrinterAdminLock = false
            };
            List<ListOfPrinters> allPrinters = new List<ListOfPrinters>();
            allPrinters.Add(aPrinter);
            Task<List<ListOfPrinters>> task = Task.FromResult(allPrinters);


            _factory.IsystemRepositoryMock.Setup(r => r.GetAllPrinters()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetAllPrinters");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }
        [Fact]
        public async Task GetAllPrinters_ValidJWT_Exception()
        {
            var token = adminJWT();
            string message = "Unknown get all printers error.";

            ListOfPrinters aPrinter = new ListOfPrinters
            {
                Id = 1,
                PrinterName = "Cleopatra",
                ApiKey = "API_IntegrationTest",
                ApiSecret = "API_IntegrationTest",
                PrinterAdminLock = false
            };
            List<ListOfPrinters> allPrinters = new List<ListOfPrinters>();
            allPrinters.Add(aPrinter);
            Task<List<ListOfPrinters>> task = Task.FromResult(allPrinters);


            _factory.IsystemRepositoryMock.Setup(r => r.GetAllPrinters()).ThrowsAsync(new Exception("Integration test exception"));


            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetAllPrinters");

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());

        }

        //Unlock and Lock Printer
        [Fact]
        public async Task LockAndUnlockPrinter_Lock_ValidJWT()
        {
            var token = adminJWT();
            string inPrinterName = "Cleopatra";
            string inCommand = "lock";


            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.UnlockAndLockPrinter(inPrinterName, inCommand)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"Admin/UnlockAndLockPrinter?PrinterName={inPrinterName}&inCommand={inCommand}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LockAndUnlockPrinter_Unlock_ValidJWT()
        {
            var token = adminJWT();
            string inPrinterName = "Cleopatra";
            string inCommand = "unlock";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.UnlockAndLockPrinter(inPrinterName, inCommand)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"Admin/UnlockAndLockPrinter?PrinterName={inPrinterName}&inCommand={inCommand}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LockAndUnlockPrinter_Invalid_ValidJWT()
        {
            var token = "Invalid JWT";
            string inPrinterName = "Cleopatra";
            string inCommand = "unlock";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.UnlockAndLockPrinter(inPrinterName, inCommand)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"Admin/UnlockAndLockPrinter?PrinterName={inPrinterName}&inCommand={inCommand}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Printing history for user
        [Fact]
        public async Task PrintingHistoryForUser_Valid_JWT()
        {
            var token = adminJWT();

            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

            string inUserName = "TestUser1";

            PrintingHistory newPrintingHistory = new PrintingHistory { UserName = "TestUser1", FileName = "test", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 100 };
            PrintingHistory newPrintingHistory1 = new PrintingHistory { UserName = "TestUser1", FileName = "test2", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 200 };

            var PrintingHistory = new List<PrintingHistory>
            {
                newPrintingHistory, newPrintingHistory1
            };


            Task<List<PrintingHistory>> task = Task.FromResult(PrintingHistory);

            _factory.IsystemRepositoryMock.Setup(r => r.PrintingHistoryForUser(inUserName)).Returns(task);

            var jsonContent = JsonConvert.SerializeObject(PrintingHistory);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/Admin/PrintingHistoryForUser?inUserName=TestUser1");
            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingHistory>>((await response.Content.ReadAsStringAsync()));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Collection(responseData,
                x =>
                {
                    Assert.Equal("TestUser1", x.UserName);
                    Assert.Equal("test", x.FileName);
                    Assert.Equal("Cleopatra", x.PrinterName);
                },
                x =>
                {
                    Assert.Equal("TestUser1", x.UserName);
                    Assert.Equal("test2", x.FileName);
                    Assert.Equal("Cleopatra", x.PrinterName);
                });
        }

        [Fact]
        public async Task PrintingHistoryForUser_Invalid_JWT()
        {
            var token = "Invalid JWT";

            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

            string inUserName = "TestUser1";

            PrintingHistory newPrintingHistory = new PrintingHistory { UserName = "TestUser1", FileName = "test", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 100 };
            PrintingHistory newPrintingHistory1 = new PrintingHistory { UserName = "TestUser1", FileName = "test2", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 200 };

            var PrintingHistory = new List<PrintingHistory>
            {
                newPrintingHistory, newPrintingHistory1
            };


            Task<List<PrintingHistory>> task = Task.FromResult(PrintingHistory);

            _factory.IsystemRepositoryMock.Setup(r => r.PrintingHistoryForUser(inUserName)).Returns(task);

            var jsonContent = JsonConvert.SerializeObject(PrintingHistory);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/Admin/PrintingHistoryForUser?inUserName=TestUser1");
            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingHistory>>((await response.Content.ReadAsStringAsync()));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Return plastic per printer
        //[Fact]
        //public async Task ReturnPlasticUsedForPrinterr_Valid_JWT()
        //{
        //    var token = adminJWT();
        //    string inPrinterName = "Cleopatra";
        //    //month or year
        //    string inCommand = "month";
        //    double plasticWeight = 100;


        //    Task<double> task = Task.FromResult(plasticWeight);

        //    _factory.IsystemRepositoryMock.Setup(r => r.ReturnPlasticUsedForPrinter(inPrinterName, inCommand)).Returns(task);

        //    var jsonContent = JsonConvert.SerializeObject(plasticWeight);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var response = await _client.GetAsync($"/Admin/ReturnPlasticUsedForPrinter?printerName={inPrinterName}&inCommand={inCommand}");
        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.Equal(plasticWeight, responseObject);
        //}
        //[Fact]
        //public async Task ReturnPlasticUsedForPrinterr_Invalid_JWT()
        //{
        //    var token = "Invalid Jwt";
        //    string inPrinterName = "Cleopatra";
        //    //month or year
        //    string inCommand = "month";
        //    double plasticWeight = 100;


        //    Task<double> task = Task.FromResult(plasticWeight);

        //    _factory.IsystemRepositoryMock.Setup(r => r.ReturnPlasticUsedForPrinter(inPrinterName, inCommand)).Returns(task);

        //    var jsonContent = JsonConvert.SerializeObject(plasticWeight);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var response = await _client.GetAsync($"/Admin/ReturnPlasticUsedForPrinter?printerName={inPrinterName}&inCommand={inCommand}");
        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);
        //    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        //    Assert.Empty(response.Content.ReadAsStringAsync().Result);
        //}

        //Get printing Queue
        [Fact]
        public async Task GetPrintingQueue_Valid_JWT()
        {
            var token = adminJWT();

            PrintingQueue newPrintToQueue = new PrintingQueue { FileName = "Testfile1", UserName = "TestUser1", PrintTime = "1340", PlasticWeight = 100 };
            PrintingQueue newPrintToQueue1 = new PrintingQueue { FileName = "Testfile2", UserName = "Tester1", PrintTime = "2340", PlasticWeight = 200 };


            var newPrintingQueueElements = new List<PrintingQueue>
            {
                newPrintToQueue,newPrintToQueue1
            };
            Task<List<PrintingQueue>> task = Task.FromResult(newPrintingQueueElements);


            _factory.IsystemRepositoryMock.Setup(r => r.GetPrintingQueue()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetPrintingQueue");
            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingQueue>>((await response.Content.ReadAsStringAsync()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Collection(responseData,
                x =>
                {
                    Assert.Equal(newPrintToQueue.UserName, x.UserName);
                    Assert.Equal(newPrintToQueue.FileName, x.FileName);
                    Assert.Equal(newPrintToQueue.PrintTime, x.PrintTime);
                    Assert.Equal(newPrintToQueue.PlasticWeight, x.PlasticWeight);
                },
                x =>
                {
                    Assert.Equal(newPrintToQueue1.UserName, x.UserName);
                    Assert.Equal(newPrintToQueue1.FileName, x.FileName);
                    Assert.Equal(newPrintToQueue1.PrintTime, x.PrintTime);
                    Assert.Equal(newPrintToQueue1.PlasticWeight, x.PlasticWeight);
                }
                );

        }
        [Fact]
        public async Task GetPrintingQueue_Invalid_JWT()
        {
            var token = "Invalid Jwt";

            PrintingQueue newPrintToQueue = new PrintingQueue { FileName = "Testfile1", UserName = "TestUser1", PrintTime = "1340", PlasticWeight = 100 };
            PrintingQueue newPrintToQueue1 = new PrintingQueue { FileName = "Testfile2", UserName = "Tester1", PrintTime = "2340", PlasticWeight = 200 };


            var newPrintingQueueElements = new List<PrintingQueue>
            {
                newPrintToQueue,newPrintToQueue1
            };
            Task<List<PrintingQueue>> task = Task.FromResult(newPrintingQueueElements);


            _factory.IsystemRepositoryMock.Setup(r => r.GetPrintingQueue()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetPrintingQueue");
            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingQueue>>((await response.Content.ReadAsStringAsync()));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);

        }

        //Cancel print
        [Fact]
        public async Task CancelOngoingPrint_Valid_JWT()
        {
            var token = adminJWT();
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelOngoingPrint(inUserName, inFileName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"Admin/CancelOngoingPrint?inUserName={inUserName}&inFileName={inFileName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CancelOngoingPrint_Invalid_JWT()
        {
            var token = "Invalid Jwt";
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelOngoingPrint(inUserName, inFileName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"Admin/CancelOngoingPrint?inUserName={inUserName}&inFileName={inFileName}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        //Remove print from queue
        [Fact]
        public async Task RemovePrintFromQueue_Valid_JWT()
        {
            var token = adminJWT();
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelQueuePrint(inFileName, inUserName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"Admin/CancelQueuePrint?inFileName={inFileName}&inUserName={inUserName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RemovePrintFromQueue_Invalid_JWT()
        {
            var token = "Invalid Jwt";
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelQueuePrint(inFileName, inUserName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"Admin/CancelQueuePrint?inFileName={inFileName}&inUserName={inUserName}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Stream from printer
        [Fact]
        public async Task StreamFromPrinter_Valid_JWT()
        {
            var token = adminJWT();
            string inPrinterName = "TestPrinter";


            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.StreamFromPrinter(inPrinterName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"Admin/StreamFromPrinter?inPrinterName={inPrinterName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task StreamFromPrinter_Invalid_JWT()
        {
            var token = "Invalid Jwt";
            string inPrinterName = "TestPrinter";


            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.StreamFromPrinter(inPrinterName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"Admin/StreamFromPrinter?inPrinterName={inPrinterName}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Make new Questions and Answers
        [Fact]
        public async Task MakeNewQAndA_ValidJWT_ValidData()
        {
            var token = adminJWT();

            QandA newQandQ = new QandA { Question = "TestQuestion", Answers = "TestAnswer" };
            var task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.MakeNewQAndA(It.Is<QandA>(b => b.Question == newQandQ.Question && b.Answers == newQandQ.Answers))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newQandQ);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/MakeNewQAndA", httpContent);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            _factory.IsystemRepositoryMock.VerifyAll();
        }
        [Fact]
        public async Task MakeNewQAndA_InvalidJWT_ValidData()
        {
            var token = "Invalid Jwt";

            QandA newQandQ = new QandA { Question = "TestQuestion", Answers = "TestAnswer" };
            var task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.MakeNewQAndA(It.Is<QandA>(b => b.Question == newQandQ.Question && b.Answers == newQandQ.Answers))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newQandQ);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/MakeNewQAndA", httpContent);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //Get Questions and Answers
        [Fact]
        public async Task GetQAndA_ValidJWT_ValidData()
        {
            var token = adminJWT();


            QandA newQandQ0 = new QandA { Id = 1, Question = "TestQuestio0n", Answers = "TestAnswer0" };
            QandA newQandQ1 = new QandA { Id = 2, Question = "TestQuestion1", Answers = "TestAnswer1" };

            Task<List<QandA>> task = Task.FromResult(new List<QandA> { newQandQ0, newQandQ1 });

            _factory.IsystemRepositoryMock.Setup(r => r.GetQAndA()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetQAndA");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<IEnumerable<QandA>>((await response.Content.ReadAsStringAsync()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Collection(responseData,
                x =>
                {
                    Assert.Equal(newQandQ0.Question, x.Question);
                    Assert.Equal(newQandQ0.Answers, x.Answers);
                    Assert.Equal(newQandQ0.Id, x.Id);

                },
                x =>
                {
                    Assert.Equal(newQandQ1.Question, x.Question);
                    Assert.Equal(newQandQ1.Answers, x.Answers);
                    Assert.Equal(newQandQ1.Id, x.Id);
                }
                );
        }

        [Fact]
        public async Task GetQAndA_InValidJWT_ValidData()
        {
            var token = "Invalid Jwt";

            QandA newQandQ0 = new QandA { Question = "TestQuestio0n", Answers = "TestAnswer0" };
            QandA newQandQ1 = new QandA { Question = "TestQuestion1", Answers = "TestAnswer1" };

            Task<List<QandA>> task = Task.FromResult(new List<QandA> { newQandQ0, newQandQ1 });

            _factory.IsystemRepositoryMock.Setup(r => r.GetQAndA()).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/Admin/GetQAndA");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Edit Questions and Answers

        [Fact]
        public async Task EditQAndA_ValidJWT_ValidData()
        {
            QandA newQandQ0 = new QandA { Id = 1, Question = "EditedContent", Answers = "TestAnswer0" };

            var token = adminJWT();

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.EditQAndA(It.Is<QandA>(b => b.Id == newQandQ0.Id && b.Question == newQandQ0.Question && b.Answers == newQandQ0.Answers))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newQandQ0);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync("/Admin/EditQAndA", httpContent);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task EditQAndA_InvalidJWT_ValidData()
        {
            QandA newQandQ0 = new QandA { Id = 1, Question = "EditedContent", Answers = "TestAnswer0" };

            var token = "Invalid Jwt";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.EditQAndA(It.Is<QandA>(b => b.Id == newQandQ0.Id && b.Question == newQandQ0.Question && b.Answers == newQandQ0.Answers))).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newQandQ0);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync("/Admin/EditQAndA", httpContent);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Delete Questions and Answers

        [Fact]
        public async Task DeleteQAndA_ValidJWT()
        {
            String message = "Questions and answers were removed.";
            var token = adminJWT();
            int Id = 1;

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.DeleteQAndA(Id)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeleteQAndA?id={Id}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }

        [Fact]
        public async Task DeleteQAndA_InValidJWT()
        {
            var token = "Invalid Jwt";
            int Id = 1;

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.DeleteQAndA(Id)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"Admin/DeleteQAndA?id={Id}", UriKind.Relative),
            };

            var response = await _client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        //////////////////////////////////////////////////////USER INTEGRATION TESTS////////////////////////////////////////////////////

        private string userJWT()
        {
            Users userForJWT = new Users
            {
                Email = "UserTest@oslomet.no",
                Username = "UserTest"
            };
            var token = CreateJWCToken(userForJWT, null);
            return token;
        }
        //Create user
        //[Fact]
        //public async Task CreateUser_ValidJWT()
        //{
        //    string message = "User was created.";
        //    var token = userJWT();

        //    User inNewUser = new User
        //    {
        //        Email = "UserTest@oslomet.no"
        //    };

        //    Task<bool> task = Task.FromResult(true);

        //    _factory.IsystemRepositoryMock.Setup(r => r.CreateUser(It.Is<User>(b => b.Email == inNewUser.Email))).Returns(task);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(inNewUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PostAsync("/User/CreateUser", httpContent);

        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.Equal(message, responseObject["message"].ToString());
        //}
        //[Fact]
        //public async Task CreateUser_ValidJWT_InvalidData()
        //{
        //    string message = "User couldn't be created.";
        //    var token = userJWT();

        //    User inNewUser = new User
        //    {
        //        Email = ""
        //    };

        //    Task<bool> task = Task.FromResult(false);

        //    _factory.IsystemRepositoryMock.Setup(r => r.CreateUser(It.Is<User>(b => b.Email == inNewUser.Email))).Returns(task);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(inNewUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PostAsync("/User/CreateUser", httpContent);

        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //    Assert.Equal(message, responseObject["message"].ToString());
        //}
        //[Fact]
        //public async Task CreateUser_InvalidJWT()
        //{
        //    var token = "Invalid JWT";

        //    User inNewUser = new User
        //    {
        //        Email = "UserTest@oslomet.no"
        //    };

        //    Task<bool> task = Task.FromResult(true);

        //    _factory.IsystemRepositoryMock.Setup(r => r.CreateUser(It.Is<User>(b => b.Email == inNewUser.Email))).Returns(task);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(inNewUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PostAsync("/User/CreateUser", httpContent);

        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        //    Assert.Empty(response.Content.ReadAsStringAsync().Result);
        //}
        //Login user
        [Fact]
        public async Task LoginUser_GenerateAndReturnJWT_LogIn()
        {
            string inUserName = "UserTest";
            string inPassword = "PasswordUserTest";
            var token = userJWT();

            _factory.IsystemRepositoryMock.Setup(r => r.LogIn(inUserName, inPassword)).ReturnsAsync((true, token));

            var response = await _client.PostAsync($"/User/LogIn?inUserName={inUserName}&inPassword={inPassword}", null);
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(token, responseObject["message"].ToString());
        }
        [Fact]
        public async Task LoginUser_ExpiredPassword()
        {
            string message = "Password expired, new email has been sent";
            string inUserName = "UserTest";
            string inPassword = "PasswordUserTest";

            _factory.IsystemRepositoryMock.Setup(r => r.LogIn(inUserName, inPassword)).ReturnsAsync((true, message));

            var response = await _client.PostAsync($"/User/LogIn?inUserName={inUserName}&inPassword={inPassword}", null);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task LoginUser_WrongPasswordAndUser()
        {
            string message = "Username or password is wrong, a new email has been sent.";
            string inUserName = "";
            string inPassword = "";

            _factory.IsystemRepositoryMock.Setup(r => r.LogIn(inUserName, inPassword)).ReturnsAsync((false, ""));

            var response = await _client.PostAsync($"/User/LogIn?inUserName={inUserName}&inPassword={inPassword}", null);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }

        ////Change user information
        //[Fact]
        //public async Task ChangeUserInformation_ValidJWT_ValidData()
        //{
        //    string message = "User data was successfully updated.";
        //    User updateUser = new User
        //    {
        //        Email = "userTest@oslomet.no"
        //    };
        //    var token = userJWT();

        //    Task<bool> task = Task.FromResult(true);
        //    _factory.IsystemRepositoryMock.Setup(r => r.ChangeUserInformation(It.Is<User>(b => b.Email == updateUser.Email))).Returns(task);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(updateUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PutAsync("/User/ChangeUserInformation", httpContent);
        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.Equal(message, responseObject["message"].ToString());
        //}

        //[Fact]
        //public async Task ChangeUserInformation_ValidJWT_InValidData()
        //{
        //    string message = "Problem with input validation.";
        //    User updateUser = new User
        //    {
        //        FirstName = "",
        //        LastName = "",
        //        Email = "@oslomet.no"
        //    };
        //    var token = userJWT();

        //    Task<bool> task = Task.FromResult(false);
        //    _factory.IsystemRepositoryMock.Setup(r => r.ChangeUserInformation(It.Is<User>(b => b.FirstName == updateUser.FirstName && b.LastName == updateUser.LastName && b.Email == updateUser.Email))).Returns(task);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(updateUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PutAsync("/User/ChangeUserInformation", httpContent);
        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //    Assert.Equal(message, responseObject["message"].ToString());
        //}

        //[Fact]
        //public async Task ChangeUserInformation_InvalidJWT_ValidData()
        //{
        //    string message = "User data was successfully updated.";
        //    User updateUser = new User
        //    {
        //        FirstName = "Test",
        //        LastName = "Test",
        //        Email = "userTest@oslomet.no"
        //    };
        //    var token = "Invalid Jwt";


        //    _factory.IsystemRepositoryMock.Setup(r => r.ChangeUserInformation(It.Is<User>(b => b.FirstName == updateUser.FirstName && b.LastName == updateUser.LastName && b.Email == updateUser.Email))).ReturnsAsync(true);

        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var jsonContent = JsonConvert.SerializeObject(updateUser);
        //    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //    var response = await _client.PutAsync("/User/ChangeUserInformation", httpContent);
        //    var responseJson = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

        //    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        //    Assert.Empty(response.Content.ReadAsStringAsync().Result);
        //}

        //User Upload
        [Fact]
        public async Task Upload_File_ValidJWT_ValidData()
        {
            string message = "File verification error.";
            var token = userJWT();

            IFormFile inFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("FileForTesting")), 0, 0, "Data", "testingFile.gcode");
            string inUserName = "UserTest";

            _factory.IsystemRepositoryMock.Setup(r => r.Upload(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync((false,0,0));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(inFile.OpenReadStream()), "testingFile", inFile.FileName);
            content.Add(new StringContent(inUserName), "UserTest");

            // Send the request
            var response = await _client.PostAsync("/User/Upload", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }

        [Fact]
        public async Task Upload_File_ValidJWT_VerificationFail()
        {
            string message = "File verification error.";
            var token = userJWT();

            IFormFile inFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("FileForTesting")), 0, 0, "Data", "testingFile.gcode");
            string inUserName = "UserTest";


            _factory.IsystemRepositoryMock.Setup(r => r.Upload(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync((false, 0, 0));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(inFile.OpenReadStream()), "testingFile", inFile.FileName);
            content.Add(new StringContent(inUserName), "UserTest");

            // Send the request
            var response = await _client.PostAsync("/User/Upload", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }

        [Fact]
        public async Task Upload_File_ValidJWT_UnknownBackendError()
        {
            string message = "Unknown backend error.";
            var token = userJWT();

            IFormFile inFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("FileForTesting")), 0, 0, "Data", "testingFile.gcode");
            string inUserName = "UserTest";


            _factory.IsystemRepositoryMock.Setup(r => r.Upload(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync((false, 0, 1));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(inFile.OpenReadStream()), "testingFile", inFile.FileName);
            content.Add(new StringContent(inUserName), "UserTest");

            // Send the request
            var response = await _client.PostAsync("/User/Upload", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }

        [Fact]
        public async Task Upload_File_ValidJWT_OtherErrors()
        {
            string message = "Unknown upload error.";
            var token = userJWT();

            IFormFile inFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("FileForTesting")), 0, 0, "Data", "testingFile.gcode");
            string inUserName = "UserTest";


            _factory.IsystemRepositoryMock.Setup(r => r.Upload(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync((false, 1, 1));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(inFile.OpenReadStream()), "testingFile", inFile.FileName);
            content.Add(new StringContent(inUserName), "UserTest");

            // Send the request
            var response = await _client.PostAsync("/User/Upload", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }

        [Fact]
        public async Task Upload_File_InvalidJWT_ValidData()
        {
            var token = "Invalid Jwt";

            IFormFile inFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("FileForTesting")), 0, 0, "Data", "testingFile.gcode");
            string inUserName = "UserTest";

            Task<bool> task = Task.FromResult(true);

            _factory.IsystemRepositoryMock.Setup(r => r.Upload(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync((true, 1, 1));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(inFile.OpenReadStream()), "testingFile", inFile.FileName);
            content.Add(new StringContent(inUserName), "UserTest");

            // Send the request
            var response = await _client.PostAsync("/User/Upload", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Delete File
        [Fact]
        public async Task DeleteFile_ValidJWT_OkData()
        {
            string message = "File deleted successfully.";
            var token = userJWT();

            User inUser = new User {
                Email = "UserTest@oslomet.nno"
            };
            string fileName = "dummyFile.gcode";

            _factory.IsystemRepositoryMock.Setup(b => b.DeleteFile(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(new { inUser, fileName });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/User/DeleteFile", httpContent);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message, responseObject["message"].ToString());
        }
        [Fact]
        public async Task DeleteFile_InvalidJWT_ValidData()
        {
            var token = "Invalid Jwt";

            User inUser = new User
            {
                Email = "UserTest@oslomet.nno"
            };
            string fileName = "dummyFile.gcode";

            _factory.IsystemRepositoryMock.Setup(b => b.DeleteFile(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(new { inUser, fileName });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/User/DeleteFile", httpContent);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Printing history for user
        [Fact]
        public async Task PrintingHistoryForuser_ValidJWT_ValidData()
        {
            var token = userJWT();

            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

            string inUserName = "TestUser1";

            PrintingHistory newPrintingHistory = new PrintingHistory { UserName = "TestUser1", FileName = "test", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 100 };
            PrintingHistory newPrintingHistory1 = new PrintingHistory { UserName = "TestUser1", FileName = "test2", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 200 };

            var PrintingHistory = new List<PrintingHistory>
            {
                newPrintingHistory, newPrintingHistory1
            };


            Task<List<PrintingHistory>> task = Task.FromResult(PrintingHistory);

            _factory.IsystemRepositoryMock.Setup(r => r.PrintingHistoryForUser(inUserName)).Returns(task);

            var jsonContent = JsonConvert.SerializeObject(PrintingHistory);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/User/PrintingHistoryForUser?inUserName=TestUser1");
            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingHistory>>((await response.Content.ReadAsStringAsync()));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Collection(responseData,
                x =>
                {
                    Assert.Equal("TestUser1", x.UserName);
                    Assert.Equal("test", x.FileName);
                    Assert.Equal("Cleopatra", x.PrinterName);
                },
                x =>
                {
                    Assert.Equal("TestUser1", x.UserName);
                    Assert.Equal("test2", x.FileName);
                    Assert.Equal("Cleopatra", x.PrinterName);
                });
        }

        [Fact]
        public async Task PrintingHistoryForuser_InvalidJWT_ValidData()
        {
            var token = "Invalid JWT";

            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

            string inUserName = "TestUser1";

            PrintingHistory newPrintingHistory = new PrintingHistory { UserName = "TestUser1", FileName = "test", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 100 };
            PrintingHistory newPrintingHistory1 = new PrintingHistory { UserName = "TestUser1", FileName = "test2", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 200 };

            var PrintingHistory = new List<PrintingHistory>
            {
                newPrintingHistory, newPrintingHistory1
            };


            Task<List<PrintingHistory>> task = Task.FromResult(PrintingHistory);

            _factory.IsystemRepositoryMock.Setup(r => r.PrintingHistoryForUser(inUserName)).Returns(task);

            var jsonContent = JsonConvert.SerializeObject(PrintingHistory);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/User/PrintingHistoryForUser?inUserName=TestUser1");
            var responseData = JsonConvert.DeserializeObject<IEnumerable<PrintingHistory>>((await response.Content.ReadAsStringAsync()));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public async Task RemovePrintFromQueue_ValidJWT_ValidData()
        {
            var token = userJWT();
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelQueuePrint(inFileName, inUserName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"User/CancelQueuePrint?inFileName={inFileName}&inUserName={inUserName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task RemovePrintFromQueue_ValidJWT_InvalidValidData()
        {
            var token = userJWT();
            string inUserName = "";
            string inFileName = "";

            Task<bool> task = Task.FromResult(false);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelQueuePrint(inFileName, inUserName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"User/CancelQueuePrint?inFileName={inFileName}&inUserName={inUserName}");

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemovePrintFromQueue_InvalidJWT_ValidData()
        {
            var token = "Invalid Jwt";
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelQueuePrint(inFileName, inUserName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"User/CancelQueuePrint?inFileName={inFileName}&inUserName={inUserName}");

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseJson);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        //Cancel print
        [Fact]
        public async Task CancelPrint_ValidJWT_ValidData()
        {
            var token = userJWT();
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelOngoingPrint(inUserName, inFileName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"User/CancelOngoingPrint?inUserName={inUserName}&inFileName={inFileName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task CancelPrint_ValidJWT_InvalidData()
        {
            var token = userJWT();
            string inUserName = "";
            string inFileName = "";

            Task<bool> task = Task.FromResult(false);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelOngoingPrint(inUserName, inFileName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"User/CancelOngoingPrint?inUserName={inUserName}&inFileName={inFileName}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async Task CancelPrint_InvalidJWT_ValidData()
        {
            var token = "Invalid Jwt";
            string inUserName = "TestUser1";
            string inFileName = "TestFile1";

            Task<bool> task = Task.FromResult(true);
            _factory.IsystemRepositoryMock.Setup(r => r.CancelOngoingPrint(inUserName, inFileName)).Returns(task);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"User/CancelOngoingPrint?inUserName={inUserName}&inFileName={inFileName}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Empty(response.Content.ReadAsStringAsync().Result);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}

