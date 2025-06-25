using System.Linq.Dynamic.Core;
using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class ShowController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<Show> _ShowTb;
        private readonly IRepository<ShowTemplate> _showTemplateTb;
        private readonly ILogger<ShowController> _logger;
        private readonly bool isEnglishCulture ;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly UserSessionModel? _currentUser;

        public ShowController(ILogger<ShowController> logger,
                                  IHttpContextAccessor accessor)
        {
            _ShowTb = _unitOfWork.GetRepositoryInstance<Show>();
            _showTemplateTb = _unitOfWork.GetRepositoryInstance<ShowTemplate>();
            _accessor = accessor;
            _currentUser = _accessor.HttpContext.Session.GetObject<UserSessionModel>("UserObject");
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            _logger = logger;
            isEnglishCulture = _accessor.HttpContext!.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";

        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var show = _ShowTb.EntitiesIQueryable().FirstOrDefault(s => s.ShowId == id);
            if (show == null)
            {
                return NotFound(); // Handle not found case
            }
            return View(show); // Assuming you have a view to edit the show
        }
        //====================================================================
        // Show Index
        [HttpGet]
        public IActionResult Index()
        {
            var ShowList = _ShowTb.EntitiesIQueryable()
                .Where(g => g.ShowCustCode == _currentCustomer!.CustCode )
                .ToList();
            return View(ShowList);
        }

        //=====================================================================
        // Show Creation Part 
        [HttpGet]
        public IActionResult Create(int NumberOfShows)
        {
            ViewBag.NumberOfShows = NumberOfShows;
            ArrangeDropdownlistData();
            return View();
        }


        [HttpPost]
        public IActionResult CreateConfirmation(ShowCreationViewModel ShowVm)
        {
            if (!ModelState.IsValid){
                ArrangeDropdownlistData();
                return View("Create"); 
            }
            TempData.SetObject("Show", ShowVm.show);

            return RedirectToAction("CreatePropagation","Show");
        }


        [HttpGet]
        public IActionResult CreatePropagation()
        {
            var shows = TempData.GetObject<List<Show>>("Show");
            var showSetting = TempData.GetObject<ShowSetting>("ShowSetting");
            var showOrder = 1;
            for(var i = 0; i < shows?.Count; i++)
            {
                shows[i].ShowSettingId = showSetting?.ShowSettingId;
                shows[i].ShowCode = Guid.NewGuid().ToString("n").Substring(0, 8);
                shows[i].ShowOrder = showOrder++;       
                shows[i].ShowIsactive = 1;
                shows[i].ShowCdate = DateTime.Now;
                shows[i].ShowUdate = DateTime.Now;
                shows[i].ShowByUserid = _currentUser?.UserId;
                shows[i].ShowCustCode = _currentCustomer?.CustCode;
        
                if (shows[i].ScheduledFrom.HasValue)
                    shows[i].IsScheduled = true;
                else
                    shows[i].IsScheduled = false;


                _ShowTb.Add(shows[i]);
            }

            TempData.UpdateObject("Show", shows);
            TempData.UpdateObject("ShowSetting", showSetting);

            return RedirectToAction("Create", "ShowDetail");
        }
        

        //======================================================================================
        // Show Toggle Part
        [HttpGet]
        [HttpGet]
        public IActionResult Toggle(int id)
        {
            var show = _ShowTb.EntitiesIQueryable().FirstOrDefault(c => c.ShowId == id);

            if (show == null)
                return NotFound("Show not found");

            var currentOrder = show.ShowOrder;

            if (show.ShowIsactive == 1)
            {
                show.ShowIsactive = 0;
                show.ShowUdate = DateTime.Now;
                show.ShowOrder = 0;
                _ShowTb.Update(show);

                _ShowTb.EntitiesIQueryable()
                    .Where(s => s.ShowOrder > currentOrder)
                    .ToList()
                    .ForEach(s =>
                    {
                        s.ShowOrder = s.ShowOrder - 1;
                        _ShowTb.Update(s);
                    });
            }
            else
            {
                show.ShowIsactive = 1;
                show.ShowUdate = DateTime.Now;
                show.ShowOrder = _ShowTb.EntitiesIQueryable()
                                       .Where(s => s.ShowCode == show.ShowCode)
                                       .Max(c => c.ShowOrder) + 1;

                _ShowTb.Update(show);
            }

            return Ok("success"); // ✅ هذا هو المطلوب
        }

        //======================================================================================
        // Show Delete Part
        [HttpGet]
        public IActionResult Delete(string showCode) // ShowCode ID
        {
            var shows= _ShowTb.EntitiesIQueryable()
                    .Where(c => c.ShowCode == showCode)
                    .ToList();
                    
            shows.ForEach(s=>_ShowTb.Remove(s.ShowId));

            return RedirectToAction("Delete", "ShowDetail",new { showCode = showCode });   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var show = _ShowTb.EntitiesIQueryable().FirstOrDefault(s => s.ShowId == id);
            if (show != null)
            {
                _ShowTb.Remove(show.ShowId);
            }
            return RedirectToAction("Index");
        }

        //======================================================================================
        private void ArrangeDropdownlistData()
        {
            var Templates = new List<SelectListItem>();
            if(isEnglishCulture){

                Templates.AddRange(_showTemplateTb.EntitiesIQueryable()
                                        .Where(r=>r.TempIsactive == 1
                                                && r.TempCustCode == _currentCustomer.CustCode)
                                        .Select(c=>
                                                new SelectListItem
                                                {
                                                Text = c.TempNameEng!.ToString(),
                                                Value = c.TempId.ToString(),
                                                }
                                        ).ToList());
            }else{
                Templates.AddRange(_showTemplateTb.EntitiesIQueryable()
                                        .Where(r=>r.TempIsactive == 1
                                             && r.TempCustCode == _currentCustomer.CustCode)
                                        .Select(c=>
                                                new SelectListItem
                                                {
                                                Text = c.TempNameAr!.ToString(),
                                                Value = c.TempId.ToString(),
                                                }
                                        ).ToList());

            }
                                          
            ViewBag.TemplateSelectedList = Templates;
        }
    }
}
