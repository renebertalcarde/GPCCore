using System;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using coreLib.Objects;
using System.Text;

namespace reports
{
    public delegate void IterateFieldsDelegate(Field field, string reportType, object data, ref Application wordApp);
    public delegate void IterateBookmarksDelegate(Bookmark bookmark, string reportType, object data, ref Document wordDoc);
    public delegate void CustomizeDocDelegate(object data, ref Application wordApp, ref Document wordDoc);
    public delegate string GetTemplateFilenameDelegate(string reportType);

    public class ReportHeaderParams
    {
        public string ReportLogoPath;
        public string ReportHeader;
        public string ReportFootNote;
        public float ReportLogo_Width = 50;
        public float ReportLogo_Height = 50;

        public ReportHeaderParams(string appPath)
        {
            ReportLogoPath = Path.Combine(appPath, "Templates", "config", "logo.png");
            ReportHeader = Common.getDocContents(Path.Combine(appPath, "Templates", "config", "header.docx"));
            ReportFootNote = Common.getDocContents(Path.Combine(appPath, "Templates", "config", "footnote.docx"));
        }
    }

    public class WordTemplateReport
    {
        private IterateFieldsDelegate IterateFieldsProc;
        private IterateBookmarksDelegate IterateBookmarksProc;
        private CustomizeDocDelegate CustomizeDocProc;
        private ReportHeaderParams ReportParams;

        public int dataId { get; set; }

        string tempFolder = HttpContext.Current.Server.MapPath("~/Temp");
        string templateFolder = HttpContext.Current.Server.MapPath("~/Templates");
        string sessionId = HttpContext.Current.Session.SessionID;

        string reportType = "";
        object data;

        bool dlWord = false;

        Application wordApp;
        Document wordDoc;

        public WordTemplateReport(IterateFieldsDelegate iterateFieldsProc = null, CustomizeDocDelegate customizeDocProc = null, ReportHeaderParams reportParams = null, IterateBookmarksDelegate iterateBookmarksProc = null)
        {
            IterateFieldsProc = iterateFieldsProc;
            IterateBookmarksProc = iterateBookmarksProc;
            CustomizeDocProc = customizeDocProc;
            ReportParams = reportParams;
        }

        public ActionResult Get (string reportType, string fn, object data, bool dlWord = false)
        {
            try
            {
                this.reportType = reportType;
                this.data = data;

                this.dlWord = dlWord;

                string workingFile = createTempWorkingFile();
                string mergedFile = mergeData(workingFile);

                FileStream stream = new FileStream(mergedFile, FileMode.Open);
                byte[] arr = new byte[stream.Length];
                stream.Read(arr, 0, (int)stream.Length);
                stream.Close();

                File.Delete(workingFile);
                File.Delete(mergedFile);

                MemoryStream pdfStream = new MemoryStream();
                pdfStream.Write(arr, 0, arr.Length);
                pdfStream.Position = 0;

                if (dlWord)
                {
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + createDLFilename(fn) + ".docx\"");

                    return new FileStreamResult(pdfStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                }
                else
                {
                    return new FileStreamResult(pdfStream, "application/pdf");
                }
            }
            catch(Exception ex)
            {
                return new Raw_ActionResult(ex.Message);
            }
        }

        private string createDLFilename(string fn)
        {
            string ret = "";

            string regex = @"[^\w\s\-\+]";

            string filename = Regex.Replace(fn, regex, "_");

            ret = string.Format("{0}-{1}-{2}",
                filename,
                reportType,
                DateTime.Now.ToString("yyMMddhhmmss")
                );

            return ret;
        }
      
        private string mergeData(string workingFile)
        {
            Object oMissing = System.Reflection.Missing.Value;
            Object oTemplatePath = workingFile;

            try
            {
                wordApp = new Application();
                wordDoc = new Document();
                wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);
            }
            catch(Exception ex)
            {
                closeDoc(ref wordApp, ref wordDoc);
                throw ex;
            }
            
            //set logo, header text and footnote in header and footer sections
            if (ReportParams != null)
            {
                setReportDoc(ref wordApp, ref wordDoc);
            }
            
            //update merge fields
            foreach (Field field in wordDoc.Fields)
            {
                if (IterateFieldsProc != null)
                {
                    IterateFieldsProc(field, reportType, data, ref wordApp);
                }                
            }

            //update bookmarks
            foreach (Bookmark bookmark in wordDoc.Bookmarks)
            {
                if (IterateBookmarksProc != null)
                {
                    IterateBookmarksProc(bookmark, reportType, data, ref wordDoc);
                }
            }

            //report-specific procedures
            if (CustomizeDocProc != null)
            {
                CustomizeDocProc(data, ref wordApp, ref wordDoc);
            }            

            object outputFileName = Path.Combine(tempFolder, tempFilename(!dlWord, "out"));

