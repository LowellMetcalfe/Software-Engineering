﻿// Author: Ryan Alderton
// SID: 1609275
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

namespace BasicGP
{
    class DBAccess
    {
        private static string conStr = Properties.Settings.Default.OverGPDBConnectionString;
        private static SqlConnection DBConnection;
        private static SqlDataAdapter loginDataAdapter;
        private static SqlDataAdapter dataAdapter;
        private static SqlCommand sqlCommand;
        private static SqlCommand loginSqlCommand;


        /// <summary>
        /// Opens the connection based on the string passed in DatabaseConnection
        /// </summary>
        public static void OpenConnection()
        {
            DBConnection = new SqlConnection(conStr);
            DBConnection.Open();
        }

        /// <summary>
        /// Closes the DB Connection
        /// </summary>
        public static void CloseConnection()
        {
            DBConnection.Close();
        }
        
        /// <summary>
        /// Get the data generated by the SQL Statment
        /// </summary>
        /// <param name="sqlQuery">The SQL query to passed in</param>
        /// <returns></returns>
        public static DataSet CheckLogin(string username, string password)
        {
            DataSet loginDataSet;
            //Open a connection to the database inside DBAccess
            OpenConnection();
            loginSqlCommand = new SqlCommand(Constants.checkLogin, DBConnection);
            loginSqlCommand.Parameters.AddWithValue("@username", username);
            loginSqlCommand.Parameters.AddWithValue("@password", password);
            //create the object dataAdapter to manipulate a table from the database specified by DBConnection
            loginDataAdapter = new SqlDataAdapter(loginSqlCommand);
            //Creat the dataSet
            loginDataSet = new DataSet();
            loginDataAdapter.Fill(loginDataSet);
            //close the DB Connections
            CloseConnection();

            return loginDataSet;
        }
       
