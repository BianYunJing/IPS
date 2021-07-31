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
    public class InspectionItemMaintainController : ControllerBase
    {

        readonly InspectionItemManagementRepository inspectionItemManagementRepository = new InspectionItemManagementRepository();
        readonly ProuductionRepository prouductionRepository = new ProuductionRepository();

        private readonly ILogger<InspectionItemMaintainController> _logger;

        public InspectionItemMaintainController(ILogger<InspectionItemMaintainController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 检验项查询查询接口
        /// </summary>
        [Authorize]
        [HttpGet("GetInspectionItemList")]
        public async Task<List<T_InsItem>> GetInspectionItemList()
        {
            return await inspectionItemManagementRepository.Query(d => d.IsEnable == "Y",200, "CreateTime asc");
        }

        /// <summary>
        /// 根据检项名、检项序号、产品名、产品序号查询接口
        /// </summary>
        [Authorize]
        [HttpPost("GetInspectionItemByInfo")]
        public async Task<List<T_InsItem>> GetInspectionItemByInfo([FromBody] T_InsItem entity)
        {
            String str = "IsEnable = 'Y'";
            if (!String.IsNullOrEmpty(entity.ProductName)) {
                str = str + "and ProductName = N'" + entity.ProductName+"'";
            }
            if (!String.IsNullOrEmpty(entity.PartNo))
            {
                str = str + "and PartNo = '" + entity.PartNo + "'";
            }
            if (!String.IsNullOrEmpty(entity.IIID))
            {
                str = str + "and IIID = '" + entity.IIID + "'";
            }
            if (!String.IsNullOrEmpty(entity.IINAME))
            {
                str = str + "and IINAME = N'" + entity.IINAME + "'";
            }
           return await inspectionItemManagementRepository.Query(str);
        }

        /// <summary>
        /// 检验项编辑接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost("UpdateInspectionItem")]
        //public String UpdateInspectionItem(T_InsItem entity)
        //{
        //    BaseResult result = new BaseResult();
        //    entity.ModifyTime = DateTime.Now;
        //    bool num = inspectionItemManagementRepository.UpdateInfo(entity);
        //    if (num)
        //    {
        //        result.resType = 0;
        //        result.resStr = "保存成功!";
        //        return JsonConvert.SerializeObject(result);
        //    }
        //    else
        //    {
        //        result.resType = 1;
        //        result.resStr = "保存失败!";
        //        return JsonConvert.SerializeObject(result);
        //    }

        //}

        /// <summary>
        /// 检验项新增接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("InsertInspectionItem")]
        public String InsertInspectionItem(List<T_InsItem> list)
        {
            BaseResult result = new BaseResult();
            List<T_InsItem> inspectionItemList = new List<T_InsItem>();
            foreach (T_InsItem obj in list) {
                Task<List<T_Prouduction>> info = prouductionRepository.Query(d => d.PartNo == obj.PartNo && d.IsActive.Equals("Y"));//根据料号查询产品名称
                if (info.Result.Count == 0) {
                    result.resType = 2;
                    result.resStr = "料号：" + obj.PartNo + "数据未维护，请核查后重新上传！";
                    return JsonConvert.SerializeObject(result);
                }

                //根据料号检测版本冲突
                T_InsItem t_InsItem = new T_InsItem();
                Task<List<T_InsItem>> idInfo = inspectionItemManagementRepository.Query(d => d.IsEnable.Equals("Y") && d.PartNo == obj.PartNo);
                if (idInfo.Result.Count > 0)
                {
                    double a = Double.Parse(idInfo.Result[0].Version);
                    double b = a + 0.1;
                    t_InsItem.Version = b.ToString("f1");
                }else {
                    t_InsItem.Version = "1.0";
                }
                
                t_InsItem.IIID = Guid.NewGuid().ToString();
                //t_InsItem.IIID = DateTime.Now.ToString("yyyyMMddHHmmssms");
                t_InsItem.IINAME = obj.IINAME;
                t_InsItem.PartNo = obj.PartNo;
                t_InsItem.InspectGroup = obj.InspectGroup;
                t_InsItem.InspectionStation = obj.InspectionStation;
                t_InsItem.IICycleTime = obj.IICycleTime;
                t_InsItem.IsNeedEquip = obj.IsNeedEquip;
                t_InsItem.IsEnable = "Y";
                t_InsItem.CreateTime = DateTime.Now;
                t_InsItem.ModifyTime = DateTime.Now;
                t_InsItem.ProductName = info.Result[0].ProductName;
                inspectionItemList.Add(t_InsItem);  
            }

            List<T_InsItem> idList = new List<T_InsItem>();
            List<string> list1 = list.Select(x => x.PartNo).Distinct().ToList();
            String str = "";
            for (int i = 0; i < list1.Count; i++) {
                if (i == 0)
                {
                    str = str + list1[i];
                }
                else {
                    str = str + ","+ list1[i];
                } 
            }
            Task<List<T_InsItem>> needInfo = inspectionItemManagementRepository.Query("IsEnable = 'Y' and PartNo in (" + str +")");
            foreach (T_InsItem t_Ins in needInfo.Result)
            {
                T_InsItem insItem = new T_InsItem();
                insItem.IIID = t_Ins.IIID;
                insItem.IINAME = t_Ins.IINAME;
                insItem.PartNo = t_Ins.PartNo;
                insItem.InspectGroup = t_Ins.InspectGroup;
                insItem.InspectionStation = t_Ins.InspectionStation;
                insItem.Version = t_Ins.Version;
                insItem.IICycleTime = t_Ins.IICycleTime;
                insItem.IsNeedEquip = t_Ins.IsNeedEquip;
                insItem.IsEnable = "N";
                insItem.CreateTime = t_Ins.CreateTime;
                insItem.ModifyTime = DateTime.Now;
                insItem.ProductName = t_Ins.ProductName;
                idList.Add(insItem);
            }

            if (idList.Count > 0)
            {
                bool updateStatus = inspectionItemManagementRepository.UpdateInfo(idList);
            }

            Task<int> count = inspectionItemManagementRepository.Add(inspectionItemList);

            if (count.Result > 0)
            {
                result.resType = 0;
                result.resStr = "上传成功";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                result.resType = 1;
                result.resStr = "上传失败";
                return JsonConvert.SerializeObject(result);
            }

        }  
        
    }
}