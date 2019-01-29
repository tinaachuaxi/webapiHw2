using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using webapi_hw1.DBHelper;

namespace webapi_hw1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientAccountController : Controller
    {
        private readonly AccountDbContext _context;

        public ClientAccountController(AccountDbContext context)
        {
            _context = context;
        }

        //ReadAll
        //atomatically recognized as https://localhost:<#port>/api/clientaccount
        [HttpGet]
        public IEnumerable<ClientAccount> GetAll()
        {
            return _context.ClientAccounts.ToList();
        }

        //ReadOne
        //atomatically recognized as https://localhost:<#port>/api/clientaccount/AccountID/{accountID}/ClientID/{clientID}
        //For Example: https://localhost:<#port>/api/clientaccount/AccountID/1/ClientID/2
        [HttpGet("AccountID/{accountID}/ClientID/{clientID}", Name = "GetClientAccount")]
        public IActionResult getByID(long accountID, long clientID)
        {
            var item = _context.ClientAccounts.Where(ca => ca.AccountID == accountID &&
                              ca.ClientID == clientID).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);

        }

        //Create
        [HttpPost]
        public IActionResult Create([FromBody]ClientAccount clientAccount)
        {
            if (_context.ClientProiles.Any(c=>c.ID == clientAccount.ClientID)
                && _context.AccountTypes.Any(a => a.ID == clientAccount.AccountID))
            {
                if(_context.ClientAccounts.Any(ca=>ca.AccountID==clientAccount.AccountID
                    && ca.ClientID == clientAccount.ClientID))
                {
                    return BadRequest();
                }
                _context.ClientAccounts.Add(clientAccount);
                _context.SaveChanges();
                return new ObjectResult(clientAccount);
            }
            return BadRequest();
        }
        //Update
        [HttpPut]
        [Route("MyEdit")]
        public IActionResult Update([FromBody]ClientAccount clientAccount)
        {
            var item = _context.ClientAccounts.Where(ca => ca.AccountID == clientAccount.AccountID &&
                                ca.ClientID == clientAccount.ClientID).FirstOrDefault();
            if (item==null)
            {
                return NotFound() ;
            }
            else
            {
                item.Balance = clientAccount.Balance;
                _context.SaveChanges();
                return new ObjectResult(item);
            }
            }
        //Delete
        [HttpDelete]
        [Route("MyDelete")]
        public IActionResult Delete(long accountID, long clientID)
        {
            var item = _context.ClientAccounts.Where(ca => ca.AccountID == accountID &&
                               ca.ClientID == clientID).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }
            _context.ClientAccounts.Remove(item);
            _context.SaveChanges();
            return new ObjectResult(item);
        }

        //Get Custom Obj
        //https://localhost:<#port>/api/clientaccount/CustomObject/AccountID/{accountID}/ClientID/{clientID}
        //example: https://localhost:<#port>/api/clientaccount/CustomObject/AccountID/1/ClientID/2
        [HttpPost("CustomObject/AccountID/{accountID}/ClientID/{clientID}")]
        public JsonResult GetCustomObject(long accountID, long clientID)
        {
            var item = _context.ClientAccounts.Where(ca => ca.AccountID == accountID &&
                                  ca.ClientID == clientID).FirstOrDefault();
            dynamic responseObj = new JObject();
            if (item != null)
            {
                //for some reasons, navigation properties are always null
                responseObj.FName = _context.ClientProiles.Where(cp => cp.ID == clientID).FirstOrDefault().FirstName;
                responseObj.LName = _context.ClientProiles.Where(cp => cp.ID == clientID).FirstOrDefault().LastName;
                responseObj.Description = _context.AccountTypes.Where(at => at.ID == accountID).FirstOrDefault().AccountDescription;
                responseObj.Balance = item.Balance;
            }
            else
            {
                responseObj.FName = null;
                responseObj.LName = null;
                responseObj.Description = null;
                responseObj.Balance = null;
            }
            return Json(responseObj);
        }
    }
}