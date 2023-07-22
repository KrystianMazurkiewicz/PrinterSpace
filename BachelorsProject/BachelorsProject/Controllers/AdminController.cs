using BachelorsProject.DAL;
using BachelorsProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QandA = BachelorsProject.DAL.QandA;


namespace BachelorsProject.Controllers
{
    
    [Route("[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        //private readonly DBInit _systemDB;
        private ILogger<AdminController> _log;
        private ISystemRepository @object;
        private readonly ISystemRepository _db;


        //constructor
        public AdminController( ILogger<AdminController> log, ISystemRepository db) // removed DBInit systemDB here,
        {
            //_systemDB = systemDB;
            _log = log;
            _db = db;
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateAdmin([FromBody] Admin inAdmin)
        {
            try{
                if(ModelState.IsValid)
                {
                    bool createUser = await _db.CreateAdmin(inAdmin);

                    if (createUser)
                    {
                        return Ok(new { message = "Admin was successfully created." });
                    }
                    else
                    {
                        return BadRequest(new { message = "Admin could not be created." });
                    }
                }
                else return BadRequest(new { message = "Problem with input validation."});
                
            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown create admin error." });
                }
        }

        [HttpDelete, Authorize(Roles = "Admin")]

        public async Task<ActionResult> DeleteUser(string userName)
        {
            try { 
                var deleteUser = await _db.DeleteUser(userName);    
                if (deleteUser)
                {
                    return Ok(new { message = "User successfully deleted." });
                }
                else
                {
                    return BadRequest(new { message = "Could not delete the user." });
                }  
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown delete user error." });
                }
        }
        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAdmin(string inAdminName)
        {
            try
            {
                var deleteUser = await _db.DeleteAdmin(inAdminName);
                if (deleteUser)
                {
                    return Ok(new { message = "Admin was successfully deleted." });
                }
                else
                {
                    return BadRequest(new { message = "Could not delete the admin." });
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown delete admin error."});
                }
        }

        // Change user info (ADMIN)  

        //[HttpPut, Authorize(Roles = "Admin")] // can also use POST but a PUT request would be appropriate as it involves updating an existing user record with new information.
        //public async Task<ActionResult> ChangeUserInformation([FromBody] User updatedUser)
        //{
        //    try
        //    {
        //        bool isUpdated = await _db.ChangeUserInformation(updatedUser);

        //        if (isUpdated)
        //        {
        //            return Ok(new { message = "User information updated successfully." });
        //        }
        //        else
        //        {
        //            return BadRequest(new { message = "Could not update user." });
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        _log.LogInformation(ex.Message);
        //        return BadRequest(new { message = "Unknown user update information error." });
        //    }
        //}

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> ReturnPrintsPerPrinter(string inPrinterName)
        {
            try
            {
                var printsPerPrinter = await _db.ReturnPrintsPerPrinter(inPrinterName);

                if (printsPerPrinter == null || printsPerPrinter.Count == 0)
                {
                    return NotFound(new { message = "Could not find prints." });
                }
                return Ok(printsPerPrinter);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown return prints per printer error." });
            }
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreatePrinter([FromBody] Printer CreatePrinter)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    bool isCreated = await _db.CreatePrinter(CreatePrinter);

                    if (isCreated)
                    {
                        return Ok(new { message = "Printer has been successfully created." });
                    }
                    else
                    {
                        return BadRequest(new { message = "Printer could not be created." });
                    }
                }
                else return BadRequest(new { message = "Problem with input validation." });
               
            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown create printer error." });
            }
        }

        [HttpDelete, Authorize(Roles = "Admin")] 
        public async Task<ActionResult> DeletePrinter(string inPrinterName)
        {
            try
            {
                bool isDeleted = await _db.DeletePrinter(inPrinterName);

                if (isDeleted)
                {
                    return Ok(new { message = "Printer has been successfully deleted." });
                }
                else
                {
                    return BadRequest(new { message = "Could not delete printer." });
                }
            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown delete printer error." });
            }
           
        }

        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdatePrinter([FromBody] Printer inPrinters)
        {
            try
            {
                bool isUpdated = await _db.UpdatePrinter(inPrinters);

                if (isUpdated)
                {
                    return Ok(new { message = "Printer has been successfully updated."});
                }
                else
                {
                    return BadRequest(new { message = "Could not update printer." });
                }
            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown update printer error." });
            }
            
            
        }

        //[HttpGet, AllowAnonymous]        
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllPrinters()
        {
            try
            {
                var printsPerPrinter = await _db.GetAllPrinters();

                if (printsPerPrinter == null || printsPerPrinter.Count == 0)
                {
                    return NotFound(new { message = "Could not get all printers." });
                }
                return Ok(printsPerPrinter);
            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown get all printers error." });
            }
        }
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var getAllUsers = await _db.GetAllUsers();

                if (getAllUsers == null || getAllUsers.Count == 0)
                {
                    return NotFound(new { message = "Could not get all users." });
                }
                return Ok(getAllUsers);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown get all users error." });
            }
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllAdmins()
        {
            try
            {
                var getAllAdmins = await _db.GetAllAdmins();

                if (getAllAdmins == null || getAllAdmins.Count == 0)
                {
                    return NotFound(new { message = "Could not get all admins." });
                }
                return Ok(getAllAdmins);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown get all admin error." });
            }
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> UnlockAndLockPrinter(string PrinterName, string inCommand)
        {
            try
            {
                var unlockAndLockPrinter = await _db.UnlockAndLockPrinter(PrinterName, inCommand);

                if (unlockAndLockPrinter == false)
                {
                    return BadRequest(new { message = "Could not unlock printer." });
                }
                return Ok($"Printer was {inCommand}ed.");
            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown unlock and lock printer error." });
            }
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> PrintingHistoryForUser(string inUserName)
        {
            try
            {
                var listPrintingHistory = await _db.PrintingHistoryForUser(inUserName);

                if (listPrintingHistory == null)
                {
                    return NotFound(new { message = "Could not find user or prints." });
                }
                return Ok(listPrintingHistory);
            } catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown printing history per user error." });
            }
            
        }

        //[HttpGet, Authorize(Roles = "Admin")]
        //public async Task<ActionResult> ReturnPlasticUsedForPrinter(string printerName, string inCommand)
        //{
        //    try
        //    {
        //        double returnPlasticUsed = await _db.ReturnPlasticUsedForPrinter(printerName, inCommand);

        //        if (returnPlasticUsed == 0)
        //        {
        //            return NotFound(new { message = "Could not find any entry for printer" });
        //        }
        //        return Ok(returnPlasticUsed);
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogInformation(ex.Message);
        //        return BadRequest(new { message = "Unknown return plastic used for printer error." });
        //    }
        //}

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetPrintingQueue()
        {
            try
            {
                var resultGetPrintingQueue = await _db.GetPrintingQueue();

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

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelOngoingPrint(string inUserName, string inFileName)
        {
            try
            {
                var cancelPrint = await _db.CancelOngoingPrint(inUserName, inFileName);

                if (!cancelPrint)
                {
                    return BadRequest(new { message = "Could not cancel print." });
                }
                else return Ok(new { message = "Print was cancelled."});
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown cancel print error." });
            }
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelQueuePrint(string inFileName, string inUserName)
        {
            try
            {
                var responseRemovePrintFromQueue = await _db.CancelQueuePrint(inFileName, inUserName);

                if (!responseRemovePrintFromQueue)
                {
                    return BadRequest(new { message = "Could no remove print from Queue." });
                }
                return Ok(responseRemovePrintFromQueue);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown remove print from queue error." });
            }
        }

        [HttpGet, Authorize(Roles = "Admin")]
        //[HttpGet, AllowAnonymous]
        public async Task<ActionResult> StreamFromPrinter(string inPrinterName)
        {
            try
            {
                var printerStream = await _db.StreamFromPrinter(inPrinterName);

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

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> MakeNewQAndA([FromBody] QandA inQandA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _db.MakeNewQAndA(inQandA);

                    if (response != true)
                    {
                        return BadRequest(new { message = "Could not make a new question and answer." });
                    }
                    return Ok(new { message = "New  question and answer has been added." });
                }
                else return BadRequest(new { message = "Problem with input validation." });
            }

            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown make a new Q&A error." });
            }
        }

        [HttpGet, Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> GetQAndA()
        {
            try
            {
                var response = await _db.GetQAndA();

                if (response == null || response.Count == 0)
                {
                    return NotFound(new { message = "Could not return all questions and answers." });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown get Q&A error."});
            }
        }

        [HttpPatch, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditQAndA([FromBody] QandA inQandA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _db.EditQAndA(inQandA);

                    if (response != true)
                    {
                        return BadRequest(new { message = "Could not update questions and answers." });
                    }
                    return Ok(response);
                }
                else return BadRequest(new { message = "Problem with input validation." });
                
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown edit Q&A error." });
            }
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteQAndA(int id)
        {
            try
            {
                var response = await _db.DeleteQAndA(id);

                if (response != true)
                {
                    return BadRequest(new { message = "Could not remove questions and answers." });
                }
                return Ok(new { message = "Questions and answers were removed." });
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return BadRequest(new { message = "Unknown delete Q&A error." });
            }
        }

        //RETURN ALL ADMINS
    }
}
