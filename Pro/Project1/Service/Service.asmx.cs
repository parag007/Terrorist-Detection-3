using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace Project1.Service
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /*  
        * Method Name   - GetTable
        * Created By    - Suraj Patil
        * Created On    - 03 Nov 2017
        * Modified By   - 
        * Modified On   - 
        * Purpose       - 
        */
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetTable()
        {
            string str_Json = string.Empty;

            DataAccessLayer Con = new DataAccessLayer();
            DataSet ds = Con.GetDataTablesFromSP("GET_TABLE", null);

            string newJson = string.Empty;
            if (ds != null)
            {
                List<object> parentList = new List<object>();

                DataTable dtDistinct = ds.Tables[0];
                DataTable dtAll = ds.Tables[1];

                if (dtDistinct.Rows.Count > 0)
                {

                    for (int i = 0; i < dtDistinct.Rows.Count; i++)
                    {
                        List<object> childList = new List<object>();

                        var objDistinct = dtDistinct.Rows[i];

                        Dictionary<string, object> f = new Dictionary<string, object>();

                        for (int j = 0; j < dtAll.Rows.Count; j++)
                        {
                            Dictionary<string, object> f2 = new Dictionary<string, object>();

                            var objAll = dtAll.Rows[j];
                            if (objDistinct["docID"].ToString() == objAll["docID"].ToString())
                            {
                                f2.Add("ID", objAll["ID"].ToString());
                                f2.Add("name", objAll["docID"].ToString());
                                f2.Add("docText", objAll["docText"].ToString());
                                f2.Add("Date", objAll["Date"].ToString());
                                f2.Add("Phone", objAll["Phone"].ToString());
                                f2.Add("Organization", objAll["Organization"].ToString());
                                f2.Add("IS_SUSPECT", objAll["IS_SUSPECT"].ToString());
                                f2.Add("IS_DELETED", objAll["IS_DELETED"].ToString());
                                childList.Add(f2);
                            }
                        }

                        f.Add("ID", -1);
                        f.Add("name", objDistinct["docID"].ToString());

                        f.Add("children", childList);

                        parentList.Add(f);

                    }

                }

                var jsonSerialiser = new JavaScriptSerializer();
                str_Json = jsonSerialiser.Serialize(parentList);
                
                str_Json = "{\"name\":\"doc\",\"children\":" + str_Json + "}";
            }

            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(str_Json);
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeletRecord()
        {
            string ID = HttpContext.Current.Request.Params["ID"];
            string IS_RETRIEVE = HttpContext.Current.Request.Params["IS_RETRIEVE"];

            string str_Json = string.Empty;

            DataAccessLayer Con = new DataAccessLayer();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@p_ID", ID);
            param.Add("@p_IS_RETRIEVE", IS_RETRIEVE);

            DataSet ds = Con.GetDataTablesFromSP("DELETE_RECORD", param);

            if (ds != null)
            {
                str_Json = "[{\"success\":1}]";
            }

            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(str_Json);
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateRecord()
        {
            string ID = HttpContext.Current.Request.Params["ID"];
            string docID = HttpContext.Current.Request.Params["docID"];
            string docText = HttpContext.Current.Request.Params["docText"];
            string Date = HttpContext.Current.Request.Params["Date"];
            string Phone = HttpContext.Current.Request.Params["Phone"];
            string Organization = HttpContext.Current.Request.Params["Organization"];

            string str_Json = string.Empty;

            DataAccessLayer Con = new DataAccessLayer();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@p_ID", ID);
            param.Add("@p_docID", docID);
            param.Add("@p_docText", docText);
            param.Add("@p_Date", Date);
            param.Add("@p_Phone", Phone);
            param.Add("@p_Organization", Organization);

            DataSet ds = Con.GetDataTablesFromSP("CREATE_UPDATE_RECORD", param);

            if (ds != null)
            {
                str_Json = "[{\"success\":1}]";
            }
            
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(str_Json);
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateRecord()
        {
            string docID = HttpContext.Current.Request.Params["docID"];
            string docText = HttpContext.Current.Request.Params["docText"];
            string Date = HttpContext.Current.Request.Params["Date"];
            string Phone = HttpContext.Current.Request.Params["Phone"];
            string Organization = HttpContext.Current.Request.Params["Organization"];

            string str_Json = string.Empty;

            DataAccessLayer Con = new DataAccessLayer();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@p_ID", 0);
            param.Add("@p_docID", docID);
            param.Add("@p_docText", docText);
            param.Add("@p_Date", Date);
            param.Add("@p_Phone", Phone);
            param.Add("@p_Organization", Organization);

            DataSet ds = Con.GetDataTablesFromSP("CREATE_UPDATE_RECORD", param);

            if (ds != null)
            {
                str_Json = "[{\"ID\":" + ds.Tables[0].Rows[0]["ID"].ToString() + "}]";
            }
            
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(str_Json);
        }


    }
}
