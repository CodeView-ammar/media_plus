using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaPlus.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ad_Devices",
                columns: table => new
                {
                    Devicesid = table.Column<int>(name: "Devices_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Devicesname = table.Column<string>(name: "Devices_name", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Devicescdate = table.Column<DateTime>(name: "Devices_cdate", type: "datetime", nullable: true),
                    Devicesgroupid = table.Column<int>(name: "Devices_groupid", type: "int", nullable: true),
                    Devicesudate = table.Column<DateTime>(name: "Devices_udate", type: "datetime", nullable: true),
                    Devicescustcode = table.Column<string>(name: "Devices_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    deviceisinterrupt = table.Column<int>(name: "device_is_interrupt", type: "int", nullable: false),
                    Devicesonoff = table.Column<int>(name: "Devices_onoff", type: "int", nullable: true),
                    Devicesofflinephoto = table.Column<string>(name: "Devices_offline_photo", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Devicesbyuserid = table.Column<int>(name: "Devices_by_userid", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Devicesid);
                });

            migrationBuilder.CreateTable(
                name: "Ad_Groups",
                columns: table => new
                {
                    groupid = table.Column<int>(name: "group_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    groupname = table.Column<string>(name: "group_name", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    groupcdate = table.Column<DateTime>(name: "group_cdate", type: "datetime", nullable: true),
                    groupudate = table.Column<DateTime>(name: "group_udate", type: "datetime", nullable: true),
                    groupisactive = table.Column<int>(name: "group_isactive", type: "int", nullable: true),
                    groupcustcode = table.Column<string>(name: "group_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    groupbyuserid = table.Column<int>(name: "group_by_userid", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ad_groups", x => x.groupid);
                });

            migrationBuilder.CreateTable(
                name: "custom_show",
                columns: table => new
                {
                    customid = table.Column<int>(name: "custom_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customdeviceid = table.Column<string>(name: "custom_device_id", type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    custommaterialid = table.Column<int>(name: "custom_material_id", type: "int", nullable: true),
                    customhtmlcode = table.Column<string>(name: "custom_html_code", type: "text", nullable: true),
                    customfromtime = table.Column<TimeSpan>(name: "custom_from_time", type: "time", nullable: true),
                    customtotime = table.Column<TimeSpan>(name: "custom_to_time", type: "time", nullable: true),
                    customcustcode = table.Column<string>(name: "custom_cust_code", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    custombyuserid = table.Column<int>(name: "custom_byuser_id", type: "int", nullable: true),
                    customcdate = table.Column<DateTime>(name: "custom_cdate", type: "datetime", nullable: true),
                    customudate = table.Column<DateTime>(name: "custom_udate", type: "datetime", nullable: true),
                    customisactive = table.Column<int>(name: "custom_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_show", x => x.customid);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    custid = table.Column<int>(name: "cust_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    custnamear = table.Column<string>(name: "cust_name_ar", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    custnameen = table.Column<string>(name: "cust_name_en", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    custvat = table.Column<string>(name: "cust_vat", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    custtrno = table.Column<string>(name: "cust_tr_no", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    custtel = table.Column<string>(name: "cust_tel", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    custemail = table.Column<string>(name: "cust_email", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    custMobileno = table.Column<string>(name: "cust_Mobile_no", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    custcdate = table.Column<DateTime>(name: "cust_cdate", type: "datetime", nullable: false),
                    custudate = table.Column<DateTime>(name: "cust_udate", type: "datetime", nullable: false),
                    custcode = table.Column<string>(name: "cust_code", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    custLicensecode = table.Column<int>(name: "cust_License_code", type: "int", nullable: true),
                    custlogo = table.Column<string>(name: "cust_logo", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    custisactive = table.Column<int>(name: "cust_isactive", type: "int", nullable: false),
                    CustToken = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.custid);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Licid = table.Column<int>(name: "Lic_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Licstartat = table.Column<DateTime>(name: "Lic_start_at", type: "datetime", nullable: false),
                    Licendat = table.Column<DateTime>(name: "Lic_end_at", type: "datetime", nullable: false),
                    Licdeviceno = table.Column<int>(name: "Lic_device_no", type: "int", nullable: false),
                    Licuserno = table.Column<int>(name: "Lic_user_no", type: "int", nullable: false),
                    Liccdate = table.Column<DateTime>(name: "Lic_cdate", type: "datetime", nullable: false),
                    Licudate = table.Column<DateTime>(name: "Lic_udate", type: "datetime", nullable: false),
                    Liccustcode = table.Column<string>(name: "Lic_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Licisactive = table.Column<int>(name: "Lic_isactive", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Licid);
                });

            migrationBuilder.CreateTable(
                name: "Material_Type",
                columns: table => new
                {
                    mtypeid = table.Column<int>(name: "mtype_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mtypenamear = table.Column<string>(name: "mtype_name_ar", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mtypenameen = table.Column<string>(name: "mtype_name_en", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mtypecdate = table.Column<DateTime>(name: "mtype_cdate", type: "datetime", nullable: true),
                    mtypeudate = table.Column<DateTime>(name: "mtype_udate", type: "datetime", nullable: true),
                    mtypeuserid = table.Column<int>(name: "mtype_user_id", type: "int", nullable: true),
                    mtypecustcode = table.Column<string>(name: "mtype_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mtypeisactive = table.Column<int>(name: "mtype_isactive", type: "int", nullable: true),
                    mtypestatichtml = table.Column<string>(name: "mtype_static_html", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material_Type", x => x.mtypeid);
                });

            migrationBuilder.CreateTable(
                name: "Role_With_Permission",
                columns: table => new
                {
                    rwpid = table.Column<int>(name: "rwp_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rwproleid = table.Column<int>(name: "rwp_role_id", type: "int", nullable: true),
                    rwppermissionid = table.Column<int>(name: "rwp_permission_id", type: "int", nullable: true),
                    rwpbyuserid = table.Column<int>(name: "rwp_by_userid", type: "int", nullable: true),
                    rwpcdate = table.Column<DateTime>(name: "rwp_cdate", type: "datetime", nullable: true),
                    rwpudate = table.Column<DateTime>(name: "rwp_udate", type: "datetime", nullable: true),
                    rwpcustcode = table.Column<string>(name: "rwp_cust_code", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_with_permission", x => x.rwpid);
                });

            migrationBuilder.CreateTable(
                name: "Show",
                columns: table => new
                {
                    showid = table.Column<int>(name: "show_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    showsettingid = table.Column<int>(name: "show_setting_id", type: "int", nullable: true),
                    showcode = table.Column<string>(name: "show_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    showtime = table.Column<int>(name: "show_time", type: "int", nullable: true),
                    showtemplateid = table.Column<int>(name: "show_template_id", type: "int", nullable: true),
                    showorder = table.Column<int>(name: "show_order", type: "int", nullable: true),
                    showbyuserid = table.Column<int>(name: "show_by_userid", type: "int", nullable: true),
                    showcdate = table.Column<DateTime>(name: "show_cdate", type: "datetime", nullable: true),
                    showudate = table.Column<DateTime>(name: "show_udate", type: "datetime", nullable: true),
                    showcustcode = table.Column<string>(name: "show_cust_code", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    showisactive = table.Column<int>(name: "show_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show", x => x.showid);
                });

            migrationBuilder.CreateTable(
                name: "Show_Contents",
                columns: table => new
                {
                    Contentsid = table.Column<int>(name: "Contents_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contentsshowcode = table.Column<string>(name: "Contents_show_code", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Contentsshowid = table.Column<int>(name: "Contents_show_id", type: "int", nullable: true),
                    Contentstxt = table.Column<string>(name: "Contents_txt", type: "text", nullable: true),
                    Contentscdate = table.Column<DateTime>(name: "Contents_cdate", type: "datetime", nullable: true),
                    Contentsudate = table.Column<DateTime>(name: "Contents_udate", type: "datetime", nullable: true),
                    Contentsbyuserid = table.Column<int>(name: "Contents_by_userid", type: "int", nullable: true),
                    Contentscustcode = table.Column<string>(name: "Contents_cust_code", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Contentsisactive = table.Column<int>(name: "Contents_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_Contents", x => x.Contentsid);
                });

            migrationBuilder.CreateTable(
                name: "Show_Details",
                columns: table => new
                {
                    showdetailsid = table.Column<int>(name: "show_details_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    showdetailsshowid = table.Column<int>(name: "show_details_showid", type: "int", nullable: true),
                    showdetailszoneid = table.Column<int>(name: "show_details_zone_id", type: "int", nullable: true),
                    showdetailsfileid = table.Column<int>(name: "show_details_file_id", type: "int", nullable: true),
                    showdetailsshowcode = table.Column<string>(name: "show_details_showcode", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    showdetailscdate = table.Column<DateTime>(name: "show_details_cdate", type: "datetime", nullable: true),
                    showdetailsudate = table.Column<DateTime>(name: "show_details_udate", type: "datetime", nullable: true),
                    showdetailscustcode = table.Column<string>(name: "show_details_cust_code", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    showdetailsisactive = table.Column<int>(name: "show_details_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_details", x => x.showdetailsid);
                });

            migrationBuilder.CreateTable(
                name: "show_htmlcode",
                columns: table => new
                {
                    showid = table.Column<int>(name: "show_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    showhtmlcode = table.Column<string>(name: "show_html_code", type: "nvarchar(max)", nullable: false),
                    showcdate = table.Column<DateTime>(name: "show_cdate", type: "datetime", nullable: true),
                    showudate = table.Column<DateTime>(name: "show_udate", type: "datetime", nullable: true),
                    showuserid = table.Column<int>(name: "show_userid", type: "int", nullable: true),
                    showisactive = table.Column<int>(name: "show_isactive", type: "int", nullable: true),
                    showbyuserid = table.Column<int>(name: "show_byuser_id", type: "int", nullable: true),
                    showcustcode = table.Column<string>(name: "show_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    showcode = table.Column<string>(name: "show_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    showsettingid = table.Column<int>(name: "show_setting_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_htmlcode", x => x.showid);
                });

            migrationBuilder.CreateTable(
                name: "Show_Material",
                columns: table => new
                {
                    matid = table.Column<int>(name: "mat_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    matshownamear = table.Column<string>(name: "mat_show_name_ar", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    matshownameen = table.Column<string>(name: "mat_show_name_en", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    matpath = table.Column<string>(name: "mat_path", type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    mattypeid = table.Column<int>(name: "mat_type_id", type: "int", nullable: false),
                    matcdate = table.Column<DateTime>(name: "mat_cdate", type: "datetime", nullable: false),
                    matudate = table.Column<DateTime>(name: "mat_udate", type: "datetime", nullable: true),
                    matbyuserid = table.Column<int>(name: "mat_byuser_id", type: "int", nullable: false),
                    matcustcode = table.Column<string>(name: "mat_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    matisactive = table.Column<int>(name: "mat_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_material", x => x.matid);
                });

            migrationBuilder.CreateTable(
                name: "Show_Setting",
                columns: table => new
                {
                    showsettingid = table.Column<int>(name: "show_setting_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    showsettingshowcode = table.Column<string>(name: "show_setting_showcode", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    showsettinggroupid = table.Column<int>(name: "show_setting_group_id", type: "int", nullable: true),
                    showsettingdeviceid = table.Column<int>(name: "show_setting_device_id", type: "int", nullable: true),
                    showsettingPresent = table.Column<int>(name: "show_setting_Present", type: "int", nullable: true),
                    showsettingnext = table.Column<int>(name: "show_setting_next", type: "int", nullable: true),
                    showsettingtotalview = table.Column<int>(name: "show_setting_total_view", type: "int", nullable: true),
                    showsettingcdate = table.Column<DateTime>(name: "show_setting_cdate", type: "datetime", nullable: true),
                    showsettingudate = table.Column<DateTime>(name: "show_setting_udate", type: "datetime", nullable: true),
                    showsettingcustcode = table.Column<string>(name: "show_setting_cust_code", type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_setting", x => x.showsettingid);
                });

            migrationBuilder.CreateTable(
                name: "Show_Template",
                columns: table => new
                {
                    tempid = table.Column<int>(name: "temp_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tempnamear = table.Column<string>(name: "temp_name_ar", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tempnameeng = table.Column<string>(name: "temp_name_eng", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    temprowno = table.Column<int>(name: "temp_row_no", type: "int", nullable: true),
                    tempcolno = table.Column<int>(name: "temp_col_no", type: "int", nullable: true),
                    tempcustcode = table.Column<string>(name: "temp_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tempcdate = table.Column<DateTime>(name: "temp_cdate", type: "datetime", nullable: true),
                    tempudate = table.Column<DateTime>(name: "temp_udate", type: "datetime", nullable: true),
                    tempbyuserid = table.Column<int>(name: "temp_by_userid", type: "int", nullable: true),
                    tempisactive = table.Column<int>(name: "temp_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_showtemplateTable", x => x.tempid);
                });

            migrationBuilder.CreateTable(
                name: "Show_Type",
                columns: table => new
                {
                    showtypeid = table.Column<int>(name: "show_type_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    showtypenamear = table.Column<string>(name: "show_type_name_ar", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    showtypenameeng = table.Column<string>(name: "show_type_name_eng", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    showtypecustcode = table.Column<string>(name: "show_type_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    showtypebyuserid = table.Column<int>(name: "show_type_by_userid", type: "int", nullable: true),
                    showtypecdate = table.Column<DateTime>(name: "show_type_cdate", type: "datetime", nullable: true),
                    showtypeudate = table.Column<DateTime>(name: "show_type_udate", type: "datetime", nullable: true),
                    showtypeisactive = table.Column<int>(name: "show_type_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_show_type", x => x.showtypeid);
                });

            migrationBuilder.CreateTable(
                name: "Template_Details",
                columns: table => new
                {
                    tempdetail = table.Column<int>(name: "temp_detail", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    temptempid = table.Column<int>(name: "temp_temp_id", type: "int", nullable: true),
                    tempzonecode = table.Column<double>(name: "temp_zone_code", type: "float", nullable: true),
                    tempzonewidth = table.Column<int>(name: "temp_zone_width", type: "int", nullable: true),
                    tempzoneheight = table.Column<int>(name: "temp_zone_height", type: "int", nullable: true),
                    tempzonefiletypeid = table.Column<int>(name: "temp_zone_file_typeid", type: "int", nullable: true),
                    tempcustcode = table.Column<string>(name: "temp_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tempcdate = table.Column<DateTime>(name: "temp_cdate", type: "datetime", nullable: true),
                    tempudate = table.Column<DateTime>(name: "temp_udate", type: "datetime", nullable: true),
                    tempbyuserid = table.Column<int>(name: "temp_by_userid", type: "int", nullable: true),
                    tempisactive = table.Column<int>(name: "temp_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_details", x => x.tempdetail);
                });

            migrationBuilder.CreateTable(
                name: "User_Permission",
                columns: table => new
                {
                    permid = table.Column<int>(name: "perm_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    permname = table.Column<string>(name: "perm_name", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    permcustcode = table.Column<string>(name: "perm_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    permudate = table.Column<DateTime>(name: "perm_udate", type: "datetime", nullable: true),
                    permcdate = table.Column<DateTime>(name: "perm_cdate", type: "datetime", nullable: true),
                    permisactive = table.Column<int>(name: "perm_isactive", type: "int", nullable: true),
                    permbyuserid = table.Column<int>(name: "perm_by_userid", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_upermission", x => x.permid);
                });

            migrationBuilder.CreateTable(
                name: "User_Role",
                columns: table => new
                {
                    roleid = table.Column<int>(name: "role_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rolenamear = table.Column<string>(name: "role_name_ar", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    rolenameen = table.Column<string>(name: "role_name_en", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    rolecdate = table.Column<DateTime>(name: "role_cdate", type: "datetime", nullable: false),
                    roleudate = table.Column<DateTime>(name: "role_udate", type: "datetime", nullable: true),
                    rolebyuserid = table.Column<int>(name: "role_byuser_id", type: "int", nullable: false),
                    rolecustcode = table.Column<string>(name: "role_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    roleisactive = table.Column<int>(name: "role_isactive", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => x.roleid);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userid = table.Column<int>(name: "user_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usernamear = table.Column<string>(name: "user_name_ar", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    usernameen = table.Column<string>(name: "user_name_en", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    userloginname = table.Column<string>(name: "user_login_name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    userpassword = table.Column<string>(name: "user_password", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    userphoto = table.Column<string>(name: "user_photo", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    usercdate = table.Column<DateTime>(name: "user_cdate", type: "datetime", nullable: false),
                    userudate = table.Column<DateTime>(name: "user_udate", type: "datetime", nullable: true),
                    userroleid = table.Column<int>(name: "user_role_id", type: "int", nullable: false),
                    usercustcode = table.Column<string>(name: "user_cust_code", type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_show_setting",
                table: "Show_Setting",
                column: "show_setting_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ad_Devices");

            migrationBuilder.DropTable(
                name: "Ad_Groups");

            migrationBuilder.DropTable(
                name: "custom_show");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "Material_Type");

            migrationBuilder.DropTable(
                name: "Role_With_Permission");

            migrationBuilder.DropTable(
                name: "Show");

            migrationBuilder.DropTable(
                name: "Show_Contents");

            migrationBuilder.DropTable(
                name: "Show_Details");

            migrationBuilder.DropTable(
                name: "show_htmlcode");

            migrationBuilder.DropTable(
                name: "Show_Material");

            migrationBuilder.DropTable(
                name: "Show_Setting");

            migrationBuilder.DropTable(
                name: "Show_Template");

            migrationBuilder.DropTable(
                name: "Show_Type");

            migrationBuilder.DropTable(
                name: "Template_Details");

            migrationBuilder.DropTable(
                name: "User_Permission");

            migrationBuilder.DropTable(
                name: "User_Role");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
