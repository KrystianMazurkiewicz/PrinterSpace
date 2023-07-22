using Moq;
using BachelorsProject.Models;
using Xunit;
using BachelorsProject.DAL;
using BachelorsProject.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net;
using PrintingHistory = BachelorsProject.DAL.PrintingHistory;
using System.IO.Pipelines;
using System.Reflection;
using QandA = BachelorsProject.DAL.QandA;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BachelorsTesting
{
    public class BachelorsProjectTest
    {
        private readonly Mock<ISystemRepository> mockIRepo = new Mock<ISystemRepository>();
        private readonly Mock<ILogger<UserController>> mockLogUser = new Mock<ILogger<UserController>>();
        private readonly Mock<ILogger<AdminController>> mockAdminLog = new Mock<ILogger<AdminController>>();
        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();

        //ADMIN

        //Create Admin
        [Fact]
        public async Task CreateAdmin_OK()
        {
            // arrange
            var inNewAdmin = new Admin
            {
                Email = "AdminUnitTest@oslomet.no"
            };
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act

            mockIRepo.Setup(x => x.CreateAdmin(inNewAdmin)).ReturnsAsync(true);
            var result = await mockAdminController.CreateAdmin(inNewAdmin);

            // assert
            var okResult = result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("Admin was successfully created.", message);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task CreateAdmin_Fail()
        {
            // arrange
            var inNewAdmin = new Admin
            {
                Email = "AdminUnitTest@oslomet.no"
            };
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act

            mockIRepo.Setup(x => x.CreateAdmin(inNewAdmin)).ReturnsAsync(false);
            var result = await mockAdminController.CreateAdmin(inNewAdmin);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Admin could not be created.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task CreateAdmin_Exception()
        {
            // arrange
            var inNewAdmin = new Admin
            {
                Email = "AdminUnitTest@oslomet.no"
            };
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            mockIRepo.Setup(x => x.CreateAdmin(inNewAdmin)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockAdminController.CreateAdmin(inNewAdmin);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown create admin error.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Get All Users
        [Fact]
        public async Task GetAllUsers_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            User newUser0 = new User { Username = "NewUser0" };
            User newUser1 = new User { Username = "NewUser1" };

            var userList = new List<User>
            {
                newUser0, newUser1
            };
            var list = userList.ToList();

            var listOfUserNames = userList.Select(x => x.Username).ToList();
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllUsers()).Returns(Task.FromResult(listOfUserNames));
            var actionResult = await mockAdminController.GetAllUsers();

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value as IEnumerable<string>;

            Assert.NotNull(okResult);
            Assert.Equal(listOfUserNames, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllUsers()).ReturnsAsync(() => null);
            var actionResult = await mockAdminController.GetAllUsers();

            // Assert
            var badResult = actionResult as NotFoundObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not get all users.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllUsers()).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.GetAllUsers();

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown get all users error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Get All Admins
        [Fact]
        public async Task GetAllAdmins_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            Admin newAdmin0 = new Admin { UserName = "newAdmin0" };
            Admin newAdmin1 = new Admin { UserName = "newAdmin1" };

            var userList = new List<Admin>
            {
                newAdmin0, newAdmin1
            };

            var list = userList.ToList();
            var listOfUserNames = userList.Select(x => x.UserName).ToList();
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllAdmins()).Returns(Task.FromResult(listOfUserNames));
            var actionResult = await mockAdminController.GetAllAdmins();

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value as IEnumerable<string>;

            Assert.NotNull(okResult);
            Assert.Equal(listOfUserNames, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetAllAdmin_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllAdmins()).ReturnsAsync(() => null);
            var actionResult = await mockAdminController.GetAllAdmins();

            // Assert
            var badResult = actionResult as NotFoundObjectResult;

            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);
            Assert.Equal("Could not get all admins.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }

        [Fact]
        public async Task GetAllAdmins_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllAdmins()).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.GetAllAdmins();

            // Assert
            var badResult = actionResult as BadRequestObjectResult;

            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);
            Assert.Equal("Unknown get all admin error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Delete users
        [Fact]
        public async Task DeleteUser_OK()
        {
            // arrange
            string inUserName = "Test";
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            mockIRepo.Setup(x => x.DeleteUser(inUserName)).ReturnsAsync(true);
            var result = await mockAdminController.DeleteUser(inUserName);

            // assert
            var okResult = result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("User successfully deleted.", message);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task DeleteUser_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";
            mockIRepo.Setup(x => x.DeleteUser(inUserName)).ReturnsAsync(false);
            var result = await mockAdminController.DeleteUser(inUserName);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not delete the user.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";
            mockIRepo.Setup(x => x.DeleteUser(inUserName)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockAdminController.DeleteUser(inUserName);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown delete user error.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Delete admin
        [Fact]
        public async Task DeleteAdmin_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string adminToDelete = "Test";
            mockIRepo.Setup(x => x.DeleteAdmin(adminToDelete)).ReturnsAsync(true);
            var result = await mockAdminController.DeleteAdmin(adminToDelete);

            // assert
            var okResult = result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("Admin was successfully deleted.", message);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task DeleteAdmin_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string adminToDelete = "Test";
            mockIRepo.Setup(x => x.DeleteAdmin(adminToDelete)).ReturnsAsync(false);
            var result = await mockAdminController.DeleteAdmin(adminToDelete);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not delete the admin.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAdmin_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string adminToDelete = "Test";
            mockIRepo.Setup(x => x.DeleteAdmin(adminToDelete)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockAdminController.DeleteAdmin(adminToDelete);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown delete admin error.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Create printer

        [Fact]
        public async Task CreatePrinter_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            Printer newPrinter = new Printer
            {
                PrinterName = "Test",
                IP = "192.168.1.44",
                ApiKey = "6f4c9a4cdf2d891854c30c626128be16a3ee853e441e051dc708e74d115a",
                ApiSecret = "6a3ee853e441e0516f4c9a4cdc708e74d115adf2d891854c30c626128be1",
            };

            mockIRepo.Setup(x => x.CreatePrinter(newPrinter)).ReturnsAsync(true);
            var result = await mockAdminController.CreatePrinter(newPrinter);

            // assert
            var okResult = result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("Printer has been successfully created.", message);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task CreatePrinter_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            Printer newPrinter = new Printer
            {
                PrinterName = "Test",
                IP = "192.168.1.44",
                ApiKey = "",
                ApiSecret = ""
            };

            mockIRepo.Setup(x => x.CreatePrinter(newPrinter)).ReturnsAsync(false);
            var result = await mockAdminController.CreatePrinter(newPrinter);

            // assert
            var badValue = result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Printer could not be created.", message);
            Assert.Equal(400, badValue.StatusCode);
        }

        [Fact]
        public async Task CreatePrinter_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            Printer newPrinter = new Printer
            {
                PrinterName = "Test",
                IP = "192.168.1.44",
                ApiKey = "",
                ApiSecret = ""
            };

            mockIRepo.Setup(x => x.CreatePrinter(newPrinter)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockAdminController.CreatePrinter(newPrinter);

            // assert
            var badValue = result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Unknown create printer error.", message);
            Assert.Equal(400, badValue.StatusCode);
        }
        //Delete printer
        [Fact]
        public async Task DeletePrinter_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string printerToDelete = "Test";

            mockIRepo.Setup(x => x.DeletePrinter(printerToDelete)).ReturnsAsync(true);
            var result = await mockAdminController.DeletePrinter(printerToDelete);

            // assert
            var okResult = result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("Printer has been successfully deleted.", message);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeletePrinter_Fail()
        {
            // arrange;
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string printerToDelete = "Test";

            mockIRepo.Setup(x => x.DeletePrinter(printerToDelete)).ReturnsAsync(false);
            var result = await mockAdminController.DeletePrinter(printerToDelete);

            // assert
            var badValue = result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Could not delete printer.", message);
            Assert.Equal(400, badValue.StatusCode);
        }

        [Fact]
        public async Task DeletePrinter_Exception()
        {
            // arrange;
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string printerToDelete = "Test";

            mockIRepo.Setup(x => x.DeletePrinter(printerToDelete)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockAdminController.DeletePrinter(printerToDelete);

            // assert
            var badValue = result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Unknown delete printer error.", message);
            Assert.Equal(400, badValue.StatusCode);
        }
        //Update Printer
        [Fact]
        public async Task UpdatePrinter_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            Printer inPrinter = new Printer
            {
                Id = 1,
                PrinterName = "Test",
                IP = "192.168.1.39",
                ApiKey = "test",
                ApiSecret = "test"
            };

            mockIRepo.Setup(x => x.UpdatePrinter(inPrinter)).ReturnsAsync(true);
            var result = await mockAdminController.UpdatePrinter(inPrinter);

            // assert
            var okResult = result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("Printer has been successfully updated.", message);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdatePrinter_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            Printer inPrinter = new Printer
            {
                Id = 1,
                PrinterName = "Test",
                IP = "192.168.1.39",
                ApiKey = "test",
                ApiSecret = "test"
            };

            mockIRepo.Setup(x => x.UpdatePrinter(inPrinter)).ReturnsAsync(false);
            var result = await mockAdminController.UpdatePrinter(inPrinter);

            // assert
            var badValue = result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Could not update printer.", message);
            Assert.Equal(400, badValue.StatusCode);
        }

        [Fact]
        public async Task UpdatePrinter_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            Printer inPrinter = new Printer
            {
                Id = 1,
                PrinterName = "Test",
                IP = "192.168.1.39",
                ApiKey = "test",
                ApiSecret = "test"
            };

            mockIRepo.Setup(x => x.UpdatePrinter(inPrinter)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockAdminController.UpdatePrinter(inPrinter);

            // assert
            var badValue = result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Unknown update printer error.", message);
            Assert.Equal(400, badValue.StatusCode);
        }

        //Return Prints Per Printer
        [Fact]
        public async Task ReturnPrintsPerPrinter_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inPrinterName = "Test";
            DateTime utcNow = DateTime.UtcNow;
            DateTime cetNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            PrintingHistory returnPrintingHistory = new PrintingHistory
            {
                Id = 1,
                UserName = "Test",
                PlasticWeight = 1,
                StartedPrintingAt = cetNow,
                PrinterName = inPrinterName,
                FileName = "Test",
                FinishedPrintingAt = cetNow.AddMinutes(60)
            };

            List<PrintingHistory> printingHistories = new List<PrintingHistory>();
            printingHistories.Add(returnPrintingHistory);

            mockIRepo.Setup(x => x.ReturnPrintsPerPrinter(inPrinterName)).ReturnsAsync(printingHistories);
            var actionResult = await mockAdminController.ReturnPrintsPerPrinter(inPrinterName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal(printingHistories, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ReturnPrintsPerPrinter_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inPrinterName = "Test";
            mockIRepo.Setup(x => x.ReturnPrintsPerPrinter(inPrinterName)).ReturnsAsync(() => null);
            var actionResult = await mockAdminController.ReturnPrintsPerPrinter(inPrinterName);

            // Assert
            var badResult = actionResult as NotFoundObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not find prints.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }

        [Fact]
        public async Task ReturnPrintsPerPrinter_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inPrinterName = "Test";

            mockIRepo.Setup(x => x.ReturnPrintsPerPrinter(inPrinterName)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.ReturnPrintsPerPrinter(inPrinterName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown return prints per printer error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }
        //Printing history for user
        [Fact]
        public async Task PrintingHistoryForUser_OK()
        {
            // arrange
            string inUserName = "Test";
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            

            DateTime utcNow = DateTime.UtcNow;
            DateTime cetNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            PrintingHistory aHistory = new PrintingHistory
            {
                Id = 1,
                UserName = "Test",
                PlasticWeight = 1,
                StartedPrintingAt = cetNow,
                PrinterName = "TestPrinter",
                FileName = "TestFile",
                FinishedPrintingAt = cetNow.AddMinutes(60)
            };

            List<PrintingHistory> printingHistories = new List<PrintingHistory>();
            printingHistories.Add(aHistory);

            mockIRepo.Setup(x => x.PrintingHistoryForUser(inUserName)).ReturnsAsync(printingHistories);
            var actionResult = await mockAdminController.PrintingHistoryForUser(inUserName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal(printingHistories, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task PrintingHistoryForUser_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";
            mockIRepo.Setup(x => x.PrintingHistoryForUser(inUserName)).ReturnsAsync(() => null);
            var actionResult = await mockAdminController.PrintingHistoryForUser(inUserName);

            // Assert
            var badResult = actionResult as NotFoundObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not find user or prints.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }

        [Fact]
        public async Task PrintingHistoryForUser_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";
            mockIRepo.Setup(x => x.PrintingHistoryForUser(inUserName)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.PrintingHistoryForUser(inUserName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown printing history per user error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Get all printers
        [Fact]
        public async Task GetAllPrinters_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            ListOfPrinters aPrinter = new ListOfPrinters
            {
                Id = 1,
                PrinterName = "Cleopatra",
                ApiKey = "test",
                ApiSecret = "test",
                PrinterAdminLock = false
            };
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            List<ListOfPrinters> allPrinters = new List<ListOfPrinters>();
            allPrinters.Add(aPrinter);
            Task<List<ListOfPrinters>> task = Task.FromResult(allPrinters);

            mockIRepo.Setup(x => x.GetAllPrinters()).ReturnsAsync(allPrinters);
            var actionResult = await mockAdminController.GetAllPrinters();

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value as IEnumerable<ListOfPrinters>;

            Assert.NotNull(okResult);
            Assert.Equal(allPrinters, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task GetAllPrinters_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllPrinters()).ReturnsAsync(() => null);
            var actionResult = await mockAdminController.GetAllPrinters();

            // Assert
            var badResult = actionResult as NotFoundObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not get all printers.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }

        [Fact]
        public async Task GetAllPrinters_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetAllPrinters()).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.GetAllPrinters();

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown get all printers error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Unlock and lock printer
        [Fact]
        public async Task UnlockAndLockPrinter_Lock()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            string inPrinter = "testPrinter";
            string inCommand = "lock";
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.UnlockAndLockPrinter(inPrinter, inCommand)).ReturnsAsync(true);
            var actionResult = await mockAdminController.UnlockAndLockPrinter(inPrinter, inCommand);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal("Printer was locked.", resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UnlockAndLockPrinter_Unlock()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            string inPrinter = "testPrinter";
            string inCommand = "unlock";
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.UnlockAndLockPrinter(inPrinter, inCommand)).ReturnsAsync(true);
            var actionResult = await mockAdminController.UnlockAndLockPrinter(inPrinter, inCommand);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal("Printer was unlocked.", resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UnlockAndLockPrinter_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            ;
            string inPrinter = "testPrinter";
            string inCommand = "unlock";
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.UnlockAndLockPrinter(inPrinter, inCommand)).ReturnsAsync(false);
            var actionResult = await mockAdminController.UnlockAndLockPrinter(inPrinter, inCommand);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not unlock printer.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task UnlockAndLockPrinter_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            string inPrinter = "testPrinter";
            string inCommand = "unlock";
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.UnlockAndLockPrinter(inPrinter, inCommand)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.UnlockAndLockPrinter(inPrinter, inCommand);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown unlock and lock printer error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Get printing queue
        [Fact]
        public async Task GetPrintingQueue_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            List<PrintingQueue> printingQueue = new List<PrintingQueue>
            {
                new PrintingQueue { Id =1, FileName = "TestFile1", UserName = "TestUser1", PrintTime = "100", PlasticWeight = 10},
                new PrintingQueue { Id =2, FileName = "TestFile2", UserName = "TestUser2", PrintTime = "200", PlasticWeight = 20},
                new PrintingQueue { Id =3, FileName = "TestFile3", UserName = "TestUser3", PrintTime = "300", PlasticWeight = 30},
            };


            mockIRepo.Setup(x => x.GetPrintingQueue()).ReturnsAsync(printingQueue);
            var actionResult = await mockAdminController.GetPrintingQueue();

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal(printingQueue, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetPrintingQueue_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            mockIRepo.Setup(x => x.GetPrintingQueue()).ReturnsAsync(() => null);
            var actionResult = await mockAdminController.GetPrintingQueue();

            // Assert
            var badResult = actionResult as NotFoundObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not find printing queue.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }

        [Fact]
        public async Task GetPrintingQueue_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };


            mockIRepo.Setup(x => x.GetPrintingQueue()).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.GetPrintingQueue();

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown get printing queue error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Remove print from queue
        [Fact]
        public async Task RemovePrintFromQueue_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            string inFileName = "TestFile";
            string inUserName = "TestUser";

            mockIRepo.Setup(x => x.CancelQueuePrint(inFileName, inUserName)).ReturnsAsync(true);
            var actionResult = await mockAdminController.CancelQueuePrint(inFileName, inUserName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal(true, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task RemovePrintFromQueue_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            string inFileName = "TestFile";
            string inUserName = "TestUser";


            mockIRepo.Setup(x => x.CancelQueuePrint(inFileName, inUserName)).ReturnsAsync(false);
            var actionResult = await mockAdminController.CancelQueuePrint(inFileName, inUserName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could no remove print from Queue.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task RemovePrintFromQueue_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inFileName = "TestFile";
            string inUserName = "TestUser";


            mockIRepo.Setup(x => x.CancelQueuePrint(inFileName, inUserName)).ThrowsAsync(new Exception("Something went wrong"));
            var actionResult = await mockAdminController.CancelQueuePrint(inFileName, inUserName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown remove print from queue error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Stream from printer
        [Fact]
        public async Task StreamFromPrinter_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            string inPrinter = "TestPrinter";

            mockIRepo.Setup(x => x.StreamFromPrinter(inPrinter)).ReturnsAsync(true);
            var actionResult = await mockAdminController.StreamFromPrinter(inPrinter);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task StreamFromPrinter_False()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inPrinter = "TestPrinter";
            mockIRepo.Setup(x => x.StreamFromPrinter(inPrinter)).ReturnsAsync(false);
            var actionResult = await mockAdminController.StreamFromPrinter(inPrinter);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not start stream.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task StreamFromPrinter_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            string inPrinter = "TestPrinter";

            mockIRepo.Setup(x => x.StreamFromPrinter(inPrinter)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.StreamFromPrinter(inPrinter);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown stream from printer error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Make new QandA
        [Fact]
        public async Task MakeNewQAndA_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            QandA newQandA = new QandA
            {
                Question = "What is a 3Printer?",
                Answers = "A 3D printer can print with molten plastic and create 3D shapes."
            };


            mockIRepo.Setup(x => x.MakeNewQAndA(newQandA)).ReturnsAsync(true);
            var actionResult = await mockAdminController.MakeNewQAndA(newQandA);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("New  question and answer has been added.", resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task MakeNewQAndA_False()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            QandA newQandA = new QandA
            {
                Question = "What is a 3Printer?",
                Answers = "A 3D printer can print with molten plastic and create 3D shapes."
            };

            mockIRepo.Setup(x => x.MakeNewQAndA(newQandA)).ReturnsAsync(false);
            var actionResult = await mockAdminController.MakeNewQAndA(newQandA);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not make a new question and answer.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task MakeNewQAndA_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            QandA newQandA = new QandA
            {
                Question = "What is a 3Printer?",
                Answers = "A 3D printer can print with molten plastic and create 3D shapes."
            };

            mockIRepo.Setup(x => x.MakeNewQAndA(newQandA)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.MakeNewQAndA(newQandA);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown make a new Q&A error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Get QandA
        [Fact]
        public async Task GetQandA_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            List<QandA> listOfAandA = new List<QandA>
            {
                new QandA { Id =1, Question = "Test question", Answers = "Test answer"},
                new QandA { Id =2, Question = "Test question two", Answers = "Test answer two"},
            };


            mockIRepo.Setup(x => x.GetQAndA()).ReturnsAsync(listOfAandA);
            var actionResult = await mockAdminController.GetQAndA();

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal(listOfAandA, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetQandA_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };


            List<QandA> listOfAandA = new List<QandA>
            {
                new QandA { Id =1, Question = "Test question", Answers = "Test answer"},
                new QandA { Id =2, Question = "Test question two", Answers = "Test answer two"},
            };


            mockIRepo.Setup(x => x.GetQAndA()).ReturnsAsync(() => null);
            var actionResult = await mockAdminController.GetQAndA();

            // Assert
            var badResult = actionResult as NotFoundObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not return all questions and answers.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }
        [Fact]
        public async Task GetQandA_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };


            List<QandA> listOfAandA = new List<QandA>
            {
                new QandA { Id =1, Question = "Test question", Answers = "Test answer"},
                new QandA { Id =2, Question = "Test question two", Answers = "Test answer two"},
            };

            mockIRepo.Setup(x => x.GetQAndA()).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.GetQAndA();

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown get Q&A error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Edit QandA
        [Fact]
        public async Task EditQAndA_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            QandA newQandA = new QandA
            {
                Id = 1,
                Question = "Test question",
                Answers = "Test answer"
            };


            mockIRepo.Setup(x => x.EditQAndA(newQandA)).ReturnsAsync(true);
            var actionResult = await mockAdminController.EditQAndA(newQandA);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.NotNull(okResult);
            Assert.Equal(true, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task EditQAndA_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            QandA newQandA = new QandA
            {
                Id = 1,
                Question = "Test question",
                Answers = "Test answer"
            };


            mockIRepo.Setup(x => x.EditQAndA(newQandA)).ReturnsAsync(false);
            var actionResult = await mockAdminController.EditQAndA(newQandA);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not update questions and answers.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task EditQAndA_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            QandA newQandA = new QandA
            {
                Id = 1,
                Question = "Test question",
                Answers = "Test answer"
            };


            mockIRepo.Setup(x => x.EditQAndA(newQandA)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.EditQAndA(newQandA);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown edit Q&A error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Cancel print
        [Fact]
        public async Task CancelPrint_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inUserName = "TestUser";
            string inFileName = "TestFile";

            mockIRepo.Setup(x => x.CancelOngoingPrint(inUserName, inFileName)).ReturnsAsync(true);
            var actionResult = await mockAdminController.CancelOngoingPrint(inUserName, inFileName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.NotNull(okResult);
            Assert.Equal("Print was cancelled.", resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task CancelPrint_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inUserName = "TestUser";
            string inFileName = "TestFile";

            mockIRepo.Setup(x => x.CancelOngoingPrint(inUserName, inFileName)).ReturnsAsync(false);
            var actionResult = await mockAdminController.CancelOngoingPrint(inUserName, inFileName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not cancel print.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task CancelPrint_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inUserName = "TestUser";
            string inFileName = "TestFile";

            mockIRepo.Setup(x => x.CancelOngoingPrint(inUserName, inFileName)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.CancelOngoingPrint(inUserName, inFileName);


            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown cancel print error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Delete Q&A
        [Fact]
        public async Task DeleteQAndA_OK()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            int inId = 1;

            mockIRepo.Setup(x => x.DeleteQAndA(inId)).ReturnsAsync(true);
            var actionResult = await mockAdminController.DeleteQAndA(inId);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.NotNull(okResult);
            Assert.Equal("Questions and answers were removed.", resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteQAndA_Fail()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);
            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            int inId = 1;

            mockIRepo.Setup(x => x.DeleteQAndA(inId)).ReturnsAsync(false);
            var actionResult = await mockAdminController.DeleteQAndA(inId);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could not remove questions and answers.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task DeleteQAndA_Exception()
        {
            // arrange
            var mockAdminController = new AdminController(mockAdminLog.Object, mockIRepo.Object);

            mockAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            int inId = 1;

            mockIRepo.Setup(x => x.DeleteQAndA(inId)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockAdminController.DeleteQAndA(inId);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown delete Q&A error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //USER

        //Create User
        //[Fact]
        //public async Task CreateUser_OK()
        //{
        //    // arrange
        //    var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
        //    mockUserController.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = mockHttpContext.Object
        //    };

        //    // act
        //    var inNewUser = new User
        //    {
        //        Email = "unitTest@oslomet.no"
        //    };

        //    mockIRepo.Setup(x => x.CreateUser(inNewUser)).ReturnsAsync(true);
        //    var result = await mockUserController.CreateUser(inNewUser);

        //    // assert
        //    var okResult = result as OkObjectResult;
        //    var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

        //    Assert.Equal("User was created.", message);
        //    Assert.Equal(200, okResult.StatusCode);
        //}
        //[Fact]
        //public async Task CreateUser_Fail()
        //{
        //    // arrange
        //    var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
        //    mockUserController.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = mockHttpContext.Object
        //    };

        //    // act
        //    var inNewUser = new User
        //    {
        //        Email = "",
        //    };

        //    mockIRepo.Setup(x => x.CreateUser(inNewUser)).ReturnsAsync(false);
        //    var result = await mockUserController.CreateUser(inNewUser);

        //    // assert
        //    var badValue = result as BadRequestObjectResult;
        //    var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

        //    Assert.Equal("User couldn't be created.", message);
        //    Assert.Equal(400, badValue.StatusCode);
        //}
        //[Fact]
        //public async Task CreateUser_Exception()
        //{
        //    // arrange

        //    var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
        //    mockUserController.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = mockHttpContext.Object
        //    };

        //    // act
        //    var inNewUser = new User
        //    {
        //        Email = "",
        //    };

        //    mockIRepo.Setup(x => x.CreateUser(inNewUser)).ThrowsAsync(new Exception("Something went wrong."));
        //    var result = await mockUserController.CreateUser(inNewUser);

        //    // assert
        //    var badValue = result as BadRequestObjectResult;
        //    var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

        //    Assert.Equal("Unknown create user error.", message);
        //    Assert.Equal(400, badValue.StatusCode);
        //}

        //Log in
        [Fact]
        public async Task LogIn_OK()
        {
            // arrange
            string inUserName = "Test";
            string inPassword = "TestPassword";
            string outJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIi" +
            "wibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act

            mockIRepo.Setup(x => x.LogIn(inUserName, inPassword)).ReturnsAsync((true, outJWT));
            var result = await mockUserController.LogIn(inUserName, inPassword);

            // assert
            var okResult = result.Result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal(outJWT, message);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task LogIn_PasswordExpired()
        {
            // arrange
            string inUserName = "Test";
            string inPassword = "TestPassword";
            string returnMessage = "Password expired, new email has been sent";
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act


            mockIRepo.Setup(x => x.LogIn(inUserName, inPassword)).ReturnsAsync((true, returnMessage));
            var result = await mockUserController.LogIn(inUserName, inPassword);

            // assert
            var badValue = result.Result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal(returnMessage, message);
            Assert.Equal(400, badValue.StatusCode);
        }

        [Fact]
        public async Task LogIn_Fail_WrongPassword()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";
            string inPassword = "TestPassword";

            mockIRepo.Setup(x => x.LogIn(inUserName, inPassword)).ReturnsAsync((false, It.IsAny<string>()));

            var result = await mockUserController.LogIn(inUserName, inPassword);
            // assert
            var badValue = result.Result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Username or password is wrong, a new email has been sent.", message);
            Assert.Equal(400, badValue.StatusCode);
        }

        [Fact]
        public async Task LogIn_Exception()
        {
            // arrange
        
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);

            // setup mock HttpContext and session
          //  mockHttpContext.SetupGet(c => c.Session).Returns(mockSession);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";
            string inPassword = "TestPassword";


            mockIRepo.Setup(x => x.LogIn(inUserName, inPassword)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockUserController.LogIn(inUserName, inPassword);

            // assert
            var badValue = result.Result as BadRequestObjectResult;
            var message = badValue.Value.GetType().GetProperty("message").GetValue(badValue.Value, null);

            Assert.Equal("Unknown login user error.", message);
            Assert.Equal(400, badValue.StatusCode);
        }

        //Upload file
        [Fact]
        public async Task Upload_User_OK()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            var fileMock = new Mock<IFormFile>();
            string inUser = "TestUser";

            mockIRepo.Setup(x => x.Upload(fileMock.Object, inUser)).ReturnsAsync((true, 1, 1));
            var result = await mockUserController.Upload(fileMock.Object, inUser);

            // assert
            var okResult = result as OkObjectResult;
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);

            Assert.Equal("File was uploaded.", message);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Upload_User_Fail()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            var fileMock = new Mock<IFormFile>();
            string inUser = "TestUser";

            mockIRepo.Setup(x => x.Upload(fileMock.Object, inUser)).ReturnsAsync((false, 0, 0));
            var result = await mockUserController.Upload(fileMock.Object, inUser);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("File verification error.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task Upload_User_BackendError()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            var fileMock = new Mock<IFormFile>();
            string inUser = "TestUser";

            mockIRepo.Setup(x => x.Upload(fileMock.Object, inUser)).ReturnsAsync((false, 0, 1));
            var result = await mockUserController.Upload(fileMock.Object, inUser);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown backend error.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task Upload_User_OtherErrors()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            var fileMock = new Mock<IFormFile>();
            string inUser = "TestUser";

            mockIRepo.Setup(x => x.Upload(fileMock.Object, inUser)).ReturnsAsync((false, 1, 1));
            var result = await mockUserController.Upload(fileMock.Object, inUser);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("File could not be uploaded, gCode and below 265mb.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task Upload_User_Exception()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            var fileMock = new Mock<IFormFile>();
            string inUser = "TestUser";

            mockIRepo.Setup(x => x.Upload(fileMock.Object, inUser)).ThrowsAsync(new Exception("Something went wrong."));
            var result = await mockUserController.Upload(fileMock.Object, inUser);

            // assert
            var badResult = result as BadRequestObjectResult;
            var message = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown upload error.", message);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Printing history for user
        [Fact]
        public async Task PrintingHistoryForUser_User_OK()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";

            DateTime utcNow = DateTime.UtcNow;
            DateTime cetNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            PrintingHistory aHistory = new PrintingHistory
            {
                Id = 1,
                UserName = "Test",
                PlasticWeight = 1,
                StartedPrintingAt = cetNow,
                PrinterName = "TestPrinter",
                FileName = "TestFile",
                FinishedPrintingAt = cetNow.AddMinutes(60)
            };


            List<PrintingHistory> printingHistories = new List<PrintingHistory>();
            printingHistories.Add(aHistory);

            mockIRepo.Setup(x => x.PrintingHistoryForUser(inUserName)).ReturnsAsync(printingHistories);
            var actionResult = await mockUserController.PrintingHistoryForUser(inUserName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            var resultValue = okResult.Value;

            Assert.Equal(printingHistories, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task PrintingHistoryForUser_User_Fail()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";

            mockIRepo.Setup(x => x.PrintingHistoryForUser(inUserName)).ReturnsAsync(() => null);
            var actionResult = await mockUserController.PrintingHistoryForUser(inUserName);

            // Assert
            var badResult = actionResult as NotFoundObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("User not found.", resultValue);
            Assert.Equal(404, badResult.StatusCode);
        }

        [Fact]
        public async Task PrintingHistoryForUser_User_Exception()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // act
            string inUserName = "Test";

            mockIRepo.Setup(x => x.PrintingHistoryForUser(inUserName)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockUserController.PrintingHistoryForUser(inUserName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown printing history error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Remove print from queue
        [Fact]
        public async Task RemovePrintFromQueue_User_OK()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inFileName = "TestFile";
            string inUserName = "TestUser";


            mockIRepo.Setup(x => x.CancelQueuePrint(inFileName, inUserName)).ReturnsAsync(true);

            var actionResult = await mockUserController.CancelQueuePrint(inFileName, inUserName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);

            var resultValue = okResult.Value;
            Assert.Equal(true, resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task RemovePrintFromQueue_User_Fail()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inFileName = "TestFile";
            string inUserName = "TestUser";


            mockIRepo.Setup(x => x.CancelQueuePrint(inFileName, inUserName)).ReturnsAsync(false);

            var actionResult = await mockUserController.CancelQueuePrint(inFileName, inUserName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Could no remove print from queue.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }


        [Fact]
        public async Task RemovePrintFromQueue_User_Exception()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inFileName = "TestFile";
            string inUserName = "TestUser";


            mockIRepo.Setup(x => x.CancelQueuePrint(inFileName, inUserName)).ThrowsAsync(new Exception("Something went wrong"));

            var actionResult = await mockUserController.CancelQueuePrint(inFileName, inUserName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown remove print from queue error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Cancel ongoing print
        [Fact]
        public async Task CancelPrint_User_OK()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            string inUserName = "TestUser";
            string inFileName = "TestFile";


            mockIRepo.Setup(x => x.CancelOngoingPrint(inUserName, inFileName)).ReturnsAsync(true);
            var actionResult = await mockUserController.CancelOngoingPrint(inUserName, inFileName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);

            var resultValue = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);
            Assert.Equal("Print was cancelled.", resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task CancelPrint_User_Fail()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inUserName = "TestUser";
            string inFileName = "TestFile";


            mockIRepo.Setup(x => x.CancelOngoingPrint(inUserName, inFileName)).ReturnsAsync(false);
            var actionResult = await mockUserController.CancelOngoingPrint(inUserName, inFileName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;

            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);
            Assert.Equal("Could not cancel print.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task CancelPrint_User_Exception()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            string inUserName = "TestUser";
            string inFileName = "TestFile";

            mockIRepo.Setup(x => x.CancelOngoingPrint(inUserName, inFileName)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockUserController.CancelOngoingPrint(inUserName, inFileName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;

            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);
            Assert.Equal("Unknown cancel print error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        //Delete file
        [Fact]
        public async Task DeleteFile_User_OK()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            User aUser = new User
            {
                Id = 1,
                Email = "test@oslomet.no"
            };
            string inFileName = "TestFile";

            mockIRepo.Setup(x => x.DeleteFile(aUser, inFileName)).ReturnsAsync(true);
            var actionResult = await mockUserController.DeleteFile(aUser, inFileName);

            // Assert
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);

            var resultValue = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);
            Assert.Equal("File deleted successfully.", resultValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteFile_User_Fail()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            User aUser = new User
            {
                Id = 1,
                Email = "test@oslomet.no"
            };
            string inFileName = "TestFile";

            mockIRepo.Setup(x => x.DeleteFile(aUser, inFileName)).ReturnsAsync(false);
            var actionResult = await mockUserController.DeleteFile(aUser, inFileName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("File could not be deleted.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task DeleteFile_User__Exception()
        {
            // arrange
            var mockUserController = new UserController(mockIRepo.Object, mockLogUser.Object);
            mockUserController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            User aUser = new User
            {
                Id = 1,
                Email = "test@oslomet.no"
            };

            string inFileName = "TestFile";

            mockIRepo.Setup(x => x.DeleteFile(aUser, inFileName)).ThrowsAsync(new Exception("Something went wrong."));
            var actionResult = await mockUserController.DeleteFile(aUser, inFileName);

            // Assert
            var badResult = actionResult as BadRequestObjectResult;
            var resultValue = badResult.Value.GetType().GetProperty("message").GetValue(badResult.Value, null);

            Assert.Equal("Unknown delete file error.", resultValue);
            Assert.Equal(400, badResult.StatusCode);
        }
    }
}