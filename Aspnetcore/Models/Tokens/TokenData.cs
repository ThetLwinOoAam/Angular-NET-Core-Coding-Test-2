namespace WebApi.Models.Tokens
{
    public class TokenData
    {
        public string Name { get; set; }
        public string AMSSessionID { get; set; }
        public string StaffName { get; set; }
        public string UserID { get; set; }
        public string StaffEmail { get; set; }
        public string CompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeNumber { get; set; }
        public string UserRolesID { get; set; }
        public string UserRightsID { get; set; }
        public string AccessToken { get; set; }
    }
}
