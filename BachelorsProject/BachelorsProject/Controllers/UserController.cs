using BachelorsProject.DAL;
using BachelorsProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol;
using SQLitePCL;

namespace BachelorsProject.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ISystemRepository _systemDB;
        private ILogger<UserController> _log;
        public ISystemRepository Object { get; }


        //constructor

        public UserController(ISystemRepository systemDB, ILogger<UserController> log)
        {
            _systemDB = systemDB;
            _log = log;
        }

        //[HttpPost, AllowAnonymous]
        ////[Authorize(Roles ="Admin")]
        //public async Task<ActionResult> CreateUser([FromBody] string inUser)
        //{
        //    try
        //    {
        //        bool response = await _systemDB.CreateUser(inUser);
        //        if (response)
        //        {
        //            _log.LogInformation("User was successfully created.");
        //            return Ok(new { message = "User was created." });
        //        }
        //        else
        //        {
        //            _log.LogInformation("User couldn't be created");
        //            return BadRequest(new { message = "User couldn't be created." });
        //        } 
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogInformation(ex.Message);
        //        return BadRequest(new { message = "Unknown create user error." });
        //    }
        //}

        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<(bool, string)>> LogIn(string inUserName, string inPassword)
        {
            try
            {
                var response = await _systemDB.LogIn(inUserName, inPassword);
                if (response.Item1 == false)
                {
                    _log.LogInformation("User: " + inUserName + " failed login.");
                    return BadRequest(new { message = "Username or password is wrong, a new email has been sent." });
                }
                //Password expired, return true and an message - new password is sent
                if (response.Item2 == "Password expired, new email has been sent" && response.Item1 == true)
                {
                    return BadRequest(new { message = response.Item2 });
                }
                //Logged in successfully, return true and JWT in item2
                if (response.Item1 == true)
                {
                    _log.LogInformation("User: " + inUserName + "logged in.");
                    //returns JWT
                    return Ok(new { message = response.Item2 });
                }
                return BadRequest((false, ""));
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return BadRequest(new { message = "Unknown login user error." });
            }
        }

        [HttpGet, Authorize(Roles = "User")]
        public async Task<ActionResult> StreamFromPrinter(string inPrinterName)
        {
            try
            {
                var printerStream = await _systemDB.StreamFromPrinter(inPrinterName);

                if (!printerStream)
                {
                    return BadRequest(new { message = "Could not start stream." });
                }
                return Ok(printerStream);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown stream from printer error." });
            }
        }

        

        [HttpPost, Authorize(Roles = "User")]
        //[HttpPost, AllowAnonymous]
        public async Task<ActionResult> Upload(IFormFile inFile, string userName)
        {
            try
            {
                var response = await _systemDB.Upload(inFile, userName);
                if (response.Item1 == true && response.Item2 != 0 && response.Item3 != 0)
                {   // response from backend is (true, plasticWeight, TimeToPrint
                    _log.LogInformation("File was uploaded successfully.");
                    return Ok(new { message = "File was uploaded.", response.Item2, response.Item3 });
                }
                // response from backend is (bool, plasticWeight, TimeToPrint)
                if (response.Item1 == false && response.Item2 == 0 && response.Item3 == 0)
                {
                    return BadRequest(new { message = "File verification error." });
                }

                if (response.Item1 == false && response.Item2 == 0 && response.Item3 == 1)
                {
                    return BadRequest(new { message = "Unknown backend error." });
                }
                else
                {
                    _log.LogInformation("File could not be uploaded" + inFile.Name+" .");
                    return BadRequest(new { message = "File could not be uploaded, gCode and below 265mb."});
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown upload error."});
            }

        }


        // deleteGcodeFile
        [HttpPost, Authorize(Roles = "User")]
        public async Task<ActionResult> DeleteFile(User inUser, string fileName)
        {
            try
            {
                // Delete file from database
                var fileToDelete = await _systemDB.DeleteFile(inUser, fileName);
                if (fileToDelete)
                {
                    return Ok(new { message = "File deleted successfully." });

                }
                return BadRequest(new { message = "File could not be deleted." });
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown delete file error." });
            }
        }

        [HttpGet, Authorize(Roles = "User")]
        public async Task<ActionResult> GetPrintingQueueForUser(string inUserName)
        {
            try
            {
                var resultGetPrintingQueue = await _systemDB.GetPrintingQueueForUser(inUserName);

                if (resultGetPrintingQueue == null || resultGetPrintingQueue.Count == 0)
                {
                    return NotFound(new { message = "Could not find printing queue." });
                }
                return Ok(resultGetPrintingQueue);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown get printing queue error." });
            }
        }



        [HttpGet, Authorize(Roles = "User")]
        public async Task<ActionResult> PrintingHistoryForUser(string inUserName)
        {
            try
            {
                var listPrintingHistory = await _systemDB.PrintingHistoryForUser(inUserName);
                if (listPrintingHistory == null || listPrintingHistory.Count == 0)
                {
                    return NotFound(new { message = "User not found." });
                }
                else
                {
                    return Ok(listPrintingHistory);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return BadRequest(new { message = "Unknown printing history error." });
            }
        }


        [HttpGet, Authorize(Roles = "User")]
        public async Task<ActionResult> CancelQueuePrint(string inFileName, string inUserName)
        {
            try
            {
                var responseRemovePrintFromQueue = await _systemDB.CancelQueuePrint(inFileName, inUserName);

                if (!responseRemovePrintFromQueue)
                {
                    return BadRequest(new { message = "Could no remove print from queue." });
                }
                return Ok(responseRemovePrintFromQueue);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return BadRequest(new { message = "Unknown remove print from queue error." });

            }

        }

        [HttpGet, Authorize(Roles = "User")]
        public async Task<ActionResult> CancelOngoingPrint(string inUserName, string inFileName)
        {
            try
            {
                bool response = await _systemDB.CancelOngoingPrint(inUserName, inFileName);
                if (response)
                {
                    _log.LogInformation("User: " + inUserName + " sent  a gcode file" +
                        "\nwit the name: " + inFileName + "to for printing"); //create function to give absolute file path
                    return Ok(new { message = "Print was cancelled." });
                }
                else
                {
                    _log.LogInformation("User: " + inUserName + " failed to send" +
                        "\nGcode file to printer, FileName: " + inFileName);
                    return BadRequest(new { message = "Could not cancel print." });
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return BadRequest(new { message = "Unknown cancel print error." });
            }
        }

        [HttpGet, Authorize(Roles = "User")]
        public async Task<ActionResult> GetQAndA()
        {
            try
            {
                var response = await _systemDB.GetQAndA();

                if (response == null || response.Count == 0)
                {
                    return NotFound(new { message = "Could not return all questions and answers." });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown get Q&A error." });
            }
        }

        //ADD STREAM FROM PRINTER


        // [AllowAnonymous]

        /*
        string apiUrl = "http://100.127.254.230/api/v1/printer/status";
        using (var httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string status = await response.Content.ReadAsStringAsync();
                    if (status == "\"printing\"" || status == "\"paused\"")
                    {
                        // printer is currently printing or paused
                        return Ok("Printer is busy");
                    }
                    else
                    {
                        // printer is available
                        return Ok("Printer is available");
                    }
                }
                else
                {
                    // request failed
                    return BadRequest("Failed to check printer status");
                }
            }
            catch (Exception ex)
            {
                // an error occurred
                return BadRequest("An error occurred while checking printer status: " + ex.Message);
            }
        }*/



        /*
         * // This is should be in PrinterController or?
         * 
        [HttpPost, AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreatePrinter(BachelorsProject.DAL.Printers printer)
        {
            if (ModelState.IsValid)
            {
                int response = await _systemDB.CreatePrinter(printer);
                if (response > 0)
                {
                    _log.LogInformation("Printer was successfully created");
                    return Ok("Printer created");
                }
                else
                {
                    _log.LogInformation("Printer couldn't be created");
                    return BadRequest("Printer couldn't be created");
                }
            }
            else
            {
                _log.LogInformation("Fail in the model input validation");
                return BadRequest("Problem with input validation");
            }
        }

        
        */


        /*
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> GetFileInformation(Files inFile)
        {
            bool response = await _systemDB.GetFileInformation(inFile);
            if (response)
            {
                _log.LogInformation("User: " + inFile.FileName + " uploaded a gcode file"+
                    "\nGcode file path: " + inFile.FilePath); //create function to give absolute file path
                return Ok(new { message = "File is a gcode file" });
            }
            else
            {
                _log.LogInformation("User: " + inFile.UserName + " uploaded a invalid file" +
                    "\nGcode file path: " + inFile.FileName);
                return BadRequest(new { message = "Invalid file format" });
            }
        }*/

        /*
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> IsPrinterAvailable()
        {
            bool response = await _systemDB.IsPrinterAvailable();
            if (response.Equals(true))
            {
                return Ok("Printer is available");
            }
            else
            {
                return BadRequest("Printer is not available");
            }
        }*/

        /*
        //Abdi
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> UploadGCodeFile(IFormFile file)
        {
            try
            {
                var result = await _systemDB.UploadGCodeFile(file);

                if (result)
                {
                    return Ok(new { status = "success", message = "File uploaded successfully." });
                }
                else
                {
                    return BadRequest(new { status = "error", message = "File upload failed." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"File upload failed: {ex.Message}" });
            }
        }


        // deleteGcodeFile
        [HttpPost, AllowAnonymous]

        public async Task<ActionResult> DeleteGCodeFile(User inUser, string fileName)
        {
            // Find user in DB
           // var user = await _systemDB.DeleteGCodeFile(inUser, fileName);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Delete Gcodefile
            var result = await DeleteGCodeFile(inUser, fileName);

            if (true)
            {
                return Ok("File deleted successfully.");
            }
            else
            {
                return BadRequest("File not found.");
            }
        }*/

        //  print history // 





        //[HttpPost, AllowAnonymous]
        //public async Task<ActionResult> SendPrintingJob(string inUserName, string inFileName, string inPrinterName)
        //{
        //    bool response = await _systemDB.SendPrintingJob(inUserName, inFileName, inPrinterName);
        //    if (response)
        //    {
        //        _log.LogInformation("User: " + inUserName + " sent  a gcode file" +
        //            "\nwit the name: " + inFileName + "to for printing"); //create function to give absolute file path
        //        return Ok(new { message = "File is a gcode file" });
        //    }
        //    else
        //    {
        //        _log.LogInformation("User: " + inUserName + " failed to send" +
        //            "\nGcode file to printer, FileName: " + inFileName);
        //        return BadRequest(new { message = "Invalid file format" });
        //    }


        //}

        //[HttpPut, Authorize(Roles = "User")]
        ////[Authorize(Roles ="Admin")]
        //public async Task<ActionResult> ChangeUserInformation([FromBody] User user)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            bool response = await _systemDB.ChangeUserInformation(user);
        //            if (response)
        //            {
        //                _log.LogInformation($"User data was updated to : FirstName: {user.FirstName} and LastName: {user.LastName}.");
        //                return Ok(new { message = "User data was successfully updated."});
        //            }
        //            else
        //            {
        //                _log.LogInformation($"User couldn't be updated, Email: {user.Email}.");
        //                return BadRequest(new { message = "User couldn't be updated."});
        //            }
        //        }
        //        else
        //        {
        //            _log.LogInformation("Fail in the model input validation.");
        //            return BadRequest(new { message = "Problem with input validation."});
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogInformation(ex.Message);
        //        return BadRequest(new { message = "Unknown change user information error."});
        //    }

        //}


    }
}
            