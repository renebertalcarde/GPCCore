using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Module.DB
{
    public class objAccountability
    {
        public void WriteLog(DateTime LogDate, string UserId, string Module, string Page, string Url, string Action, string Details, object Data)
        {
            using(accountabilityDataContext context = new accountabilityDataContext())
            {
                tblAccountability item = new tblAccountability
                {
                    LogDate = LogDate,
                    UserId = UserId,
                    Module = Module,
                    Page = Page,
                    Url = Url,
                    Action = Action,
                    Details = Details
                };

                if (Data != null)
                {
                    item.Data = JsonConvert.SerializeObject(Data);
                }

                context.tblAccountabilities.InsertOnSubmit(item);
                context.SubmitChanges();
            }
        }
    }
}