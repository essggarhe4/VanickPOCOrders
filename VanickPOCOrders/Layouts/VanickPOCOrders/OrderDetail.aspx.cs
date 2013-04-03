using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace VanickPOCOrders.Layouts.VanickPOCOrders
{
    public partial class OrderDetail : LayoutsPageBase
    {
        string orderID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetDetail();
            }
        }

        private void GetDetail()
        {
            orderID = this.Page.Request.QueryString["orderid"];
            if (!string.IsNullOrEmpty(orderID))
            {
                DisplayDetail(orderID);
            }
        }        

        private void DisplayDetail(string OrderID)
        {
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        List<objOrderDetail> lstOrder = new List<objOrderDetail>();
                        SPList OrderList = web.Lists["OrdersDetail"];
                        SPQuery query = new SPQuery();
                        query.Query = string.Format("<Where><Eq><FieldRef Name='OrderID' /><Value Type='Text'>{0}</Value></Eq></Where>", OrderID);
                        SPListItemCollection coll = OrderList.GetItems(query);
                        DataRow dr;
                        foreach (SPItem gt in coll)
                        {
                            lstOrder.Add(new objOrderDetail
                            {
                                ID = gt.ID.ToString(),
                                Detail = gt["Title"].ToString()
                            });    
                        }

                        //lstOrder.Add(new objOrderDetail
                        //{
                        //    ID = "1",
                        //    Detail = "uno"
                        //});

                        CalendaRepeater.DataSource = lstOrder;
                        CalendaRepeater.DataBind();

                        foreach (RepeaterItem item in CalendaRepeater.Items)
                        {
                            HtmlGenericControl iddd = (HtmlGenericControl)item.FindControl("OARDERiddIV"); 
                           
                            TextBox box = (TextBox)item.FindControl("OrdenDetailText");
                            box.Text = lstOrder.Find(o => o.ID == iddd.InnerText).Detail;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        protected void SaveBtn_Click(Object sender, EventArgs e)
        {
            List<objOrderDetail> savelist = new List<objOrderDetail>();

            foreach (RepeaterItem item in CalendaRepeater.Items)
            {
                HtmlGenericControl iddd = (HtmlGenericControl)item.FindControl("OARDERiddIV");
                TextBox box = (TextBox)item.FindControl("OrdenDetailText");
                savelist.Add(new objOrderDetail
                {
                    ID = iddd.InnerText,
                    Detail = box.Text
                });
            }

            UpdateIrtems(savelist);

            UpdateOrder();


            Response.Redirect("/_layouts/VanickPOCOrders/orders.aspx");
        }

        private void UpdateIrtems(List<objOrderDetail> listtosave)
        {
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        web.AllowUnsafeUpdates = true;
                        web.Update();
                        List<objOrderDetail> lstOrder = new List<objOrderDetail>();
                        SPList OrderList = web.Lists["OrdersDetail"];

                        foreach (objOrderDetail ob in listtosave)
                        {
                            SPListItem item = OrderList.GetItemById(int.Parse(ob.ID));
                            item["Title"] = ob.Detail;
                            item.Update();
                        }
                        OrderList.Update();
                        web.AllowUnsafeUpdates = false;
                    }
                }
            }
            catch
            {

            }
        }

        private void UpdateOrder()
        {
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        web.AllowUnsafeUpdates = true;
                        web.Update();
                        List<objOrderDetail> lstOrder = new List<objOrderDetail>();
                        SPList OrderList = web.Lists["Orders"];
                        SPListItem item = OrderList.GetItemById(int.Parse(this.Page.Request.QueryString["orderid"].ToString()));                        
                        item["Order Status"] = string.Format("Last update {0}", DateTime.Now.ToLongTimeString());
                        item.Update();                        
                        OrderList.Update();
                        web.AllowUnsafeUpdates = false;
                    }
                }
            }
            catch
            {

            }
        }
    }
}
