using BachelorsProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace BachelorsProject.DAL
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool PasswordValid { get; set; } //true yes, false no
        public DateTime PasswordCreatedOn { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedOn { get; set; }

        public Users() {
            Email = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Role = Role.User ;
            CreatedOn = System.DateTime.Now;
        }
    }


    public class PrintingQueue
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string UserName { get; set; }
        public string PrintTime { get; set; }
        public double PlasticWeight { get; set; }

        public PrintingQueue()
        {
            FileName = string.Empty;
            UserName = string.Empty;
            PrintTime = string.Empty;
        }
    }

    public class ListOfPrinters
    {
        public int Id { get; set; }
        public string PrinterName { get; set; }
        public string IP { get; set; }
        //public int Port { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        //printer can be "Locked, Printing, Error, Available"
        public string? PrinterStatus { get; set; }
        public bool? PrinterAdminLock { get; set; }

        public ListOfPrinters() 
        { 
            PrinterName = string.Empty;
            IP = string.Empty;
            //Port = 0;
            PrinterStatus = string.Empty;
            ApiKey = string.Empty;
            ApiSecret = string.Empty;
        }
    }

    public class UploadedFilesInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullFilePath { get; set; }
        public string FileName { get; set; }
        //Weight => grams
        public double PlasticWeight { get; set; } 
        public double PrintingTime { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }

        public UploadedFilesInfo()
        {
            UserName = string.Empty;
            FullFilePath = string.Empty;
            FileName = string.Empty;
            PlasticWeight = 0;
            PrintingTime = 0;
            Deleted = false;
            CreatedOn = System.DateTime.Now;
        }
    }

    public class PrintingHistory
    {
        public int Id { get; set; }
        public string UserName { get; set; } 
        public string FileName { get; set; }
        public string PrinterName { get; set; }
        public double PlasticWeight { get; set; }
        public DateTime StartedPrintingAt { get; set; }
        public DateTime FinishedPrintingAt { get; set; }

        public PrintingHistory()
        {
            FileName = string.Empty;
            PrinterName = string.Empty;
            FinishedPrintingAt = DateTime.UtcNow;
            UserName = string.Empty;
            PlasticWeight = 0;
        }
    }

    public class Admins
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool PasswordValid { get; set; } //true yes, false no
        public DateTime PasswordCreatedOn { get; set; }

        public Admins()
        {
            UserName = string.Empty;
            Email= string.Empty;
            Password = string.Empty;
            Role = Role.Admin;
            CreatedOn = DateTime.UtcNow;
        }
    }

    public enum Role
    {
        Admin,
        User
    }

    public class QandA
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answers { get; set; }

        public QandA()
        {
            Question = string.Empty;
            Answers = string.Empty;
        }
    }


    public class PrintFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string UserName { get; set; }
        public double PlasticWeight { get; set; }
        public double PrintTime { get; set; } 
        public string PrinterName { get; set; }

        public PrintFile()
        {
            FileName = string.Empty;
            UserName = string.Empty; 
            PrinterName = string.Empty;
            PlasticWeight = 0;
            PrintTime = 0;
        }
    }


    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {
            Database.EnsureCreated();
            //Printers = Set<Printers>();
            //PrintFiles = Set<PrintFile>();
        }

        public DbSet<Users>? Users { get; set; }
        //public DbSet<Transaction>? Transactions { get; set; }
        public DbSet<PrintingQueue>? PrintingQueues { get; set; }
        public DbSet<ListOfPrinters>? ListOfPrinters { get; set; }
        public DbSet<UploadedFilesInfo>? UploadedFilesInfo { get; set; }
        public DbSet<PrintingHistory> PrintingHistory { get; set; }
        public DbSet<Admins> Admins{ get; set; }
        public DbSet<QandA> QandA { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}

//LEAVE THIS CODE; REMOVE ONLY AT END
/* 
namespace BachelorsProject.DAL

{

    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        //need to figure our roles per user
       // public User() { } //constructor
    }

    public class Transactions
    {
        public int Id { get; set; }
        public DateTime DateTiem { get; set; }
        public string FileName { get; set; }
       // public Transaction() { }

        //lazy loading
        virtual public User User { get; set; }

    }

    public class PrintingQueues
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool PrinterStatus { get; set; }
        //public PrintingQueue() { }
    }
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options)
                : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("test.db");
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<PrintingQueues> PrintingQueues { get; set; }

        //use lazy loading
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        } 
        
    }
}
*/