﻿using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Configuration
    {
        public int ID { get; set; }
        public string? Websitename { get; set; }
        public string? EmailSupport { get; set; }
        public string? EmailContact { get; set; }
        public string? Hotline { get; set; }
        public string? Address { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? Instagram { get; set; }
        public string? Skype { get; set; }
        public string? TimeWork { get; set; }
        public double? Currency { get; set; }
        public double? MinPackage { get; set; }
        public double? MinOrder { get; set; }
        public string? InfoContent { get; set; }
        public string? WeightPrice { get; set; }
        public string? PercentOrder { get; set; }
        public string? AcitveAccountContent { get; set; }
        public double? Hoahongtongsale { get; set; }
        public double? QuanLyCurrencyDH { get; set; }
        public double? QuanLyDHPercent { get; set; }
        public double? QuanLyCurrencyND { get; set; }
        public bool? IsShowNoti { get; set; }
        public string? BankInfor { get; set; }
        public int? SearchBaiDuTimes { get; set; }
        public bool? IsMobileReview { get; set; }
        public string? VersionNo { get; set; }
        public string? UrlFAQ { get; set; }
    }
}
