using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GoogleVisualizationAPI;

public partial class test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt1 = new DataTable();
        dt1.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "Date" });
        dt1.AddDataColumn(new DataColumn() { DataType = DataType.Number, Name = "Russia" });
        dt1.AddDataColumn(new DataColumn() { DataType = DataType.Number, Name = "Usa" });
        dt1.AddDataColumn(new DataColumn() { DataType = DataType.Number, Name = "Germany" });
        dt1.AddDataCell(new DataCell() { Caption = "01/03/2009", Value = new List<string> { "123", "30", "144" } });
        dt1.AddDataCell(new DataCell() { Caption = "01/06/2009", Value = new List<string> { "50", "60", "40" } });
        dt1.AddDataCell(new DataCell() { Caption = "01/09/2009", Value = new List<string> { "80", "180", "80" } });
        dt1.AddDataCell(new DataCell() { Caption = "01/12/2009", Value = new List<string> { "150", "50", "120" } });
        GVAPI.Visualization.Add(new Visualization() { Package = VisualizationPackage.LineChart, DataTable = dt1, DestObjectName = this.visLineChart.ID });

        DataTable dt2 = new DataTable();
        dt2.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "Name" });
        dt2.AddDataColumn(new DataColumn() { DataType = DataType.Number, Name = "Value" });
        dt2.AddDataColumn(new DataColumn() { DataType = DataType.Number, Name = "Value" });
        dt2.AddDataCell(new DataCell() { Caption = "Germany", Value = new List<string> { "20" } });
        dt2.AddDataCell(new DataCell() { Caption = "Poland", Value = new List<string> { "150" } });
        dt2.AddDataCell(new DataCell() { Caption = "France", Value = new List<string> { "70" } });
        GVAPI.Visualization.Add(new Visualization() { Package = VisualizationPackage.PieChart, DataTable = dt2, DestObjectName = this.visPieChart.ID });

        DataTable dt3 = new DataTable();
        dt3.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "Name" });
        dt3.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "Manager" });
        dt3.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "Tooltip" });
        dt3.AddDataCell(new DataCell() { Caption = "Alexander Bykin", Value = new List<string> { "", "Perfect Programmer" } });
        dt3.AddDataCell(new DataCell() { Caption = "Steve Ballmer", Value = new List<string> { "", "Microsoft General President" } });
        dt3.AddDataCell(new DataCell() { Caption = "Scott Guthrie", Value = new List<string> { "", "Vice president in the Microsoft Developer Division." } });
        GVAPI.Visualization.Add(new Visualization() { Package = VisualizationPackage.OrgChart, DataTable = dt3, DestObjectName = this.visOrgChart.ID });

        DataTable dt4 = new DataTable();
        dt4.AddDataColumn(new DataColumn() { DataType = DataType.Date, Name = "Date" });
        dt4.AddDataColumn(new DataColumn() { DataType = DataType.Number, Name = "Sold Pencils" });
        dt4.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "title1" });
        dt4.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "text1" });
        dt4.AddDataColumn(new DataColumn() { DataType = DataType.Number, Name = "Sold Pens" });
        dt4.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "title2" });
        dt4.AddDataColumn(new DataColumn() { DataType = DataType.String, Name = "text2" });
        dt4.AddDataCell(new DataCell() { Caption = "01/03/2009", Value = new List<string> { "3000", "", "", "40645", "" } });
        dt4.AddDataCell(new DataCell() { Caption = "01/06/2009", Value = new List<string> { "14045", "", "", "20374", "" } });
        dt4.AddDataCell(new DataCell() { Caption = "01/09/2009", Value = new List<string> { "55022", "", "", "50766", "" } });
        dt4.AddDataCell(new DataCell() { Caption = "01/12/2009", Value = new List<string> { "75284", "", "", "14334", "" } });
        GVAPI.Visualization.Add(new Visualization() { Package = VisualizationPackage.AnnotatedTimeLine, DataTable = dt4, DestObjectName = this.visAnnotatedTimeLine.ID });
    }
}