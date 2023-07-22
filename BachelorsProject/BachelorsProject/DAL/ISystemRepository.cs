using BachelorsProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using QandA = BachelorsProject.DAL.QandA;

namespace BachelorsProject.DAL
{
    public interface ISystemRepository

    {
        //Accessible to ADMIN and USER
        Task<(bool, string)> LogIn(string inUserName, string inPassword);
        Task<(bool, double, double)> Upload(IFormFile inFile, string userName);
        Task<bool> CancelOngoingPrint(string userName, string inFileName);
        //Task<bool> ChangeUserInformation(User updatedUser);
        Task<bool> CancelQueuePrint(string inFileName, string inUserName);
        Task<List<PrintingQueue>> GetPrintingQueueForUser(string inUserName);

        //Accessible to USER
        Task<bool> CreateUser(string inUser);
        Task<bool> GetFileInformation(Files inFiles);
        Task<bool> DeleteFile(User inUser, string fileName);
        Task<List<PrintingHistory>> ReturnPrintsPerPrinter(string inPrinterName);

        //TESTING
        //Task<bool> SendPrintingJob(string inUserName, string inFileName, string inPrinterName);
        //Task<bool> CheckStatusOfAllPrinters();
        //////////////////

        //Accessible to ADMIN
        Task<bool> CreateAdmin(Admin inAdmin);
        Task<bool> DeleteAdmin(string inAdmin);
        Task<List<string>> GetAllUsers();
        Task<List<string>> GetAllAdmins();
        Task<bool> CreatePrinter(Printer CreatePrinter);
        Task<bool> DeletePrinter(string inPrinterName);
        Task<bool> UpdatePrinter(Printer inPrinter);
        Task<List<ListOfPrinters>> GetAllPrinters();
        Task<bool> DeleteUser(string userName);
        //Task<ListOfPrinters> GetPrinterInformation(string inPrinterName);

        Task<List<PrintingQueue>> GetPrintingQueue();
        Task<List<PrintingHistory>> PrintingHistoryForUser(string inUserName);
        Task<double> ReturnPlasticUsedForPrinter(string inPrinterName, string inCommand);
        Task <bool> UnlockAndLockPrinter (string PrinterName, string inCommand);
        Task<bool> GetImageFromPrinter();
        Task<bool> StreamFromPrinter(string inPrinterName);

        //ADMIN RELATED TO Q&A
        Task<bool> MakeNewQAndA(QandA inQandA);
        Task<List<QandA>> GetQAndA();
        Task<bool> EditQAndA(QandA inQandA);
        Task<bool> DeleteQAndA(int id);
    }
}
