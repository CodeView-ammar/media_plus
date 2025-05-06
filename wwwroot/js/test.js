
// async function GetNestedData(id){
//     var NestedData = "<div class=\"row\">";
//     NestedData += "<div class=\"form-group col\">";
//     NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//     NestedData += "<Label class=\"form-control text-center\" tabindex=\"6\">State</Label>";
//     NestedData += "</div>";
//     NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//     NestedData += "<Label class=\"form-control text-center\" tabindex=\"6\">Zone Height</Label>";
//     NestedData += "</div>";
//     NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//     NestedData += "<Label class=\"form-control text-center\" tabindex=\"6\">Zone Width</Label>";
//     NestedData += "</div>";
//     NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//     NestedData += "<Label class=\"form-control text-center\" tabindex=\"6\">Zone Name</Label>";
//     NestedData += "</div>";
//     NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//     NestedData += "<Label class=\"form-control text-center\" tabindex=\"6\">Zone Id</Label>";
//     NestedData += "</div>";
//     NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//     NestedData += "<Label class=\"form-control text-center\" tabindex=\"6\">#</Label>";
//     NestedData += "</div>";
//     NestedData += "</div>";
//     NestedData += "</div>";
//     NestedData += "<form role=\"form\" action=\"/ShowTemplate/UpdateTemplatesDetails\" method=\"post\">";

//     $.ajax({
//         url: '/ShowTemplate/GetTemplatesDetails/'+ id ,
//         type: 'GET',
//         cache: false,
//         async:false,
//         success: function (data) {
//             var i = 0;
//             data.forEach(x=>{
//                 NestedData += "<div class=\"row\">";
//                 NestedData += "<div class=\"form-group col\">";
//                 NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//                 NestedData += "<Label id=\"TempDetails_\""+i+"\"__TimeIsactive\" name=\"TempDetails[\""+i+"\"].TimeIsactive\" class=\"form-control text-center\" tabindex=\"6\" disabled >Active</Label>";
//                 NestedData += "</div>";
//                 NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//                 NestedData += "<input type=\"number\" id=\"TempDetails_\""+i+"\"__TempZoneHeight\" name=\"TempDetails[\""+i+"\"].TempZoneHeight\" class=\"form-control text-center\" value=\"" + x.tempZoneHeight + "\" tabindex=\"6\"/>";
//                 NestedData += "</div>";
//                 NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//                 NestedData += "<input type=\"number\" id=\"TempDetails_\""+i+"\"__TempZoneWidth\" name=\"TempDetails[\""+i+"\"].TempZoneWidth\" class=\"form-control text-center\" value=\"" + x.tempZoneWidth + "\" tabindex=\"6\"/>";
//                 NestedData += "</div>";
//                 NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//                 NestedData += "<Label class=\"form-control text-center\" tabindex=\"6\" disabled >Zone "+i+"</Label>";
//                 NestedData += "</div>";
//                 NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//                 NestedData += "<Label id=\"TempDetails_\""+i+"\"__TempZoneCode\" name=\"TempDetails[\""+i+"\"].TempZoneCode\" class=\"form-control text-center\" tabindex=\"6\" disabled >"+ x.tempZoneCode +"</Label>";
//                 NestedData += "</div>";
//                 NestedData += "<div class=\"col-xs-4 col-sm-2 col-md-2\">"
//                 NestedData += "<Label class=\"form-control id=\"TempDetails_\""+i+"\"__TempDetail\" name=\"TempDetails[\""+i+"\"].TempDetail\" text-center\" tabindex=\"6\">"+i+"</Label>";
//                 NestedData += "</div>";
//                 NestedData += "</div>";
//                 NestedData += "</div>";
//                 i++;
//         });
//         }
//     })
//     NestedData += "<button type=\"submit\" class=\"btn btn-success\" style=\"float:left;\">Update</button>";
//     NestedData += "</form> ";
//     return NestedData;
// }
