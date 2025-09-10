using CAF.GstMatching.Models;
using CAF.GstMatching.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.GstMatching.Business.Interface
{
    public interface IUserBusiness
    {
        Task markAsAdmin(string userEmail);
        Task saveUserValidUpto(UserValidUptoModel userValidUpto);
        Task<UserValidUptoModel> getUserValidUpto(string userEmail);
        Task UpdateUserValidUpto(string userEmail, int days);
        Task<List<MainUserModel>> GetAllUsers();
        Task MapClientToAdmin(string email, string flag, List<AdminClientModel> clients);
        Task<List<AdminClientModel>> GetAllClients();

        Task UpdatePasswordAsync(string clientGstin, string password); // update status in request
        Task<MainUserModel> GetUserNameAsync(string email);
        Task<MainUserModel> GetUserDetails(string gstin);
        Task<List<StateCodeModel>> StateCodeList();

		Task<List<AdminMailModel>> GetAdminUsers();
		Task<List<AdminMailModel>> GetMainAdminUsers();
		Task<List<AdminClientModel>> GetAdminClients(string Email);

        Task saveClientAPIData(ClientAPIDataModel data);
        Task<ClientAPIDataModel> GetClientAPIData(string ClientGstin);

        Task savePaymentDetails(PaymentDetailModel paymentDetails);
        Task<PaymentDetailModel> GetPaymentDetails(string orderId, string clientEmail, string clientGstin);

        Task saveMenuItemsDetails(MenuItemsModel MenuItemsDetails);
        Task<MenuItemsModel> getMenuItemsDetails(string userEmail);

        Task UpdatePasswordToYes(string userEmail);

        Task SaveUserLoginHistory(UserLoginHistoryModel loginHistory);
        Task UpdateUserLogoutTime(string email, string gstin);

    }
}
