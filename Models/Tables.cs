using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicTables.Models
{
    public class Tables
    {

        public SqlConnection sqlConnection;
        SqlDataAdapter Adaptor;
        DataTable Tablo;
        SqlCommand komut;
        public static List<string> prilist = new List<string>();
        public List<Tableproperty> tablenameList { get; set; }
        public SqlConnection Connection()
        {
            sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=DESKTOP;Initial Catalog=DOS;Integrated Security=True;";
            sqlConnection.Open();
            return sqlConnection;
        }
        public List<Tableproperty> FillList()
        {
            Connection();
            if (sqlConnection.State == ConnectionState.Open)
            {
                komut = new SqlCommand("GetTableNames", sqlConnection);
                komut.CommandType = CommandType.StoredProcedure;
                Tablo = new DataTable();
                Adaptor = new SqlDataAdapter(komut);
                Adaptor.Fill(Tablo);
                tablenameList = new List<Tableproperty>();
                for (int i = 0; i < Convert.ToInt32(Tablo.Rows.Count); i++)
                {
                    Tableproperty tableproperty = new Tableproperty();
                    tableproperty.TABLE_NAME = Convert.ToString(Tablo.Rows[i]["TABLE_NAME"]);
                    tablenameList.Add(tableproperty);
                }

            }
            return tablenameList;
        }
        public DataTable FillByTableName(string tablenames) 
        {
           
            Tableproperty.tablename = tablenames;
            prikey();
            Connection();
            if (sqlConnection.State == ConnectionState.Open)
            {
                komut = new SqlCommand("GetRecordofDynamictable", sqlConnection);
                komut.CommandType = CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@tableName", Tableproperty.tablename);
                Tablo = new DataTable();
                Adaptor = new SqlDataAdapter(komut);
                Adaptor.Fill(Tablo);
            }
            return Tablo;
        }
        public DataTable Recordİnfobyid(int i, string tablename)
        {
          
            if (sqlConnection.State == ConnectionState.Open)
            {
                komut = new SqlCommand("Recordİnfobyid", sqlConnection);
                komut.CommandType = CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@id", i);
                komut.Parameters.AddWithValue("@tableName", Tableproperty.tablename);
                komut.Parameters.AddWithValue("@pri", prilist[0]);
                Tablo = new DataTable();
                Adaptor = new SqlDataAdapter(komut);
                Adaptor.Fill(Tablo);
            }
            return Tablo;
        }
        public List<string> prikey()
        {

            Connection();
            prilist.Clear();
            if (sqlConnection.State == ConnectionState.Open)
            {
                komut = new SqlCommand("prikey", sqlConnection);
                komut.CommandType = CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@TableName", Tableproperty.tablename);
                Tablo = new DataTable();
                Adaptor = new SqlDataAdapter(komut);
                Adaptor.Fill(Tablo);
                int satir = Convert.ToInt32(Tablo.Rows.Count);

                for (int i = 0; i < satir; i++)
                {
                    string a;
                    a = Tablo.Rows[i]["COLUMN_NAME"].ToString(); 
                    prilist.Add(a);
                }


            }

            return prilist;
        }
        public void delete(int id)
        {
                Connection();
                if (sqlConnection.State == ConnectionState.Open)
                {
                    string x = "Delete from " + Tableproperty.tablename + " where " + prilist[0].ToString() + " =" + id; // tablo ismi ve gelen id nin eşitleneceği primarykey verilen delete sorgusu

                    komut = new SqlCommand(x, sqlConnection);

                    komut.ExecuteNonQuery();

                }
                Connection().Close();
            

        }
        public DataTable GetTableColumn()
        {
            DataTable createtable = new DataTable();
            Connection();
            if (sqlConnection.State == ConnectionState.Open)
            {
                komut = new SqlCommand("getcolumnnamewithforeigtable", sqlConnection);
                komut.CommandType = CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@Tables", Tableproperty.tablename);
                Adaptor = new SqlDataAdapter(komut);
                Adaptor.Fill(createtable);
            }
            return createtable;
        }
    }
    public class Tableproperty
    {
        public String TABLE_NAME { get; set; }
        public static String tablename { get; set; }
    } 

}

