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
    public delegate void CustomizeWorkbookDelegate(object data, ref Workbook workbook);


    public class asposeCellsReport
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

        public asposeCellsReport(CustomizeWorkbookDelegate CustomizeWorkbookProc = null, string applicationName = "", string owner = "")
        {
            this.CustomizeWorkbookProc = CustomizeWorkbookProc;
            this.applicationName = applicationName;
            this.owner = owner;
        }

        public ActionResult DownloadFile(string dlFile, string fn)
        {
            FileStream stream = new FileStream(dlFile, FileMode.Open);
            byte[] arr = new byte[stream.Length];
            stream.Read(arr, 0, (int)stream.Length);
            stream.Close();

            File.Delete(dlFile);

            MemoryStream excelStream = new MemoryStream();
            excelStream.Write(arr, 0, arr.Length);
            excelStream.Position = 0;

            return new FileStreamResult(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public ActionResult Get(string fn, object data)
        {
            try
            {
                this.data = data;

                string mergedFile = mergeData();
                return DownloadFile(mergedFile, fn);
            }
            catch (Exception ex)
            {
                return new Raw_ActionResult(ex.Message);
            }
        }

        private string mergeData(string fileNameSuffix = "out")
        {
            workbook = new Workbook();

            //report-specific procedures
            CustomizeWorkbookProc?.Invoke(data, ref workbook);

            string outputFileName = Path.Combine(tempFolder, tempFilename(fileNameSuffix));

            Procs.SetWorkbookProperties(ref workbook, applicationName, owner);

            workbook.Save(outputFileName, SaveFormat.Xlsx);

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