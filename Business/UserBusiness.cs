using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.GstMatching.Business.Interface;
using CAF.GstMatching.Helper;
using CAF.GstMatching.Models;
using CAF.GstMatching.Models.UserModel;

namespace CAF.GstMatching.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly UserHelper _userHelper;
        public UserBusiness(UserHelper userHelper)
        {
            _userHelper = userHelper;
        }
        public async Task markAsAdmin(string userEmail)
        {
            await _userHelper.markAsAdmin(userEmail);
        }
        public async Task saveUserValidUpto(UserValidUptoModel userValidUpto)
        {
            await _userHelper.saveUserValidUpto(userValidUpto);
        }
        public async Task<UserValidUptoModel> getUserValidUpto(string userEmail)
        {
            return await _userHelper.getUserValidUpto(userEmail);
        }
        public async Task UpdateUserValidUpto(string userEmail, int days)
        {
            await _userHelper.UpdateUserValidUpto(userEmail, days);
        }
        public async Task<List<MainUserModel>> GetAllUsers()
        {
            return await _userHelper.GetAllUsers();
        }
        public async Task MapClientToAdmin(string email, string flag, List<AdminClientModel> clients)
        {
            await _userHelper.MapClientToAdmin(email, flag, clients);
        }
        public async Task<List<AdminClientModel>> GetAllClients()
        {
            return await _userHelper.GetAllClients();
        }       

        public async Task UpdatePasswordAsync(string email, string newPassword)
        {
            await _userHelper.UpdatePasswordAsync(email, newPassword);
        }


        public async Task<MainUserModel> GetUserNameAsync(string email)
        {
            return await _userHelper.GetUserNameAsync(email);           
        }
        public async Task<MainUserModel> GetUserDetails(string gstin)
        {
            return await _userHelper.GetUserDetails(gstin);
        }
        public async Task<List<StateCodeModel>> StateCodeList()
        {
            return await _userHelper.StateCodeList();
           
        }

        public async Task<List<AdminMailModel>> GetAdminUsers()
        {
			return await _userHelper.GetAdminUsers();
		}
        public async Task<List<AdminMailModel>> GetMainAdminUsers()
        {
            return await _userHelper.GetMainAdminUsers();
        }
        public async Task<List<AdminClientModel>> GetAdminClients(string Email)
        {
            return await _userHelper.GetAdminClients(Email);
        }

        public async Task saveClientAPIData(ClientAPIDataModel data)
        {
            await _userHelper.saveClientAPIData(data);
        }
        public async Task<ClientAPIDataModel> GetClientAPIData(string ClientGstin)
        {
            return await _userHelper.GetClientAPIData(ClientGstin);
        }

        public async Task savePaymentDetails(PaymentDetailModel paymentDetails)
        {
            await _userHelper.savePaymentDetails(paymentDetails);
        }
        public async Task<PaymentDetailModel> GetPaymentDetails(string orderId, string clientEmail, string clientGstin)
        {
            return await _userHelper.GetPaymentDetails(orderId, clientEmail, clientGstin);
        }


    }
}
