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
        /// 来样导入查询接口
        /// </summary>
        [Authorize]
        [HttpGet("GetIncomingSampleImportList")]
        public async Task<List<T_ImportData>> GetIncomingSampleImportList()
        {
            return await incomingSampleImportRepository.Query(d => d.IsCreate == "N");
        }

        /// <summary>
        /// 根据收样时间、品名、料号查询接口
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
        /// 来样导入编辑接口,注意：编辑不能传空字符串的字段，未修改的只能null值
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
                result.resStr = "已创建检验任务的不能修改信息!";
                return JsonConvert.SerializeObject(result);
            }
            entity.ModifyTime = DateTime.Now;
            bool num = incomingSampleImportRepository.UpdateInfo(entity);
            if (num)
            {
                result.resType = 0;
                result.resStr = "保存成功!";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                result.resType = 1;
                result.resStr = "保存失败!";
                return JsonConvert.SerializeObject(result);
            }

        }

        /// <summary>
        /// 来样导入新增接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("InsertIncomingSampleImport")]
        public String InsertIncomingSampleImport(List<T_ImportData> list)
        {
            BaseResult result = new BaseResult();
            foreach (T_ImportData obj in list) {
                //核对类型信息是否已维护
                Task<List<T_InspectionType>> typeInfo = inspectionTypeManagementRepository.Query(d => d.TypeName == obj.Type);
                if (typeInfo.Result.Count == 0) {
                    result.resType = 2;
                    result.resStr = "类型未维护，请在检验类型功能页面完成数据维护！";
                    return JsonConvert.SerializeObject(result);
                }

                //核对料号和检验群组信息是否已维护
                Task<List<T_InsItem>> insItems = inspectionItemManagementRepository.Query(d => d.PartNo == obj.PartNo && d.InspectGroup == obj.Group && d.IsEnable.Equals("Y"));
                if (insItems.Result.Count == 0) {
                    result.resType = 3;
                    result.resStr = "料号和组别信息未维护，请在检验项功能页面完成数据维护！";
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
                result.resStr = "上传成功";
                return JsonConvert.SerializeObject(result);
            }
            else {
                result.resType = 1;
                result.resStr = "上传失败";
                return JsonConvert.SerializeObject(result);
            }
        }

    }
}