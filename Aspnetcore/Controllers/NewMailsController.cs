using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Entities;
using WebApi.Models.Users;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Autofac.Util;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.IO;
using WebApi.Models.Messaging;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NewMailsController : AppBaseController, IDisposable
    {
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        readonly Disposable _disposable;
        private INewMailService _mailService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private ILogger _log;
        public NewMailsController(
            INewMailService mailService,
            IMapper mapper,
            ILogger<MailsController> log,
            IOptions<AppSettings> appSettings,
            DataContext context)
        {
            _mailService = mailService;
            _mapper = mapper;
            _log = log;
            _appSettings = appSettings.Value;
            _disposable = new Disposable();
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
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }

        [HttpGet("folderData/{paramFolderId}/{pageNumber}/{rowsOfPage}")]
        public async Task<IActionResult> GetSentFolderAsync(int paramFolderId, int pageNumber, int rowsOfPage)
        {
            if (paramFolderId < 0 || paramFolderId > 1)
            {
                return BadRequest(new { message = "Invalid Folder" });
            }

            if (pageNumber < 1)
            {
                return BadRequest(new { message = "Invalid Page Number" });
            }

            if (rowsOfPage < 1)
            {
                return BadRequest(new { message = "Invalid Page Size" });
            }

            MailQueryRequestModel mailQueryRequest = new MailQueryRequestModel {
                SendingUserID = Convert.ToInt32(TokenData.UserID),
                SendingStaffMail = TokenData.StaffEmail,
                SendingStaffName = TokenData.StaffEmail,
                PageNumber = pageNumber,
                ParamFolderId = paramFolderId,
                RowsOfPage = rowsOfPage
            };
            var mailsPageObj = await _mailService.GetSentFolderAsync(mailQueryRequest);
            return Ok(mailsPageObj);
        }
    }
}
