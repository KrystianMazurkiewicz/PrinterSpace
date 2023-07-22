using BachelorsProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BachelorsProject.DAL
{
    public class DBInit
    {
        public object Users { get; internal set; }
        //test

        public static void Initialize(IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.CreateScope();

            var db = serviceScope.ServiceProvider.GetService<SystemContext>();
            if(db != null)
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.SaveChanges();
            }
            TimeZoneInfo centralEUZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime centralEuTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralEUZone);

   

            //test hashing algo

            User user = new User {Email = "s348832@oslomet.com" };
            User user2 = new User {Email = "s348888@oslomet.com"};

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            string hashedAndSaltedPassword = passwordHasher.HashPassword(user, "dfsidfjasdmfkasdmfasio234iu2" );
            string hashedAndSaltedPassword1 = passwordHasher.HashPassword(user2, "dfsidfjasdmfkasdmfasio234iudadasdasda2");

            var testUser = new Users {Password = hashedAndSaltedPassword, Email = "TestUser1@oslomet.no", Username = "TestUser1", PasswordValid= false, CreatedOn = centralEuTime.AddYears(-1), PasswordCreatedOn= centralEuTime.AddYears(-1) };
            var testUser1 = new Users {Password = hashedAndSaltedPassword1, Email = "Tester1@oslomet.no", Username = "Tester1", PasswordValid = true, CreatedOn = centralEuTime.AddDays(-30), PasswordCreatedOn = centralEuTime.AddDays(-30) };
            var testUser2 = new Users {Password = hashedAndSaltedPassword1, Email = "s495782@oslomet.no", Username = "s495782", PasswordValid = true, CreatedOn = centralEuTime.AddDays(-30), PasswordCreatedOn = centralEuTime.AddDays(-30) };


            var listofUsers = new List<Users>
            {
                testUser, testUser1,testUser2
            };
            db.Users.AddRangeAsync(listofUsers);

            //ADMINS
            PasswordHasher<Admin> passwordHasherAdmin = new PasswordHasher<Admin>();
            Admins newAdmin = new Admins { UserName = "admin0", Email = "admin0@oslomet.no", Role = (Role)Models.Role.Admin, Password = passwordHasherAdmin.HashPassword(new Admin(),"adaskfhadhfadjuhjadsfj"), CreatedOn = centralEuTime };
            Admins newAdmin1 = new Admins {UserName = "admin1", Email = "admin1@oslomet.no", Role = (Role)Models.Role.Admin, Password = passwordHasherAdmin.HashPassword(new Admin(), "adaskfhadhfadjuhjadsfj"), CreatedOn = centralEuTime };

            var listofAdmins = new List<Admins>
            {
                newAdmin, newAdmin1
            };
            db.Admins.AddRangeAsync(listofAdmins);

            //PRINTERS
            var PrinterCleopatra = new ListOfPrinters { PrinterName = "Cleopatra", IP = "100.127.254.195", ApiKey = "bce85abf98d196883c467e6e130e6603", ApiSecret = "6a0daf0a3067d3a2e5fb7752d03cf64970b45176d2ea19889eb9e1415c9bea25", PrinterAdminLock = false, PrinterStatus = ""};
           // var PrinterLucy = new ListOfPrinters { PrinterName = "Lucy", IP = "100.127.254.200", ApiKey = "bce85abf98d196883c467e6e130e6603", ApiSecret = "6a0daf0a3067d3a2e5fb7752d03cf64970b45176d2ea19889eb9e1415c9bea25", PrinterAdminLock = false, PrinterStatus ="idle" };
            //var PrinterBoby = new ListOfPrinters { PrinterName = "Boby", IP = "100.127.254.400", ApiKey = "bce85abf986e130e6603d196883c467e", ApiSecret = "f0a3067d3a2e5f6a0dab7752d03cf64970b45176d2ea19889eb9e1415c9bea25", PrinterAdminLock = false, PrinterStatus = "error" };

            var listOfPrinters = new List<ListOfPrinters>
            {
                PrinterCleopatra //PrinterLucy, PrinterBoby
            };
            db.ListOfPrinters.AddRangeAsync(listOfPrinters);

 
            PrintingHistory newPrintingHistory = new PrintingHistory { UserName = "TestUser1", FileName = "test", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 100 };
            PrintingHistory newPrintingHistory1 = new PrintingHistory { UserName = "Tester1", FileName = "test2", StartedPrintingAt = centralEuTime, FinishedPrintingAt = centralEuTime.AddHours(1), PrinterName = "Cleopatra", PlasticWeight = 200 };
            PrintingHistory newPrintingHistory2 = new PrintingHistory { UserName = "s495782", FileName = "test3", StartedPrintingAt = centralEuTime.AddHours(-3), FinishedPrintingAt = centralEuTime.AddHours(-2), PrinterName = "Cleopatra", PlasticWeight = 300 };
            PrintingHistory newPrintingHistory3 = new PrintingHistory { UserName = "s495782", FileName = "test3", StartedPrintingAt = centralEuTime.AddHours(-2), FinishedPrintingAt = centralEuTime.AddHours(-1), PrinterName = "Lucy", PlasticWeight = 300 };
            var PrintingHistory = new List<PrintingHistory>
            {
                newPrintingHistory, newPrintingHistory1, newPrintingHistory2, newPrintingHistory3
            };
            db.PrintingHistory.AddRangeAsync(PrintingHistory);


            ////printer files
            //PrintFile newPrint0 = new PrintFile { FileName = "Print1", UserName = "s348832", PrinterName = "Cleopatra", PrintTime = 408, PlasticWeight = 239 };
            //PrintFile newPrint1 = new PrintFile { FileName = "Print2", UserName = "s348832", PrinterName = "Cleopatra", PrintTime = 123, PlasticWeight = 534 };
            //PrintFile newPrint2 = new PrintFile { FileName = "Print3", UserName = "s348832", PrinterName = "Cleopatra", PrintTime = 434, PlasticWeight = 234 };
            //PrintFile newPrint3 = new PrintFile { FileName = "Print4", UserName = "s495782", PrinterName = "Boby", PrintTime = 464, PlasticWeight = 567 };
            //PrintFile newPrint4 = new PrintFile { FileName = "Print5", UserName = "s495782", PrinterName = "Boby", PrintTime = 423, PlasticWeight = 122 };
            //PrintFile newPrint5 = new PrintFile { FileName = "Print6", UserName = "s495782", PrinterName = "Boby", PrintTime = 132, PlasticWeight = 135 };

            //var newPrintFile = new List<PrintFile>
            //{
            //    newPrint0,newPrint1, newPrint2, newPrint3,newPrint4, newPrint5
            //};
            //db.print.AddRangeAsync(newPrintFile);





            PrintingQueue newPrintToQueue = new PrintingQueue { FileName = "Testfile1", UserName = "TestUser1", PrintTime = "1340", PlasticWeight = 100 };
            PrintingQueue newPrintToQueue1 = new PrintingQueue { FileName = "Testfile2", UserName = "Tester1", PrintTime = "2340", PlasticWeight = 200 };
            PrintingQueue newPrintToQueue2 = new PrintingQueue { FileName = "Testfile3", UserName = "s495782", PrintTime = "3340", PlasticWeight = 300 };
            PrintingQueue newPrintToQueue3 = new PrintingQueue { FileName = "Testfile4", UserName = "s495782", PrintTime = "4340", PlasticWeight = 400 };

            var newPrintingQueueElements = new List<PrintingQueue>
            {
                newPrintToQueue,newPrintToQueue1,newPrintToQueue2,newPrintToQueue3
            };
            db.PrintingQueues.AddRangeAsync(newPrintingQueueElements);

            QandA newQandA = new QandA { Question = "When and how long can I print ?", Answers = "Printing is allowed on weekdays from 9:00 to 21:00. During public holidays printing is forbidden." };
            QandA newQandA1 = new QandA { Question = "How long can I print?", Answers = "Prints can start between 9:00 and must end by 21.00 the same day. If your print takes more time than the opening time, please contact the administrators at MakerSpace." };
            QandA newQandA2 = new QandA { Question = "I do not remember if I printed all the necessary pieces. What can I do?", Answers = "Navigate to your main menu and press/go to \"Your Prints\" where you will find all of your submitted prints." };
            QandA newQandA3 = new QandA { Question = "How can I cancel a print that has already been started?", Answers = "Navigate to \"Your Prints\" and click on one of " +
                "your current prints to expand the information section. Select the \"Cancel Print\" button to cancel the print.  If you cancel a print due to a mistake caused by MakerSpaces printers, your won't be charged for a reprint. If printing was cancelled due to your own mistake, please contact MakerSpace." };
            QandA newQand4 = new QandA { Question = "I have other questions that are not listed here. What do I do?", Answers = "In your main menu you will find a \"Contact Us\" option. Please write an email to the provided contact." };

            var newListOfQandA = new List<QandA>
            {
               newQandA,newQandA1,newQandA2,newQandA3,newQand4
            };
            db.QandA.AddRangeAsync(newListOfQandA);

            UploadedFilesInfo newFile = new UploadedFilesInfo { UserName = "TestUser1", FullFilePath = "C:\\Users\\Test\\Documents\\Test\\Bachelors-Project\\test.gcode", FileName = "test.gcode", PlasticWeight = 100, PrintingTime = 100, Deleted = false, CreatedOn = centralEuTime.AddMinutes(-100) };
            UploadedFilesInfo newFile1 = new UploadedFilesInfo { UserName = "Tester1", FullFilePath = "C:\\Users\\Test\\Documents\\Test\\Bachelors-Project\\test1.gcode", FileName = "test1.gcode", PlasticWeight = 200, PrintingTime = 200, Deleted = false, CreatedOn = centralEuTime.AddMinutes(-300) };
            UploadedFilesInfo newFile2 = new UploadedFilesInfo { UserName = "Tester1", FullFilePath = "C:\\Users\\Test\\Documents\\Test\\Bachelors-Project\\test1.gcode", FileName = "test1.gcode", PlasticWeight = 300, PrintingTime = 300, Deleted = true, CreatedOn = centralEuTime.AddMinutes(-700) };

            var newListUploadedFilesInfo = new List<UploadedFilesInfo>
            {
               newFile, newFile1, newFile2
            };
            db.UploadedFilesInfo.AddRangeAsync(newListUploadedFilesInfo);

            if (testUser == null) {
                throw new Exception("No null can be added");
            }

            db.SaveChanges();

        }
    }
}




       

        /*
            public SystemDB(DbContextOptions<SystemDB> options) : base(options)
        {

            Database.EnsureCreated();
        }

        public DbSet<Files> Files { get; set; }
    }
}*/



        /*
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<SystemContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }


        }
    }*/
