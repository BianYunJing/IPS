using SqlSugar;
using System;

namespace IPS.API.Models
{
    /// <summary>
    /// 来样导入表T_ImportData
    /// </summary>
    [SugarTable("T_ImportData")]
    public class T_ImportData
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "Id")]
        public String Id { get; set; }
        public DateTime? ReceiveDate { get; set; }

        public string Type { get; set; }

        public string PartNo { get; set; }

        public string ProductName { get; set; }

        public string BatchNo { get; set; }

        public string Receiver { get; set; }

        public string Group { get; set; }

        public string Acceptor { get; set; }

        public DateTime? AcceptDate { get; set; }

        public string Remark { get; set; }

        public string IsCreate { get; set; }

        [SugarColumn(ColumnName = "DateForSearch", IsIgnore = true)]
        public string DateForSearch { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}
