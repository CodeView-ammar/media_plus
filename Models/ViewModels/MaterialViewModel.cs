using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;
using Microsoft.AspNetCore.Mvc;
 
namespace MediaPlus.Models.ViewModels
{
    public class MaterialViewModel
    {
    public int MatId { get; set; }

    [MaxLength(10,ErrorMessage = "MaxLength of Material Name is 10" )]
    [Required(ErrorMessage = "Material Name is required")]
    [DuplicationCheckMaterialAr]
    [RegularExpression(@"^[ء-ي\s]+$", ErrorMessage = "Only Arabic characters are allowed.")]
    public string? MatShowNameAr { get; set; }

    [MaxLength(10,ErrorMessage = "MaxLength of Material Name is 10" )]
    [Required(ErrorMessage = "Material Name is required")]
    [DuplicationCheckMaterialEn]
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only English characters are allowed.")]
    public string? MatShowNameEn { get; set; }

    public string? MatFilePath { get; set; }
    
    [TakeOneInputFile]
    public MatFileViewModel? MatFile { get; set; }

    [Required(ErrorMessage ="Material Type is required")]
    public int MatTypeId { get; set; }
    public string? MatTypeName {get; set;}

    public DateTime? MatCdate { get; set; }
    public DateTime? MatUdate { get; set; }

    public int? MatByuserId { get; set; }
    
    public string? MatCustCode { get; set; }
    public string? CustomerName {get; set;}

    public bool MatIsactive {get; set;}
    }
}