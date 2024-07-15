using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using project.learning.Libs;

namespace project.learning.Controllers
{
    [Route("project.learning/[controller]")]
    public class PinjamanController : Controller
    {
        private BaseController bc = new BaseController();
        private lMessage mc = new lMessage();
        private lConvert lc = new lConvert();

        [HttpGet("getDataPinjamanPerCycle")]
        public JObject getDataPinjamanPerCycle()
        {
            string spname = "loan.get_data_pinjaman_per_cycle";
            var retObject = new List<dynamic>();
            var data = new JObject();
            try
            {
                data = new JObject();
                retObject = bc.getDataToObject(spname);
                data.Add("status", mc.GetMessage("api_output_ok"));
                data.Add("message", mc.GetMessage("process_success"));
                data.Add("data", lc.convertDynamicToJArray(retObject));
            }
            catch (Exception ex)
            {
                data = new JObject();
                data.Add("status", mc.GetMessage("api_output_not_ok"));
                data.Add("message", ex.Message);
            }
            return data;
        }

        [HttpGet("getDataPengajuanPerCycle")]
        public JObject getDataPengajuanPerCycle()
        {
            string spname = "workflow.get_data_pengajuan_per_cycle";
            var retObject = new List<dynamic>();
            var data = new JObject();
            try
            {
                data = new JObject();
                retObject = bc.getDataToObject(spname);
                data.Add("status", mc.GetMessage("api_output_ok"));
                data.Add("message", mc.GetMessage("update_success"));
                data.Add("data", lc.convertDynamicToJArray(retObject));
            }
            catch (Exception ex)
            {
                data = new JObject();
                data.Add("status", mc.GetMessage("api_output_not_ok"));
                data.Add("message", ex.Message);
            }
            return data;
        }
    }
}
