using SqlSugar;
using System;

namespace IPS.API.Models
{
    /// <summary>
    /// ��Ʒά����T_Prouduction
    /// </summary>
    [SugarTable("T_Prouduction")]
    public class T_Prouduction
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "Id")]
        public String Id { get; set; }

        public String PartNo { get; set; }

        public String ProductName { get; set; }

        public String IsActive { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
