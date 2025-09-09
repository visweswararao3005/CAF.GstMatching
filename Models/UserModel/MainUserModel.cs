using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Models.UserModel
{
    public class MainUserModel
    {
        public string EXERTIDCOL { get; set; }
        public string REFEXERTIDCOL { get; set; }
        public Nullable<bool> EXERTSTATUS { get; set; }
        public string USERCODE { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string ROLECODE { get; set; }
        public string ROLENAME { get; set; }
        public string LANGUAGE { get; set; }
        public string Themes { get; set; }
        public string EXERTENTITYCODE { get; set; }
        public string EXERTENTITYNAME { get; set; }
        public string EXERTUSERCODE { get; set; }
        public string EXERTUSERNAME { get; set; }
        public string EXERTROLLCODE { get; set; }
        public string EXERTROLLNAME { get; set; }
        public string ConfirmPassword { get; set; }
        public string Defaultpage { get; set; }
        public string PageLayoutType { get; set; }
        public Nullable<System.DateTime> EXERTENTRYDATE { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; } // Client Gstin
        public Nullable<decimal> LowerLimit { get; set; }
        public Nullable<decimal> UpperLimit { get; set; }
        public string Currency { get; set; }
        public string Type_ { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
        public string LoginAttempt { get; set; }
        public Nullable<decimal> RowsPerPage { get; set; }
        public Nullable<decimal> LkUpRowsPerPage { get; set; }
        public string FullName { get; set; }
        public string MobilePIN { get; set; }
        public string DepartmentHOD { get; set; }
        public Nullable<System.DateTime> LastPasswordSet { get; set; }
        public string ExpiryTimeSpan { get; set; }
        public string ForgotPasswordToken { get; set; }
        public Nullable<decimal> FailureCount { get; set; }
        public Nullable<System.DateTime> LastLockedDate { get; set; }
        public string PasswordChanged { get; set; }
        public Nullable<decimal> FailureCount1 { get; set; }
        public Nullable<System.DateTime> LastLockedDate1 { get; set; }
        public string Status { get; set; }
    }
}
