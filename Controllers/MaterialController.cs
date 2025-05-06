 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Web;

namespace MediaPlus.Controllers
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
[AuthorizeCustFilter]
    public class MaterialController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly bool isEnglishCulture ;
        private readonly IRepository<ShowMaterial> _MaterialTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<MaterialType> _materialtypeTb;
        private readonly ILogger<MaterialController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;


        public MaterialController(ILogger<MaterialController> logger,
                                  IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor)
        {
            _MaterialTb = _unitOfWork.GetRepositoryInstance<ShowMaterial>();
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _materialtypeTb =  _unitOfWork.GetRepositoryInstance<MaterialType>();
            _logger = logger;
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            env = _env;
        }

        [HttpGet]
        public IActionResult GetMaterialPath(int id)
        {
            var material = _MaterialTb.EntitiesIQueryable()
                .Where(g => g.MatId == id)
                .Select(x => new MaterialViewModel
                {
                    MatFilePath = x.MatPath
                })
                .FirstOrDefault(); // ÊäÝíÐ ÇáÇÓÊÚáÇã æÌáÈ ÚäÕÑ æÇÍÏ

            if (material != null)
            {
                return Json(new { path = material.MatFilePath });
            }

            return Json(new { path = "" });
        }


        [HttpGet]
        public IActionResult Index()
        {

            var MaterialData = 
                _MaterialTb.EntitiesIQueryable()
                            .Where(g=>g.MatCustCode == _currentCustomer.CustCode)
                            .Select(x =>
                                    new MaterialViewModel
                                    {
                                        MatId = x.MatId,
                                        MatShowNameAr = isEnglishCulture ? "null" : x.MatShowNameAr,
                                        MatShowNameEn = isEnglishCulture?  x.MatShowNameEn : "null",
                                        MatCdate = x.MatCdate,
                                        MatUdate = x.MatUdate,
                                        MatFilePath = x.MatPath,
                                        MatIsactive = x.MatIsactive==1,
                                        CustomerName = isEnglishCulture ? _customerTb.EntitiesIQueryable()
                                                                            .FirstOrDefault(c => c.CustCode == x.MatCustCode).CustNameEn
                                                                        : _customerTb.EntitiesIQueryable()
                                                                            .FirstOrDefault(c => c.CustCode == x.MatCustCode).CustNameAr,
                                        MatTypeName = isEnglishCulture ? _materialtypeTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(m => m.MtypeId == x.MatTypeId).MtypeNameEn
                                                                    : _materialtypeTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(m => m.MtypeId == x.MatTypeId).MtypeNameEn
                                            }); 

       

            return View(MaterialData);
        }

        //=====================================================================
        // Material Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            ArrangeDropdownlistData(_accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(MaterialViewModel MaterialVM)
        {   
            var currentCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode;
            if (!ModelState.IsValid){
                ArrangeDropdownlistData(currentCustCode);
                return View("Create");
            }

            var material = new ShowMaterial();

            if(MaterialVM.MatFile.attachedMaterialFile != null)
            {
                // Upload the valid photo to spacific folder
                var newGuid = Guid.NewGuid().ToString().Substring(0,18);
                var extension = Path.GetExtension(MaterialVM.MatFile.attachedMaterialFile.FileName);
                var filename = newGuid + extension;
                var path = Path.Combine(env.WebRootPath, "upload/show_material", filename);

                using (var fileStream = System.IO.File.Create(path))
                {
                    await MaterialVM.MatFile.attachedMaterialFile.CopyToAsync(fileStream);
                }
                material.MatPath = filename + "," + MaterialVM.MatFile.attachedMaterialFile.ContentType;
                
            }else{
                material.MatPath = MaterialVM.MatFile.attachedText+" ,txt";
            }


            material.MatId = MaterialVM.MatId;
            material.MatShowNameAr = MaterialVM.MatShowNameAr;
            material.MatShowNameEn = MaterialVM.MatShowNameEn;
            material.MatCdate = DateTime.Now;
            material.MatUdate = DateTime.Now;
            material.MatIsactive = 1;
            material.MatByuserId = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId; // Get from session
            material.MatCustCode = currentCustCode;
            material.MatTypeId = MaterialVM.MatTypeId;

            _MaterialTb.Add(material);

            return RedirectToAction("Index", "Material");
        
        }


        //===================================================================== 
        // ShowMaterial Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ArrangeDropdownlistData(_accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode);
            
            var ShowMaterial = _MaterialTb.EntitiesIQueryable()
                                          .Where(c => c.MatId == id )
                                          .Select(x=>
                                        new MaterialViewModel{
                                            MatId = x.MatId,
                                            MatShowNameAr = x.MatShowNameAr,
                                            MatShowNameEn = x.MatShowNameEn,
                                            MatCdate = x.MatCdate,
                                            MatUdate = x.MatUdate,
                                            MatCustCode = x.MatCustCode,
                                            MatByuserId = x.MatByuserId,
                                            MatTypeId = x.MatTypeId,
                                            MatIsactive = x.MatIsactive == 1,
                                            MatFile = new MatFileViewModel
                                            {
                                                attachedText =  x.MatPath,
                                                attachedMaterialFile = null  
                                            }
                                        }
                                      ).FirstOrDefault();

            if (ShowMaterial == null)
            {
                return NotFound();
            }

            return View(ShowMaterial);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(MaterialViewModel MaterialVM){

            if (!ModelState.IsValid){
                ArrangeDropdownlistData(MaterialVM.MatCustCode);
                return View("Edit");
            } 

            var targetMaterial = _MaterialTb.GetEntity(MaterialVM.MatId);
           
            // if he choose to add discription earse photo and add discription else update photo
            if(MaterialVM.MatFile.attachedMaterialFile != null)
            {
                //delete previous photo if exist 
                if(targetMaterial.MatPath.Split("/")[1] != "txt"){
                    System.IO.File.Delete(Path.Combine(env.WebRootPath, "upload/show_material", targetMaterial.MatPath.Split(",")[0]));
                }

                // Upload the valid photo to spacific folder
                var newGuid = Guid.NewGuid().ToString().Substring(0,18);
                var extension = Path.GetExtension(MaterialVM.MatFile.attachedMaterialFile.FileName);
                var filename = newGuid + extension;
                var path = Path.Combine(env.WebRootPath, "upload/show_material", filename);

                using (var fileStream = System.IO.File.Create(path))
                {
                    await MaterialVM.MatFile.attachedMaterialFile.CopyToAsync(fileStream);
                }
                targetMaterial.MatPath = filename + "," + MaterialVM.MatFile.attachedMaterialFile.ContentType;

            }else if(MaterialVM.MatFile.attachedText != null){

                if(!targetMaterial.MatPath.Contains("txt")){
                    System.IO.File.Delete(Path.Combine(env.WebRootPath, "upload/show_material", targetMaterial.MatPath.Split(",")[0]));
                }

                targetMaterial.MatPath = MaterialVM.MatFile.attachedText+"/txt";
            }

            targetMaterial.MatShowNameAr = MaterialVM.MatShowNameAr;
            targetMaterial.MatShowNameEn = MaterialVM.MatShowNameEn;
            targetMaterial.MatUdate = DateTime.Now;
            targetMaterial.MatCdate = MaterialVM.MatCdate.Value;
            targetMaterial.MatIsactive = MaterialVM.MatIsactive ? 1 : 0;
            targetMaterial.MatTypeId = MaterialVM.MatTypeId;
            targetMaterial.MatByuserId = MaterialVM.MatByuserId.Value;
            targetMaterial.MatCustCode = MaterialVM.MatCustCode;
                
             
               
            _MaterialTb.Update(targetMaterial);

            return RedirectToAction("Index", "Material");
        }

       
       //======================================================================
       // Material Delete Part
       [HttpPost]
       public IActionResult Delete(int id)
       {
            // Delete attached file if exist
            var selectedFile = _MaterialTb.GetEntity(id);

            if(selectedFile.MatPath.Split("/")[1] != "txt"){
                var path = Path.Combine(env.WebRootPath, "upload/show_material", selectedFile.MatPath.Split(",")[0]);
                System.IO.File.Delete(path);
            }

            _MaterialTb.Remove(id);
            
            return RedirectToAction("Index", "Material");
       }

        //======================================================================
        // ShowMaterial Download Photo Part
        [HttpGet]
        public IActionResult Download(int Id)
        {
            var selectedFile = _MaterialTb.GetEntity(Id);

            if (selectedFile == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(selectedFile.MatPath))
            {
                var pathParts = selectedFile.MatPath.Split("/");
                var fileParts = selectedFile.MatPath.Split(",");

                // Check if the file is not a .txt file
                if (fileParts.Length > 1 && fileParts[1] != "txt")
                {
                    var path = "~/upload/show_material/" + fileParts[0];
                    var contentType = fileParts[1];
                    var fileName = fileParts[0];

                    Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
                    Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(-3).ToString("R"));
                    Response.Headers.Add("Cache-Control", "no-cache");
                    return File(path, contentType, fileName);
                }
                else
                {
                    // Handle the case where the file is a .txt file
                    if (pathParts.Length > 0)
                    {
                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(pathParts[0]));
                        return File(stream, "text/plain", "default.txt");
                    }
                    else
                    {
                        // Handle the error case where MatPath format is invalid
                        return BadRequest("Invalid file path.");
                    }
                }
            }
            else
            {
                // Handle the case where MatPath is null or empty
                return BadRequest("File path cannot be null or empty.");
            }
        }


        // =====================================================================
       // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _MaterialTb.GetEntity(Id);

            targetElement.MatIsactive = targetElement.MatIsactive == 1 ? 0 : 1;

            _MaterialTb.Update(targetElement);

            return Ok("success");
       }
  
        private void ArrangeDropdownlistData(string currentCustCode)
        {
            var types = new List<SelectListItem>();
 
            if (!isEnglishCulture) {
                types.AddRange(_materialtypeTb.EntitiesIQueryable()
                                                .Where(m => m.MtypeCustCode == currentCustCode)
                                                .Select(c=>
                                                    new SelectListItem
                                                    {
                                                        Text = c.MtypeNameEn,
                                                        Value = c.MtypeId.ToString(),
                                                    }
                                                ).ToList());
            }else{
                types.AddRange(_materialtypeTb.EntitiesIQueryable()
                                                .Where(m => m.MtypeCustCode == currentCustCode)
                                                .Select(c=>
                                                    new SelectListItem
                                                    {
                                                        Text = c.MtypeNameAr,
                                                        Value = c.MtypeId.ToString()
                                                    }
                                                ).ToList());
            }
                                                
            ViewBag.TypeSelectedList = types;
        }
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.