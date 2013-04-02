using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VanickPOCOrders.Layouts.VanickPOCOrders
{
    public partial class Orders : LayoutsPageBase
    {
        public DataTable table;

        private SPGridView grid;
        private ObjectDataSource gridDS;

        protected void Page_Load(object sender, EventArgs e)
        {
            //CreateOrders();
        }

        protected sealed override void Render(HtmlTextWriter writer)
        {
            //GenerateColumns();
            //grid.DataBind();
            OrdersGridsp.DataSource = SelectData();
            OrdersGridsp.DataBind();
            base.Render(writer);
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }
        protected sealed override void CreateChildControls()
        {
            const string GRIDID = "grid";
            const string DATASOURCEID = "gridDS";

            gridDS = new ObjectDataSource();
            gridDS.ID = DATASOURCEID;
            gridDS.SelectMethod = "SelectData";
            gridDS.TypeName = this.GetType().AssemblyQualifiedName;
            gridDS.ObjectCreating += new ObjectDataSourceObjectEventHandler(gridDS_ObjectCreating);
            this.Controls.Add(gridDS);

            grid = new SPGridView();
            grid.ID = GRIDID;            
            grid.DataSourceID = gridDS.ID;
            grid.AutoGenerateColumns = false;

            // Paging
            grid.AllowPaging = true;
            grid.PageSize = 5;

            // Sorting
            grid.AllowSorting = true;

            this.Controls.Add(grid);

            SPGridViewPager pager = new SPGridViewPager();
            pager.GridViewId = grid.ID;

            this.Controls.Add(pager);
        }

        private void gridDS_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }

        public DataTable SelectData()
        {
            DataTable dataSource = new DataTable();

            dataSource.Columns.Add("ID");
            dataSource.Columns.Add("Name");
            dataSource.Columns.Add("Region");
            dataSource.Columns.Add("Total Sales");

            dataSource.Rows.Add(1, "J. Smith", "Europe", 10000);
            dataSource.Rows.Add(2, "J. Smith", "North America", 15000);
            dataSource.Rows.Add(3, "J. Smith", "Asia", 5000);
            dataSource.Rows.Add(4, "S. Jones", "Europe", 7000);
            dataSource.Rows.Add(5, "S. Jones", "North America", 30000);
            dataSource.Rows.Add(6, "S. Jones", "Asia", 8700);
            dataSource.Rows.Add(7, "W. Nguyen", "Europe", 3000);
            dataSource.Rows.Add(8, "W. Nguyen", "North America", 50000);
            dataSource.Rows.Add(9, "W. Nguyen", "Asia", 25000);

            return dataSource;
        }

        private void GenerateColumns()
        {
            BoundField column = new BoundField();
            column.DataField = "ID";
            column.SortExpression = "ID";
            column.HeaderText = "ID";
            grid.Columns.Add(column);

            column = new BoundField();
            column.DataField = "Name";
            column.SortExpression = "Name";
            column.HeaderText = "Name";
            grid.Columns.Add(column);

            column = new BoundField();
            column.DataField = "Region";
            column.SortExpression = "Region";
            column.HeaderText = "Region";
            grid.Columns.Add(column);

            column = new BoundField();
            column.DataField = "Total Sales";
            column.SortExpression = "Total Sales";
            column.HeaderText = "Total Sales";
            grid.Columns.Add(column);
        }

        private void CreateOrders()
        {
            try
            {
                using(SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using(SPWeb web = site.OpenWeb())
                    {
                        table = new DataTable();
                        table.Columns.Add("Order ID", typeof(Int32));
                        table.Columns.Add("Order", typeof(string));
                        table.Columns.Add("Order Status", typeof(string));

                        SPList OrderList = web.Lists["Orders"];
                        SPQuery query = new SPQuery();
                        SPListItemCollection coll = OrderList.GetItems(query);

                        foreach (SPItem gt in coll)
                        {
                            DataRow dr;
                            dr = table.NewRow();
                            dr["Order ID"] = gt.ID;
                            dr["Order"] = gt["Title"].ToString();
                            if(gt["Order Status"]!= null)
                                dr["Order Status"] = gt["Order Status"].ToString();
                            table.Rows.Add(dr);
                        }

                        //OrdersGridf.DataSource = table;
                        //OrdersGridf.DataBind();

                    }
                }
            }
            catch
            {

            }
        }
    }
}
