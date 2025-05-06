using System.Drawing.Printing;
using System.Text;
 using MediaPlus.DBModels;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class ShowHTMLController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<ShowHtmlcode> _ShowHtmlTb;
        private readonly IRepository<ShowTemplate> _showtemplateTb;
        private readonly IRepository<ShowDetail> _showDetailsTb;
        private readonly IRepository<ShowMaterial> _MaterialTb;
        private readonly IRepository<TemplateDetail> _templateDetailsTb;
        private readonly IRepository<MaterialType> _MaterialTypeTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly IWebHostEnvironment _env;

        private readonly UserSessionModel _currentUser;
        private readonly CustomerSessionModel? _currentCustomer;

        public ShowHTMLController(IHttpContextAccessor accessor
                                ,IWebHostEnvironment env)                                                  
        {
            _env = env;
            _accessor = accessor;
            _ShowHtmlTb = _unitOfWork.GetRepositoryInstance<ShowHtmlcode>();
            _showtemplateTb = _unitOfWork.GetRepositoryInstance<ShowTemplate>();
            _showDetailsTb = _unitOfWork.GetRepositoryInstance<ShowDetail>();
            _MaterialTb = _unitOfWork.GetRepositoryInstance<ShowMaterial>();
            _templateDetailsTb = _unitOfWork.GetRepositoryInstance<TemplateDetail>();
            _MaterialTypeTb = _unitOfWork.GetRepositoryInstance<MaterialType>();
            _currentUser = _accessor.HttpContext!.Session.GetObject<UserSessionModel>("UserObject");
            _currentCustomer = _accessor.HttpContext?.Session.GetObject<CustomerSessionModel>("CustomerObject");
        }

       //=====================================================================
        // Show HTML Creation Part 

        [HttpGet]
        public IActionResult CreatePropagation()
        {
            var showSetting = TempData.GetObject<ShowSetting>("ShowSetting");
            var showDetails = TempData.GetObject<List<ShowDetail>>("ShowDetail");
            var shows = TempData.GetObject<List<Show>>("Show");

            // Here we create HTML code for each show

            for (var i = 0; i < shows?.Count; i++)
            {
                _ShowHtmlTb.Add(
                    new ShowHtmlcode()
                    {
                        ShowHtmlCode1 = GetHtmlCode(shows[i]),
                        ShowCdate = DateTime.Now,
                        ShowUdate = DateTime.Now,
                        ShowCustCode = _currentCustomer?.CustCode,
                        ShowByuserId = _currentUser?.UserId,
                        ShowIsactive = 1,
                        ShowUserid = shows[i].ShowByUserid,
                        ShowSettingId = showSetting!.ShowSettingId,
                        ShowCode = showSetting.ShowSettingShowcode,
                    }
                );
            }
            return RedirectToAction("Index", "Show");
        }

        //======================================================================================
        // Show Delete Part
        [HttpGet]
       public IActionResult Delete(string id) // Contents Show Code Id
       {
            _ShowHtmlTb.EntitiesIQueryable().Where(c => c.ShowCode == id)
                                            .ForEachAsync(s=>_ShowHtmlTb.Remove(s.ShowId));

            return RedirectToAction("Index", "Show");
       }

        //======================================================================================
        private string GetHtmlCode(Show show)
        {
            var showTemplate = _showtemplateTb.GetEntity((int)show.ShowTemplateId);
            int rows = (int)showTemplate.TempRowNo;
            int cols = (int)showTemplate.TempColNo;
            var show2DArray = new string[rows, cols];

            var showDetails = _showDetailsTb.EntitiesIQueryable()
                                            .Where(c => c.ShowDetailsShowid == show.ShowId)
                                            .ToList();

            foreach (var item in showDetails)
            {
                var material = _MaterialTb.GetEntity((int)item.ShowDetailsFileId);
                var materialType = _MaterialTypeTb.GetEntity((int)material.MatTypeId);
                var zone = _templateDetailsTb.GetEntity((int)item.ShowDetailsZoneId);

                var zoneCode = zone.TempZoneCode.ToString().Replace("٫", ".").Replace(",", ".").Split('.');
                int row = int.Parse(zoneCode[0].Trim()) - 1;
                int col = int.Parse(zoneCode[1].Trim()) - 1;

                string placeholderValue = material.MatPath.Split(",")[0];
                if (materialType.MtypeNameEn == "photo" || materialType.MtypeNameEn == "video")
                {
                    string domainName = "http://" + _accessor.HttpContext.Request.Host.Value;
                    placeholderValue = $"{domainName}/upload/show_material/{placeholderValue}";
                }

                bool isYouTubeLink = placeholderValue.Contains("youtube.com") || placeholderValue.Contains("youtu.be");

                string htmlCode = materialType.MtypeNameEn.ToLower() switch
                {
                    "photo" => $@"
<div class='cell'>
    <img src='{placeholderValue}' class='media-item'>
</div>",

                    "video" => $@"
<div class='cell'>
    <video autoplay loop muted playsinline class='media-item video-item'>
        <source src='{placeholderValue}' type='video/mp4'>
    </video>
</div>",

                    "link" when isYouTubeLink => $@"
<div class='cell'>
    <iframe src='{GetEmbedUrl(placeholderValue)}' class='media-item youtube-item' allowfullscreen></iframe>
</div>",

                    "link" => $@"
<div class='cell'>
    <a href='{placeholderValue}' target='_blank' class='link-item'>{material.MatShowNameEn}</a>
</div>",

                    "description" => $@"
<div class='cell'>
    <div class='scrolling-text'>{placeholderValue}</div>
</div>",

                    _ => $@"
<div class='cell'>
    {placeholderValue}
</div>"
                };

                show2DArray[row, col] = htmlCode;
            }

            var sbHTMLCode = new StringBuilder();
            sbHTMLCode.Append($@"
<!DOCTYPE html>
<html>
<head>
    <title>Media Plus Front End</title>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        html, body {{ width: 100vw; height: 100vh; overflow: hidden; }}
        .container {{
            width: 100vw;
            height: 100vh;
            display: flex;
            flex-direction: column;
        }}
        .row {{
            flex: 1;
            display: flex;
        }}
        .cell {{
            flex: 1;
            overflow: hidden;
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            background: white;
        }}
        .media-item {{
            width: 100%;
            height: 100%;
            object-fit: cover;
        }}
        .video-item {{
            object-fit: contain !important;
        }}
        .youtube-item {{
            width: 100%;
            height: 100%;
            border: none;
        }}
        .link-item {{
            padding: 1rem 2rem;
            background: #0066cc;
            color: white;
            text-decoration: none;
            border-radius: 8px;
            font-size: 1.2rem;
            width: 100%;
            height: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        @keyframes scroll {{
            0% {{ transform: translateX(100%); }}
            100% {{ transform: translateX(-100%); }}
        }}
        .scrolling-text {{
            animation: scroll 15s linear infinite;
            font-size: 1.5rem;
            color: #333;
            font-weight: bold;
            padding: 1rem;
            white-space: nowrap;
            position: absolute;
        }}
    </style>
</head>
<body>
<div class='container'>");

            for (var i = 0; i < rows; i++)
            {
                sbHTMLCode.Append("<div class='row'>");
                for (var j = 0; j < cols; j++)
                {
                    sbHTMLCode.Append(show2DArray[i, j] ?? "<div class='cell'></div>");
                }
                sbHTMLCode.Append("</div>");
            }

            sbHTMLCode.Append("</div></body></html>");
            return sbHTMLCode.ToString();
        }

        private string GetEmbedUrl(string url)
        {
            if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            {
                var videoId = System.Text.RegularExpressions.Regex.Match(
                    url, @"(?:youtu\.be\/|v=|\/embed\/|\/v\/|\/vi\/|\/user\/\S+\/\S+\/|watch\?v=)([\w-]+)")
                    .Groups[1].Value;
                return $"https://www.youtube.com/embed/{videoId}?autoplay=1&mute=1&controls=0&loop=1&playlist={videoId}&modestbranding=1&showinfo=0&rel=0&fs=0&disablekb=1&iv_load_policy=3";
            }

            return url;
        }

    }

}
