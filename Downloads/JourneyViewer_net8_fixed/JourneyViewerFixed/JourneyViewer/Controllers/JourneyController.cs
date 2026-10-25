using JourneyViewer.Data;
using JourneyViewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace JourneyViewer.Controllers
{
    public class JourneyController : Controller
    {
        private readonly AppDbContext _context;

        public JourneyController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var flows = _context.Flows
                .Where(x => x.FlowType == 9)
                .Where(x => !string.IsNullOrEmpty(x.FlowUniqueName))
                .Select(x => x.FlowUniqueName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return View(flows);
        }


        // Call GETFLOWLAYOUT stored procedure
        private List<LayoutViewModel> GetFlowLayouts(string flowName)
        {
            var layouts = new List<LayoutViewModel>();

            var connectionString =
                _context.Database.GetConnectionString();

            using var connection =
                new OracleConnection(connectionString);

            using var command =
                new OracleCommand(
                    "GETFLOWLAYOUT",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;


            // Input parameter
            command.Parameters.Add(
                "p_FlowName",
                OracleDbType.NVarchar2).Value =
                flowName;


            // Output cursor
            command.Parameters.Add(
                "cv_1",
                OracleDbType.RefCursor).Direction =
                ParameterDirection.Output;


            connection.Open();


            using var reader =
                command.ExecuteReader();


            while (reader.Read())
            {
                layouts.Add(
                    new LayoutViewModel
                    {
                        LayoutId =
                            Convert.ToInt64(
                                reader["LAYOUTID"]),

                        LayoutName =
                            reader["LAYOUTNAME"] == DBNull.Value
                            ? null
                            : reader["LAYOUTNAME"].ToString(),

                        LayoutVersion =
                            reader["LAYOUTVERSION"] == DBNull.Value
                            ? null
                            : reader["LAYOUTVERSION"].ToString(),

                        UniqueLayoutName =
                            reader["UNIQUELAYOUTNAME"] == DBNull.Value
                            ? null
                            : reader["UNIQUELAYOUTNAME"].ToString(),

                        MasterId =
                            reader["MASTERID"] == DBNull.Value
                            ? null
                            : Convert.ToInt64(
                                reader["MASTERID"]),

                        MasterName =
                            reader["MASTERNAME"] == DBNull.Value
                            ? null
                            : reader["MASTERNAME"].ToString(),

                        LayoutJson =
                            reader["LAYOUTJSON"] == DBNull.Value
                            ? null
                            : reader["LAYOUTJSON"].ToString()
                    });
            }

            return layouts;
        }


        // Display layouts under selected flow
        public IActionResult LayoutDetails(string flowName)
        {
            if (string.IsNullOrWhiteSpace(flowName))
                return BadRequest();

            var layouts =
                GetFlowLayouts(flowName);

            ViewBag.FlowName =
                flowName;

            return View(layouts);
        }


        // Display EDS details
        public IActionResult EDSDetails(
     long layoutId,
     string flowName)
        {
            if (string.IsNullOrWhiteSpace(flowName))
                return BadRequest();

            // Get layouts from stored procedure
            var layouts = GetFlowLayouts(flowName);

            // Find the selected layout
            var layout = layouts.FirstOrDefault(
                x => x.LayoutId == layoutId);

            if (layout == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(layout.LayoutJson))
                return NotFound("Layout JSON is empty.");

            // Parse Layout JSON
            JObject json = JObject.Parse(layout.LayoutJson);

            List<EDSViewModel> edsList = new();

            // Find EDS nodes
            var edsNodes = json
                .Descendants()
                .Where(x =>
                    x.Type == JTokenType.Object &&
                    x["datasourceid"] != null
                );

            var crmMappings = _context.EDSCrmMappings
    .ToDictionary(x => x.EDSId);

            foreach (var node in edsNodes)
            {
                var edsId = Convert.ToInt64(
                    node["datasourceid"]?.ToString()
                );

                var eds = new EDSViewModel
                {
                    // Existing EDS details
                    EDSId = edsId,

                    EDSName = node["name"]?.ToString(),

                    DocumentName = node["documentName"]?.ToString(),

                    displayName = node["displayName"]?.ToString(),

                    type = node["type"]?.ToString()
                };

                // Get CRM mapping from DB
                if (crmMappings.TryGetValue(edsId, out var mapping))
                {
                    eds.CrmEDS = mapping.CrmEDS;
                    eds.CrmEDSName = mapping.CrmEDSName;
                }

                edsList.Add(eds);
            }

            return View(edsList);
        }
    }
}