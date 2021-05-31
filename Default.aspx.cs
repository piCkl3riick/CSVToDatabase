using System;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;


namespace ImportCSVPersons
{
    public partial class _Default : Page
    {
        private string GetConnectionString() {
            return ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        }

        private void CreateDatabaseTable(DataTable dt, string tableName)
        {
           // string sqlQuery = "Create Table Person (ID INT,FirstName VARCHAR (50),Surname VARCHAR (50),Age INT,Sex CHAR (10),Mobile VARCHAR (10),Active BIT)";
            string sqlQuery = string.Empty;
            string sqlDBType = string.Empty;
            string dataType = string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(string.Format("Create Table {0} (", tableName));
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                int maxLength = 0;
                // Retrieves all datatypes in CSV file to assign type in database
                dataType = dt.Columns[i].DataType.ToString();
                if (dataType == "System.Int32")
                {
                    sqlDBType = "INT";
                }
                else if (dataType == "System.String")
                {
                    sqlDBType = "VARCHAR";
                    maxLength = dt.Columns[i].MaxLength;
                } else
                {
                }
                if (maxLength > 0)
                {
                    sb.AppendFormat(string.Format("{0} {1} ({2}),", dt.Columns[i].ColumnName, sqlDBType, maxLength));
                }
                else
                    sb.AppendFormat(string.Format("{0} {1},", dt.Columns[i].ColumnName, sqlDBType));
            }

            sqlQuery = sb.ToString();
            sqlQuery = sqlQuery.Trim().TrimEnd(',');
            sqlQuery = sqlQuery + " )";

            using (SqlConnection sqlConn = new SqlConnection(GetConnectionString()))
            {
                sqlConn.Open();
                using (SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn))
                {
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }


        private void LoadDataToDatabase(string tableName, string fileFullPath, string delimeter)
        {
            string sqlQuery = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(string.Format("BULK INSERT {0} ", tableName));
            sb.AppendFormat(string.Format(" FROM '{0}'", fileFullPath));
            sb.AppendFormat(string.Format(" WITH ( FIELDTERMINATOR = '{0}' , ROWTERMINATOR = '\n' )", delimeter));
            sqlQuery = sb.ToString();

            using (SqlConnection sqlConn = new SqlConnection(GetConnectionString()))
            {
                sqlConn.Open();
                using (SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn))
                {
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }

        private void UploadAndProcessFile()
        {
            if (FileUpload1.HasFile)
            {
                FileInfo fileInfo = new FileInfo(FileUpload1.PostedFile.FileName);
                if (fileInfo.Name.Contains(".csv"))
                {
                    string fileName = fileInfo.Name.Replace(".csv", "").ToString();
                    string csvFilePath = Server.MapPath("UploadedCSVFiles") + "\\" + fileInfo.Name;

                    //Save the CSV file in the Server inside 'UploadedCSVFiles'   
                    FileUpload1.SaveAs(csvFilePath);

                    //Fetch the location of CSV file   
                    string filePath = Server.MapPath("UploadedCSVFiles") + "\\";

                    string strSql = string.Format("SELECT * FROM [{0}]", fileInfo.Name);
                    string strCSVConnString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='text;HDR=YES;'", filePath);

                    // load the data from CSV to DataTable   
                    DataTable dtCSV = new DataTable();
                    DataTable dtSchema = new DataTable();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(strSql, strCSVConnString))
                    {
                        adapter.FillSchema(dtCSV, SchemaType.Mapped);
                        adapter.Fill(dtCSV);
                    }

                    if (dtCSV.Rows.Count > 0)
                    {
                        CreateDatabaseTable(dtCSV, fileName);
                        Label2.Text = string.Format("The table ({0}) has been successfully created to the database.", fileName);
                        string fileFullPath = filePath + fileInfo.Name;
                        LoadDataToDatabase(fileName, fileFullPath, ",");
                        Label1.Text = string.Format("({0}) records has been loaded to the table {1}.", dtCSV.Rows.Count, fileName);
                    }
                    else
                    {
                        lblError.Text = "File is empty.";
                    }
                }
                else
                {
                    lblError.Text = "Unable to recognize file.";
                }
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnImport_Click(object sender, EventArgs e)
        {
            UploadAndProcessFile();
        }
    }
}
