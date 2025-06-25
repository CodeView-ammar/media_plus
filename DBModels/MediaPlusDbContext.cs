using System;
using System.Collections.Generic;
using MediaPlus.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MediaPlus.DBModels;

public partial class MediaPlusDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public MediaPlusDbContext()
    {
    }

    public MediaPlusDbContext(DbContextOptions<MediaPlusDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;  
    }

    public virtual DbSet<AdDevice> AdDevices { get; set; }

    public virtual DbSet<AdGroup> AdGroups { get; set; }

    public virtual DbSet<CustomShow> CustomShows { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<License> Licenses { get; set; }

    public virtual DbSet<MaterialType> MaterialTypes { get; set; }
    public virtual DbSet<PaymentType> PaymentType { get; set; }
    public virtual DbSet<RoleWithPermission> RoleWithPermissions { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<ShowContent> ShowContents { get; set; }

    public virtual DbSet<ShowDetail> ShowDetails { get; set; }

    public virtual DbSet<ShowHtmlcode> ShowHtmlcodes { get; set; }

    public virtual DbSet<ShowMaterial> ShowMaterials { get; set; }

    public virtual DbSet<ShowSetting> ShowSettings { get; set; }

    public virtual DbSet<ShowTemplate> ShowTemplates { get; set; }

    public virtual DbSet<ShowType> ShowTypes { get; set; }

    public virtual DbSet<TemplateDetail> TemplateDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connString = "Data Source=codeview\\ammar;Initial Catalog=media_plu;Integrated Security=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connString);

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdDevice>(entity =>
        {
            entity.HasKey(e => e.DevicesId).HasName("PK_Devices");

            entity.ToTable("Ad_Devices");

            entity.Property(e => e.DevicesId).HasColumnName("Devices_id");
            entity.Property(e => e.DeviceIsInterrupt).HasColumnName("device_is_interrupt");
            entity.Property(e => e.DevicesByUserid).HasColumnName("Devices_by_userid");
            entity.Property(e => e.DevicesCdate)
                .HasColumnType("datetime")
                .HasColumnName("Devices_cdate");
            entity.Property(e => e.DevicesCustCode)
                .HasMaxLength(50)
                .HasColumnName("Devices_cust_code");
            entity.Property(e => e.DevicesGroupid).HasColumnName("Devices_groupid");
            entity.Property(e => e.DevicesName)
                .HasMaxLength(50)
                .HasColumnName("Devices_name");
            entity.Property(e => e.DevicesOfflinePhoto)
                .HasMaxLength(50)
                .HasColumnName("Devices_offline_photo");
            entity.Property(e => e.DevicesOnoff).HasColumnName("Devices_onoff");
            entity.Property(e => e.DevicesUdate)
                .HasColumnType("datetime")
                .HasColumnName("Devices_udate");
        });
        modelBuilder.Entity<PaymentType>()
        .ToTable("Payment_Type")
        .Property(p => p.TypeId).HasColumnName("type_id");
        modelBuilder.Entity<PaymentType>()
            .Property(p => p.TypeName).HasColumnName("type_name");
        modelBuilder.Entity<PaymentType>()
            .Property(p => p.TypeShortName).HasColumnName("type_short_name");
        modelBuilder.Entity<PaymentType>()
            .Property(p => p.TypeIsActive).HasColumnName("type_isactive");
        modelBuilder.Entity<PaymentType>()
            .Property(p => p.TypeCustCode).HasColumnName("type_cust_code");
        modelBuilder.Entity<PaymentType>()
            .Property(p => p.TypeCreateAt).HasColumnName("type_create_at");
        modelBuilder.Entity<PaymentType>()
            .Property(p => p.TypeUpdateAt).HasColumnName("type_update_at");
        modelBuilder.Entity<PaymentType>()
            .Property(p => p.TypeAddByUserId).HasColumnName("type_add_byuserid");
        
        
        modelBuilder.Entity<AdGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK_ad_groups");

            entity.ToTable("Ad_Groups");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupByUserid).HasColumnName("group_by_userid");
            entity.Property(e => e.GroupCdate)
                .HasColumnType("datetime")
                .HasColumnName("group_cdate");
            entity.Property(e => e.GroupCustCode)
                .HasMaxLength(50)
                .HasColumnName("group_cust_code");
            entity.Property(e => e.GroupIsactive).HasColumnName("group_isactive");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasColumnName("group_name");
            entity.Property(e => e.GroupUdate)
                .HasColumnType("datetime")
                .HasColumnName("group_udate");
        });

        modelBuilder.Entity<CustomShow>(entity =>
        {
            entity.HasKey(e => e.CustomId);

            entity.ToTable("custom_show");

            entity.Property(e => e.CustomId).HasColumnName("custom_id");
            entity.Property(e => e.CustomByuserId).HasColumnName("custom_byuser_id");
            entity.Property(e => e.CustomCdate)
                .HasColumnType("datetime")
                .HasColumnName("custom_cdate");
            entity.Property(e => e.CustomCustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("custom_cust_code");
            entity.Property(e => e.CustomDeviceId)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("custom_device_id");
            entity.Property(e => e.CustomFromTime).HasColumnName("custom_from_time");
            entity.Property(e => e.CustomHtmlCode)
                .HasColumnType("text")
                .HasColumnName("custom_html_code");
            entity.Property(e => e.CustomIsactive).HasColumnName("custom_isactive");
            entity.Property(e => e.CustomMaterialId).HasColumnName("custom_material_id");
            entity.Property(e => e.CustomToTime).HasColumnName("custom_to_time");
            entity.Property(e => e.CustomUdate)
                .HasColumnType("datetime")
                .HasColumnName("custom_udate");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustId);

            entity.Property(e => e.CustId).HasColumnName("cust_id");
            entity.Property(e => e.CustCdate)
                .HasColumnType("datetime")
                .HasColumnName("cust_cdate");
            entity.Property(e => e.CustCode)
                .HasMaxLength(50)
                .HasColumnName("cust_code");
            entity.Property(e => e.CustEmail)
                .HasMaxLength(50)
                .HasColumnName("cust_email");
            entity.Property(e => e.CustIsactive).HasColumnName("cust_isactive");
            entity.Property(e => e.CustLicenseCode).HasColumnName("cust_License_code");
            entity.Property(e => e.CustLogo)
                .HasMaxLength(50)
                .HasColumnName("cust_logo");
            entity.Property(e => e.CustMobileNo)
                .HasMaxLength(50)
                .HasColumnName("cust_Mobile_no");
            entity.Property(e => e.CustNameAr)
                .HasMaxLength(50)
                .HasColumnName("cust_name_ar");
            entity.Property(e => e.CustNameEn)
                .HasMaxLength(50)
                .HasColumnName("cust_name_en");
            entity.Property(e => e.CustTel)
                .HasMaxLength(50)
                .HasColumnName("cust_tel");
            //entity.Property(e => e.CustToken)
            //    .HasMaxLength(50)
            //    .IsUnicode(false);
            entity.Property(e => e.CustTrNo)
                .HasMaxLength(50)
                .HasColumnName("cust_tr_no");
            entity.Property(e => e.CustUdate)
                .HasColumnType("datetime")
                .HasColumnName("cust_udate");
            entity.Property(e => e.CustVat)
                .HasMaxLength(50)
                .HasColumnName("cust_vat");
        });

        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(e => e.LicId);

            entity.Property(e => e.LicId).HasColumnName("Lic_id");
            entity.Property(e => e.LicCdate)
                .HasColumnType("datetime")
                .HasColumnName("Lic_cdate");
            entity.Property(e => e.LicCustCode)
                .HasMaxLength(50)
                .HasColumnName("Lic_cust_code");
            entity.Property(e => e.LicDeviceNo).HasColumnName("Lic_device_no");
            entity.Property(e => e.LicEndAt)
                .HasColumnType("datetime")
                .HasColumnName("Lic_end_at");
            entity.Property(e => e.LicIsactive).HasColumnName("Lic_isactive");
            entity.Property(e => e.LicStartAt)
                .HasColumnType("datetime")
                .HasColumnName("Lic_start_at");
            entity.Property(e => e.LicUdate)
                .HasColumnType("datetime")
                .HasColumnName("Lic_udate");
            entity.Property(e => e.LicUserNo).HasColumnName("Lic_user_no");
        });

        modelBuilder.Entity<MaterialType>(entity =>
        {
            entity.HasKey(e => e.MtypeId);

            entity.ToTable("Material_Type");

            entity.Property(e => e.MtypeId).HasColumnName("mtype_id");
            entity.Property(e => e.MtypeCdate)
                .HasColumnType("datetime")
                .HasColumnName("mtype_cdate");
            entity.Property(e => e.MtypeCustCode)
                .HasMaxLength(50)
                .HasColumnName("mtype_cust_code");
            entity.Property(e => e.MtypeIsactive).HasColumnName("mtype_isactive");
            entity.Property(e => e.MtypeNameAr)
                .HasMaxLength(50)
                .HasColumnName("mtype_name_ar");
            entity.Property(e => e.MtypeNameEn)
                .HasMaxLength(50)
                .HasColumnName("mtype_name_en");
            entity.Property(e => e.MtypeStaticHtml).HasColumnName("mtype_static_html");
            entity.Property(e => e.MtypeUdate)
                .HasColumnType("datetime")
                .HasColumnName("mtype_udate");
            entity.Property(e => e.MtypeUserId).HasColumnName("mtype_user_id");
        });

        modelBuilder.Entity<RoleWithPermission>(entity =>
        {
            entity.HasKey(e => e.RwpId).HasName("PK_role_with_permission");

            entity.ToTable("Role_With_Permission");

            entity.Property(e => e.RwpId).HasColumnName("rwp_id");
            entity.Property(e => e.RwpByUserid).HasColumnName("rwp_by_userid");
            entity.Property(e => e.RwpCdate)
                .HasColumnType("datetime")
                .HasColumnName("rwp_cdate");
            entity.Property(e => e.RwpCustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rwp_cust_code");
            entity.Property(e => e.RwpPermissionId).HasColumnName("rwp_permission_id");
            entity.Property(e => e.RwpRoleId).HasColumnName("rwp_role_id");
            entity.Property(e => e.RwpUdate)
                .HasColumnType("datetime")
                .HasColumnName("rwp_udate");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.ShowId).HasName("PK_show");

            entity.ToTable("Show");

            entity.Property(e => e.ShowId).HasColumnName("show_id");
            entity.Property(e => e.ShowByUserid).HasColumnName("show_by_userid");
            entity.Property(e => e.ShowCdate)
                .HasColumnType("datetime")
                .HasColumnName("show_cdate");
            entity.Property(e => e.ShowCode)
                .HasMaxLength(50)
                .HasColumnName("show_code");
            entity.Property(e => e.ShowCustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("show_cust_code");
            entity.Property(e => e.ShowIsactive).HasColumnName("show_isactive");
            entity.Property(e => e.ShowOrder).HasColumnName("show_order");
            entity.Property(e => e.ShowSettingId).HasColumnName("show_setting_id");
            entity.Property(e => e.ShowTemplateId).HasColumnName("show_template_id");
            entity.Property(e => e.ShowTime).HasColumnName("show_time");
            entity.Property(e => e.ShowUdate)
                .HasColumnType("datetime")
                .HasColumnName("show_udate");
        });

        modelBuilder.Entity<ShowContent>(entity =>
        {
            entity.HasKey(e => e.ContentsId).HasName("PK_show_Contents");

            entity.ToTable("Show_Contents");

            entity.Property(e => e.ContentsId).HasColumnName("Contents_id");
            entity.Property(e => e.ContentsByUserid).HasColumnName("Contents_by_userid");
            entity.Property(e => e.ContentsCdate)
                .HasColumnType("datetime")
                .HasColumnName("Contents_cdate");
            entity.Property(e => e.ContentsCustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Contents_cust_code");
            entity.Property(e => e.ContentsIsactive).HasColumnName("Contents_isactive");
            entity.Property(e => e.ContentsShowCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Contents_show_code");
            entity.Property(e => e.ContentsShowId).HasColumnName("Contents_show_id");
            entity.Property(e => e.ContentsTxt)
                .HasColumnType("text")
                .HasColumnName("Contents_txt");
            entity.Property(e => e.ContentsUdate)
                .HasColumnType("datetime")
                .HasColumnName("Contents_udate");
        });

        modelBuilder.Entity<ShowDetail>(entity =>
        {
            entity.HasKey(e => e.ShowDetailsId).HasName("PK_show_details");

            entity.ToTable("Show_Details");

            entity.Property(e => e.ShowDetailsId).HasColumnName("show_details_id");
            entity.Property(e => e.ShowDetailsCdate)
                .HasColumnType("datetime")
                .HasColumnName("show_details_cdate");
            entity.Property(e => e.ShowDetailsCustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("show_details_cust_code");
            entity.Property(e => e.ShowDetailsFileId).HasColumnName("show_details_file_id");
            entity.Property(e => e.ShowDetailsIsactive).HasColumnName("show_details_isactive");
            entity.Property(e => e.ShowDetailsShowcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("show_details_showcode");
            entity.Property(e => e.ShowDetailsShowid).HasColumnName("show_details_showid");
            entity.Property(e => e.ShowDetailsUdate)
                .HasColumnType("datetime")
                .HasColumnName("show_details_udate");
            entity.Property(e => e.ShowDetailsZoneId).HasColumnName("show_details_zone_id");
        });

        modelBuilder.Entity<ShowHtmlcode>(entity =>
        {
            entity.HasKey(e => e.ShowId);

            entity.ToTable("show_htmlcode");

            entity.Property(e => e.ShowId).HasColumnName("show_id");
            entity.Property(e => e.ShowByuserId).HasColumnName("show_byuser_id");
            entity.Property(e => e.ShowCdate)
                .HasColumnType("datetime")
                .HasColumnName("show_cdate");
            entity.Property(e => e.ShowCode)
                .HasMaxLength(50)
                .HasColumnName("show_code");
            entity.Property(e => e.ShowCustCode)
                .HasMaxLength(50)
                .HasColumnName("show_cust_code");
            entity.Property(e => e.ShowHtmlCode1).HasColumnName("show_html_code");
            entity.Property(e => e.ShowIsactive).HasColumnName("show_isactive");
            entity.Property(e => e.ShowSettingId).HasColumnName("show_setting_id");
            entity.Property(e => e.ShowUdate)
                .HasColumnType("datetime")
                .HasColumnName("show_udate");
            entity.Property(e => e.ShowUserid).HasColumnName("show_userid");
        });

        modelBuilder.Entity<ShowMaterial>(entity =>
        {
            entity.HasKey(e => e.MatId).HasName("PK_show_material");

            entity.ToTable("Show_Material");

            entity.Property(e => e.MatId).HasColumnName("mat_id");
            entity.Property(e => e.MatByuserId).HasColumnName("mat_byuser_id");
            entity.Property(e => e.MatCdate)
                .HasColumnType("datetime")
                .HasColumnName("mat_cdate");
            entity.Property(e => e.MatCustCode)
                .HasMaxLength(50)
                .HasColumnName("mat_cust_code");
            entity.Property(e => e.MatIsactive).HasColumnName("mat_isactive");
            entity.Property(e => e.MatPath)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("mat_path");
            entity.Property(e => e.MatShowNameAr)
                .HasMaxLength(50)
                .HasColumnName("mat_show_name_ar");
            entity.Property(e => e.MatShowNameEn)
                .HasMaxLength(50)
                .HasColumnName("mat_show_name_en");
            entity.Property(e => e.MatTypeId).HasColumnName("mat_type_id");
            entity.Property(e => e.MatUdate)
                .HasColumnType("datetime")
                .HasColumnName("mat_udate");
        });

        modelBuilder.Entity<ShowSetting>(entity =>
        {
            entity.HasKey(e => e.ShowSettingId).HasName("PK_show_setting");

            entity.ToTable("Show_Setting");

            entity.HasIndex(e => e.ShowSettingId, "IX_show_setting").IsUnique();

            entity.Property(e => e.ShowSettingId).HasColumnName("show_setting_id");
            entity.Property(e => e.ShowSettingCdate)
                .HasColumnType("datetime")
                .HasColumnName("show_setting_cdate");
            entity.Property(e => e.ShowSettingCustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("show_setting_cust_code");
            entity.Property(e => e.ShowSettingDeviceId).HasColumnName("show_setting_device_id");
            entity.Property(e => e.ShowSettingGroupId).HasColumnName("show_setting_group_id");
            entity.Property(e => e.ShowSettingNext).HasColumnName("show_setting_next");
            entity.Property(e => e.ShowSettingPresent).HasColumnName("show_setting_Present");
            entity.Property(e => e.ShowSettingShowcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("show_setting_showcode");
            entity.Property(e => e.ShowSettingTotalView).HasColumnName("show_setting_total_view");
            entity.Property(e => e.ShowSettingUdate)
                .HasColumnType("datetime")
                .HasColumnName("show_setting_udate");
        });

        modelBuilder.Entity<ShowTemplate>(entity =>
        {
            entity.HasKey(e => e.TempId).HasName("PK_showtemplateTable");

            entity.ToTable("Show_Template");

            entity.Property(e => e.TempId).HasColumnName("temp_id");
            entity.Property(e => e.TempByUserid).HasColumnName("temp_by_userid");
            entity.Property(e => e.TempCdate)
                .HasColumnType("datetime")
                .HasColumnName("temp_cdate");
            entity.Property(e => e.TempColNo).HasColumnName("temp_col_no");
            entity.Property(e => e.TempCustCode)
                .HasMaxLength(50)
                .HasColumnName("temp_cust_code");
            entity.Property(e => e.TempIsactive).HasColumnName("temp_isactive");
            entity.Property(e => e.TempNameAr)
                .HasMaxLength(50)
                .HasColumnName("temp_name_ar");
            entity.Property(e => e.TempNameEng)
                .HasMaxLength(50)
                .HasColumnName("temp_name_eng");
            entity.Property(e => e.TempRowNo).HasColumnName("temp_row_no");
            entity.Property(e => e.TempUdate)
                .HasColumnType("datetime")
                .HasColumnName("temp_udate");
        });

        modelBuilder.Entity<ShowType>(entity =>
        {
            entity.HasKey(e => e.ShowTypeId).HasName("PK_show_type");

            entity.ToTable("Show_Type");

            entity.Property(e => e.ShowTypeId).HasColumnName("show_type_id");
            entity.Property(e => e.ShowTypeByUserid).HasColumnName("show_type_by_userid");
            entity.Property(e => e.ShowTypeCdate)
                .HasColumnType("datetime")
                .HasColumnName("show_type_cdate");
            entity.Property(e => e.ShowTypeCustCode)
                .HasMaxLength(50)
                .HasColumnName("show_type_cust_code");
            entity.Property(e => e.ShowTypeIsactive).HasColumnName("show_type_isactive");
            entity.Property(e => e.ShowTypeNameAr)
                .HasMaxLength(50)
                .HasColumnName("show_type_name_ar");
            entity.Property(e => e.ShowTypeNameEng)
                .HasMaxLength(50)
                .HasColumnName("show_type_name_eng");
            entity.Property(e => e.ShowTypeUdate)
                .HasColumnType("datetime")
                .HasColumnName("show_type_udate");
        });

        modelBuilder.Entity<TemplateDetail>(entity =>
        {
            entity.HasKey(e => e.TempDetail).HasName("PK_template_details");

            entity.ToTable("Template_Details");

            entity.Property(e => e.TempDetail).HasColumnName("temp_detail");
            entity.Property(e => e.TempByUserid).HasColumnName("temp_by_userid");
            entity.Property(e => e.TempCdate)
                .HasColumnType("datetime")
                .HasColumnName("temp_cdate");
            entity.Property(e => e.TempCustCode)
                .HasMaxLength(50)
                .HasColumnName("temp_cust_code");
            entity.Property(e => e.TempIsactive).HasColumnName("temp_isactive");
            entity.Property(e => e.TempTempId).HasColumnName("temp_temp_id");
            entity.Property(e => e.TempUdate)
                .HasColumnType("datetime")
                .HasColumnName("temp_udate");
            entity.Property(e => e.TempZoneCode).HasColumnName("temp_zone_code");
            entity.Property(e => e.TempZoneFileTypeid).HasColumnName("temp_zone_file_typeid");
            entity.Property(e => e.TempZoneHeight).HasColumnName("temp_zone_height");
            entity.Property(e => e.TempZoneWidth).HasColumnName("temp_zone_width");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserCdate)
                .HasColumnType("datetime")
                .HasColumnName("user_cdate");
            entity.Property(e => e.UserCustCode)
                .HasMaxLength(50)
                .HasColumnName("user_cust_code");
            entity.Property(e => e.UserLoginName)
                .HasMaxLength(50)
                .HasColumnName("user_login_name");
            entity.Property(e => e.UserNameAr)
                .HasMaxLength(50)
                .HasColumnName("user_name_ar");
            entity.Property(e => e.UserNameEn)
                .HasMaxLength(50)
                .HasColumnName("user_name_en");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(50)
                .HasColumnName("user_password");
            entity.Property(e => e.UserPhoto)
                .HasMaxLength(50)
                .HasColumnName("user_photo");
            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
            entity.Property(e => e.UserUdate)
                .HasColumnType("datetime")
                .HasColumnName("user_udate");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.PermId).HasName("PK_upermission");

            entity.ToTable("User_Permission");

            entity.Property(e => e.PermId).HasColumnName("perm_id");
            entity.Property(e => e.PermByUserid).HasColumnName("perm_by_userid");
            entity.Property(e => e.PermCdate)
                .HasColumnType("datetime")
                .HasColumnName("perm_cdate");
            entity.Property(e => e.PermCustCode)
                .HasMaxLength(50)
                .HasColumnName("perm_cust_code");
            entity.Property(e => e.PermIsactive).HasColumnName("perm_isactive");
            entity.Property(e => e.PermName)
                .HasMaxLength(50)
                .HasColumnName("perm_name");
            entity.Property(e => e.PermUdate)
                .HasColumnType("datetime")
                .HasColumnName("perm_udate");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_user_role");

            entity.ToTable("User_Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleByuserId).HasColumnName("role_byuser_id");
            entity.Property(e => e.RoleCdate)
                .HasColumnType("datetime")
                .HasColumnName("role_cdate");
            entity.Property(e => e.RoleCustCode)
                .HasMaxLength(50)
                .HasColumnName("role_cust_code");
            entity.Property(e => e.RoleIsactive).HasColumnName("role_isactive");
            entity.Property(e => e.RoleNameAr)
                .HasMaxLength(50)
                .HasColumnName("role_name_ar");
            entity.Property(e => e.RoleNameEn)
                .HasMaxLength(50)
                .HasColumnName("role_name_en");
            entity.Property(e => e.RoleUdate)
                .HasColumnType("datetime")
                .HasColumnName("role_udate");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
