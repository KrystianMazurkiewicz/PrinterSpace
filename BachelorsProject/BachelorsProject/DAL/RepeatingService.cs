using BachelorsProject.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BachelorsProject.DAL;

public class RepeatingService : BackgroundService
{
    //trigger the timer every 60 second
    //private readonly PeriodicTimer _timerShort = new(TimeSpan.FromSeconds(5));
    //private readonly PeriodicTimer _timerLong = new(TimeSpan.FromHours(23));

    private readonly SystemRepository? _systemRepository;
    private readonly UserController? _userController;
    private readonly ILogger<RepeatingService> _log;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly SystemContext _db;

    public RepeatingService(ILogger<RepeatingService> log, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider)
    {
        _log = log;
        _serviceScopeFactory = serviceScopeFactory;
        _serviceProvider = serviceProvider;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        //while (await _timerShort.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        var _timerOnceAMinute = new PeriodicTimer(TimeSpan.FromSeconds(5));
        int i = 0;

        while (await _timerOnceAMinute.WaitForNextTickAsync())
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();
                
                var dbContext = scope.ServiceProvider.GetService<SystemContext>();
                var logger = scope.ServiceProvider.GetService<ILogger<SystemRepository>>();
                var webHostingEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();

                SystemRepository iteration = new SystemRepository(dbContext, logger, webHostingEnvironment);
                iteration.PeriodicTimerOnceAMinute();
                i++;
                if ((i % 10) == 0)
                {
                    iteration.PeriodicTimerOnceADay();
                    i = 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the database failure
                // Log the error or perform any necessary actions

                // Optionally, introduce a delay before restarting the function
                await Task.Delay(TimeSpan.FromSeconds(1)); // Add a delay of 10 seconds before restarting

                // Continue the loop to restart the function
                continue;
            }
        }


        //while (await _timerOnceAMinute.WaitForNextTickAsync())
        //{
        //    using (var scope = _serviceScopeFactory.CreateScope())
        //    {
        //        var dbContext = scope.ServiceProvider.GetService<SystemContext>();
        //        var logger = scope.ServiceProvider.GetService<ILogger<SystemRepository>>();
        //        var webHostingEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();

        //        SystemRepository iteration = new SystemRepository(dbContext, logger, webHostingEnvironment);
        //        iteration.PeriodicTimerOnceAMinute();
        //        i++;
        //        if ((i % 10) == 0)
        //        {
        //            iteration.PeriodicTimerOnceADay();
        //            i = 0;
        //        }

        //    }
        //}
    }
    //protected override async Task ExecuteAsyncTwo(CancellationToken stoppingToken)
    //{
    //    while (await _timerLong.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
    //    {
    //        using (var scope = _serviceScopeFactory.CreateScope())
    //        {
    //            var dbContext = scope.ServiceProvider.GetService<SystemContext>();
    //            var logger = scope.ServiceProvider.GetService<ILogger<SystemRepository>>();
    //            var webHostingEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();

    //            SystemRepository test = new SystemRepository(dbContext, logger, webHostingEnvironment);
    //            test.PeriodicTimerOnceADay();


    //        }
    //    }
    //}
}

        /*
        using (var scope = _serviceScopeFactory.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetService<SystemContext>();

        await _systemRepository.CheckStatusOfAllPrinters();


        //Periodic functions

        async Task<bool> CheckStatusOfAllPrinters()  //finish fun
        {
            try
            {
                var allPrinters = await dbContext.ListOfPrinters.ToListAsync();


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
                        if (response.IsSuccessStatusCode) //header has to be 200 to check the status
                        {
                            string status = await response.Content.ReadAsStringAsync();
                            if (status == "idle" || status == "printing" || status == "error" || status == "maintenance" || status == "booting")
                            {
                                ListOfPrinters chagedPrinter = allPrinters[i];
                                chagedPrinter.PrinterStatus = status;

                                //_db.ListOfPrinters.Update(chagedPrinter);
                                await dbContext.SaveChangesAsync();
                            }
                            //return true; //remove if not lopp breaks
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //if(Exception.ReferenceEquals)
                _log.LogInformation(ex.Message);
                return false;
            }


        }

       await CheckStatusOfAllPrinters();


    }*/
    
        /*
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            //check which printers are avaliable and set their status in the DB (status can be, error, avaliable, paused)
            try
            {
                SystemRepository test = new SystemRepository(this);
                var respons = test.PeriodicTimer();



                Console.WriteLine("Running " + respons + "\nTime: " +  DateTime.Now.ToString("O"));
            }
            catch (Exception ex) {
                _log.LogInformation(ex.Message);
            }
           

            //check the currnet time, set a lock on new prints

            //check

       
        }

        while (await _timerLong.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            //check which printers are avaliable and set their status in the DB (status can be, error, avaliable, paused)
            try
            {
                SystemRepository test = new SystemRepository(this);
                test.PeriodicTimerOnceADay();


                Console.WriteLine("Running Once a day delete function"  + "\nTime: " + DateTime.Now.ToString("O"));
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
            }


            //check the currnet time, set a lock on new prints

            //check
        }
        */
           


