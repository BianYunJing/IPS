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
    public class ProductionMaintainController : ControllerBase
    {

        readonly ProuductionRepository prouductionRepository = new ProuductionRepository();
        readonly ProuductionCopyRepository prouductionCopyRepository = new ProuductionCopyRepository();

        private readonly ILogger<ProductionMaintainController> _logger;

        public ProductionMaintainController(ILogger<ProductionMaintainController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 产品查询接口-所有有效产品数据
        /// </summary>
        [Authorize]
        [HttpGet("GetProuductionList")]
        public async Task<List<T_Prouduction>> GetProuductionList()
        {
            return await prouductionRepository.Query(d => d.IsActive.Equals("Y"));
        }

        /// <summary>
        /// 根据品名或者料号查询接口
        /// </summary>
        [Authorize]
        [HttpPost("GetProuductionByInfo")]
        public async Task<List<T_Prouduction>> GetProuductionByInfo([FromBody] T_Prouduction entity)
        {
            if (!String.IsNullOrEmpty(entity.PartNo) && !String.IsNullOrEmpty(entity.ProductName))
            {
                return await prouductionRepository.Query(d => d.PartNo == entity.PartNo && d.ProductName == entity.ProductName && d.IsActive.Equals("Y"));
            }
            else if (!String.IsNullOrEmpty(entity.PartNo))
            {
                return await prouductionRepository.Query(d => d.PartNo == entity.PartNo && d.IsActive.Equals("Y"));
            }
            else {
                return await prouductionRepository.Query(d => d.ProductName == entity.ProductName && d.IsActive.Equals("Y"));
            }
        }

        /// <summary>
        /// 产品上传接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("InsertProuduction")]
        public String InsertProuduction(List<T_Prouduction_copy> list)
        {
            BaseResult result = new BaseResult();
            //1.在产品临时表中验证数据是否有重复料号
            try
            {
                Task<int> num = prouductionCopyRepository.Add(list);
                if (num.Result == 0) {
                    result.resType = 2;
                    result.resStr = "上传失败！表格中存在重复料号内容，请修改产品数据！";
                    return JsonConvert.SerializeObject(result);
                }
            }catch (Exception e){
                result.resType = 2;
                result.resStr = "上传失败！表格中存在重复料号内容，请修改产品数据！";
                return JsonConvert.SerializeObject(result);
            }

            //2.循环获取数据库中已存在的旧料号，并设置为无效
            List<T_Prouduction> prouductionList = new List<T_Prouduction>();
            List<T_Prouduction> idList = new List<T_Prouduction>();
            foreach (T_Prouduction_copy obj in list)
            {
                T_Prouduction t_Prouduction = new T_Prouduction();
                t_Prouduction.PartNo = obj.PartNo;
                t_Prouduction.ProductName = obj.ProductName;
                t_Prouduction.CreateTime = DateTime.Now;
                t_Prouduction.IsActive = "Y";
                t_Prouduction.Id = Guid.NewGuid().ToString();
                prouductionList.Add(t_Prouduction);

                Task<List<T_Prouduction>> info = prouductionRepository.Query(d => d.PartNo == t_Prouduction.PartNo && d.IsActive.Equals("Y"));
                if (info.Result.Count > 0) {
                    T_Prouduction prouduction = new T_Prouduction();
                    prouduction.Id = info.Result[0].Id;
                    prouduction.IsActive = "N";
                    prouduction.PartNo = info.Result[0].PartNo;
                    prouduction.ProductName = info.Result[0].ProductName;
                    prouduction.CreateTime = info.Result[0].CreateTime;
                    idList.Add(prouduction);
                }
            }
            if (idList.Count > 0) {
                bool updateStatus = prouductionRepository.UpdateInfo(idList);
            }
           
            //3批量插入产品表
            Task<int> count = prouductionRepository.Add(prouductionList);

            //4.清空产品临时表
            bool deleteStatus = prouductionCopyRepository.Delete();

            if (count.Result > 0 && deleteStatus)
            {
                result.resType = 0;
                result.resStr = "上传成功";
                return JsonConvert.SerializeObject(result);
            }
            else {
                result.resType = 0;
                result.resStr = "上传失败";
                return JsonConvert.SerializeObject(result);
            }  
        }
    }
}