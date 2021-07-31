using SqlSugar;
using System;

namespace IPS.API.Models
{
    /// <summary>
    /// 检验任务表T_InspectionMession
    /// </summary>
    [SugarTable("T_InspectionMession")]
    public class T_InspectionMession
    {
        public DateTime? PlanDate { get; set; }

        public string MessionID { get; set; }

        public string Type { get; set; }

        public string PartNo { get; set; }

        public string ProductName { get; set; }

        public string BatchNo { get; set; }

        public string IIID { get; set; }

        public string IINAME { get; set; }

        public string InspectGroup { get; set; }

        public string Version { get; set; }

        public string Remark { get; set; }

        public string IsManulMaintain { get; set; }

        public int? SplitCnt { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}
