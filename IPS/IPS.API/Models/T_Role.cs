using SqlSugar;
using System;

namespace IPS.API.Models
{
    /// <summary>
    /// 人员管理表T_Role
    /// </summary>
    [SugarTable("T_Role")]
    public class T_Role
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "RoleID")]
        public String RoleID { get; set; }

        public String RoleName { get; set; }

        public int? Status { get; set; }

        public string RoleGroup { get; set; }

        public string RoleStation { get; set; }

        public int? Grade { get; set; }

        public String Email { get; set; }

        public int? Level { get; set; }

        public String Password { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}
