using CosmxMESClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient.Services {
    public class BakeService:MESServiceBase {
        public async Task<ApiResponse> CheckCellAsync( string cellName,string projCode,string computerName ) {
            var request = new CheckCellRequest
                {
                CellName = cellName,
                ProjCode = projCode,
                ComputerName = computerName
                };

            Log($"烘烤电芯校验 - 电芯: {cellName}, 型号: {projCode}");
            return await PostAsync("Bake/CheckCell_GK",request);
            }

        public async Task<ApiResponse> UploadBakeDataAsync( UploadBakeDataRequest request ) {
            Log($"上传烘烤数据 - 电芯: {request.Cell_Name}, 炉号: {request.Cavity}");
            return await PostAsync("Bake/UploadBakeData_GK",request);
            }

        public async Task<ApiResponse> GetWaterInfoAsync( string cellName ) {
            var request = new {
                strCellName = cellName
                };
            Log($"获取水分数据 - 电芯: {cellName}");
            return await PostAsync("Bake/WaterInfo",request);
            }

        public async Task<ApiResponse> UploadProcBakeDataAsync( string projCode,string machineNo,string furnaceNo,
            string cardNo,string dataTime,float vacuum,int tmpCount,float[] tmps ) {
            var request = new
            {
                Proj_Code = projCode,
                Machine_No = machineNo,
                Fumace_No = furnaceNo,
                Card_No = cardNo,
                Data_time = dataTime,
                Vacuum = vacuum,
                TMP_Count = tmpCount,
                TMPs = tmps
                };

            Log($"上传烘烤过程数据 - 设备: {machineNo}, 温度点数: {tmpCount}");
            return await PostAsync("Bake/UploadProcBakeData",request);
            }
        }
    }