            if (dlWord)
            {
                wordDoc.SaveAs(ref outputFileName,
                            ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }
            else
            {
                object fileFormat = WdSaveFormat.wdFormatPDF;
                wordDoc.SaveAs(ref outputFileName,
                            ref fileFormat, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }

            closeDoc(ref wordApp, ref wordDoc);

            return outputFileName.ToString();
        }

        private void closeDoc(ref Application wordApp, ref Document wordDoc)
        {
            var doc_close = (_Document)wordDoc;
            doc_close.Close(false);
            var applicationclose = (_Application)wordApp;
            applicationclose.Quit();

            reports.Procs.releaseExcelObject(wordDoc);
            reports.Procs.releaseExcelObject(wordApp);
        }
        
        private void setReportDoc(ref Application wordApp, ref Document wordDoc)
        {
            if (wordDoc.Bookmarks.Exists("companyLogo"))
            {
                object oBookMark = "companyLogo";
                object oRange = wordDoc.Bookmarks.get_Item(ref oBookMark).Range;
                object saveWithDocument = true;
                object oMissing = System.Reflection.Missing.Value;
                
                InlineShape shape = wordDoc.InlineShapes.AddPicture(ReportParams.ReportLogoPath, oMissing, saveWithDocument, oRange);
                shape.Width = ReportParams.ReportLogo_Width;
                shape.Height = ReportParams.ReportLogo_Height;
            }

            foreach (Section wordSection in wordDoc.Sections)
            {
                Range range = wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;

                foreach (Field field in range.Fields)
                {
                    if (Common.getFieldName(field) == "companyheader")
                    {
                        field.Select();
                        wordApp.Selection.TypeText(ReportParams.ReportHeader);
                    }
                }

                range = wordSection.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;

                foreach (Field field in range.Fields)
                {
                    if (Common.getFieldName(field) == "footnote")
                    {
                        field.Select();
                        wordApp.Selection.TypeText(ReportParams.ReportFootNote);
                    }
                }
            }

            
        }

        private string createTempWorkingFile()
        {
            string sourceFile = Path.Combine(templateFolder, string.Format("{0}.docx", reportType));
            string destinationFile = Path.Combine(tempFolder, tempFilename(false));

            File.Copy(sourceFile, destinationFile, true);

            return destinationFile;
        }

        private string tempFilename(bool IsPdf, string suffix = "")
        {
            string suff = suffix == "" ? "" : "-" + suffix;
            string ext = IsPdf ? "pdf" : "docx";
            return string.Format("{0}-{1}{2}.{3}", sessionId, reportType, suff, ext);
        }

       
    }

    public static class Common
    {
        public static string getDocContents(string path)
        {
            string ret = "";

            if (File.Exists(path))
            {
                Object oMissing = System.Reflection.Missing.Value;
                Object oTemplatePath = path;

                Application wordApp = new Application();
                Document wordDoc = new Document();
                wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);

                StringBuilder sb = new StringBuilder();

                foreach (Paragraph p in wordDoc.Paragraphs)
                {
                    sb.Append(p.Range.Text);
                }

                ret = sb.ToString();

                var doc_close = (_Document)wordDoc;
                doc_close.Close(false);
                var applicationclose = (_Application)wordApp;
                applicationclose.Quit();
            }

            return ret;
        }

        public static void setField(object value, Field field, ref Application wordApp, string prefix = "", string suffix = "", bool doubleAsInt = false, string dataFormatString = "")
        {
            string s = "";

            field.Select();
            if (value == null)
            {
                field.Delete();
            }
            else if (value.GetType() == typeof(string))
            {
                string ss = value.ToString();

                if (ss == "")
                {
                    field.Delete();
                }
                else
                {
                    s = ss;
                }
            }
            else if (value.GetType() == typeof(int))
            {
                if (dataFormatString == "")
                {
                    s = ((int)value).ToString();
                }
                else
                {
                    s = ((int)value).ToString(dataFormatString);
                }
            }
            else if (value.GetType() == typeof(bool))
            {
                if ((bool)value)
                {
                    s = HttpContext.Current.Server.MapPath("~/Images/check.png");
                }
                else
                {
                    field.Delete();
                }
            }
            else if (value.GetType() == typeof(double))
            {
                if (dataFormatString == "")
                {
                    if (doubleAsInt)
                    {
                        s = ((double)value).ToString("#,##0");
                    }
                    else
                    {
                        s = ((double)value).ToString("#,##0.00");
                    }
                }
                else
                {
                    s = ((double)value).ToString(dataFormatString);
                }
            }
            else if (value.GetType() == typeof(DateTime))
            {
                if (dataFormatString == "")
                {
                    s = ((DateTime)value).ToString("d MMM yyyy");
                }
                else
                {
                    s = ((DateTime)value).ToString(dataFormatString);
                }
            }

            if (s != "")
            {
                if (prefix != "")
                {
                    s = prefix + s;
                }

                if (suffix != "")
                {
                    s = s + suffix;
                }

                wordApp.Selection.TypeText(s);
            }
        }

        public static string getFieldName(Field field)
        {
            string ret = "";

            try
            {
                Range rngFieldCode = field.Code;
                string fieldText = rngFieldCode.Text;
                if (fieldText.Contains("MERGEFIELD"))
                {
                    ret = fieldText.Replace("MERGEFIELD", "").Trim().ToLower();
                    int i = ret.IndexOf("\\");
                    if (i != -1)
                    {
                        ret = ret.Substring(0, i - 1).Trim();
                    }
                }
            }
            catch { }

            return ret;
        }
    }
}