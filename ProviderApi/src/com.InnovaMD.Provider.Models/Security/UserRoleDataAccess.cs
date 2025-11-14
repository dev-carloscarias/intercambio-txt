
namespace com.InnovaMD.Provider.Models.Security
{
    public class UserRoleDataAccess
    {
        public UserRoleDataAccess()
        {
        }

        public UserRoleDataAccess(DataAccessTypes dataAccessType, int dataAccessValue, bool isActive)
        {
            Type = new DataAccessType { DataAccessTypeId = (int)dataAccessType };
            DataAccessValue = dataAccessValue;
            IsActive = isActive;
        }


        public UserRoleDataAccess(DataAccessType dataAccessType, int dataAccessValue)
        {
            Type = dataAccessType;
            DataAccessValue = dataAccessValue;
        }

        public int? UserRoleDataAccessId { get; set; }

        public DataAccessType Type { get; set; }

        public int DataAccessValue { get; set; }

        public int? UserRoleId { get; set; }

        public bool? IsActive { get; set; }
    }
}
