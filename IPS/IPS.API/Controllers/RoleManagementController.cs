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
        /// �û���ѯ�ӿ�-�����û�����
        /// </summary>
        [Authorize]
        [HttpGet("GetRoleList")]
        public async Task<List<T_Role>> GetRoleList()
        {
            return await roleRepository.Query();
        }

        /// <summary>
        /// ����Ա���Ż���������ѯ�ӿ�
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
        /// �û�ע��ӿڣ������û���
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
                result.resStr = "���û���ע��!";
                return JsonConvert.SerializeObject(result);
            }
            String password = aes.Encrypt(entity.Password,"YZJ123!");
            //String tet = aes.Decrypt(password, "YZJ123!");
            entity.Password = password;
            entity.Status = 1;
            entity.Level = 3;
            entity.CreateTime = DateTime.Now;
            entity.ModifyTime = DateTime.Now;
            Task<int> num = roleRepository.Add(entity);//�����û���

            T_RoleModifyLog t_RoleModifyLog = new T_RoleModifyLog();
            t_RoleModifyLog.RoleID = entity.RoleID;
            t_RoleModifyLog.RoleName = entity.RoleName;
            t_RoleModifyLog.Status = entity.Status;
            t_RoleModifyLog.Email = entity.Email;
            t_RoleModifyLog.Level = entity.Level;
            t_RoleModifyLog.Password = entity.Password;
            t_RoleModifyLog.CreateTime = entity.CreateTime;
            t_RoleModifyLog.ModifyTime = entity.ModifyTime;
            Task<int> num1 = roleModifyLogRepository.Add(t_RoleModifyLog);//�����û���־��

            if (num.Result > 0 && num1.Result > 0)
            {
                result.resType = 0;
                result.resStr = "ע��ɹ�!";
                return JsonConvert.SerializeObject(result);
            }
            else {
                result.resType = 1;
                result.resStr = "ע��ʧ��!";
                return JsonConvert.SerializeObject(result);
            }
            
        }

        /// <summary>
        /// �û��༭�ӿ�,ע�⣺�༭���ܴ����ַ������ֶΣ�δ�޸ĵ�ֻ��nullֵ
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateRole")]
        public String UpdateRole([FromBody] T_Role entity)
        {
            entity.ModifyTime = DateTime.Now;
            bool num = roleRepository.UpdateInfo(entity);//�����û���
            
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
            Task<int> num1 = roleModifyLogRepository.Add(t_RoleModifyLog);//�����û���־��

            BaseResult result = new BaseResult();
            if (num && num1.Result > 0)
            {
                result.resType = 0;
                result.resStr =  "�༭�ɹ�!";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                result.resType = 1;
                result.resStr = "�༭ʧ��!";
                return JsonConvert.SerializeObject(result);
            }
        }

        /// <summary>
        /// �û���¼�ӿ�
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
                lr.resStr = "���û�δע��!";
            }
            else {
                String password = aes.Decrypt(list.Result[0].Password, "YZJ123!");
                if (password.Equals(entity.Password))
                {
                    lr.resType = (int)list.Result[0].Level;
                    lr.resStr = "��¼�ɹ�!";
                }
                else
                {
                    lr.resType = 0;
                    lr.resStr = "������󣬵�¼ʧ��!";
                }
            }
            return JsonConvert.SerializeObject(lr);
        }       
    }
}