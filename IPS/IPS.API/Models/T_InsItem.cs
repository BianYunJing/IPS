using SqlSugar;
using System;

namespace IPS.API.Models
{
    /// <summary>
    /// 检验项维护表T_InsItem
    /// </summary>
    [SugarTable("T_InsItem")]
    public class T_InsItem
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "IIID")]
        public String IIID { get; set; }

        public String IINAME { get; set; }

        public string PartNo { get; set; }

        public string ProductName { get; set; }

        public string InspectGroup { get; set; }

        public String InspectionStation { get; set; }

        public String Version { get; set; }

        public String IICycleTime { get; set; }

        public String IsNeedEquip { get; set; }

        public String IsEnable { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}
