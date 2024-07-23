using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using project.learning.Libs;
using System.Collections.Generic;
using System;

namespace project.learning.Controllers
{
    [Route("project.learning/[controller]")]
    public class v1Controller : Controller
    {
        private BaseController bc = new BaseController();
        private lMessage mc = new lMessage();
        private lConvert lc = new lConvert();

        [HttpGet("getDataPinjamanPerCycle")]
        public JObject getDataPinjamanPerCycle()
        {
            // db : lms
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
            // db : idc.en
            string spname = "workflow.get_data_pengajuan_per_cycle";
            var retObject = new List<dynamic>();
            var data = new JObject();
            try
            {
                data = new JObject();
                retObject = bc.getDataToObjectEN(spname);
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
    }
}
