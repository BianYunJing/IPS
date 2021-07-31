using IPS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.ObjectRepository;
using Common.AESClass;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Commom;

namespace IPS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleManagementController : ControllerBase
    {

        readonly RoleRepository roleRepository = new RoleRepository();
        readonly RoleModifyLogRepository roleModifyLogRepository = new RoleModifyLogRepository();
        private readonly ILogger<RoleManagementController> _logger;
        AESClass aes = new AESClass();
        public RoleManagementController(ILogger<RoleManagementController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 用户查询接口-所有用户数据
        /// </summary>
        [Authorize]
        [HttpGet("GetRoleList")]
        public async Task<List<T_Role>> GetRoleList()
        {
            return await roleRepository.Query();
        }

        /// <summary>
        /// 根据员工号或者姓名查询接口
        /// </summary>
        [Authorize]
        [HttpPost("GetRoleByInfo")]
        public async Task<List<T_Role>> GetRoleByInfo([FromBody] T_Role entity)
        {
            if (!string.IsNullOrEmpty(entity.RoleID) && !string.IsNullOrEmpty(entity.RoleName))
            {
                return await roleRepository.Query(d => d.RoleID == entity.RoleID && d.RoleName == entity.RoleName);
            }
            else if (!string.IsNullOrEmpty(entity.RoleID))
            {
                return await roleRepository.Query(d => d.RoleID == entity.RoleID);
            }
            else {
                return await roleRepository.Query(d => d.RoleName == entity.RoleName);
            }
        }

        /// <summary>
        /// 用户注册接口（新增用户）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("InsertRole")]
        public String InsertRole([FromBody] T_Role entity)
        {
            BaseResult result = new BaseResult();
            Task<List<T_Role>> list = roleRepository.Query(d => d.RoleID == entity.RoleID);
            if (list.Result != null && list.Result.Count > 0)
            {
                result.resType = 2;
                result.resStr = "该用户已注册!";
                return JsonConvert.SerializeObject(result);
            }
            String password = aes.Encrypt(entity.Password,"YZJ123!");
            //String tet = aes.Decrypt(password, "YZJ123!");
            entity.Password = password;
            entity.Status = 1;
            entity.Level = 3;
            entity.CreateTime = DateTime.Now;
            entity.ModifyTime = DateTime.Now;
            Task<int> num = roleRepository.Add(entity);//插入用户表

            T_RoleModifyLog t_RoleModifyLog = new T_RoleModifyLog();
            t_RoleModifyLog.RoleID = entity.RoleID;
            t_RoleModifyLog.RoleName = entity.RoleName;
            t_RoleModifyLog.Status = entity.Status;
            t_RoleModifyLog.Email = entity.Email;
            t_RoleModifyLog.Level = entity.Level;
            t_RoleModifyLog.Password = entity.Password;
            t_RoleModifyLog.CreateTime = entity.CreateTime;
            t_RoleModifyLog.ModifyTime = entity.ModifyTime;
            Task<int> num1 = roleModifyLogRepository.Add(t_RoleModifyLog);//插入用户日志表

            if (num.Result > 0 && num1.Result > 0)
            {
                result.resType = 0;
                result.resStr = "注册成功!";
                return JsonConvert.SerializeObject(result);
            }
            else {
                result.resType = 1;
                result.resStr = "注册失败!";
                return JsonConvert.SerializeObject(result);
            }
            
        }

        /// <summary>
        /// 用户编辑接口,注意：编辑不能传空字符串的字段，未修改的只能null值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateRole")]
        public String UpdateRole([FromBody] T_Role entity)
        {
            entity.ModifyTime = DateTime.Now;
            bool num = roleRepository.UpdateInfo(entity);//更新用户表
            
            Task<List<T_Role>> list = roleRepository.Query(d => d.RoleID == entity.RoleID);
            T_RoleModifyLog t_RoleModifyLog = new T_RoleModifyLog();
            t_RoleModifyLog.RoleID = list.Result[0].RoleID;
            t_RoleModifyLog.RoleName = list.Result[0].RoleName;
            t_RoleModifyLog.Status = list.Result[0].Status;
            t_RoleModifyLog.RoleGroup = list.Result[0].RoleGroup;
            t_RoleModifyLog.RoleStation = list.Result[0].RoleStation;
            t_RoleModifyLog.Grade = list.Result[0].Grade;
            t_RoleModifyLog.Email = list.Result[0].Email;
            t_RoleModifyLog.Level = list.Result[0].Level;
            t_RoleModifyLog.Password = list.Result[0].Password;
            t_RoleModifyLog.CreateTime = list.Result[0].CreateTime;
            t_RoleModifyLog.ModifyTime = list.Result[0].ModifyTime;
            Task<int> num1 = roleModifyLogRepository.Add(t_RoleModifyLog);//插入用户日志表

            BaseResult result = new BaseResult();
            if (num && num1.Result > 0)
            {
                result.resType = 0;
                result.resStr =  "编辑成功!";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                result.resType = 1;
                result.resStr = "编辑失败!";
                return JsonConvert.SerializeObject(result);
            }
        }

        /// <summary>
        /// 用户登录接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public string Login([FromBody] T_Role entity)
        {
            BaseResult lr = new BaseResult();
            Task<List<T_Role>> list = roleRepository.Query(d => d.RoleID == entity.RoleID);
            if (list.Result.Count == 0)
            {
                lr.resType = 4;
                lr.resStr = "该用户未注册!";
            }
            else {
                String password = aes.Decrypt(list.Result[0].Password, "YZJ123!");
                if (password.Equals(entity.Password))
                {
                    lr.resType = (int)list.Result[0].Level;
                    lr.resStr = "登录成功!";
                }
                else
                {
                    lr.resType = 0;
                    lr.resStr = "密码错误，登录失败!";
                }
            }
            return JsonConvert.SerializeObject(lr);
        }       
    }
}