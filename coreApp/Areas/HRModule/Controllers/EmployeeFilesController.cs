using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Interfaces;
using coreLib.Objects;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using HtmlAgilityPack;
using coreLib.Extensions;
using Module.DB;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeFilesController : HLBase_NoCoreAuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateFolder(string xPath, string newFolderName)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                HtmlDocument doc = getItems();
                string path = getPath(doc, xPath);

                if (System.IO.File.Exists(path))
                {
                    path = new FileInfo(path).Directory.FullName;
                }

                path = Path.Combine(path, newFolderName);

                if (Directory.Exists(path))
                {
                    throw new Exception("Folder already exists");
                }

                Directory.CreateDirectory(path);

                res.Remarks = "Folder was successfully created";

                doc = getItems();
                HtmlNode n = doc.DocumentNode.Descendants("li").Where(x => path.EndsWith(x.Attributes["path"].Value)).Single();
                res.Data = n.Attributes["xpath"].Value;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        [HttpPost]
        public JsonResult DeleteFolder(string xPath)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                HtmlDocument doc = getItems();
                string path = getPath(doc, xPath);

                if (System.IO.File.Exists(path))
                {
                    path = new FileInfo(path).Directory.FullName;
                }

                if (!Directory.Exists(path))
                {
                    throw new Exception("Folder does not exists");
                }

                if (path == RootPath())
                {
                    throw new Exception("Cannot delete root folder");
                }

                DirectoryInfo di = new DirectoryInfo(path);
                if (IsFixedFolder(di.Name))
                {
                    throw new Exception("Cannot delete a fixed folder");
                }

                string folderPath = new FileInfo(path).Directory.FullName;

                Directory.Delete(path, true);

                res.Remarks = "Folder was successfully deleted";

                doc = getItems();
                HtmlNode n = doc.DocumentNode.Descendants("li").Where(x => folderPath.EndsWith(x.Attributes["path"].Value)).FirstOrDefault();
                if (n != null)
                {
                    res.Data = n.Attributes["xpath"].Value;
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        [HttpPost]
        public JsonResult RenameFolder(string xPath, string newFolderName)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                HtmlDocument doc = getItems();
                string path = getPath(doc, xPath);

                if (System.IO.File.Exists(path))
                {
                    path = new FileInfo(path).Directory.FullName;
                }

                if (!Directory.Exists(path))
                {
                    throw new Exception("Folder does not exists");
                }

                if (path == RootPath())
                {
                    throw new Exception("Cannot rename root folder");
                }

                DirectoryInfo di = new DirectoryInfo(path);
                string newPath = Path.Combine(di.Parent.FullName, newFolderName);

                if (IsFixedFolder(di.Name))
                {
                    throw new Exception("Cannot rename a fixed folder");
                }

                if (Directory.Exists(newPath))
                {
                    throw new Exception("A folder with the the new folder name already exists");
                }

                Directory.Move(path, newPath);

                res.Remarks = "Folder was successfully renamed";

                doc = getItems();
                HtmlNode n = doc.DocumentNode.Descendants("li").Where(x => newPath.EndsWith(x.Attributes["path"].Value)).Single();
                res.Data = n.Attributes["xpath"].Value;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        [HttpPost]
        public JsonResult DeleteFile(string xPath)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                HtmlDocument doc = getItems();
                string path = getPath(doc, xPath);
                
                if (!System.IO.File.Exists(path))
                {
                    throw new Exception("File does not exists");
                }

                string folderPath = new FileInfo(path).Directory.FullName;

                System.IO.File.Delete(path);

                res.Remarks = "File was successfully deleted";

                doc = getItems();
                HtmlNode n = doc.DocumentNode.Descendants("li").Where(x => folderPath.EndsWith(x.Attributes["path"].Value)).FirstOrDefault();
                if (n != null)
                {
                    res.Data = n.Attributes["xpath"].Value;
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        [HttpPost]
        public JsonResult RenameFile(string xPath, string newFileName)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                HtmlDocument doc = getItems();
                string path = getPath(doc, xPath);

                if (!System.IO.File.Exists(path))
                {
                    throw new Exception("Original file does not exists");
                }

                FileInfo fi = new FileInfo(path);
                string ext = fi.Extension;

                string newPath = Path.Combine(fi.Directory.FullName, newFileName + ext);

                if (System.IO.File.Exists(newPath))
                {
                    throw new Exception("A file with the new filename already exists");
                }

                System.IO.File.Copy(path, newPath);
                System.IO.File.Delete(path);

                res.Remarks = "File was successfully renamed";

                doc = getItems();
                HtmlNode n = doc.DocumentNode.Descendants("li").Where(x => newPath.EndsWith(x.Attributes["path"].Value)).Single();
                res.Data = n.Attributes["xpath"].Value;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        [HttpPost]
        public JsonResult MoveToFolder(string srcXPath, string destXPath)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                HtmlDocument doc = getItems();
                string src = getPath(doc, srcXPath);
                string dest = getPath(doc, destXPath);

                DirectoryInfo di = new DirectoryInfo(dest);

                bool isFile = System.IO.File.Exists(src);
                string newPath;

                if (isFile)
                {
                    FileInfo fi = new FileInfo(src);                    
                    if (fi.Directory.FullName == di.FullName)
                    {
                        throw new Exception("Cannot move to the same location");
                    }
                    newPath = Path.Combine(dest, fi.Name);
                    System.IO.File.Move(src, newPath);

                    res.Remarks = "File was successfully moved";
                }
                else
                {
                    DirectoryInfo diSrc = new DirectoryInfo(src);
                    if (diSrc.Parent.FullName == di.FullName)
                    {
                        throw new Exception("Cannot move to the same location");
                    }
                    newPath = Path.Combine(dest, diSrc.Name);
                    Directory.Move(src, newPath);

                    res.Remarks = "Folder was successfully moved";
                }

                doc = getItems();
                HtmlNode n = doc.DocumentNode.Descendants("li").Where(x => newPath.EndsWith(x.Attributes["path"].Value)).Single();
                res.Data = n.Attributes["xpath"].Value;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }


        public ActionResult PreviewFile(string xPath)
        {
            HtmlDocument doc = getItems();
            string path = getPath(doc, xPath);

            FileInfo fi = new FileInfo(path);

            if (!fi.Exists)
            {
                return new Raw_ActionResult("File does not exist");
            }

            string contentType = new MimeMappingStealer().GetMimeMapping(path);


            return File(path, contentType);
        }

        public ActionResult GetFiles()
        {            
            return Content(getItems().DocumentNode.OuterHtml);
        }

        private HtmlDocument getItems()
        {
            string path = RootPath();
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach(string p in FixedSettings.EmployeeFixedFolders.Split(','))
            {
                string _p = Path.Combine(path, p);
                if (!Directory.Exists(_p))
                {
                    Directory.CreateDirectory(_p);
                }
            }

            DirectoryInfo di = new DirectoryInfo(path);

            HtmlDocument dir = getHtml(di);
            bool hasChildren = dir.DocumentNode.HasChildNodes;

            string s = hasChildren ? dir.DocumentNode.OuterHtml : "";

            string rawHtml = string.Format("<ul><li class=\"folder" + (hasChildren ? " haschildren" : "") + "\" data-name=\"Root\"><div><i class=\"fa fa-folder\"></i>Root</div>{0}</li></ul>", s);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(rawHtml);

            doc.DocumentNode.Descendants("li").ToList().ForEach(x =>
            {
                x.Attributes.Add("xpath", x.XPath);
                x.Attributes.Add("path", getPath(x, true));
            });

            return doc;
        }

        private HtmlDocument getHtml(DirectoryInfo di)
        {
            HtmlDocument doc = new HtmlDocument();

            foreach (var directory in di.GetDirectories())
            {
                HtmlDocument dir = getHtml(directory);
                bool hasChildren = dir.DocumentNode.HasChildNodes;
                bool isFixedFolder = IsFixedFolder(directory.Name);
                
                var n = HtmlNode.CreateNode("<li></li>");
                var div = HtmlNode.CreateNode("<div></div>");
                
                n.AddClass("folder");
                
                if (isFixedFolder)
                {
                    n.AddClass("fixed-folder");
                }
                
                n.Attributes.Add("data-name", directory.Name);
                div.InnerHtml = directory.Name + (hasChildren ? " (" + dir.DocumentNode.Descendants("li").Where(x => x.HasClass("file")).Count() + ")" : "");
                div.PrependChild(icon(false, false));

                if (hasChildren)
                {
                    n.AddClass("haschildren");                    
                }

                div.PrependChild(iconCollapse());

                n.AppendChild(div);

                if (dir.DocumentNode.HasChildNodes)
                {
                    n.AppendChild(dir.DocumentNode);
                }
                
                getUL(ref doc).AppendChild(n);
            }

            foreach (var file in di.GetFiles())
            {
                string contentType = new MimeMappingStealer().GetMimeMapping(file.FullName);

                bool isImage = contentType == "image/jpeg";
                bool isPdf = contentType == "application/pdf";

                var n = HtmlNode.CreateNode("<li></li>");
                var div = HtmlNode.CreateNode("<div></div>");
                n.AddClass("file");                
                if (isImage) n.AddClass("is-image");
                if (isPdf) n.AddClass("is-pdf");
                
                n.Attributes.Add("data-name", file.Name);
                div.InnerHtml = file.Name;
                div.PrependChild(icon(true, isImage));
                div.PrependChild(iconCollapse());

                n.AppendChild(div);

                getUL(ref doc).AppendChild(n);                
            }
            
            return doc;
        }

        private bool IsFixedFolder(string folderName)
        {
            return FixedSettings.EmployeeFixedFolders.Split(',').Where(x => x.ToCleanString() == folderName.ToCleanString()).Any();
        }
        
        private HtmlNode getUL(ref HtmlDocument doc)
        {
            HtmlNode ul = null;
            if (doc.DocumentNode.HasChildNodes)
            {
                ul = doc.DocumentNode.FirstChild;
            }
            else
            {
                ul = HtmlNode.CreateNode("<ul></ul>");
                doc.DocumentNode.AppendChild(ul);
            }
            return ul;
        }

        private string getPath(HtmlDocument doc, string xpath, bool relativePath = false)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                return RootPath();
            }

            HtmlNode node = doc.DocumentNode.SelectNodes(xpath).FirstOrDefault();
            return getPath(node, relativePath);
        }

        private string getPath(HtmlNode node, bool relativePath = false)
        {
            bool isFile = node.HasClass("file");

            List<string> tmp = new List<string>();
            string v;

            if (!isFile)
            {
                v = node.Attributes["data-name"].Value.ToCleanString();
                if (v != "root") tmp.Add(v);
            }

            foreach (var n in node.Ancestors("li"))
            {
                v = n.Attributes["data-name"].Value.ToCleanString();
                if (v == "root") continue;

                tmp.Add(v);
            }

            List<string> tmp2 = new List<string>();
            for (int i = tmp.Count - 1; i >= 0; i--)
            {
                tmp2.Add(tmp[i]);
            }

            if (isFile)
            {
                v = node.Attributes["data-name"].Value.ToCleanString();
                tmp2.Add(v);
            }

            if (relativePath)
            {
                return "\\" + Path.Combine(tmp2.ToArray());
            }
            else
            {
                List<string> comp = new List<string> { Server.MapPath("~/EmployeeFiles"), employee.EmployeeId.ToString() };
                comp.AddRange(tmp2);

                return Path.Combine(comp.ToArray()).ToCleanString();
            }
        }
        

        public JsonResult UploadFiles()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                string xPath = Request.Form["xpath"];
                var file = Request.Files[0];

                string path = "";
                if (xPath == "null")
                {
                    path = RootPath();
                }
                else
                {
                    HtmlDocument doc = getItems();
                    path = getPath(doc, xPath);                    
                }

                if (System.IO.File.Exists(path))
                {
                    path = new FileInfo(path).Directory.FullName;
                }

                path = Path.Combine(path, file.FileName);

                file.SaveAs(path);
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        private string RootPath()
        {
            return Path.Combine(Server.MapPath("~/EmployeeFiles"), employee.EmployeeId.ToString()).ToCleanString();
        }

        private HtmlNode icon(bool isFile = true, bool isImage = true)
        {
            HtmlNode n = HtmlNode.CreateNode("<i></i>");
            n.AddClass("fa");
            if (isFile)
            {
                if (isImage)
                {
                    n.AddClass("fa-file-image-o");
                }
                else
                {
                    n.AddClass("fa-file-text-o");
                }                
            }
            else
            {
                n.AddClass("fa-folder");
            }
            return n;
        }

        private HtmlNode iconCollapse()
        {
            HtmlNode n = HtmlNode.CreateNode("<a href=\"#\" class=\"collapse-trigger\"></a>");
            HtmlNode plus = HtmlNode.CreateNode("<i></i>");
            HtmlNode minus = HtmlNode.CreateNode("<i></i>");

            plus.AddClass("fa");
            plus.AddClass("fa-plus-square-o");
            plus.AddClass("expand");

            minus.AddClass("fa");
            minus.AddClass("fa-minus-square-o");
            minus.AddClass("shrink");

            n.AppendChild(plus);
            n.AppendChild(minus);
            return n;
        }
    }
    
}