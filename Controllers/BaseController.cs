using Microsoft.AspNetCore.Mvc;
using OzzeJobTrackerRestApi.Helpers;
using OzzeJobTrackerRestApi.Models;
using System.Data;
using System.Data.SqlClient;
using UnityObjects;

namespace OzzeJobTrackerRestApi.Controllers
{
    [ApiController]
    public class BaseController : Controller
    {
        [HttpGet("789/list")]
        public IActionResult GetAmbars(string firmaNo)
        {
            List<Ambar> ambarList = new List<Ambar>();

            DataTable dataTable = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(AllVariables.ConnectionString);
            SqlDataAdapter data = new SqlDataAdapter($"select * from L_CAPIWHOUSE where FIRMNR=789", sqlConnection);
            data.Fill(dataTable);
            ambarList = (from DataRow dr in dataTable.Rows
                         select new Ambar()
                         {
                             LogicalReference = dr["LOGICALREF"].ToString(),
                             FirmaNo = dr["FIRMNR"].ToString(),
                             Adi = dr["NAME"].ToString(),
                             No = dr["Nr"].ToString()
                         }).ToList();

            return Ok(ambarList);
        }

        [HttpPost("createAmbarFisi")]
        public IActionResult CreateAmbarFisi([FromBody] AmbarFisi ambarFisi)
        {
            try
            {
                string belgeNoPrefix = ambarFisi.HedefAmbarNo == 110 ? "A_" : "B_";

                UnityApplication UnityApp = new UnityApplication();
                UnityApp.Login("Aktarim", "akt", 789);

                Data items = UnityApp.NewDataObject(DataObjectType.doMaterialSlip);
                items.New();
                //items.DataFields.FieldByName("INTERNAL_REFERENCE").Value = 41990;
                items.DataFields.FieldByName("GROUP").Value = 3;
                items.DataFields.FieldByName("TYPE").Value = 25;
                items.DataFields.FieldByName("NUMBER").Value = "~";
                items.DataFields.FieldByName("DATE").Value = DateTime.Now.ToShortDateString();
                //items.DataFields.FieldByName("TIME").Value = 150994944;
                items.DataFields.FieldByName("DOC_NUMBER").Value = ambarFisi.BelgeNo;/*belgeNoPrefix + ambarFisi.KaynakAmbarKodu.ToString() + "_" + ambarFisi.HedefAmbarKodu.ToString();*/
                items.DataFields.FieldByName("SOURCE_WH").Value = ambarFisi.KaynakAmbarNo;
                items.DataFields.FieldByName("DEST_WH").Value = ambarFisi.HedefAmbarNo;
                items.DataFields.FieldByName("SOURCE_DIVISION_NR").Value = ambarFisi.KaynakAmbarNo;
                items.DataFields.FieldByName("DEST_DIVISION_NR").Value = ambarFisi.HedefAmbarNo;
                //items.DataFields.FieldByName("RC_RATE").Value = 459.52;
                //items.DataFields.FieldByName("CREATED_BY").Value = 1000;
                //items.DataFields.FieldByName("DATE_CREATED").Value = 06.09.2026;
                //items.DataFields.FieldByName("HOUR_CREATED").Value = 10;
                //items.DataFields.FieldByName("MIN_CREATED").Value = 34;
                //items.DataFields.FieldByName("SEC_CREATED").Value = 49;
                //items.DataFields.FieldByName("CURRSEL_TOTALS").Value = 1;
                //items.DataFields.FieldByName("DATA_REFERENCE").Value = 41990;

                Lines transactions_lines = items.DataFields.FieldByName("TRANSACTIONS").Lines;
                transactions_lines.AppendLine();
                //transactions_lines[transactions_lines.Count - 1].FieldByName("INTERNAL_REFERENCE").Value = 216688;
                transactions_lines[transactions_lines.Count - 1].FieldByName("ITEM_CODE").Value = ambarFisi.MalzemeKodu;
                transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_TYPE").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("SOURCEINDEX").Value = ambarFisi.KaynakAmbarNo;
                transactions_lines[transactions_lines.Count - 1].FieldByName("DESTINDEX").Value = ambarFisi.HedefAmbarNo;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_NUMBER").Value = 1;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("GL_CODE1").Value = 153.10.001;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("GL_CODE2").Value = 153.10.001;
                transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = ambarFisi.Miktar;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("RC_XRATE").Value = 459.52;
                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                //transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("DATA_REFERENCE").Value = 216688;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("EU_VAT_STATUS").Value = 4;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("PR_RATE").Value = 1;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("PROJECT_CODE").Value = OZZE + KASPI;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("ORGLINKREF").Value = 216689;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("EDT_CURR").Value = 1;
                transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE2").Value = ambarFisi.HareketOzelKodu2; 
                //transactions_lines[transactions_lines.Count - 1].FieldByName("GTIP_CODE").Value = 6403999600;
                //items.DataFields.FieldByName("SHIP_DATE").Value = 06.09.2026;
                //items.DataFields.FieldByName("SHIP_TIME").Value = 169944921;
                //items.DataFields.FieldByName("DOC_DATE").Value = 06.09.2026;
                //items.DataFields.FieldByName("DOC_TIME").Value = 169944921;
                //items.DataFields.FieldByName("GUID").Value = 3212507D - 87A3 - 41C5 - 857E-73BAE28C4205;
                if (items.Post() == true)
                {
                    UnityApp.Disconnect();
                    return Ok("Перемещение было успешно создано.");
                }
                else
                {
                    if (items.ErrorCode != 0)
                    {
                        UnityApp.Disconnect();
                        return BadRequest("DBError(" + items.ErrorCode.ToString() + ")-" + items.ErrorDesc + items.DBErrorDesc);
                    }
                    else if (items.ValidateErrors.Count > 0)
                    {
                        string hataMesaji = "XML ErrorList:";
                        for (int i = 0; i < items.ValidateErrors.Count; i++)
                        {
                            hataMesaji += "(" + items.ValidateErrors[i].ID.ToString() + ") - " + items.ValidateErrors[i].Error;
                        }

                        UnityApp.Disconnect();
                        return BadRequest(hataMesaji);
                    }
                    else
                    {
                        UnityApp.Disconnect();
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("updateMalzeme")]
        public IActionResult UpdateMalzeme([FromBody] Malzeme malzeme)
        {
            UnityApplication UnityApp = new UnityApplication();
            UnityApp.Login("Aktarim", "akt", 789);

            try
            {
                UnityObjects.Data items = UnityApp.NewDataObject(UnityObjects.DataObjectType.doMaterial);
                if (items.Read(Convert.ToInt32(malzeme.LogicalReference)))
                {
                    try
                    {
                        items.DataFields.FieldByName("CODE").Value = malzeme.Kod;
                        items.DataFields.FieldByName("NAME").Value = malzeme.Aciklama;
                        items.DataFields.FieldByName("GTIPCODE").Value = malzeme.GtipKodu;
                        items.DataFields.FieldByName("NAME3").Value = malzeme.Aciklama2;
                        items.DataFields.FieldByName("NAME4").Value = malzeme.Aciklama3;
                        items.DataFields.FieldByName("FREIGHT_TYPE_CODE5").Value = malzeme.AnaBirimBarkodu;
                        items.DataFields.FieldByName("FREIGHT_TYPE_CODE6").Value = malzeme.Barkod2;
                        items.DataFields.FieldByName("FREIGHT_TYPE_CODE7").Value = malzeme.Barkod3;
                        items.DataFields.FieldByName("VAT").Value = malzeme.Kdv;

                        if (items.Post() == true)
                        {
                            UnityApp.Disconnect();
                            return Ok("POST OK !");
                        }
                        else
                        {
                            if (items.ErrorCode != 0)
                            {
                                UnityApp.Disconnect();
                                return BadRequest("DBError(" + items.ErrorCode.ToString() + ")-" + items.ErrorDesc + items.DBErrorDesc);
                            }
                            else if (items.ValidateErrors.Count > 0)
                            {
                                string hataMesaji = "XML ErrorList:";
                                for (int i = 0; i < items.ValidateErrors.Count; i++)
                                {
                                    hataMesaji += "(" + items.ValidateErrors[i].ID.ToString() + ") - " + items.ValidateErrors[i].Error;
                                }

                                UnityApp.Disconnect();
                                return BadRequest(hataMesaji);
                            }
                            else
                            {
                                UnityApp.Disconnect();
                                return BadRequest();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    UnityApp.Disconnect();
                    return BadRequest("Başarısız");
                }
            }
            catch (Exception)
            {
                UnityApp.Disconnect();
                return BadRequest("Başarısız");
            }
        }

        [HttpGet("getImage/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                string imageFolderPath = @"D:\ftp\picture";
                string imagePath = Path.Combine(imageFolderPath, fileName);

                if (System.IO.File.Exists(imagePath))
                {
                    byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                    string mimeType = "image/jpeg";
                    return File(imageBytes, mimeType);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("getAmbarStok/{stokKodu}")]
        public IActionResult GetAmbarStok(string stokKodu)
        {
            try
            {
                DataTable dataTable = new DataTable();
                SqlConnection sqlConnection = new SqlConnection(AllVariables.ConnectionString);
                SqlDataAdapter data = new SqlDataAdapter($"Set DateFormat DMY SELECT CODE As [Stok Kodu], NAME As Name, SPECODE As ozelkod, SPECODE2 As ozelkod2, SPECODE3 As ozelkod3, SPECODE4 As ozelkod4, SPECODE5 As ozelkod5, ISONR As İskonto, STGRPCODE As GRUP, PRODCOUNTRY As UretimYeri, FREIGHTTYPCODE1 As Barkod, ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (0) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (0) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [MERKEZ STCK], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (1) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (1) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [WEB], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (100) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (100) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [MGA], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (101) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (101) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [KHS], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (102) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (102) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [MSW], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (103) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (103) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [DSP], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (105) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (105) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [KRH], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (104) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) - ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (104) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [OZZ], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (100) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) AS [KZ2 ALIM], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (102) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) AS [KZ3 ALIM], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (103) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) AS [KZ4 ALIM], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (101) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) AS [KZ1 ALIM], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (105) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) AS [KZ5 ALIM], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 1, 2, 3, 13, 14, 25, 50, 15, 16, 17, 18, 19 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (104) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (1, 2) ) ) ), 0 ) AS [OZZE ALIM], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (100) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [KZ2 SAT], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (102) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [KZ3 SAT], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (103) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [KZ4 SAT], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (105) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [KZ5 SAT], ISNULL ( ( Select SUM( AMOUNT *( ( CASE UINFO2 WHEN '' then 1 When 0 then 1 else UINFO2 end ) / ( CASE UINFO1 WHEN '' then 1 when 0 then 1 else UINFO1 end ) ) ) From Lbs_db_fc..LG_789_01_STLINE Where ( ( TRCODE in ( 6, 7, 8, 11, 12, 25, 51, 20, 21, 22, 23, 24 ) ) ) AND (STOCKREF = StkKart.LOGICALREF) AND (DATE_ <= '31.12.2026') and ( SOURCEINDEX IN (104) ) and (CANCELLED = 0) AND (LINETYPE = 0) AND ( ( IOCODE IN (3, 4) ) ) ), 0 ) AS [OZZE SAT] FROM Lbs_db_fc..LG_789_ITEMS AS StkKart Where StkKart.Code='{stokKodu}' and ( StkKart.CardType in (1, 2, 3, 10, 11, 12) ) and (StkKart.ACTIVE = 0) Order by StkKart.CODE", sqlConnection);
                data.Fill(dataTable);
                DataRow dr = dataTable.Rows[0];
                AmbarStok ambarStok = new AmbarStok()
                {
                    MerkezStck = dr[0].ToString(),
                    Web = dr[1].ToString(),
                    Mga = dr[2].ToString(),
                    Khs = dr[3].ToString(),
                    Msw = dr[4].ToString(),
                    Dsp = dr[5].ToString(),
                    Krh = dr[6].ToString(),
                    Ozz = dr[7].ToString()
                };

                return Ok(dataTable);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }

        }

        [HttpPost("createSiparisFisi")]
        public IActionResult CreateSiparisFisi([FromBody] SiparisFisi siparisFisi)
        {
            if (siparisFisi == null || siparisFisi.Kalemler == null)
            {
                return BadRequest("SiparisFisi veya Kalemler boş olamaz.");
            }

            try
            {
                UnityApplication UnityApp = new UnityApplication();
                UnityApp.Login("Aktarim", "akt", 202);

                Data order = UnityApp.NewDataObject(DataObjectType.doSalesOrder);
                order.New();
                order.DataFields.FieldByName("NUMBER").Value = "~";
                order.DataFields.FieldByName("DOC_NUMBER").Value = siparisFisi.BelgeNo;
                order.DataFields.FieldByName("ARP_CODE").Value = siparisFisi.CariKod;
                order.DataFields.FieldByName("DATE").Value = siparisFisi.Tarih.ToShortDateString();

                Lines transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;

                // Sipariş kalemlerini ekleyelim
                foreach (var kalem in siparisFisi.Kalemler)
                {
                    transactions_lines.AppendLine();
                    var line = transactions_lines[transactions_lines.Count - 1];
                    line.FieldByName("TYPE").Value = 0; // Malzeme
                    line.FieldByName("ITEM_CODE").Value = kalem.MalzemeKodu;
                    line.FieldByName("QUANTITY").Value = kalem.Miktar;
                    line.FieldByName("UNIT_CODE").Value = kalem.Birim;
                    line.FieldByName("PRICE").Value = kalem.Fiyat; // Eğer fiyatı kaldırmak isterseniz bu satırı kaldırabilirsiniz.
                    line.FieldByName("TOTAL").Value = kalem.Toplam;
                    line.FieldByName("DISCOUNT_DISTR").Value = kalem.IskontoTutari;
                    line.FieldByName("VAT_BASE").Value = kalem.VergiMatrahı;
                    line.FieldByName("TOTAL_NET").Value = kalem.ToplamNet;
                    line.FieldByName("RC_XRATE").Value = kalem.DovizKuru;
                }

                // İskonto uygulaması
                if (siparisFisi.Iskonto > 0)
                {
                    transactions_lines.AppendLine();
                    var discountLine = transactions_lines[transactions_lines.Count - 1];
                    discountLine.FieldByName("TYPE").Value = 2; // İskonto tipi
                    discountLine.FieldByName("DETAIL_LEVEL").Value = 1;
                    discountLine.FieldByName("QUANTITY").Value = 0;
                    discountLine.FieldByName("TOTAL").Value = siparisFisi.IskontoTutari;
                    discountLine.FieldByName("DISCOUNT_RATE").Value = siparisFisi.Iskonto;
                    discountLine.FieldByName("RC_XRATE").Value = siparisFisi.DovizKuru;
                }

                if (order.Post() == true)
                {
                    UnityApp.Disconnect();
                    return Ok("POST OK !");
                }
                else
                {
                    if (order.ErrorCode != 0)
                    {
                        UnityApp.Disconnect();
                        return BadRequest("DBError(" + order.ErrorCode.ToString() + ")-" + order.ErrorDesc + order.DBErrorDesc);
                    }
                    else if (order.ValidateErrors.Count > 0)
                    {
                        string hataMesaji = "XML ErrorList:";
                        for (int i = 0; i < order.ValidateErrors.Count; i++)
                        {
                            hataMesaji += "(" + order.ValidateErrors[i].ID.ToString() + ") - " + order.ValidateErrors[i].Error;
                        }

                        UnityApp.Disconnect();
                        return BadRequest(hataMesaji);
                    }
                    else
                    {
                        UnityApp.Disconnect();
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
