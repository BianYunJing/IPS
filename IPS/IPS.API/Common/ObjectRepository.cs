using Common.IBaseRepository;
using IPS.API.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Commom.BaseRepository;

namespace Common.ObjectRepository
{
    public class RoleRepository : BaseRepository<T_Role>, IBaseRepository<T_Role>
    {

    }
    public class RoleModifyLogRepository : BaseRepository<T_RoleModifyLog>, IBaseRepository<T_RoleModifyLog>
    {

    }

    public class ProuductionRepository : BaseRepository<T_Prouduction>, IBaseRepository<T_Prouduction>
    {

    }

    public class ProuductionCopyRepository : BaseRepository<T_Prouduction_copy>, IBaseRepository<T_Prouduction_copy>
    {

    }
    public class InspectionTypeManagementRepository : BaseRepository<T_InspectionType>, IBaseRepository<T_InspectionType>
    {

    }

    public class InspectionItemManagementRepository : BaseRepository<T_InsItem>, IBaseRepository<T_InsItem>
    {

    }

    public class IncomingSampleImportRepository : BaseRepository<T_ImportData>, IBaseRepository<T_ImportData>
    {

    }
}
