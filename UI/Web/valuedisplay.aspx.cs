using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;
using System.Data;

public partial class valuedisplay : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        try
        {
            DataTable GridData = new DataTable();
            GridData.Columns.Add("Item", typeof(string)); // Add columns
	        GridData.Columns.Add("Value", typeof(string));
            DataTable CustomTable = OSAESql.RunSQL("SELECT item_name, Item_label FROM osae.osae_v_object_property_array where object_name='Custom Property List' ORDER BY item_name").Tables[0];
            //Response.Write("Rows= " + CustomTable.Rows.Count);
            foreach (DataRow row in CustomTable.Rows)
            {
                String[] SplitString = row.Field<String>(0).Split(new string[] { ":=" }, StringSplitOptions.None);
                String MyLabel = row.Field<String>(1);
                				
				if (String.Equals(SplitString[1], "State", StringComparison.OrdinalIgnoreCase))
				{
					//Response.Write("<br>State");
					DataRow MyRow = OSAESql.RunSQL("SELECT CONCAT(object_name, ' - State') As Item, state_name As 'Value'  FROM osae_v_object WHERE Object_name= '" + SplitString[0] + "'").Tables[0].Rows[0];
					
					if (MyLabel == "")
					{
						MyLabel = MyRow[0].ToString();
					}
					GridData.Rows.Add(MyLabel, MyRow[1]);
				}
				else
				{
					//Response.Write("<br>Property");
					DataRow MyRow = OSAESql.RunSQL("SELECT CONCAT(object_name, ' - ', property_name) As Item, property_value As 'Value'  FROM osae_v_object_property WHERE Object_name= '" + SplitString[0] + "' AND property_name= '" + SplitString[1] + "'").Tables[0].Rows[0];
					
					if (MyLabel == "")
					{
						MyLabel = MyRow[0].ToString();
					}
					GridData.Rows.Add(MyLabel, MyRow[1]);
				}
            }
            valueDisplayGridView.DataSource = GridData;
            valueDisplayGridView.DataBind();
        }
        catch (Exception ex)
        {
            Master.Log.Error("Error retreiving values", ex);
        }
    }
}