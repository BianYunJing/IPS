using SqlSugar;
using System;

namespace IPS.API.Models
{
    /// <summary>
    /// 产品维护临时表T_Prouduction_copy
    /// </summary>
    [SugarTable("T_Prouduction_copy")]
    public class T_Prouduction_copy
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "PartNo")]
        public String PartNo { get; set; }

        public String ProductName { get; set; }

        public String IsActive { get; set; }

        public DateTime? CreateTime { get; set; }

        public String Id { get; set; }
    }
}
