using System;
using System.IO;
//using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using coreLib.Objects;
using System.Text;
using Aspose.Words.Drawing;
using Aspose.Words.Layout;
using Aspose.Cells;
using coreLibWeb;

namespace reports.AsposeLib
{

    public static class ExcelProcs
    {
        public static void AutoFitRows(ref Workbook workbook, int startIndex, int endIndex)
        {
            Worksheet worksheet = workbook.Worksheets[0];

            Style style = workbook.CreateStyle();
            style.IsTextWrapped = true;

            for (int r = startIndex; r <= endIndex; r++)
            {
                worksheet.Cells.Rows[r].ApplyStyle(style, new StyleFlag());
            }

            worksheet.AutoFitRows(startIndex, endIndex);
        }
    }

    public class asposeCellsReport2
    {
        private CustomizeWorkbookDelegate CustomizeWorkbookProc;

        public string tempFolder = HttpContext.Current.Server.MapPath("~/Temp");
        string templateFolder = HttpContext.Current.Server.MapPath("~/Templates");
        string sessionId = HttpContext.Current.Session.SessionID;
        public string customWorkingFilename = "";

        object data;
        Workbook workbook;

        string applicationName;
        string owner;

        bool toPDF = false;
        string footer;

        public asposeCellsReport2(CustomizeWorkbookDelegate CustomizeWorkbookProc = null, string applicationName = "", string owner = "", bool toPDF = false, string footer = "")
        {
            this.CustomizeWorkbookProc = CustomizeWorkbookProc;
            this.applicationName = applicationName;
            this.owner = owner;
            this.toPDF = toPDF;
            this.footer = footer;
        }

        public ActionResult DownloadFile(string dlFile, string fn)
        {
            FileStream fs = new FileStream(dlFile, FileMode.Open);
            byte[] arr = new byte[fs.Length];
            fs.Read(arr, 0, (int)fs.Length);
            fs.Close();

            File.Delete(dlFile);

            MemoryStream stream = new MemoryStream();
            stream.Write(arr, 0, arr.Length);
            stream.Position = 0;

            if (toPDF)
            {
                return new FileStreamResult(stream, "application/pdf");
            }
            else
            {
                return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        public ActionResult Get(string reportType, string fn, object data)
        {
            try
            {
                this.data = data;

                string mergedFile = mergeData(reportType);
                return DownloadFile(mergedFile, fn);
            }
            catch (Exception ex)
            {
                return new Raw_ActionResult(ex.Message);
            }
        }

        private string mergeData(string fn, string fileNameSuffix = "out")
        {
            string templatePath = Path.Combine(templateFolder, fn + ".xlsx");
            string workingCopyFileName = Path.Combine(tempFolder, tempFilename(Guid.NewGuid().ToString()));
            string outputFileName = Path.Combine(tempFolder, tempFilename(fileNameSuffix));

            File.Copy(templatePath, workingCopyFileName);

            FileStream stream = new FileStream(workingCopyFileName, FileMode.Open);
            workbook = new Workbook(stream);

            //report-specific procedures
            CustomizeWorkbookProc?.Invoke(data, ref workbook);

            if (!string.IsNullOrEmpty(footer))
            {
                //set footer
                Aspose.Cells.PageSetup pageSetup = workbook.Worksheets[0].PageSetup;
                pageSetup.SetFooter(0, footer);
            }

            Procs.SetWorkbookProperties(ref workbook, applicationName, owner);

            workbook.Save(outputFileName, toPDF ? SaveFormat.Pdf : SaveFormat.Xlsx);

            return outputFileName.ToString();
        }

        private string tempFilename(string suffix = "")
        {
            string suff = suffix == "" ? "" : "-" + suffix;
            string ext = "xlsx";
            return string.Format("{0}-{1}.{2}", string.IsNullOrEmpty(customWorkingFilename) ? sessionId : customWorkingFilename, suff, ext);
        }

       
    }
}