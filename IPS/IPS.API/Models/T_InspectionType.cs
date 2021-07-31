using SqlSugar;
using System;

namespace IPS.API.Models
{
    /// <summary>
    /// 检验类型表T_InspectionType
    /// </summary>
    [SugarTable("T_InspectionType")]
    public class T_InspectionType
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "TypeID")]
        public String TypeID { get; set; }

        public String TypeName { get; set; }

        public String IsManulMaintain { get; set; }

        public DateTime? ModifyTime { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
