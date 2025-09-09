using CAF.GstMatching.Data;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Helper
{
    public class UserHelper
    {
        private readonly ApplicationDbContext _context;
        public UserHelper(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task markAsAdmin(string userEmail)
        {
            await _context.Database.ExecuteSqlCommandAsync("UPDATE TBL_PRD_USERS SET Status = '1' WHERE Email = @p0", userEmail);

            //         var user = await _context.TBL_PRD_USERS
            //	.FirstOrDefaultAsync(p => p.Email == userEmail);
            //         if (user != null)
            //         {
            //	user.Status = "1" ; // Mark the user as Admin
            //	await _context.SaveChangesAsync();
            //}
        }
        public async Task saveUserValidUpto(UserValidUptoModel userValidUpto)
        {
            var existingUser = await _context.UserValidUptoes
                            .Where(u => u.UserEmail == userValidUpto.userEmail && u.UserGstin == userValidUpto.userGstin)
                            .OrderByDescending(u => u.Id)
                            .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                // Update existing record
                if(existingUser.AccessToDate == null )
                {
                    existingUser.AccessToDate = DateTime.Now ;
                }

                // Add new record
                var newUser = new UserValidUpto
                {
                    UserEmail = existingUser.UserEmail,
                    UserGstin = existingUser.UserGstin,
                    UserName = existingUser.UserName,
                    AccessFromDate = DateTime.Now,
                    AccessToDate = userValidUpto.accessToDate,
                };
                _context.UserValidUptoes.Add(newUser);
            }
            else
            {
                // Add new record
                var newUser = new UserValidUpto
                {
                    UserEmail = userValidUpto.userEmail,
                    UserGstin = userValidUpto.userGstin,
                    UserName = userValidUpto.userName,
                    AccessFromDate = DateTime.Now,
                    AccessToDate = null
                };
                _context.UserValidUptoes.Add(newUser);
            }
            await _context. SaveChangesAsync();
        }
        public async Task<UserValidUptoModel> getUserValidUpto(string userEmail)
        {
            var clientUser = await _context.UserValidUptoes
                    .Where(u => u.UserEmail == userEmail)
                    .OrderByDescending(u => u.Id)
                    .Select(p => new UserValidUptoModel
                    {
                        userEmail = p.UserEmail,
                        userGstin = p.UserGstin,
                        userName = p.UserName,
                        accessFromDate = p.AccessFromDate,
                        accessToDate = p.AccessToDate
                    })
                    .FirstOrDefaultAsync(); // Only the latest record

            return clientUser;
        }
        public async Task UpdateUserValidUpto(string userEmail, int days)
        {
            var existingUser = await _context.UserValidUptoes
                .Where(u => u.UserEmail == userEmail)
                .OrderByDescending(u => u.Id)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (!existingUser.AccessToDate.HasValue || existingUser.AccessToDate.Value < DateTime.Now)
                {
                    // If date is null or already expired, set from today and Add new record
                    var newUser = new UserValidUpto
                    {
                        UserEmail = existingUser.UserEmail,
                        UserGstin = existingUser.UserGstin,
                        UserName = existingUser.UserName,
                        AccessFromDate = DateTime.Now,
                        AccessToDate = DateTime.Now.AddDays(days)
                    };
                    _context.UserValidUptoes.Add(newUser);
                }
                else
                {
                    // If still valid (future date), extend from current expiry
                    existingUser.AccessToDate = existingUser.AccessToDate.Value.AddDays(days);
                }

                // Save changes
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<MainUserModel>> GetAllUsers()
        {
            var AllUsers = await _context.TBL_PRD_USERS
                 .Select(p => new MainUserModel
                 {
                     USERNAME = p.Entity_Name,
                     Designation = p.Designation,
                     Email = p.Email,
                     Status = p.Status,
                 }).ToListAsync();
            return AllUsers;
        }
        public async Task MapClientToAdmin(string email, string flag, List<AdminClientModel> clients)
        {
            if (flag == "Add")
            {
                foreach (var client in clients)
                {
                    var gstin = client.ClientGSTIN?.Trim();
                    var clientName = client.ClientName?.Trim();

                    // Check for existing active mapping
                    var exists = await _context.Admin_Clients
                        .FirstOrDefaultAsync(x => x.Email == email &&
                                                  x.Client_GSTIN == gstin &&
                                                  x.ToDate == null);

                    if (exists == null)
                    {
                        var newEntry = new Admin_Clients
                        {
                            Email = email,
                            Client_GSTIN = gstin,
                            ClientName = clientName,
                            FromDate = DateTime.Now,
                            ToDate = null
                        };

                        _context.Admin_Clients.Add(newEntry);
                    }
                }
            }
            if (flag == "Remove")
            {
                foreach (var client in clients)
                {
                    var gstin = client.ClientGSTIN?.Trim();

                    var existing = await _context.Admin_Clients
                        .FirstOrDefaultAsync(x => x.Email == email &&
                                                  x.Client_GSTIN == gstin &&
                                                  x.ToDate == null);

                    if (existing != null)
                    {
                        existing.ToDate = DateTime.Now;
                    }
                }

            }

            // Save all changes in one call
            await _context.SaveChangesAsync();
        }

        public async Task<List<AdminClientModel>> GetAllClients()
        {
           var ClientUsers =   await _context.TBL_PRD_USERS
                .Where(p => p.Status == null) 
                .Select(p => new AdminClientModel
                {
                    ClientName = p.Entity_Name,
                    ClientGSTIN = p.Designation,
                }).ToListAsync();
            return ClientUsers ;
        }

        public async Task UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _context.TBL_PRD_USERS
                .FirstOrDefaultAsync(p => p.Email == email);

            if (user != null)
            {
                user.PASSWORD = newPassword;  // Update the password field
                user.LastPasswordSet = DateTime.Now;  // Update the LastPasswordSet timestamp

                await _context.SaveChangesAsync();
                Console.WriteLine($"Password for {email} updated successfully.");
            }
            else
            {
                Console.WriteLine($"No user found with email: {email}");
            }
        }     
        public async Task<MainUserModel> GetUserNameAsync(string email)
        {
            return await _context.TBL_PRD_USERS
                .Where(p => p.Email == email)
                .Select(p => new MainUserModel
                {
                    EXERTIDCOL = p.EXERT_ID_COL,
                    EXERTSTATUS = p.EXERT_STATUS,
                    USERCODE = p.USER_CODE,
                    USERNAME = p.Entity_Name,
                    PASSWORD = p.PASSWORD,
                    ROLECODE = p.ROLE_CODE,
                    ROLENAME = p.ROLE_NAME,
                    LANGUAGE = p.LANGUAGE,
                    ConfirmPassword = p.Confirm_Password,
                    EXERTENTRYDATE = p.EXERT_ENTRY_DATE,
                    Email = p.Email,
                    Designation = p.Designation,
                    Address = p.Address,
                    LastPasswordSet = p.LastPasswordSet,
                    PasswordChanged = p.Password_Changed,





                    EXERTENTITYCODE = p.EXERT_ENTITY_CODE,
                    EXERTENTITYNAME = p.EXERT_ENTITY_NAME,
                    EXERTUSERCODE = p.EXERT_USER_CODE,
                    EXERTUSERNAME = p.EXERT_USER_NAME,
                    EXERTROLLCODE = p.EXERT_ROLL_CODE,
                    EXERTROLLNAME = p.EXERT_ROLL_NAME,                 
                    Defaultpage = p.Default_page,
                    PageLayoutType = p.Page_Layout_Type,                  
                    EntityCode = p.Entity_Code,
                    EntityName = p.Entity_Name,
                    LowerLimit = p.Lower_Limit,
                    UpperLimit = p.Upper_Limit,
                    Currency = p.Currency,
                    Type_ = p.Type_,                 
                    Department = p.Department,
                    LoginAttempt = p.LoginAttempt,
                    RowsPerPage = p.RowsPerPage,
                    LkUpRowsPerPage = p.LkUpRowsPerPage,
                    FullName = p.Full_Name,
                    MobilePIN = p.Mobile_PIN,
                    DepartmentHOD = p.Department_HOD,                 
                    ExpiryTimeSpan = p.ExpiryTimeSpan,
                    ForgotPasswordToken = p.ForgotPasswordToken,
                    FailureCount = p.Failure_Count,
                    LastLockedDate = p.Last_Locked_Date,                  
                    FailureCount1 = p.Failure_Count1,
                    LastLockedDate1 = p.Last_Locked_Date1,
                    Status = p.Status,
                    REFEXERTIDCOL = p.REF_EXERT_ID_COL,
                    Themes = p.Themes,

                })
                 .FirstOrDefaultAsync();
        }

        public async Task<MainUserModel> GetUserDetails(string gstin)
        {
            return await _context.TBL_PRD_USERS
                .Where(p => p.Designation == gstin && p.Status == "1" )
                .Select(p => new MainUserModel
                {
                    USERNAME = p.Entity_Name,
                    Email = p.Email,
                    Designation = p.Designation,


                    PASSWORD = p.PASSWORD,
                    EXERTIDCOL = p.EXERT_ID_COL,
                    EXERTSTATUS = p.EXERT_STATUS,
                    USERCODE = p.USER_CODE,
                    ROLECODE = p.ROLE_CODE,
                    ROLENAME = p.ROLE_NAME,
                    LANGUAGE = p.LANGUAGE,
                    ConfirmPassword = p.Confirm_Password,
                    EXERTENTRYDATE = p.EXERT_ENTRY_DATE,
                    Address = p.Address,
                    LastPasswordSet = p.LastPasswordSet,
                    PasswordChanged = p.Password_Changed,
                }).FirstOrDefaultAsync();
        }
        public async Task<List<StateCodeModel>> StateCodeList()
        {
            var stateCodeList = await _context.GST_State_Codes
                .Select(p => new StateCodeModel
                {
                    StateName = p.Name,
                    StateCode = p.Code,
                })
                .ToListAsync();

            return stateCodeList;
        }
        public async Task<List<AdminMailModel>> GetAdminUsers()
        {
            var adminUsers = await _context.TBL_PRD_USERS
                .Where(p => p.Status == "1")
                .Select(p => new AdminMailModel
                {
					Email = p.Email,
				})
				.ToListAsync();
            return adminUsers;
        }
        public async Task<List<AdminMailModel>> GetMainAdminUsers()
        {
            var adminUsers = await _context.TBL_PRD_USERS
                .Where(p => p.Status == "Admin")
                .Select(p => new AdminMailModel
                {
                    Email = p.Email,
                })
                .ToListAsync();
            return adminUsers;
        }
        public async Task<List<AdminClientModel>> GetAdminClients(string Email)
        {
			var adminClients = await _context.Admin_Clients
                .Where(p => p.Email == Email && p.ToDate == null)
				.Select(p => new AdminClientModel
                {
                    ClientName = p.ClientName,
                    ClientGSTIN = p.Client_GSTIN,
				})
				.ToListAsync();

			return adminClients;
		}

        public async Task saveClientAPIData(ClientAPIDataModel data)
        {
            var Data = new Client_Api_datum
            {
                Client_Gstin = data.ClientGstin,
                API_IRIS_Username = data.APIIRISUsername,
                API_IRIS_Password = data.APIIRISPassword,
                Gst_Portal_Username = data.GstPortalUsername,
            };
            _context.Client_Api_Data.Add(Data);
            await _context.SaveChangesAsync();
        }
        public async Task<ClientAPIDataModel> GetClientAPIData(string ClientGstin)
        {
            var clientAPIData = await _context.Client_Api_Data
                .Where(x => x.Client_Gstin == ClientGstin)
                .FirstOrDefaultAsync();

            if (clientAPIData != null)
            {
                return new ClientAPIDataModel
                {
                    ClientGstin = clientAPIData.Client_Gstin,
                    APIIRISUsername = clientAPIData.API_IRIS_Username,
                    APIIRISPassword = clientAPIData.API_IRIS_Password,
                    GstPortalUsername = clientAPIData.Gst_Portal_Username
                };
            }
            return null;
        }

        public async Task savePaymentDetails(PaymentDetailModel paymentDetails)
        {
            var details = await _context.PaymentDetails
              .FirstOrDefaultAsync(p => p.OrderId == paymentDetails.OrderId
                                     && p.ClientEmail == paymentDetails.ClientEmail
                                     && p.ClientGstin == paymentDetails.ClientGstin
                                     );
            if (details != null)
            {
                details.PaymentId = paymentDetails.PaymentId;
                details.PaymentStatus = paymentDetails.PaymentStatus;
                details.PaymentDate = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            else
            {
                var Details = new PaymentDetail
                {
                    ClientName = paymentDetails.ClientName,
                    ClientGstin = paymentDetails.ClientGstin,
                    ClientEmail = paymentDetails.ClientEmail,
                    ClientPhone = paymentDetails.ClientPhone,
                    ClientAddress = paymentDetails.ClientAddress,
                    Amount = paymentDetails.Amount,
                    Days = paymentDetails.Days,
                    OrderId = paymentDetails.OrderId,
                    PaymentId = paymentDetails.PaymentId,
                    PaymentStatus = paymentDetails.PaymentStatus,
                    PaymentDate = DateTime.Now,
                    TransactionId = paymentDetails.TransactionId
                };
                _context.PaymentDetails.Add(Details);
            }


            await _context.SaveChangesAsync();
        }
        public async Task<PaymentDetailModel> GetPaymentDetails(string orderId, string clientEmail, string clientGstin)
        {
            var paymentDetails = await _context.PaymentDetails
                .Where(p => p.OrderId == orderId && p.ClientEmail == clientEmail && p.ClientGstin == clientGstin)
                .Select(p => new PaymentDetailModel
                {
                    Id = p.Id,
                    ClientName = p.ClientName,
                    ClientGstin = p.ClientGstin,
                    ClientEmail = p.ClientEmail,
                    ClientPhone = p.ClientPhone,
                    ClientAddress = p.ClientAddress,
                    Amount = p.Amount,
                    Days = p.Days,
                    OrderId = p.OrderId,
                    PaymentId = p.PaymentId,
                    PaymentStatus = p.PaymentStatus,
                    PaymentDate = p.PaymentDate,
                    TransactionId = p.TransactionId
                })
                .FirstOrDefaultAsync();

            return paymentDetails;
        }

    }
}
