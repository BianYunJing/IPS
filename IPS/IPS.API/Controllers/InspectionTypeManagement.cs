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
using Commom;
using Newtonsoft.Json;

namespace IPS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InspectionTypeManagementController : ControllerBase
    {

        readonly InspectionTypeManagementRepository inspectionTypeManagementRepository = new InspectionTypeManagementRepository();

        private readonly ILogger<InspectionTypeManagementController> _logger;

        public InspectionTypeManagementController(ILogger<InspectionTypeManagementController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// �������Ͳ�ѯ�ӿ�
        /// </summary>
        [Authorize]
        [HttpGet("GetInspectionTypeList")]
        public async Task<List<T_InspectionType>> GetInspectionTypeList()
        {
            return await inspectionTypeManagementRepository.Query();
        }

        /// <summary>
        /// ����id���߼������Ʋ�ѯ�ӿ�
        /// </summary>
        [Authorize]
        [HttpPost("GetInspectionTypeByInfo")]
        public async Task<List<T_InspectionType>> GetProuductionByInfo([FromBody] T_InspectionType entity)
        {
            if (!string.IsNullOrEmpty(entity.TypeID) && !string.IsNullOrEmpty(entity.TypeName))
            {
                return await inspectionTypeManagementRepository.Query(d => d.TypeID == entity.TypeID && d.TypeName == entity.TypeName);
            }
            else if (!string.IsNullOrEmpty(entity.TypeID))
            {
                return await inspectionTypeManagementRepository.Query(d => d.TypeID == entity.TypeID);
            }
            else
            {
                return await inspectionTypeManagementRepository.Query(d => d.TypeName == entity.TypeName);
            }
        }

        /// <summary>
        /// �������������ӿ�
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("InsertInspectionType")]
        public String InsertInspectionType(List<T_InspectionType> list)
        {
            BaseResult result = new BaseResult();
            
            foreach (T_InspectionType obj in list)
            {
                Task<List<T_InspectionType>>  info = inspectionTypeManagementRepository.Query(d => d.TypeName == obj.TypeName);
                if (info.Result.Count > 0) {
                    result.resType = 2;
                    result.resStr = "�����������ơ�" + info.Result[0].TypeName + "�� �Ѵ���" + "���޷�������";
                    return JsonConvert.SerializeObject(result);
                }
                obj.TypeID = Guid.NewGuid().ToString();
                obj.CreateTime = DateTime.Now;
                obj.ModifyTime = DateTime.Now;
            }
            Task<int> count = inspectionTypeManagementRepository.Add(list);
            if (count.Result > 0)
            {
                result.resType = 0;
                result.resStr = "�����ɹ�";
                return JsonConvert.SerializeObject(result);
            }
            else {
                result.resType = 1;
                result.resStr = "����ʧ��";
                return JsonConvert.SerializeObject(result);
            }
           
        }
    }
}