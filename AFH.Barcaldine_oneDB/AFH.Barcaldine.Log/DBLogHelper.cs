using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;


using AFH.Common.DataBaseAccess;
using AFH.Barcaldine.Common;

namespace AFH.Barcaldine.Log
{
    public class DBLogHelper
    {
        private LogModule _log = new LogModule();

        public DBLogHelper(string system, string systemModule)
        {
            this._log.System = system;
            this._log.SystemModel = systemModule;
        }

        public void WriteLog(string content)
        {
            this._log.LogContent = content;
            this.WriteLog(_log);
        }

        public void WriteLog(string content, string referenceID)
        {
            this._log.LogContent = content;
            this._log.ReferenceID = referenceID;
            this.WriteLog(_log);
        }

        public void WriteLog(string content, string errorMessage, string referenceID)
        {
            this._log.LogContent = content;
            this._log.ErrorMessage = errorMessage;
            this._log.ReferenceID = referenceID;
            this.WriteLog(_log);
        }


        public void WriteLog(LogModule log)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("INSERT INTO LogInfo ");
                sql.AppendLine("(LogContent, ErrorMessage, LogTime, System, SystemModel, ReferenceID) ");
                sql.AppendLine(" Values (@LogContent, @ErrorMessage, getdate(), @System, @SystemModel, @ReferenceID)");

                SqlParameter[] paras = new SqlParameter[5];
                paras[0] = new SqlParameter("@LogContent", Common.VariableConvert.ConvertStringToDBValue(log.LogContent));
                paras[1] = new SqlParameter("@ErrorMessage", Common.VariableConvert.ConvertStringToDBValue(log.ErrorMessage));
                paras[2] = new SqlParameter("@System", Common.VariableConvert.ConvertStringToDBValue(log.System));
                paras[3] = new SqlParameter("@SystemModel", Common.VariableConvert.ConvertStringToDBValue(log.SystemModel));
                paras[4] = new SqlParameter("@ReferenceID", Common.VariableConvert.ConvertStringToDBValue(log.ReferenceID));

                SqlAccess mySqlAccess = new SqlAccess();
                mySqlAccess.ExecuteNonQuery(sql.ToString(), paras);
            }
            catch
            {
            }

        }

    }
}
