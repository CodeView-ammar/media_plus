namespace MediaPlus.Services
{
    public class CustomerSessionModel
    {
        public int CustId { get; set; }
         public string CustNameAr { get; set; }

        public string CustNameEn { get; set; }

        public string CustVat { get; set; }

        public string CustTrNo { get; set; }

        public string CustTel { get; set; }

        public string CustEmail { get; set; }

        public string CustMobileNo { get; set; }

        public DateTime CustCdate { get; set; }

        public DateTime CustUdate { get; set; }

        public string CustCode { get; set; }

        public int? CustLicenseCode { get; set; }

        public string CustLogo { get; set; }

        public int CustIsactive { get; set; }
    }
}