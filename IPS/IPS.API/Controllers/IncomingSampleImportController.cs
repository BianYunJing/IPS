using IPS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.ObjectRepository;
using Microsoft.AspNetCore.Authorization;
using Commom;
using Newtonsoft.Json;
using SqlSugar;

namespace IPS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncomingSampleImportController : ControllerBase
    {

        readonly IncomingSampleImportRepository incomingSampleImportRepository = new IncomingSampleImportRepository();
        readonly InspectionTypeManagementRepository inspectionTypeManagementRepository = new InspectionTypeManagementRepository();
        readonly InspectionItemManagementRepository inspectionItemManagementRepository = new InspectionItemManagementRepository();

        private readonly ILogger<IncomingSampleImportController> _logger;

        public IncomingSampleImportController(ILogger<IncomingSampleImportController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// ���������ѯ�ӿ�
        /// </summary>
        [Authorize]
        [HttpGet("GetIncomingSampleImportList")]
        public async Task<List<T_ImportData>> GetIncomingSampleImportList()
        {
            return await incomingSampleImportRepository.Query(d => d.IsCreate == "N");
        }

        /// <summary>
        /// ��������ʱ�䡢Ʒ�����ϺŲ�ѯ�ӿ�
        /// </summary>
        [Authorize]
        [HttpPost("GetIncomingSampleImportByInfo")]
        public async Task<List<T_ImportData>> GetIncomingSampleImportByInfo([FromBody] T_ImportData entity)
        {
            String str = "IsCreate = 'N'";
            if (!String.IsNullOrEmpty(entity.ProductName)) {
                str = str + "and ProductName = N'" + entity.ProductName+"'";
            }
            if (!String.IsNullOrEmpty(entity.PartNo))
            {
                str = str + "and PartNo = '" + entity.PartNo + "'";
            }
            if (entity.DateForSearch != null)
            {
                str = str + "and CONVERT(varchar(100), ReceiveDate, 23) = '" + entity.DateForSearch + "'";
            }
           return await incomingSampleImportRepository.Query(str);
        }

        /// <summary>
        /// ��������༭�ӿ�,ע�⣺�༭���ܴ����ַ������ֶΣ�δ�޸ĵ�ֻ��nullֵ
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateIncomingSampleImport")]
        public String UpdateIncomingSampleImport(T_ImportData entity)
        {
            BaseResult result = new BaseResult();
            Task<List<T_ImportData>> info = incomingSampleImportRepository.Query(d => d.Id == entity.Id && d.IsCreate.Equals("Y"));
            if (info.Result.Count > 0) {
                result.resType = 2;
                result.resStr = "�Ѵ�����������Ĳ����޸���Ϣ!";
                return JsonConvert.SerializeObject(result);
            }
            entity.ModifyTime = DateTime.Now;
            bool num = incomingSampleImportRepository.UpdateInfo(entity);
            if (num)
            {
                result.resType = 0;
                result.resStr = "����ɹ�!";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                result.resType = 1;
                result.resStr = "����ʧ��!";
                return JsonConvert.SerializeObject(result);
            }

        }

        /// <summary>
        /// �������������ӿ�
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("InsertIncomingSampleImport")]
        public String InsertIncomingSampleImport(List<T_ImportData> list)
        {
            BaseResult result = new BaseResult();
            foreach (T_ImportData obj in list) {
                //�˶�������Ϣ�Ƿ���ά��
                Task<List<T_InspectionType>> typeInfo = inspectionTypeManagementRepository.Query(d => d.TypeName == obj.Type);
                if (typeInfo.Result.Count == 0) {
                    result.resType = 2;
                    result.resStr = "����δά�������ڼ������͹���ҳ���������ά����";
                    return JsonConvert.SerializeObject(result);
                }

                //�˶��Ϻźͼ���Ⱥ����Ϣ�Ƿ���ά��
                Task<List<T_InsItem>> insItems = inspectionItemManagementRepository.Query(d => d.PartNo == obj.PartNo && d.InspectGroup == obj.Group && d.IsEnable.Equals("Y"));
                if (insItems.Result.Count == 0) {
                    result.resType = 3;
                    result.resStr = "�Ϻź������Ϣδά�������ڼ������ҳ���������ά����";
                    return JsonConvert.SerializeObject(result);
                }
                obj.ProductName = insItems.Result[0].ProductName;
                obj.IsCreate = "N";
                obj.CreateTime = DateTime.Now;
                obj.ModifyTime = DateTime.Now;
                obj.ReceiveDate = DateTime.Now;
                obj.Id = Guid.NewGuid().ToString();
            }

            Task<int> count = incomingSampleImportRepository.Add(list);
            if (count.Result > 0)
            {
                result.resType = 0;
                result.resStr = "�ϴ��ɹ�";
                return JsonConvert.SerializeObject(result);
            }
            else {
                result.resType = 1;
                result.resStr = "�ϴ�ʧ��";
                return JsonConvert.SerializeObject(result);
            }
        }

    }
}