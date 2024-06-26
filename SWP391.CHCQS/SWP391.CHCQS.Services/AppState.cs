﻿
using Microsoft.AspNetCore.Identity;
using SWP391.CHCQS.DataAccess.Repository.IRepository;
using SWP391.CHCQS.Utility;

namespace SWP391.CHCQS.Services
{
    public  class AppState
    {
                
        //private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private static AppState _instance;
        public int SLIndex { get; set; }
        public int ENIndex { get; set; }
        public int MGIndex { get; set; }

        public int SLMax { get; set; }
        public int ENMax { get; set; }
        public int MGMax { get; set; }

        private readonly Random _random = new ();

        private AppState(UserManager<IdentityUser> userManager) {
            _userManager = userManager;
            
            SLMax =  userManager.GetUsersInRoleAsync(SD.Role_Seller).GetAwaiter().GetResult().Where(x => x.LockoutEnd == null || x.LockoutEnd <= DateTimeOffset.Now).Count();
            ENMax = userManager.GetUsersInRoleAsync(SD.Role_Engineer).GetAwaiter().GetResult().Where(x => x.LockoutEnd == null || x.LockoutEnd <= DateTimeOffset.Now).Count();
            MGMax = userManager.GetUsersInRoleAsync(SD.Role_Manager).GetAwaiter().GetResult().Where(x => x.LockoutEnd == null || x.LockoutEnd <= DateTimeOffset.Now).Count();

            SLIndex =  _random.Next(1, SLMax);
            ENIndex = _random.Next(1, ENMax);
            MGIndex = _random.Next(1, MGMax);

        }

        private static readonly object _lock = new object();
        public static AppState Instance(UserManager<IdentityUser> userManager)
        {
            //nếu _instance chưa dc tạo thì tạo ra - Singleton
            if(_instance == null)
            {
                //chặn thread khác đi vào - only one - thằng còn lại đợi
                lock(_lock)
                {
                    //check phát nữa cho chắc ko nó ko có là toang
                    if(_instance == null)
                    {
                        _instance = new AppState(userManager);
                    }
                }
                _instance = new AppState(userManager);
            }//nếu có rồi thì tiến hành kiểm tra số lượng nhân viên còn trung khớp hay ko
            else
            {
                lock (_lock)
                {
                    var SLCount = userManager.GetUsersInRoleAsync(SD.Role_Seller).GetAwaiter().GetResult().Where(x => x.LockoutEnd == null || x.LockoutEnd <= DateTimeOffset.Now).Count();
                    var ENCount = userManager.GetUsersInRoleAsync(SD.Role_Engineer).GetAwaiter().GetResult().Where(x => x.LockoutEnd == null || x.LockoutEnd <= DateTimeOffset.Now).Count();
                    var MGCount = userManager.GetUsersInRoleAsync(SD.Role_Manager).GetAwaiter().GetResult().Where(x => x.LockoutEnd == null || x.LockoutEnd <= DateTimeOffset.Now).Count();

                    if (_instance.SLMax != SLCount)
                    {
                        _instance.SLMax = SLCount;
                    }
                    if (_instance.ENMax != ENCount)
                    {
                        _instance.ENMax = ENCount;
                    }
                    if (_instance.MGMax != MGCount)
                    {
                        _instance.MGMax = MGCount;
                    }
                }

            }
            return _instance;
        }
        //lần lượt sl, en, mg
        public Tuple<int, int, int> GetDelegationIndex()
        {
            //gán giá trị trả về cho Tuple
            var sl = SLIndex;
            var en = ENIndex;
            var mg = MGIndex;

            //thực hiện update lại index
            if (SLIndex == SLMax)
                SLIndex = 1;
            else SLIndex++;

            if (ENIndex == ENMax)
                ENIndex = 1;
            else ENIndex++;

            if (MGIndex == MGMax)
                MGIndex = 1;
            else MGIndex++;

            return Tuple.Create(sl -1 , en -1 , mg - 1);
        }

        //private Tuple<string, string, string> GetStaffIdDelegation()
        //{

        //    return Tuple.Create(null, null, null);
        //}

		//Cách gọi ra AppState: 
		//var delegationService = AppState.Instance(_userManager).GetDelegationIndex()
		//demo.Item1
		//demo.Item2
		//demo.Item3

		//để lấy StaffId cho seller  thì query:  _unitOfWork.Staff.Where((x) = >x.Id.Contains(SD.SelerIdKey)).SkipWhile((entity, index) => index < demo.Item1 - 1).FirstOrdefault().Id;
	}
}
