using ClosedXML.Excel;
using ClosedXML.Extensions;
using MCHMIS.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf;
using Microsoft.AspNetCore.Http.Extensions;
using System.IO;

using System.Net;

using System.Web;
using Microsoft.AspNetCore.Hosting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using DinkToPdf.Contracts;

namespace MCHMIS.Services
{
    public interface IExportService
    {
        FileStreamResult ExportToExcel(IEnumerable<object> list, string fileName);

        Task ExportToPDFAsync(string url, string reportTitle, string paperKind = "A3");

        byte[] ExportToPDF(string path);

        string GetQueryString(object obj);
    }

    public class ExportService : IExportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly ILogService _logService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConverter _converter;

        public ExportService(ApplicationDbContext context, IHttpContextAccessor http,
            IHostingEnvironment hostingEnvironment,
            IConverter converter,
            ILogService logService)
        {
            _context = context;
            _logService = logService;
            _http = http;
            _converter = converter;
            _hostingEnvironment = hostingEnvironment;
        }

        public FileStreamResult ExportToExcel(IEnumerable<object> list, string fileName)
        {
            var wb = new XLWorkbook();
            // Add all DataTables in the DataSet as a worksheets
            var ds = new DataSet();
            var worksheetName = fileName;

            var ws = wb.Worksheets.Add("MCHMIS Export");
            fileName = fileName + ".xlsx";
            ws.Cell(1, 1).InsertTable(list);
            ws.Columns().AdjustToContents();
            var xlTable = ws.Tables.FirstOrDefault();
            if (xlTable != null) xlTable.ShowAutoFilter = false;
            return wb.Deliver(fileName);
        }

        public byte[] ExportToPDF(string path)
        {
            var test = _http.HttpContext.Request.GetDisplayUrl();
            var displayUrl = _http.HttpContext.Request.GetDisplayUrl();
            var rightPath = _http.HttpContext.Request.Path;
            var root = displayUrl.Replace(rightPath, "") + "/export/";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A3,
                    Margins = new MarginSettings() { Top = 10 },

                    // Out =path,
                },
                Objects = {
                    new ObjectSettings()
                    {
                        Page = root+"/"+path,
                        PagesCount = true,
                        FooterSettings = { FontSize = 6, Center = "Page [page] of [toPage]", Line = true, Spacing = 2.812},
                    },
                }
            };
            //   converter.Convert(doc);
            var file = _converter.Convert(doc);
            return file;
            //  return File(file, "application/pdf", "EmployeeReport.pdf");
        }

        private static Uri GetUri(HttpRequest request)
        {
            var builder = new UriBuilder();
            builder.Scheme = request.Scheme;
            builder.Host = request.Host.Value;
            builder.Path = request.Path;
            builder.Query = request.QueryString.ToUriComponent();
            return builder.Uri;
        }

        public async Task ExportToPDFAsync(string url, string reportTitle, string paperKind = "A3")
        {
            var displayUrl = _http.HttpContext.Request.GetDisplayUrl();
            var rightPath = _http.HttpContext.Request.Path;
            var root = displayUrl.Replace(rightPath, "") + "/export/";
            url = root + url;
            // url = url.Replace(" ", "-");
            string HTMLContent = "";
            using (var client = new WebClient())
            {
                HTMLContent = client.DownloadString(url);
            }
            var title = reportTitle + ".pdf";
            HTMLContent = HTMLContent.Replace("<br>", "<br />");

            var pdf = GetPDF(HTMLContent, paperKind);

            var response = _http.HttpContext.Response;
            _http.HttpContext.Response.Clear();
            // _http.HttpContext.Response.ClearContent();
            //  _http.HttpContext.Response.ClearHeaders();
            _http.HttpContext.Response.ContentType = "application/pdf";
            // HttpContext.Current.Response.AddHeader("Content-Disposition",string.Format("attachmentfilename=\"" + title + "\"; size={0}", pdf.Length));
            _http.HttpContext.Response.Headers["Content-Disposition"] = "attachment;filename=\"" + title + "\"";

            await response.Body.WriteAsync(pdf, 0, pdf.Length);

            response.Headers.Add("Content-Type", "application/pdf");
            response.Body.Flush();

            _http.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        }

        public byte[] GetPDF(string html, string paperKind)
        {
            byte[] bytesArray = null;
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A3, 30, 30, 30, 30);
                if (paperKind.Equals("A4"))
                {
                    document = new Document(PageSize.A4, 30, 30, 30, 30);
                }

                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                // writer.PageEvent = new ITextEvents();
                document.Open();
                using (var strReader = new StringReader(html))
                {
                    //HtmlWorker htmlWorker = new HtmlWorker(doc);
                    //htmlWorker.StartDocument();
                    //htmlWorker.Parse(txtReader);
                    //htmlWorker.EndDocument();
                    //htmlWorker.Close();
                }
                document.Close();

                bytesArray = ms.ToArray();
            }
            return bytesArray;

            //MemoryStream ms = new MemoryStream();
            //TextReader txtReader = new StringReader(html);
            //Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

            //doc.SetMargins(doc.LeftMargin, doc.RightMargin, 35, 0);

            //PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);

            //doc.Open();

            //HtmlWorker htmlWorker = new HtmlWorker(doc);
            //htmlWorker.StartDocument();
            //htmlWorker.Parse(txtReader);
            //htmlWorker.EndDocument();
            //htmlWorker.Close();
            //doc.Close();

            //bytesArray = ms.ToArray();
            //return bytesArray;
        }

        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }
}