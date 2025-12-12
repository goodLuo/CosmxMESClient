using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient.Models {
    #region 公共接口请求模型
    public class GetProjCodeRequest {
        public string MachineNo {
            get; set;
            }
        public string ProcName {
            get; set;
            }
        public string GXDM {
            get; set;
            }
        }

    public class SaveMarkingInfoRequest {
        public string strMarking {
            get; set;
            }
        public string strTMType {
            get; set;
            }
        public string strUpdateUser {
            get; set;
            }
        public List<string> listCells {
            get; set;
            }
        }

    public class CardCheckRequest {
        public string CardNo {
            get; set;
            }
        public string MachineNo {
            get; set;
            }
        public string ProcName {
            get; set;
            }
        }

    public class BindTrayRequest {
        public string strCellName {
            get; set;
            }
        public string strTrayCode {
            get; set;
            }
        public string strChannelNo {
            get; set;
            }
        public string strProjCode {
            get; set;
            }
        public string strMachineNo {
            get; set;
            }
        }
    #endregion

    #region 烘烤工序请求模型
    public class CheckCellRequest {
        public string CellName {
            get; set;
            }
        public string ProjCode {
            get; set;
            }
        public string ComputerName {
            get; set;
            }
        }

    public class UploadBakeDataRequest {
        public string Cell_Name {
            get; set;
            }
        public string Proj_Code {
            get; set;
            }
        public string Quality {
            get; set;
            }
        public string NGReason {
            get; set;
            }
        public string StartDate {
            get; set;
            }
        public string Baking_Start_Time {
            get; set;
            }
        public string Baking_End_Time {
            get; set;
            }
        public string Distribution_Num {
            get; set;
            }
        public string Cavity {
            get; set;
            }
        public string Lay {
            get; set;
            }
        public string Position {
            get; set;
            }
        public string Vacuum_Chamber {
            get; set;
            }
        public string Valve_Vacuum {
            get; set;
            }
        public string Temperature {
            get; set;
            }
        public string Machine_No {
            get; set;
            }
        public string Operator {
            get; set;
            }
        public string Water_Content {
            get; set;
            }
        public string IsTestSample {
            get; set;
            }
        public string Remark {
            get; set;
            }
        public string Extensions_1 {
            get; set;
            }
        public string Extensions_2 {
            get; set;
            }
        public string Bakeagain_Start_Time {
            get; set;
            }
        public string Bakeagain_End_Time {
            get; set;
            }
        public string Bakeagain_Temperature {
            get; set;
            }
        public string Bakeagain_Vacuum {
            get; set;
            }
        }
    #endregion

    #region 注液工序请求模型
    public class GetIJParaRequest {
        public string strProjCode {
            get; set;
            }
        public string strIJType {
            get; set;
            }
        }

    public class ElectCheckRequest {
        public string strElectrolyte {
            get; set;
            }
        public string strProjCode {
            get; set;
            }
        public string strMachineNo {
            get; set;
            }
        public string strIJType {
            get; set;
            }
        }

    public class CheckConditionRequest {
        public string strCellName {
            get; set;
            }
        public string strProjCode {
            get; set;
            }
        public string strMachineNo {
            get; set;
            }
        public string strIJType {
            get; set;
            }
        }

    public class SaveIJDataRequest {
        public string strCellName {
            get; set;
            }
        public string strIJBefore {
            get; set;
            }
        public string strIJAfter {
            get; set;
            }
        public string strIJWeight {
            get; set;
            }
        public string strZYZ {
            get; set;
            }
        public string strOperator {
            get; set;
            }
        public string strDateTime {
            get; set;
            }
        public string strBatch {
            get; set;
            }
        public string strPO {
            get; set;
            }
        public string strMachineNo {
            get; set;
            }
        public string Before_MachineNo {
            get; set;
            }
        public string Before_DateTime {
            get; set;
            }
        public string After_MachineNo {
            get; set;
            }
        public string After_DateTime {
            get; set;
            }
        public string strIJType {
            get; set;
            }
        }
    #endregion
    }
