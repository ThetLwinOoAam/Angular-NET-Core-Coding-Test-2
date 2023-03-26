namespace WebApi.Models.Messaging
{
    public class MailQueryRequestModel
    {
        public int ParamFolderId { get; set; }
        public int PageNumber { get; set; }
        public int RowsOfPage { get; set; }
        public int SendingUserID { get; set; }
        public string SendingStaffName { get; set; }
        public string SendingStaffMail { get; set; }
        public int Offset { get { return (PageNumber - 1) * RowsOfPage; } }

    }
}
