using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;

namespace MediaPlus.Models.ViewModels
{
    public class PaymentTypeViewModel
    {
        [Key]
        public int TypeId { get; set; } // type_id İí ŞÇÚÏÉ ÇáÈíÇäÇÊ
        public string TypeName { get; set; } // type_name İí ŞÇÚÏÉ ÇáÈíÇäÇÊ
        public string TypeShortName { get; set; } // type_short_name İí ŞÇÚÏÉ ÇáÈíÇäÇÊ
        public bool TypeIsActive { get; set; } // type_isactive İí ŞÇÚÏÉ ÇáÈíÇäÇÊ
        public string? TypeCustCode { get; set; } // type_cust_code İí ŞÇÚÏÉ ÇáÈíÇäÇÊ
        public DateTime? TypeCreateAt { get; set; } // type_create_at İí ŞÇÚÏÉ ÇáÈíÇäÇÊ
        public DateTime? TypeUpdateAt { get; set; } // type_update_at İí ŞÇÚÏÉ ÇáÈíÇäÇÊ
        public int? TypeAddByUserId { get; set; } // type_add_byuserid İí ŞÇÚÏÉ ÇáÈíÇäÇÊ

    }

}