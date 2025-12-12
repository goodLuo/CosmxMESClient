using CosmxMESClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient.Services {
    public class IJInfoService:MESServiceBase {
        public async Task<ApiResponse> GetIJParaAsync( string projCode,string ijType ) {
            var request = new GetIJParaRequest
                {
                strProjCode = projCode,
                strIJType = ijType
                };

            Log($"获取注液参数 - 型号: {projCode}, 类型: {ijType}");
            return await PostAsync("IJInfo/GetIJPara_GK",request);
            }

        public async Task<ApiResponse> ElectCheckAsync( string electrolyte,string projCode,string machineNo,string ijType ) {
            var request = new ElectCheckRequest
                {
                strElectrolyte = electrolyte,
                strProjCode = projCode,
                strMachineNo = machineNo,
                strIJType = ijType
                };

            Log($"电解液验证 - 电解液: {electrolyte}, 型号: {projCode}");
            return await PostAsync("IJInfo/ElectCheckWithMachineNo_GK",request);
            }

        public async Task<ApiResponse> CheckConditionAsync( string cellName,string projCode,string machineNo,string ijType ) {
            var request = new CheckConditionRequest
                {
                strCellName = cellName,
                strProjCode = projCode,
                strMachineNo = machineNo,
                strIJType = ijType
                };

            Log($"注液前检查 - 电芯: {cellName}, 类型: {ijType}");
            return await PostAsync("IJInfo/CheckCondition_GK",request);
            }

        public async Task<ApiResponse> SaveIJDataAsync( SaveIJDataRequest request ) {
            Log($"保存注液数据 - 电芯: {request.strCellName}, 类型: {request.strIJType}");
            return await PostAsync("IJInfo/SaveData_GK",request);
            }

        public async Task<ApiResponse> HKDataCheckAsync( string cellName ) {
            Log($"烘烤数据检查 - 电芯: {cellName}");
            return await PostAsync($"IJInfo/HKDataCheck?strCellName={cellName}",new {
                });
            }
        }
    }
