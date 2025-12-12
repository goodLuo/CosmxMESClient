using CosmxMESClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient.Services {
    public class PublicLibraryService:MESServiceBase {
        public async Task<ApiResponse> GetProjCodeAsync( string machineNo,string procName = "",string gxdm = "" ) {
            var request = new GetProjCodeRequest
                {
                MachineNo = machineNo,
                ProcName = procName,
                GXDM = gxdm
                };

            Log($"获取产品型号 - 设备: {machineNo}, 工序: {procName}");
            return await PostAsync("PubLibrary/GetProjCode",request);
            }

        public async Task<ApiResponse> SaveMarkingInfoAsync( string marking,string tmType,string updateUser,List<string> cells ) {
            var request = new SaveMarkingInfoRequest
                {
                strMarking = marking,
                strTMType = tmType,
                strUpdateUser = updateUser,
                listCells = cells
                };

            Log($"保存标记信息 - 标记: {marking}, 条码数量: {cells?.Count??0}");
            return await PostAsync("PubLibrary/SaveMarkingInfo",request);
            }

        public async Task<ApiResponse> CheckCardAsync( string cardNo,string machineNo,string procName ) {
            var request = new CardCheckRequest
                {
                CardNo = cardNo,
                MachineNo = machineNo,
                ProcName = procName
                };

            Log($"刷卡验证 - 卡号: {cardNo}, 设备: {machineNo}");
            return await PostAsync("CardCheck/CheckCard",request);
            }

        public async Task<ApiResponse> BindTrayAsync( string cellName,string trayCode,string channelNo,string projCode,string machineNo ) {
            var request = new BindTrayRequest
                {
                strCellName = cellName,
                strTrayCode = trayCode,
                strChannelNo = channelNo,
                strProjCode = projCode,
                strMachineNo = machineNo
                };

            Log($"绑定料框 - 电芯: {cellName}, 料框: {trayCode}");
            return await PostAsync("PubLibrary/GK_BarcodeBinding",request);
            }

        public async Task<ApiResponse> UnbindTrayAsync( string cellName,string trayCode,string channelNo,string projCode,string machineNo = "" ) {
            var request = new
            {
                strCellName = cellName,
                strTrayCode = trayCode,
                strChannelNo = channelNo,
                strProjCode = projCode,
                strMachineNo = machineNo
                };

            Log($"解绑料框 - 电芯: {cellName}, 料框: {trayCode}");
            return await PostAsync("PubLibrary/GK_BarcodeUnBind",request);
            }

        public async Task<ApiResponse> GetCellMarkingAsync( string cellName ) {
            var request = new {
                strCellName = cellName
                };
            Log($"获取电芯标记 - 电芯: {cellName}");
            return await PostAsync("PubLibrary/GetCellMarking",request);
            }
        }
    }
