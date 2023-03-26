using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Messaging;
using WebApi.Services;
using System.Linq;

namespace WebApi.Repository
{
    public interface IMailRepository
    {
        Task<MailsPageObj> GetSentFolderAsync(MailQueryRequestModel mailQueryRequest);
    }
    public class MailRepository : IMailRepository
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        private ILogger _log;

        public MailRepository(DataContext context, IOptions<AppSettings> appSettings, ILogger<MailService> log)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _log = log;
        }
        public async Task<MailsPageObj> GetSentFolderAsync(MailQueryRequestModel mailQueryRequest)
        {
            var query = from m in _context.Mail 
                        join ru in _context.Users on m.ReceivingUserID equals ru.UserID
                        where m.SendingUserID == mailQueryRequest.SendingUserID

                        select new MailModel { 
                            Id = m.Id,
                            SendingStaffName = mailQueryRequest.SendingStaffName,
                            SendingStaffEmail = mailQueryRequest.SendingStaffMail,
                            ReceivingStaffName = ru.StaffName,
                            ReceivingStaffEmail = ru.StaffEmail,
                            Subject = m.Subject,
                            Message = m.Message,
                            SentTime = m.SentTime,
                            SentSuccessToSMTPServer = m.SentSuccessToSMTPServer,
                            Read = m.Read,
                            Starred = m.Starred,
                            Important = m.Important,
                            HasAttachments = m.HasAttachments,
                            Label = m.Label,
                            Folder = m.Folder
                        };
            var mails = await query.OrderByDescending(x => x.SentTime).Skip(mailQueryRequest.Offset).Take(mailQueryRequest.RowsOfPage).ToListAsync();
            var totalRows = _context.Mail.Where(x => x.Folder == mailQueryRequest.ParamFolderId && x.SendingUserID == mailQueryRequest.SendingUserID).Count();

            MailsPageObj mailsPageObj = new MailsPageObj { totalRows = totalRows, rowsOfPage = mailQueryRequest.RowsOfPage, pageNumber = mailQueryRequest.PageNumber, results = mails };
            return mailsPageObj;

        }
    }
}
