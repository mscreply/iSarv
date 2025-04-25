using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iSarv.Pages.Shared.Components.FileManager
{
    public class FileManagerDialog : PageModel
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public bool AllowUploadFile = true;
        public bool AllowCreateFolder = true;
        public bool AllowDeleteFile = true;
        public bool AllowDeleteFolder = true;
        private string AllowedAllExtensions = "";
        private string AllowedFileExtensions = "doc,docx,pdf,xls,xlsx,txt,csv,html,psd,sql,log,fla,xml,ade,adp,ppt,pptx";
        private string AllowedImageExtensions = "jpg,jpeg,png,gif,bmp,tiff";
        private string AllowedArchiveExtensions = "zip,rar,7z";
        private string AllowedMediaExtensions = "mov,mpeg,mp4,avi,mpg,wma,mp3,m4a,ac3,aiff,mid";
        public string UploadPath = "App_Files\\uploads\\";
        public int MaxUploadSizeMb = 100;
        private string _rootPath = "";
        public string PopupCloseCode = "tinymce.activeEditor.windowManager.close()";

        public string CurrPath = "";
        public string Type = "0";
        public ArrayList ObjFItems = new ArrayList();
        public string AllowedFileExt = "";

        private int _colNum = 0;
        private string[] _folders = Array.Empty<string>();
        private string[] _files = Array.Empty<string>();

        public FileManagerDialog(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void OnGet()
        {
            _rootPath = Path.Combine(_hostEnvironment.WebRootPath, UploadPath);

            Type = Request.Query["type"] + "";
            CurrPath = Request.Query["currpath"] + "";

            //check inputs
            if (CurrPath.Length > 0)
            {
                CurrPath = CurrPath.TrimEnd('\\') + "\\";
            }

            //set the apply string, based on the passed type
            if (Type == "")
            {
                Type = "0";
            }
            switch (Type)
            {
                case "1":
                    AllowedFileExt = AllowedImageExtensions;
                    break;
                case "2":
                    AllowedFileExt = AllowedAllExtensions;
                    break;
                case "3":
                    AllowedFileExt = AllowedMediaExtensions;
                    break;
                default:
                    AllowedFileExt = AllowedAllExtensions;
                    break;
            }

            if (CurrPath != "")
            {
                // add "up one" folder
                var objFItem = new ClsFileItem
                {
                    Name = "..",
                    IsFolder = true,
                    IsFolderUp = true,
                    ColNum = getNextColNum(),
                    Path = getUpOneDir(CurrPath),
                    ClassType = "dir",
                    ThumbImage = "/lib/FileManager/img/ico/folder_return.png"
                };
                ObjFItems.Add(objFItem);
            }

            //load folders
            _folders = Directory.GetDirectories(Path.Combine(_rootPath, CurrPath));
            foreach (var folder in _folders)
            {
                var objFItem = new ClsFileItem
                {
                    Name = Path.GetFileName(folder),
                    IsFolder = true,
                    ColNum = getNextColNum(),
                    Path = CurrPath + Path.GetFileName(folder),
                    ClassType = "dir",
                    ThumbImage = "/lib/FileManager/img/ico/folder.png"
                };
                ObjFItems.Add(objFItem);
            }

            // load files
            _files = Directory.GetFiles(_rootPath + CurrPath);
            foreach (var file in _files)
            {
                var objFItem = new ClsFileItem
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    IsFolder = false,
                    Path = CurrPath + Path.GetFileName(file),
                };

                // check to see if it's the type of file we are looking at
                objFItem.ColNum = getNextColNum();
                // get display class type
                objFItem.ClassType = getObjClassType(Path.GetFileName(file));
                if (System.IO.File.Exists(_hostEnvironment.WebRootPath + "\\lib\\FileManager\\img\\ico\\" + Path.GetExtension(file).TrimStart('.').ToUpper() + ".png"))
                {
                    objFItem.ThumbImage = "/lib/FileManager/img/ico/" + Path.GetExtension(file).TrimStart('.').ToUpper() + ".png";
                }
                else
                {
                    objFItem.ThumbImage = "/lib/FileManager/img/ico/Default.png";
                }
                ObjFItems.Add(objFItem);
            } // foreach
            
        }

        public async Task<RedirectResult> OnPostAsync(IFormFile file, string currPath, string type)
        {
            var path = Path.Combine(_hostEnvironment.WebRootPath, UploadPath, currPath ?? "", file.FileName);
            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Redirect("FileManagerDialog?currpath=" + currPath + "&type=" + type);
        }

        public RedirectResult OnPostNewFolder(string folder, string currPath, string type)
        {
            var path = Path.Combine(_hostEnvironment.WebRootPath, UploadPath, currPath?? "", folder?? "");
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Redirect("FileManagerDialog?currpath=" + currPath + "&type=" + type);
        }
        
        public RedirectResult OnPostDelFolder(string folder, string currPath, string type)
        {
            var path = Path.Combine(_hostEnvironment.WebRootPath, UploadPath, folder ?? "");
            if(Directory.Exists(path))
                Directory.Delete(path, true);
            return Redirect("FileManagerDialog?currpath=" + currPath + "&type=" + type);
        }

        public RedirectResult OnPostDelFile(string file, string currPath, string type)
        {
            var path = Path.Combine(_hostEnvironment.WebRootPath, UploadPath, file ?? "");
            if(System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            return Redirect("FileManagerDialog?currpath=" + currPath + "&type=" + type);
        }

        public string getBreadCrumb()
        {
            string Ret;
            string[] folders;
            string tempPath = "";
            int intCount = 0;

            Ret = "<li><a href=\"FileManagerDialog?type=" + Type +  "&currpath=\"><i class=\"icon-home\"></i></a>";
            folders = CurrPath.Split('\\');

            foreach (string folder in folders) 
            {
                if (folder != "")
                {
                    tempPath += folder + "\\";
                    intCount++;

                    if (intCount == (folders.Length - 1))
                    {
                        Ret += " <span class=\"divider\">/</span></li> <li class=\"active\">" + folder + "</li>";
                    }
                    else
                    {
                        Ret += " <span class=\"divider\">/</span></li> <li><a href=\"FileManagerDialog?type=" + Type + "&currpath=" + tempPath + "\">" + folder + "</a>";
                    }
                }
            }   // foreach

            return Ret;
        }   // getBreadCrumb 
    
        private string getObjClassType(string? fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLower().TrimStart('.');
            if (AllowedFileExtensions.Split(",").Contains(ext)) return "1";
            if (AllowedImageExtensions.Split(",").Contains(ext)) return "2";
            if (AllowedArchiveExtensions.Split(",").Contains(ext)) return "3";
            if (AllowedMediaExtensions.Split(",").Contains(ext)) return "4";
            return "";
        }

        public string getEndOfLine(int intColNum) 
        {
            if (intColNum == 6)
            {
                return "</div><div class=\"space10\"></div>";
            }
            else
            {
                return "";
            }
        } // getEndOfLine

        public string getStartOfLine(int intColNum) 
        {
            if (intColNum == 1)
            {
                return "<div class=\"row-fluid\">";
            }
            else
            {
                return "";
            }
        } // getStartOfLine

        private int getNextColNum()
        {
            _colNum++;
            if (_colNum > 6)
            {
                _colNum = 1;
            }
            return _colNum;
        } // getNextColNum

        private string getUpOneDir(string strInput)
        {
            string[] arrTemp;

            arrTemp = strInput.TrimEnd('\\').Split('\\');
            arrTemp[arrTemp.Length - 1] = "";
            return String.Join("\\", arrTemp);
        }
        
    }

    public class ClsFileItem
    {
        public string Name = "";
        public int ColNum;       // list is 6 columns wide, which one is ?
        public bool IsFolder;
        public bool IsFolderUp;
        public string Path = "";
        public string ClassType = "";
        public string ThumbImage = "";
    }

}