        /// <summary>
        /// Fetches data from the server
        /// </summary>
        /// <param name="data">Data to be passed to the server - first element should always be the function</param>
        public static DataSet getData(params string[] data)
        {
            // Create a dataset called dataSet
            DataSet dataSet;
            // Clean the dataSet and the dataAdapter
            dataSet = null;
            dataAdapter = null;
            int ID = 0;
            int findID;

            // Open the DB Connection
            OpenConnection();
            
            Int32.TryParse(data[1], out ID);
            
            //ID = data.Length <= 0 ? Int32.Parse(data[1]) : 0;

            // Switch statement based on what is in data[0]
            switch (data[0])
            {
                case "findPatient":

                    // Switch based on data[1] which is ID or name and DOB
                    switch (data[1])
                    {
                        case "id":
                            // Try to parse data[2] (ID) to an int32 and output as pID
                            Int32.TryParse(data[2], out findID);
                            // Instantiate an sqlCommand on the DBConnection
                            sqlCommand = new SqlCommand(Constants.getPatientByID, DBConnection);
                            // add parameters to the sql command (Prevents again SQLI)
                            sqlCommand.Parameters.AddWithValue("@id", findID);
                            break;
                        case "name&dob":
                            Console.WriteLine(data[3]);
                            DateTime dateOfBirth = DateTime.Parse(data[3]);

                            string name = data[2];
                            sqlCommand = new SqlCommand(Constants.getPatientByDOB, DBConnection);
                            sqlCommand.Parameters.AddWithValue("@name", name);
                            sqlCommand.Parameters.AddWithValue("@DOB", dateOfBirth);
                            break;
                        default:
                            //dataSet = null;
                            break;
                    }
                    break;
                case "patientAppointments":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getAppointments, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@id", ID);
                    break;
                case "patientAppointmentsView":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getAppointmentsForView, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@id", ID);
                    sqlCommand.Parameters.AddWithValue("@date", DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd")));
                    break;
                case "patientAppointmentsEdit":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getAppointmentsForEdit, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@id", ID);
                    sqlCommand.Parameters.AddWithValue("@date", DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd")));
                    break;
                case "testResults":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getTestResults, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@id", ID);
                    break;
                case "patientPresciptions":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getPrescriptions, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@id", ID);
                    break;
                case "availability":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getAvailability, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@id", ID);
                    break;
                case "duty":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getDuty, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@day", data[1]);
                    break;
                case "prescriptionDuration":
                    // Instantiate an sqlCommand on the DBConnection
                    sqlCommand = new SqlCommand(Constants.getPrescriptionDuration, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    sqlCommand.Parameters.AddWithValue("@id", ID);
                    break;
                case "employeeID":
                    sqlCommand = new SqlCommand(Constants.getEmployeeIDByName, DBConnection);
                    sqlCommand.Parameters.AddWithValue("@title", data[1]);
                    sqlCommand.Parameters.AddWithValue("@firstname", data[2]);
                    sqlCommand.Parameters.AddWithValue("@lastname", data[3]);
                    break;
                case "showEmployeeAvailability":
                    sqlCommand = new SqlCommand(Constants.showEmployeeAvailability, DBConnection);
                    sqlCommand.Parameters.AddWithValue("@employeeID", ID);
                    sqlCommand.Parameters.AddWithValue("@date", DateTime.Parse(data[2]));
                    break;
                case "selectAllPatients":
                    sqlCommand = new SqlCommand(Constants.getAllPatients, DBConnection);
                    break;
                case "getAppointmentID":
                    sqlCommand = new SqlCommand(Constants.getAppointmentID, DBConnection);
                    sqlCommand.Parameters.AddWithValue("@eID", Int32.Parse(data[1]));
                    sqlCommand.Parameters.AddWithValue("@NHNumber", Int32.Parse(data[2]));
                    sqlCommand.Parameters.AddWithValue("@date", DateTime.Parse(data[3]));
                    sqlCommand.Parameters.AddWithValue("@time", data[4]);
                    Console.WriteLine(data[1], data[2], DateTime.Parse(data[3]), DateTime.Parse(data[4]).ToShortTimeString());
                    break;
                default:
                    dataSet = null;
                    break;
            }
            // Add the value of the sqlCommand to the sqlDataAdapter
            dataAdapter = new SqlDataAdapter(sqlCommand);
            // create a data set
            dataSet = new DataSet();
            // Fill the dataAdapter with the data from the dataSet
            dataAdapter.Fill(dataSet);
            // Close the DB Connection
            CloseConnection();
            // Return the dataset
            return dataSet;
        }

        /// <summary>
        /// Posts Data to server
        /// </summary>
        /// <param name="data">Data to be passed to the server - first element should always be the function</param>
        public static DataSet postData(params string[] data)
        {
            int count;
            DataSet dataSet;

            dataSet = null;
            dataAdapter = null;
            OpenConnection();

            switch (data[0])
            {
                case "registerPatient":
                    // Instantiate an sqlCommand on the DBConnection
                    // TODO: Concat Address
                    SqlCommand sqlCommand = new SqlCommand(Constants.postPatient, DBConnection);
                    // add parameters to the sql command (Prevents again SQLI)
                    //database will auto increment the id
                    //sqlCommand.Parameters.AddWithValue("@NHNumber", data[1]);
                    sqlCommand.Parameters.AddWithValue("@Name", data[1]);
                    sqlCommand.Parameters.AddWithValue("@Title", data[2]);
                    sqlCommand.Parameters.AddWithValue("@DOB", DateTime.Parse(data[3]));
                    sqlCommand.Parameters.AddWithValue("@PhoneNumber", data[4]);
                    sqlCommand.Parameters.AddWithValue("@Address", data[5]);
                    sqlCommand.Parameters.AddWithValue("@Allergies", data[6]);
                    sqlCommand.Parameters.AddWithValue("@Diabetes", data[7]);
                    sqlCommand.Parameters.AddWithValue("@Smoker", data[8]);
                    sqlCommand.Parameters.AddWithValue("@Asthma", data[9]);

                    count = sqlCommand.ExecuteNonQuery();

                    if(count > 0)
                    {
                        RegisterForm.showMessage("Success!", "Patient was added successfully!");
                    } else
                    {
                        RegisterForm.showMessage("Error!", "There was an error and the patient was not added.");
                    }
                    count = 0;
                    break;
                case "newAppointment":
                    sqlCommand = new SqlCommand(Constants.postAppointment, DBConnection);
                    // Add params to the above SQL query (Prevents against SQLI)  
                    sqlCommand.Parameters.AddWithValue("@NHNumber", data[1]);
                    sqlCommand.Parameters.AddWithValue("@Date", DateTime.Parse(data[2]));
                    sqlCommand.Parameters.AddWithValue("@Time", DateTime.Parse(data[3]));
                    sqlCommand.Parameters.AddWithValue("@Description", data[4]);
                    sqlCommand.Parameters.AddWithValue("@firstname",data[5]);
                    sqlCommand.Parameters.AddWithValue("@lastname",data[6]);
                    sqlCommand.Parameters.AddWithValue("@title", data[7]);

                    count = sqlCommand.ExecuteNonQuery();

                    if(count > 0)
                    {
                        RegisterForm.showMessage("Success!", "Patient was added successfully!");
                    } else
                    {
                        RegisterForm.showMessage("Error!", "There was an error and the patient was not added.");
                    }

                    count = 0;
                    break;
                default:
                    dataSet = null;
                    Console.WriteLine("default");
                    break;
            }
            
            return dataSet;
        }

        public static void updateData(params string[] data)
        {
            OpenConnection();
            switch(data[0])
            {
                case "editAppointment":
                    sqlCommand = new SqlCommand(Constants.updateAppointment, DBConnection);
                    sqlCommand.Parameters.AddWithValue("@eID", data[1]);
                    sqlCommand.Parameters.AddWithValue("@NHNumber", Int32.Parse(data[2]));
                    sqlCommand.Parameters.AddWithValue("@date", DateTime.Parse(data[3]));
                    sqlCommand.Parameters.AddWithValue("@time", DateTime.Parse(data[4]));
                    sqlCommand.Parameters.AddWithValue("@desc", data[5]);
                    sqlCommand.Parameters.AddWithValue("@aID", Int32.Parse(data[6]));
                    int count = sqlCommand.ExecuteNonQuery();

                    if (count > 0)
                    {
                        RegisterForm.showMessage("Success!", "Appointment was edited sucessfully!");
                    }
                    else
                    {
                        RegisterForm.showMessage("Error!", "There was an error and the appointment was not edited.");
                    }
                    break;
                case "extendPrescription":
                    sqlCommand = new SqlCommand(Constants.extendPrescriptionDuration, DBConnection);
                    sqlCommand.Parameters.AddWithValue("@Date", DateTime.Today);
                    sqlCommand.Parameters.AddWithValue("@prescriptionID", data[1]);
                    count = sqlCommand.ExecuteNonQuery();

                    if (count > 0)
                    {
                        RegisterForm.showMessage("Success!", "Prescription was extended sucessfully!");
                    }
                    else
                    {
                        RegisterForm.showMessage("Error!", "There was an error and the prescription was not extended.");
                    }
                    break;
            }

           
        }

        public static void deleteData(params string[] data)
        {
            OpenConnection();
            sqlCommand = new SqlCommand(Constants.cancelAppointment, DBConnection);
            sqlCommand.Parameters.AddWithValue("@NHNumber", data[0]);
            sqlCommand.Parameters.AddWithValue("@date", DateTime.Parse(data[1]));
            sqlCommand.Parameters.AddWithValue("@time", data[2]);
            int count = sqlCommand.ExecuteNonQuery();

            if (count > 0)
            {
                RegisterForm.showMessage("Success!", "Sucessfully Deleted.");
            }
            else
            {
                RegisterForm.showMessage("Error!", "There was an error and the entry was not deleted.");
            }

        }
    }
}
