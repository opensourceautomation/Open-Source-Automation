using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OSAE;

public partial class importexport : System.Web.UI.Page
{
    int multi = 0;
    List<string> xObjL = new List<string>();
    List<ExportObject> exportObjList = new List<ExportObject>();
    
    string xType = "";

    public void RaisePostBackEvent(string eventArgument)
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx?ReturnUrl=importexport.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("ObjectType Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");
        if (IsPostBack)
        {
            if (FileLoader.HasFile)
            {
                if (System.IO.Path.GetExtension(FileLoader.FileName).ToLower() != ".jpg" && System.IO.Path.GetExtension(FileLoader.FileName).ToLower() != ".png" && System.IO.Path.GetExtension(FileLoader.FileName).ToLower() != ".jpeg" && System.IO.Path.GetExtension(FileLoader.FileName).ToLower() != ".gif" && System.IO.Path.GetExtension(FileLoader.FileName).ToLower() != ".sql" && System.IO.Path.GetExtension(FileLoader.FileName).ToLower() != ".osapkg")
                {
                    Master.Log.Error("Incorrect Import File Type");
                    lblImportWarn.Visible = true;
                }
                else
                {
                    lblImportWarn.Visible = false;
                    btnImport.Enabled = true;
                    btnClear1.Enabled = true;
                }
            }
            else
            {
                btnImport.Enabled = false;
                btnClear1.Enabled = false;
            }
            if (ddlImportType.SelectedValue == "Package")
            {
                ddlImportType.Enabled = false;
                btnClear1.Enabled = true;
            }
        }
        else
        {
            if (Request["eType"] != null)
            {
                ddlExportType.SelectedValue = Request["eType"];
                ddlExportType_SelectedIndexChanged(this, null);
                ddlObjToExport.SelectedValue = Request["eObject"];
                ddlObjToExport_SelectedIndexChanged(this, null);
                if (Request["eType"] == "Screen")
                {
                    ckbExportMulti.Checked = true;
                    btnAddFile_Click(this, null);
                    txtZipName.Focus();
                }
            }
        }
        if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "move")
        {
            if (ckbExportMulti.Checked)
            {
                btnAddFile_Click(this, null);
            }
        }
        if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "remove")
        {
            int sItem = lstFileList.SelectedIndex;
            lstFileList.Items.RemoveAt(sItem);
        }
        ddlObjToExport.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(ddlObjToExport, "move"));
        lstFileList.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(ddlObjToExport, "remove"));
    }

    protected void ckbImportMulti_CheckedChanged(object sender, EventArgs e)
    {
        string impType = "";
        if (ddlImportType.SelectedValue == "Package")
        {
            ddlImportType.Enabled = false;
            FileLoader.Visible = true;
            FileLoader.Enabled = true;
            impType = ".osapkg, *.*";
            FileLoader.Attributes.Add("Accept", impType);
        }
        else
        {
            ddlImportType.Enabled = true;
            FileLoader.Visible = false;
            FileLoader.Enabled = false;
        }
    }

    protected void ddlImportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string oType = xType;
        xType = ddlImportType.SelectedItem.Text;
        btnImport.Enabled = false;
        btnClear1.Enabled = false;
        if (xType != "Select an Import Type")
        {
            FileLoader.Visible = true;
            FileLoader.Enabled = true;
            string impType = "";
            if( xType == "Image")
            {
                impType = "image/*";
            }
            else if (xType == "Package")
            {
                impType = ".osapkg";
            }
            else
            {
                impType = ".sql";
            }
            FileLoader.Attributes.Add("Accept", impType);
            lblSelectImport.Text = "Click Browse to select the " + xType + " file to Import: ";
            if (FileLoader.HasFile)
            {
                if (oType != xType)
                {
                    btnImport.Enabled = false;
                    btnClear1.Enabled = false;
                }
                else
                {
                    btnImport.Enabled = true;
                    btnClear1.Enabled = true;
                }
            }
            else
            {
                btnImport.Enabled = false;
                btnClear1.Enabled = false;
            }
        }
        else
        {
            FileLoader.Visible = false;
            FileLoader.Enabled = false;
            lblSelectImport.Text = "Select an Import Type above.";
            btnImport.Enabled = false;
            btnClear1.Enabled = false;
        }
    }

    protected void FileLoader_DataBinding(object sender, EventArgs e)
    {
        if(FileLoader.HasFile)
        {
            if (Path.GetExtension(FileLoader.FileName).ToLower() != ".jpg" && Path.GetExtension(FileLoader.FileName).ToLower() != ".png" && Path.GetExtension(FileLoader.FileName).ToLower() != ".jpeg" && Path.GetExtension(FileLoader.FileName).ToLower() != ".gif" && Path.GetExtension(FileLoader.FileName).ToLower() != ".sql" && Path.GetExtension(FileLoader.FileName).ToLower() != ".osapkg")
            {
                Master.Log.Error("Incorrect Import File Type");
                lblImportWarn.Visible = true;
            }
            else
            {
                lblImportWarn.Visible = false;
                btnImport.Enabled = true;
                btnClear1.Enabled = true;
            }
        }
        else
        {
            btnImport.Enabled = false;
            btnClear1.Enabled = false;
            lblImportWarn.Visible = false;
        }
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        if(FileLoader.HasFile)
        {
            if (Path.GetExtension(FileLoader.FileName).ToLower() != ".jpg" && Path.GetExtension(FileLoader.FileName).ToLower() != ".png" && Path.GetExtension(FileLoader.FileName).ToLower() != ".jpeg" && Path.GetExtension(FileLoader.FileName).ToLower() != ".gif" && Path.GetExtension(FileLoader.FileName).ToLower() != ".sql" && Path.GetExtension(FileLoader.FileName).ToLower() != ".osapkg")
            {
                lblImportWarn.Visible = true;
                Master.Log.Info("Can Not Import Incorrect File Type");
                return;
            }
            else
            {
                lblImportWarn.Visible = false;
                string iType = ddlImportType.SelectedValue;
                string ifType = Path.GetExtension(FileLoader.FileName).ToLower();
                if(iType == "Sql")
                {
                    if(ifType == ".sql")
                    {
                        Master.Log.Info("Importing new SQL file: " + FileLoader.FileName);
                        importObject(FileLoader.PostedFile.InputStream);
                        Master.Log.Info("Imported SQL file: " + FileLoader.FileName + " - By User: " + Session["Username"]);
                    }
                    else
                    {
                        lblImportWarn.Visible = true;
                        Master.Log.Error("Can Not Import, Incorrect File Type");
                        return;
                    }
                }
                else if (iType == "Image")
                {
                    if (Path.GetExtension(FileLoader.FileName).ToLower() != ".jpg" && Path.GetExtension(FileLoader.FileName).ToLower() != ".png" && Path.GetExtension(FileLoader.FileName).ToLower() != ".jpeg" && Path.GetExtension(FileLoader.FileName).ToLower() != ".gif")
                    {
                        lblImportWarn.Visible = true;
                        Master.Log.Info("Image not added, incompatiable file type.");
                        return;
                    }
                    else
                    {
                        if (FileLoader.PostedFile.ContentLength < 2502400) //202400
                        {
                            Master.Log.Info("Importing new Image file: " + FileLoader.FileName);
                            importImage(FileLoader.PostedFile.InputStream, FileLoader.FileName);
                            Master.Log.Info("Imported Image: " + FileLoader.FileName + " - By User: " + Session["Username"]);
                        }
                        else
                        {
                            Master.Log.Error("Image not added, file is to large.");
                            return; //file to big
                        }
                    }
                }
                else if(iType == "Package")
                {
                    if (ifType == ".osapkg")
                    {
                        ICSharpCode.SharpZipLib.Zip.ZipFile zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(FileLoader.PostedFile.InputStream);
                        zf.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.On;
                        Master.Log.Info("Importing new Package: " + FileLoader.FileName);
                        foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry in zf)
                        {
                            if (!zipEntry.IsFile)
                            {
                                continue;
                            }
                            zipEntry.ForceZip64();
                            string entryFileName = zipEntry.Name;
                            string fType = Path.GetExtension(entryFileName);
                            entryFileName = Path.GetFileNameWithoutExtension(entryFileName);
                            //byte[] buffer = new byte[4096];     // 4K is optimum
                            Stream zipStream = zf.GetInputStream(zipEntry);
                            if(fType == ".sql")
                            {
                                Master.Log.Info("Importing new SQL file: " + entryFileName);
                                importObject(zipStream);
                            }
                            else if(fType == ".png" || fType == ".jpg" || fType == ".jpeg" || fType == ".gif")
                            {
                                Master.Log.Info("Importing new Image file: " + entryFileName + fType);
                                importImage(zipStream, entryFileName);
                            }
                        }
                        Master.Log.Info("Imported package: " + FileLoader.FileName + " - By User: " + Session["Username"]);
                    }
                    else
                    {
                        lblImportWarn.Visible = true;
                        return;
                    }
                }
                else
                {
                    lblImportWarn.Visible = true;
                    return;
                }
            }
        }
        else
        {
            lblImportWarn.Visible = false;
            btnImport.Enabled = false;
            btnClear1.Enabled = false;
        }
        btnClear_Click(this, null);
    }

    private void importObject(Stream stream)
    {
        StreamReader reader = new StreamReader(stream);
        string objSQL = "";
        do
        {
            string sqlLine = reader.ReadLine();
            objSQL += sqlLine;
        } while (reader.Peek() != -1);
        reader.Close();
        OSAESql.RunSQL(objSQL);
    }

    private void importImage(Stream stream, String Name)
    {
        try
        {
            OSAEImage img = new OSAEImage();
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            img.Data = ms.ToArray();
            img.Name = Name;
            img.Type = Path.GetFileNameWithoutExtension(Name); //System.IO.Path.GetExtension(FileLoader.FileName).ToLower().Substring(1)
            var imageManager = new OSAE.OSAEImageManager();
            imageManager.AddImage(img);
        }
        catch
        {
            Master.Log.Error("There was an issue Importting the imgage: " + Name);
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/importexport.aspx");
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        string xType = ddlExportType.SelectedItem.Text;
        bool xPack = ckbExportMulti.Checked;
        if (xPack)
        {
            Master.Log.Info("Creating package: " + txtZipName.Text);
            if (txtZipName.Text == "")
            {
                lblFNError.Visible = true;
                return;
            }
            else
            {
                lblFNError.Visible = false;
            }

            using (MemoryStream OutputStream = new MemoryStream())
            {
                // Setup Zip Stream
                string zipFileName = txtZipName.Text + ".osapkg";
                ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(OutputStream);
                zipStream.SetLevel(3);
                zipStream.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.On;
                // Add each object on list to Zip in reverse order.
                int lstCount = lstFileList.Items.Count;
                if (lstCount > 0)
                {
                    //foreach (ListItem lstItem in lstFileList.Items)
                    while(lstCount > 0)
                    {
                        ListItem lstItem = lstFileList.Items[lstCount - 1];
                        int iSplit = lstItem.Text.IndexOf("::");
                        string[] args = new string[2];  //lstItem.Text.Split(':',':');
                        args[0] = lstItem.Text.Substring(0, iSplit);
                        args[1] = lstItem.Text.Substring(iSplit + 2);
                        ExportObject xObj = new ExportObject(args[0], args[1]);
                        Master.Log.Info("Adding file: " + xObj.ExportFileName + " to package: " + txtZipName.Text);
                        ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(xObj.ExportFileName);
                        zipEntry.DateTime = DateTime.Now;
                        zipEntry.Size = xObj.byteData.Length;
                        zipStream.PutNextEntry(zipEntry);
                        zipStream.Write(xObj.byteData, 0, xObj.byteData.Length);
                        zipStream.Flush();
                        zipStream.CloseEntry();
                        lstCount = lstCount - 1;
                    }
                }

                // Finish up Zip
                zipStream.IsStreamOwner = false;
                zipStream.Close();
                OutputStream.Position = 0;
                byte[] byteArray = OutputStream.GetBuffer();
                Int64 leng = byteArray.Length;
                Response.Clear();
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + zipFileName);
                Response.AppendHeader("Content-Length", leng.ToString());
                Response.ContentType = "application/zip";
                Response.BinaryWrite(byteArray);
                Response.Flush();
                Master.Log.Info("Exported package: " + txtZipName.Text + " - By User: " + Session["Username"]);
            }
        }
        else
        {
            // Only 1 File
            lstFileList.Items.Clear();
            ExportObject sExport = new ExportObject(ddlObjToExport.SelectedValue, ddlExportType.SelectedValue);
            Master.Log.Info("Exporting File: " + sExport.ExportFileName + " - By User: " + Session["Username"]);
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=\"" + sExport.ExportFileName + "\"");
            Response.Charset = "";
            if (sExport.DataType == "Text")
            {
                Response.ContentType = "application/text";
                StringBuilder sb = new StringBuilder(sExport.stringData);
                Response.Output.Write(sb.ToString());
            }
            else if(sExport.Type == "Image")
            {
                Response.ContentType = "image/" + Path.GetExtension(sExport.ExportFileName);
                Response.BinaryWrite(sExport.byteData);
            }
            else
            {
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Length", sExport.ByteSize.ToString());
                Response.BinaryWrite(sExport.byteData);
            }
            Response.Flush();
            Response.End();
            //Master.Log.Info("Exported file: " + sExport.ExportFileName + " - By User: " + Session["Username"]);
        }
        btnClear_Click(this, null);
    }

    protected void ddlObjToExport_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ckbExportMulti.Checked)
        {
            if (ddlObjToExport.SelectedIndex >= 0)
            {
                btnAddFile.Enabled = true;
            }
            else
            {
                btnAddFile.Enabled = false;
            }
            if(lstFileList.Items.Count > 0)
            {
                btnExport.Visible = true;
                btnExport.Enabled = true;
                btnClear2.Visible = true;
                btnClear2.Enabled = true;
            }
            else
            {
                btnExport.Visible = false;
                btnExport.Enabled = false;
                btnClear2.Visible = false;
                btnClear2.Enabled = false;
            }
        }
        else
        {
            if (ddlObjToExport.SelectedIndex >= 0)
            {
                btnExport.Visible = true;
                btnExport.Enabled = true;
                btnClear2.Visible = true;
                btnClear2.Enabled = true;
            }
            else
            {
                btnExport.Visible = false;
                btnExport.Enabled = false;
                btnClear2.Visible = false;
                btnClear2.Enabled = false;
            }
        }
    }

    protected void ckbExportMulti_CheckedChanged(object sender, EventArgs e)
    {
        if(ckbExportMulti.Checked)
        {
            btnAddFile.Visible = true;
            lblFileList.Visible = true;
            
            lstFileList.Visible = true;
            lstFileList.Enabled = true;
            if (lstFileList.Items.Count > 0)
            {
                lblZipName.Visible = true;
                txtZipName.Visible = true;
                btnExport.Visible = true;
                //if (!string.IsNullOrEmpty(txtZipName.Text))
                //{
                //    btnExport.Visible = true;
                //    btnExport.Enabled = true;
                //}
            }
            else
            {
                lblZipName.Visible = false;
                txtZipName.Visible = false;
                btnExport.Visible = false;
            }
        }
        else
        {
            lblZipName.Visible = false;
            txtZipName.Visible = false;
            btnAddFile.Visible = false;
            btnAddFile.Enabled = false;
            lstFileList.Visible = false;
            lstFileList.Enabled = false;
            if (ddlObjToExport.SelectedIndex >= 0)
            {
                btnExport.Visible = true;
                btnExport.Enabled = true;
            }
            else
            {
                btnExport.Visible = false;
                btnExport.Enabled = false;
            }
        }
    }

    protected void btnAddFile_Click(object sender, EventArgs e)
    {
        xType = ddlExportType.SelectedValue;
        //string aF = createExportFileName(xType, ddlObjToExport.SelectedValue);
        ExportObject exObj = new ExportObject(ddlObjToExport.SelectedValue, xType);
        string fileLstName = @ddlObjToExport.SelectedValue + "::" + xType;
        if(lstFileList.Width.Value < fileLstName.Length * 7) { lstFileList.Width = fileLstName.Length * 7; }
        if (inFileList(fileLstName))
        {
            // Do nothing, Object already in the Export List.
        }
        else
        {
            if (ddlExportType.SelectedValue == "Screen")
            {
                OSAEObjectCollection screenObjects = OSAEObjectManager.GetObjectsByContainer(ddlObjToExport.SelectedValue);
                foreach (OSAEObject obj in screenObjects)
                {
                    if (obj.Type == "CONTROL USER SELECTOR" || obj.Type == "CONTROL SCREEN OBJECTS")
                    {
                        // Do not create objects for:  User Selector or Screen Objects
                    }
                    else
                    {
                        fileLstName = @obj.Name + "::Object";
                        if (lstFileList.Width.Value < fileLstName.Length * 7) { lstFileList.Width = fileLstName.Length * 7; }
                        if (inFileList(fileLstName))
                        {
                            // Do nothing, Object already in the List.
                        }
                        else
                        {
                            // Add Object To File List
                            lstFileList.Items.Add(fileLstName);

                            // Check if object has External Files (Images or Files)
                            checkForExternal(obj.Name);
                        }
                    }
                }
            }
            else
            {
                // // Add Object To File List
                lstFileList.Items.Add(fileLstName);

                // Check if object has External Files (Images or Files)
                checkForExternal(ddlObjToExport.SelectedValue);
            }
        }

        if (lstFileList.Items.Count > 0)
        {
            lblZipName.Visible = true;
            txtZipName.Visible = true;
            btnExport.Visible = true;
            btnExport.Enabled = true;
            btnClear2.Visible = true;
            btnClear2.Enabled = true;
        }
        else
        {
            btnExport.Visible = false;
            btnExport.Enabled = false;
            btnClear2.Visible = false;
            btnClear2.Enabled = false;
        }
    }

    private void checkForExternal(string objName)
    {
        OSAEObjectPropertyCollection objProps = OSAEObjectPropertyManager.GetObjectProperties(objName);
        List<OSAEObjectProperty> oP = objProps.ToList<OSAEObjectProperty>();
        int iCount = (oP.Count(r => r.DataType == "Image"));
        int fCount = (oP.Count(r => r.DataType == "File"));
        if (iCount > 0)
        {
            // Add Image files to zip
            List<OSAEObjectProperty> imgList = (oP.FindAll(r => r.DataType == "Image"));
            foreach (OSAEObjectProperty ip in imgList)
            {
                if (!string.IsNullOrEmpty(ip.Value))
                {
                    string fileLstName = @ip.Value + "::Image";
                    if (inFileList(fileLstName))
                    {
                        // Do not add image to package. Already in list.
                    }
                    else
                    {
                        lstFileList.Items.Add(fileLstName);
                    }
                }
            }
        }
        if (fCount > 0)
        {
            // Add External files to zip
            List<OSAEObjectProperty> fileList = (oP.FindAll(r => r.DataType == "File"));
            foreach (OSAEObjectProperty fp in fileList)
            {
                if (!string.IsNullOrEmpty(fp.Value))
                {
                    string fileLstName = @fp.Value + "::File";
                    if (inFileList(fileLstName))
                    {
                        // Do not add image to package. Already in list.
                    }
                    else
                    {
                        lstFileList.Items.Add(fileLstName);
                        //exportObjList.Add(exObj);
                    }
                }
            }
        }
    }

    private bool inFileList(string fName)
    {
        if (lstFileList.Items.Count > 0)
        {
            for (int i = 0; i < lstFileList.Items.Count; i++)
            {
                if (lstFileList.Items[i].Text == fName)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false; ;
        }
    }

    protected void txtZipName_Changed(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(txtZipName.Text))
        {
            btnExport.Enabled = false;
        }
        else
        {
            btnExport.Enabled = true;
        }
    }

    protected void ddlExportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string xType = ddlExportType.SelectedItem.Text;
        if (xType != "Select an Export Type")
        {
            ddlObjToExport.Enabled = true;
            ddlObjToExport.Items.Clear();
            lblExportObj.Text = "Select the " + xType + " object to Export: ";
            switch (xType)
            {
                case "Image":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    string command = "SELECT * from osae_images";
                    DataSet ds = OSAESql.RunSQL(command);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[2].ToString());
                    }
                    break;

                case "Log":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    ddlObjToExport.Items.Add("Debug Log");
                    ddlObjToExport.Items.Add("Event Log");
                    ddlObjToExport.Items.Add("Method Log");
                    ddlObjToExport.Items.Add("Server Log");
                    command = "SELECT Logger from osae_log GROUP BY Logger";
                    ds = OSAESql.RunSQL(command);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    }
                    break;

                case "Object":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    command = "SELECT object_name from osae_object GROUP BY object_name";
                    ds = OSAESql.RunSQL(command);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    }
                    break;

                case "ObjectType":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    command = "SELECT object_type from osae_object_type GROUP BY object_type";
                    ds = OSAESql.RunSQL(command);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    }
                    break;

                case "Pattern":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    command = "SELECT pattern from osae_pattern GROUP BY pattern";
                    ds = OSAESql.RunSQL(command);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    }
                    break;

                case "Reader":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    //command = "SELECT  from osae_pattern GROUP BY pattern";
                    //ds = OSAESql.RunSQL(command);
                    //foreach (DataRow dr in ds.Tables[0].Rows)
                    //{
                    //    ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    //}
                    break;

                case "Schedule":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    command = "SELECT schedule_name from osae_schedule_recurring GROUP BY schedule_name";
                    ds = OSAESql.RunSQL(command);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    }
                    break;

                case "Screen":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    DataSet scrID = OSAESql.RunSQL("SELECT object_type_id from osae_object_type WHERE object_type = 'SCREEN'");
                    string scrnID = scrID.Tables[0].Rows[0].ItemArray[0].ToString();
                    command = "SELECT object_name from osae_object WHERE object_type_id = " + scrnID + " GROUP BY object_name";
                    ds = OSAESql.RunSQL(command);
                    ckbExportMulti.Checked = true;
                    ckbExportMulti_CheckedChanged(this, EventArgs.Empty);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    }
                    break;

                case "Script":
                    ddlObjToExport.SelectionMode = ListSelectionMode.Single;
                    command = "SELECT script_name from osae_script GROUP BY script_name";
                    ds = OSAESql.RunSQL(command);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ddlObjToExport.Items.Add(@dr.ItemArray[0].ToString());
                    }
                    break;
            }
            if (ddlObjToExport.SelectedIndex == -1) { btnExport.Enabled = false; }
        }
        else
        {
            ddlObjToExport.Items.Clear();
            ddlObjToExport.Enabled = false;
            btnExport.Enabled = false;
            lblExportObj.Text = " No Object to Export: ";
        }
    }
}

