using DynamicTables.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DynamicTables.Controllers
{
    public class HomeController : Controller
    {
        Tables tableList = new Tables();

        // GET: Home
        public ActionResult Index()
        {
            return View(tableList.FillList());
        }
         public JsonResult getCity(string id)
        {
            string countrystring = "select * from City where CountryId=" + id + "";
            List<SelectListItem> list = new List<SelectListItem>();
            tableList.Connection();
            if (tableList.sqlConnection.State == ConnectionState.Open)
            {
                SqlCommand komut = new SqlCommand(countrystring, tableList.sqlConnection);
                DataTable Tablo = new DataTable();
                SqlDataAdapter Adaptor = new SqlDataAdapter(komut);
                Adaptor.Fill(Tablo);

                foreach (DataRow row in Tablo.Rows)
                {

                    list.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[1]), Value = Convert.ToString(row.ItemArray[0]) });
                    
                }
                
            }
            return Json(new { result = list }, JsonRequestBehavior.AllowGet);
        }
        // GET: Home/Create
        public ActionResult Create()
        {
            FillforeignDictionay();
            ViewBag.prikeylist = Tables.prilist;
            return View(tableList.GetTableColumn());
        }
        public SelectList GenelSelectList(string ad)

        {
            List<SelectListItem> genel = new List<SelectListItem>();
            int a = tableList.GetTableColumn().Rows.Count;
            string b;
            b = "select  * " + " from " + ad;

            tableList.Connection();
            if (tableList.sqlConnection.State == ConnectionState.Open)
            {
                SqlCommand komut = new SqlCommand(b, tableList.sqlConnection);
                DataTable Tablo = new DataTable();
                SqlDataAdapter Adaptor = new SqlDataAdapter(komut);
                Adaptor.Fill(Tablo);

                foreach (DataRow row in Tablo.Rows)
                {

                    genel.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[1]), Value = Convert.ToString(row.ItemArray[0]) });
                    
                }
            }
            return new SelectList(genel, "Value", "Text" );
        }
        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            try
            {
                tableList.Connection();
                string sqlcom = "";
                sqlcom = "insert into " + Tableproperty.tablename + " (";
                int index1;
                foreach (var key in form.AllKeys)
                {
                    if (!Tables.prilist.Contains(key))
                    { sqlcom += key + " ,"; }


                }
                index1 = sqlcom.LastIndexOf(',');
                sqlcom = sqlcom.Remove(index1, 1);
                sqlcom += ") values ('";
                foreach (var key in form.AllKeys)
                {
                    if (!Tables.prilist.Contains(key))
                    { sqlcom += form[key] + "','"; }


                }
                index1 = sqlcom.LastIndexOf(','); 
                sqlcom = sqlcom.Remove(index1, 1);
                index1 = sqlcom.LastIndexOf('\'');
                sqlcom = sqlcom.Remove(index1, 1);
                sqlcom += ")";

                SqlCommand komut = new SqlCommand(sqlcom, tableList.sqlConnection);

                komut.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch
            {
                FillforeignDictionay();//foreignkey listesini doldurup viewa yönlendirir
                ViewBag.prikeylist = Tables.prilist;//primarykey listesini doldurup viewa yönlendirir
                ViewBag.Hata = "Islem Gerceklesmedi! \n Lütfen bütün alanları doldurarak tekrar deneyiniz.";//Hata olusursa mesajı viewa yönlendirir
                return View(tableList.GetTableColumn());//kolon isimlerini gönderir
            }

        }
        // GET: Home/Edit/5
        public ActionResult Edit(String Table_name)
        {
            return View(tableList.FillByTableName(Table_name));//secilen tabloya göre verileri listelenmesi için FillByTableName methodu çağrılır
        }
        public ActionResult EditRecord(int id)
        {
            FillforeignDictionay();//foreignkeyi doldurarak viewa gönderilir
            ViewBag.prikeylist = Tables.prilist;//primarykey listesi doldurulur ve viewa göndeerilir
             return View(tableList.Recordİnfobyid(id, Tableproperty.tablename));//verilen id ye göre veriler doldurulur recordinfobyid methodu ile

        }
        [HttpPost]
        public ActionResult EditRecord(int id, FormCollection form)//verilen idye ait bilgilerin güncellenmesi
        {
            try
            {
                tableList.Connection();

                string sqlcom = "";
                sqlcom = "Update  " + Tableproperty.tablename + "  set ";


                foreach (var key in form.AllKeys)
                {
                    if (!Tables.prilist.Contains(key))//primarykey değilse kolon ismi ve değeri formcollectiondaki key kullanılarak sorguya eklenir
                    {
                        sqlcom += key + " = '" + form[key] + "' ,";
                    }

                }
                int index1 = sqlcom.LastIndexOf(',');//sorgu sonundaki , karakterini ulur ve kaldırır
                sqlcom = sqlcom.Remove(index1, 1);
                sqlcom += " where " + Tables.prilist[0].ToString() + "=" + id;//primarykey ismi listeden çekilip parametre gelen idye eşlenir 
                SqlCommand komut = new SqlCommand(sqlcom, tableList.Connection());
                komut.ExecuteNonQuery();
                return RedirectToAction("Index");

            }
            catch
            {
                FillforeignDictionay();//foreignkey dolduruluo viewbag kullanılarak viewa iletilir
                ViewBag.prikeylist = Tables.prilist;//primarykey listesi doldurulup viewa gönderilir
                ViewBag.Hata = "Islem Gerceklesmedi! \n Lütfen bütün alanları doldurarak tekrar deneyiniz.";//Edit işlemi gerçekleşmezse hata mesajını viewa gönderir
                return View(tableList.Recordİnfobyid(id, Tableproperty.tablename));//verilen id ye göre veriler doldurulur recordinfobyid methodu ile

            }

        }

        public ActionResult Delete(int id)
        {
            try
            {
                tableList.delete(id);// verilen id ye göre delete metodu çalıştırılır

            
            }
            catch
            {
                TempData["Hata"] = "Silme Islemi Gercekleşmedi. Tekrar Deneyiniz! ";//Silme işlemi gerçekleşmediğinde index sayfasına hata gönderir.
            }
            return RedirectToAction("Index");
        }

        public void FillforeignDictionay()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            DataTable dataTable = tableList.GetTableColumn();
            int a = tableList.GetTableColumn().Rows.Count;

            if (dataTable.Rows.Count != 0)
            {
                for (int i = 0; i < a; i++)
                {

                    if (dataTable.Rows[i]["Referenced_column_name"].ToString() != "")//foreignkeyi varsa 
                    {
                        dictionary.Add(dataTable.Rows[i]["Referenced_column_name"].ToString(), dataTable.Rows[i]["Referenced_object"].ToString());
                        //foreignkey kolonunu ve referenced tablonun ismi dictionarye eklenir

                    }
                }

                ViewBag.foreigncolumn = dictionary;//dictionary viewa gönderilir
            }
        }
    }
}
