
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

 
namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class ShowDetailController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<ShowDetail> _ShowDetailTb;
        private readonly IRepository<ShowTemplate> _showTemplateTb;
        private readonly IRepository<ShowMaterial> _showMaterialTb;
        private readonly IRepository<TemplateDetail> _showTemplateDetailTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserSessionModel _currentUser;
        private readonly CustomerSessionModel? _currentCustomer;

        public ShowDetailController(IHttpContextAccessor accessor)
        {
            
            _showMaterialTb = _unitOfWork.GetRepositoryInstance<ShowMaterial>();
            _ShowDetailTb = _unitOfWork.GetRepositoryInstance<ShowDetail>();
            _showTemplateDetailTb = _unitOfWork.GetRepositoryInstance<TemplateDetail>();
            _showTemplateTb = _unitOfWork.GetRepositoryInstance<ShowTemplate>();
            _accessor = accessor;
            _currentUser = _accessor.HttpContext!.Session.GetObject<UserSessionModel>("UserObject");
            _currentCustomer = _accessor.HttpContext?.Session.GetObject<CustomerSessionModel>("CustomerObject");
        }


        
        //=====================================================================
        // Show Creation Part 
        [HttpGet]
        public IActionResult Create(ShowCreationViewModel? showVm = null)
        {
            if(showVm.showDetail == null){
                TempData.SetObject("currentShow",0);
                TempData.ClearObject("ShowDetail");
            }
            

            var shows = TempData.GetObject<List<Show>>("Show");
            var currentShowIndex = TempData.GetObject<int>("currentShow");

            if (showVm.showDetail != null){
                //fetch data
                var oldShowDetails = TempData.GetObject<List<ShowDetail>>("ShowDetail") ?? new List<ShowDetail>{};
                oldShowDetails.AddRange(showVm.showDetail);
                //put data
                TempData.UpdateObject("ShowDetail",oldShowDetails);
            }

            if(shows.Count > currentShowIndex){

                var tempId = shows[currentShowIndex].ShowTemplateId;
                var showTemplate = _showTemplateTb.GetEntity((int)tempId);
                ViewBag.NumberColumn = showTemplate.TempColNo;
                ViewBag.NumberRow = showTemplate.TempRowNo;
                ViewBag.CurrentShowId =  shows[currentShowIndex].ShowId;
                ViewBag.CurrentShowIndex = currentShowIndex;
                if (shows[currentShowIndex].ScheduledFrom.HasValue)
                {
                    ViewBag.IsScheduled = true;
                }
                else
                {
                    ViewBag.IsScheduled = false; // Optional: explicitly set to false if needed
                }
                ViewBag.ScheduledFrom = shows[currentShowIndex].ScheduledFrom;
                ViewBag.ScheduledTo = shows[currentShowIndex].ScheduledTo;

                ViewBag.ScheduledRunNo = 0;
                ArrangeDropdownlistData(showTemplate.TempId
                                        , (int)showTemplate.TempRowNo
                                        , (int)showTemplate.TempColNo);

                TempData.SetObject("Show",shows);
                TempData.UpdateObject("currentShow",currentShowIndex + 1);
                ModelState.Clear();
                return View();
            }

            TempData.SetObject("Show",shows);

            return RedirectToAction("CreatePropagation","ShowDetail");
        }

        //=====================================================================
        // Continue Creation Part 

        [HttpGet]
        public IActionResult CreatePropagation()
        {
            var showSetting = TempData.GetObject<ShowSetting>("ShowSetting");
            var showDetails = TempData.GetObject<List<ShowDetail>>("ShowDetail");
            var shows = TempData.GetObject<List<Show>>("Show");
            for(int i = 0; i < showDetails?.Count; i++){
                showDetails[i].ShowDetailsShowid = showDetails[i].ShowDetailsShowid;
                showDetails[i].ShowDetailsFileId = showDetails[i].ShowDetailsFileId;
                showDetails[i].ShowDetailsZoneId = showDetails[i].ShowDetailsZoneId; 
                showDetails[i].ShowDetailsShowcode = showSetting?.ShowSettingShowcode;
                showDetails[i].ShowDetailsCustCode = _currentCustomer?.CustCode;
                showDetails[i].ShowDetailsIsactive = 1;
                showDetails[i].ShowDetailsCdate = DateTime.Now;
                showDetails[i].ShowDetailsUdate = DateTime.Now;
                _ShowDetailTb.Add(showDetails[i]);
            };

            TempData.UpdateObject("Show", shows);
            TempData.UpdateObject("ShowDetail",showDetails);
            TempData.UpdateObject("ShowSetting", showSetting);
            
            return RedirectToAction("CreatePropagation", "ShowHTML");
        }
        //======================================================================================
        // Show Delete Part
        [HttpGet]
       public IActionResult Delete(string showCode) //  Show Code
       {
            _ShowDetailTb.EntitiesIQueryable()
                         .Where(c => c.ShowDetailsShowcode == showCode)
                         .ToList()    
                         .ForEach(s=>_ShowDetailTb.Remove(s.ShowDetailsId));

        
            return RedirectToAction("Delete", "ShowHTML",new { showCode = showCode });   
       }
       
        //======================================================================================
        private void ArrangeDropdownlistData(int templateID, int rowNum, int colNum)
        {
            var templateDetails = new List<TemplateDetail>(); 
            var materials = new List<SelectListItem>();
           
            templateDetails.AddRange(_showTemplateDetailTb.EntitiesIQueryable()
                                        .Where(r=>r.TempIsactive == 1
                                                && r.TempCustCode == _currentCustomer.CustCode
                                                && r.TempTempId == templateID)
                                        .ToList());
            
            materials.AddRange(_showMaterialTb.EntitiesIQueryable()
                                        .Where(r=>r.MatIsactive == 1
                                                && r.MatCustCode == _currentCustomer.CustCode)
                                        .Select(c=>
                                                new SelectListItem
                                                {
                                                    Text = c.MatShowNameEn!.ToString(),
                                                    Value = c.MatId.ToString(),
                                                }
                                        ).ToList());


            int index = 0;
            var twoDimensionalZones = new Zone[rowNum, colNum];

            if (templateDetails.Count < rowNum * colNum)
            {
                throw new Exception("templateDetails does not contain enough elements to fill the 2D array.");
            }

            for (int x = 0; x < rowNum; x++)
            {
                for (int y = 0; y < colNum; y++)
                {
                    //Set the values of 2 dimensional array
                    var detail = templateDetails[index];

                    twoDimensionalZones[x, y] = new Zone()
                    {
                        ZoneId = detail.TempDetail.ToString(),
                        ZoneName = "Zone " + detail.TempZoneCode.ToString()[..3], // same as [0..3]
                        ZoneHeight = detail.TempZoneHeight.Value,
                        ZoneWidth = detail.TempZoneWidth.Value,
                    };

                    index++;
                }
            }

            ViewData["Zones"] =  twoDimensionalZones;
            ViewBag.MaterialSelectedList = materials;
        }
    }
}