class ExportObject
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string DataType { get; set; }
    public byte[] byteData { get; set; }
    public string stringData { get; set; }
    public string ExportFileName { get; set; }
    public long ByteSize { get; set; }

    public ExportObject(string oName, string oType)
    {
        Name = oName;
        Type = oType;
        createExportFileName(this);
        fillObject(this);
    }

    public void fillObject(ExportObject exObj)
    {
        if(exObj.Type == "Image")
        {
            OSAEImageManager IM = new OSAEImageManager();
            OSAEImage img = IM.GetImage(exObj.Name);
            exObj.ExportFileName = exObj.Name + "." + img.Type;
            exObj.byteData = img.Data;
            exObj.ByteSize = img.Data.Length;
        }
        else if(exObj.Type == "File")
        {
            using (FileStream fs = new FileStream(exObj.Name, FileMode.Open, FileAccess.Read))
            {
                byte[] filebytes = new byte[fs.Length];
                int numBytesToRead = (int)fs.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fs.Read(filebytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = filebytes.Length;
                exObj.ByteSize = filebytes.Length;
            }
        }
        else if(exObj.Type == "Log")
        {
            DataSet myDataSet = new DataSet();
            if(exObj.Name == "Debug Log")
            {
                myDataSet = OSAESql.RunSQL("SELECT * FROM osae_debug_log");
                DataTable dt = myDataSet.Tables[0];
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sb.Append(dt.Columns[k].ColumnName + ',');
                }
                sb.Append("\r\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                    }
                    sb.Append("\r\n");
                }
                exObj.stringData = sb.ToString();
                exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
                exObj.ByteSize = exObj.byteData.Length;
            }
            else if (exObj.Name == "Event Log")
            {
                myDataSet = OSAESql.RunSQL("SELECT * FROM osae_event_log");
                DataTable dt = myDataSet.Tables[0];
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sb.Append(dt.Columns[k].ColumnName + ',');
                }
                sb.Append("\r\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                    }
                    sb.Append("\r\n");
                }
                exObj.stringData = sb.ToString();
                exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
                exObj.ByteSize = exObj.byteData.Length;
            }
            else if (exObj.Name == "Method Log")
            {
                myDataSet = OSAESql.RunSQL("SELECT * FROM osae_method_log");
                DataTable dt = myDataSet.Tables[0];
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sb.Append(dt.Columns[k].ColumnName + ',');
                }
                sb.Append("\r\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                    }
                    sb.Append("\r\n");
                }
                exObj.stringData = sb.ToString();
                exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
                exObj.ByteSize = exObj.byteData.Length;
            }
            else
            {
                if (exObj.Name == "Server Log")
                {
                    myDataSet = OSAESql.RunSQL("SELECT * FROM osae_log");
                }
                else
                {
                    myDataSet = OSAESql.RunSQL("SELECT * FROM osae_log WHERE Logger = '" + exObj.Name + "'");
                }
                DataTable dt = myDataSet.Tables[0];
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sb.Append(dt.Columns[k].ColumnName + ',');
                }
                sb.Append("\r\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                    }
                    sb.Append("\r\n");
                }
                exObj.stringData = sb.ToString();
                exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
                exObj.ByteSize = exObj.byteData.Length;
            }
        }
        else if (exObj.Type == "Object")
        {
            string sql = OSAE.OSAEObjectManager.ObjectExport(exObj.Name);
            exObj.stringData = sql;
            exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
            exObj.ByteSize = exObj.byteData.Length;
        }
        else if (exObj.Type == "ObjectType")
        {
            string sql = OSAE.OSAEObjectTypeManager.ObjectTypeExport(exObj.Name);
            exObj.stringData = sql;
            exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
            exObj.ByteSize = exObj.byteData.Length;
        }
        else if (exObj.Type == "Pattern")
        {
            string sql = OSAE.OSAEScriptManager.PatternExport(exObj.Name);
            exObj.stringData = sql;
            exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
            exObj.ByteSize = exObj.byteData.Length;
        }
        else if (exObj.Type == "Readers")
        {
            //string sql = OSAE.OSAEReadersManager.ReaderExport(exObj.Name);
            //exObj.stringData = sql;
            //exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
            //exObj.ByteSize = exObj.byteData.Length;
        }
        else if (exObj.Type == "Schedule")
        {
            string sql = OSAE.OSAEScheduleManager.ScheduleExport(exObj.Name);
            exObj.stringData = sql;
            exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
            exObj.ByteSize = exObj.byteData.Length;
        }
        else if (exObj.Type == "Screen")
        {
            string sql = OSAE.OSAEObjectManager.ObjectExport(exObj.Name);
            exObj.stringData = sql;
            exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
            exObj.ByteSize = exObj.byteData.Length;
        }
        else if (exObj.Type == "Script")
        {
            string sql = OSAE.OSAEScriptManager.ExportScript(exObj.Name);
            exObj.stringData = sql;
            exObj.byteData = Encoding.UTF8.GetBytes(exObj.stringData);
            exObj.ByteSize = exObj.byteData.Length;
        }
    }

    protected void createExportFileName(ExportObject exObj)
    {
        int httpIndx = exObj.Name.IndexOf("http");
        string fileObjName = "";
        if (httpIndx > 0)
        {
            fileObjName = exObj.Name.Remove(httpIndx, 7);
        }
        else
        {
            fileObjName = exObj.Name;
        }
        switch (exObj.Type)
        {
            case "Image":
                exObj.ExportFileName = @fileObjName + ".img";
                exObj.DataType = "Byte";
                break;

            case "File":
                exObj.ExportFileName = @fileObjName;
                exObj.DataType = "Byte";
                break;

            case "Log":
                exObj.ExportFileName = @fileObjName + ".csv";
                exObj.DataType = "Text";
                break;

            case "Object":
                exObj.ExportFileName = @fileObjName + ".sql";
                exObj.DataType = "Text";
                break;

            case "ObjectType":
                exObj.ExportFileName = @fileObjName + ".sql";
                exObj.DataType = "Text";
                break;

            case "Reader":
                exObj.ExportFileName = @fileObjName + ".sql";
                exObj.DataType = "Text";
                break;

            case "Pattern":
                exObj.ExportFileName = @fileObjName + ".sql";
                exObj.DataType = "Text";
                break;

            case "Schedule":
                exObj.ExportFileName = @fileObjName + ".sql";
                exObj.DataType = "Text";
                break;

            case "Script":
                exObj.ExportFileName = @fileObjName + ".sql";
                exObj.DataType = "Text";
                break;

            default:
                exObj.ExportFileName = @fileObjName + ".sql";
                exObj.DataType = "Text";
                break;
        }
    }
}