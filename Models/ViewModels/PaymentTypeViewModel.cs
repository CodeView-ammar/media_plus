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
        public int TypeId { get; set; } // type_id �� ����� ��������
        public string TypeName { get; set; } // type_name �� ����� ��������
        public string TypeShortName { get; set; } // type_short_name �� ����� ��������
        public bool TypeIsActive { get; set; } // type_isactive �� ����� ��������
        public string? TypeCustCode { get; set; } // type_cust_code �� ����� ��������
        public DateTime? TypeCreateAt { get; set; } // type_create_at �� ����� ��������
        public DateTime? TypeUpdateAt { get; set; } // type_update_at �� ����� ��������
        public int? TypeAddByUserId { get; set; } // type_add_byuserid �� ����� ��������

    }

}