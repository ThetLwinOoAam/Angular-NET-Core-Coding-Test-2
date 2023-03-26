using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;

using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net.Mail;
using CsvHelper;
using System.Text;
using System.Globalization;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using WebApi.Models.Messaging;
using WebApi.Repository;

namespace WebApi.Services
{
   
    public interface INewMailService
    {
        Task<MailsPageObj> GetSentFolderAsync(MailQueryRequestModel mailQueryRequest);
    }

    public class NewMailService : INewMailService, IDisposable
    {
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private readonly AppSettings _appSettings;
        private readonly IMailRepository mailRepository;
        private ILogger _log;

        public NewMailService(IMailRepository mailRepository, IOptions<AppSettings> appSettings, ILogger<MailService> log)
        {
            this.mailRepository = mailRepository;
            _appSettings = appSettings.Value;
            _log = log;
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
            }

            disposed = true;
        }

        public async Task<MailsPageObj> GetSentFolderAsync(MailQueryRequestModel mailQueryRequest)
        { 
            return await mailRepository.GetSentFolderAsync(mailQueryRequest);

        }
        

    }
}