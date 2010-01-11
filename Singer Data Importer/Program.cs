using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using SingerDispatch;
using SingerDispatch.Database;
using System.Data.SqlClient;
using System.Configuration;

namespace Data_Importer
{
    class Program
    {
        private const string AccessConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""C:\Users\Simon Twogood\Documents\Work\Singer\SingerPrototypeDB08262005_1100.mdb""";
        
        private Dictionary<string, CompanyPriorityLevel> PriorityLevels;
        private Dictionary<bool?, string> CompanyTypes;
        private Dictionary<string, ProvincesAndState> ProvincesAndStates;
        private Dictionary<string, AddressType> AddressTypes;
        private Dictionary<string, ContactType> ContactTypes;

        private Dictionary<int?, Address> NewAddresses;

        static void Main(string[] args)
        {
            Program prog = new Program();            
            prog.Run();
        }

        public Program()
        {
            CleanDatabase();
            SetupReferences();

            NewAddresses = new Dictionary<int?, Address>();
        }

        private void CleanDatabase()
        {
            Console.WriteLine("Initializing database...");

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionParameters"].ConnectionString))
            {
                connection.Open();
                                
                SqlConnection.ClearPool(connection);

                var sql = String.Format("DROP DATABASE [{0}]", connection.Database);
                connection.ChangeDatabase("master");

                using (var command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            var linq = new SingerDispatchDataContext(ConfigurationManager.ConnectionStrings["ConnectionParameters"].ConnectionString);

            var builder = new DatabaseBuilder(linq);
            builder.CreateNewDatabase();
        }

        private void SetupReferences()
        {
            Console.WriteLine("Setting up import lookups...");

            var linq = new SingerDispatchDataContext(ConfigurationManager.ConnectionStrings["ConnectionParameters"].ConnectionString);

            PriorityLevels = new Dictionary<string, CompanyPriorityLevel>();
            foreach (var level in (from l in linq.CompanyPriorityLevels select l).ToList())
            {
                switch (level.Name)
                {
                    case "1. Elite":
                        PriorityLevels["Gas Plant > 1,000,000"] = level;
                        break;
                    case "2. Prestige":
                        PriorityLevels["Gas Plant < 1,000,000"] = level;
                        break;
                    case "3. Premier":
                        PriorityLevels["High Dollar"] = level;
                        break;
                    case "4. Regular":
                        PriorityLevels[""] = level;
                        PriorityLevels["Regular User"] = level;
                        break;
                    case "5. Cash on Delivery":
                        PriorityLevels["COD or Rarely Used"] = level;
                        break;
                    case "6. Do not haul":
                        PriorityLevels["Do not Haul"] = level;
                        break;
                }
            }


            CompanyTypes = new Dictionary<bool?, string>();
            CompanyTypes.Add(true, "Singer Specialized");
            CompanyTypes.Add(false, "M.E. Signer Enterprise");


            ProvincesAndStates = new Dictionary<string, ProvincesAndState>();
            foreach (var item in (from ps in linq.ProvincesAndStates select ps).ToList())
            {
                ProvincesAndStates[item.Name] = item;
            }


            AddressTypes = new Dictionary<string, AddressType>();
            foreach (var item in (from at in linq.AddressTypes select at).ToList())
            {
                AddressTypes[item.Name] = item;
            }

            ContactTypes = new Dictionary<string, ContactType>();
            foreach (var item in (from t in linq.ContactTypes select t).ToList())
            {
                ContactTypes[item.Name] = item;
            }
        }

        public void Run()
        {
            var companies = new List<Company>();
            
            using (var connection = new OleDbConnection(AccessConnectionString))
            {
                connection.Open();

                Console.Write("Importing companies");                
                var select = "SELECT c.*, n.companyNoteNotes FROM tbl_Company AS c LEFT JOIN tbl_CompanyNotes AS n ON c.companyId = n.companyId";
                using (var command = new OleDbCommand(select, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            companies.Add(CreateCompanyFromRow(connection, reader));
                            Console.Write(".");
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Saving companies to database...");

            InsertCompanies(companies);
            
            Console.WriteLine("Import complete!");
            Console.WriteLine();

            Console.WriteLine("Hit enter to continue");
            Console.ReadLine();
        }

        private Company CreateCompanyFromRow(OleDbConnection connection, OleDbDataReader reader)
        {
            String sql;
            var company = new Company();

            company.ArchiveID = (int)reader["companyId"];
            company.Name = reader["companyName"] == DBNull.Value ? null : (string)reader["companyName"];
            company.OperatingAs = reader["companyOperatingAs"] == DBNull.Value ? null : (string)reader["companyOperatingAs"];
            company.AccPacVendorCode = reader["companyAccPac"] == DBNull.Value ? null : (string)reader["companyAccPac"];
            company.AvailableCredit = reader["companyCreditLimit"] == DBNull.Value ? null : (decimal?)reader["companyCreditLimit"];
            company.EquifaxComplete = reader["companyIsCreditApproved"] == DBNull.Value ? null : (bool?)reader["companyIsCreditApproved"];
            company.Notes = reader["companyNoteNotes"] == DBNull.Value ? null : (string)reader["companyNoteNotes"];
            var priorityLevel = reader["companyPriorityType"] == DBNull.Value ? null : (string)reader["companyPriorityType"];
            var isSingerCustomer = reader["companyIsBillFromSpecialized"] == DBNull.Value ? null : (bool?)reader["companyIsBillFromSpecialized"];
                        
            if (company.Name == company.OperatingAs)
                company.OperatingAs = null;

            if (priorityLevel == null)
                priorityLevel = "";

            if (isSingerCustomer == null)
                isSingerCustomer = true;

            company.PriorityLevelID = PriorityLevels[priorityLevel].ID;
            company.Type = CompanyTypes[isSingerCustomer];
            

            // Add all addresses for this company
            sql = String.Format("SELECT a.*, c.cityName, c.provinceAbr AS cityProvinceAbr FROM tbl_Address a LEFT JOIN tbl_City c ON a.cityId = c.cityId WHERE a.companyId = {0} AND a.addressIsValid = 1", company.ArchiveID);
            using (var command = new OleDbCommand(sql, connection))
            {
                using (var innerReader = command.ExecuteReader())
                {
                    while (innerReader.Read())
                    {
                        var address = CreateAddressFromRow(connection, innerReader);

                        if (address != null)
                            company.Addresses.Add(address);
                    }
                }
            }


            // Add any existing contacts to this company
            sql = String.Format("SELECT c.*, t.contactType FROM tbl_Contact c LEFT JOIN join_tbl_Contact_ContactType t ON c.contactId = t.contactId WHERE companyId = {0}", company.ArchiveID);
            using (var command = new OleDbCommand(sql, connection))
            {
                using (var innerReader = command.ExecuteReader())
                {
                    while (innerReader.Read())
                    {
                        var contact = CreateContactFromRow(connection, innerReader);

                        if (contact != null)
                        {
                            // Figure out what address to put them into...

                            try
                            {
                                if (contact.Address == null)
                                    contact.Address = company.Addresses.First();
                                else
                                    contact.Email = contact.Email + "";

                                contact.Address.Contacts.Add(contact);
                            }
                            catch { }
                        }   

                    }
                }
            }


            // Add all recorded commodities for this company
            sql = String.Format("SELECT * FROM tbl_Commodity WHERE companyId = {0}", company.ArchiveID);


            return company;
        }

        private Address CreateAddressFromRow(OleDbConnection connection, OleDbDataReader reader)
        {
            if (reader["addressLine1"] == DBNull.Value)
                return null;

            string sql;
            var address = new Address();

            address.ArchiveID = (int)reader["addressId"];
            address.Line1 = reader["addressLine1"] == DBNull.Value ? null : (string)reader["addressLine1"];
            address.Line2 = reader["addressLine2"] == DBNull.Value ? null : (string)reader["addressLine2"];
            address.City = reader["cityName"] == DBNull.Value ? null : (string)reader["cityName"];
            address.PostalZip = reader["addressPostalCode"] == DBNull.Value ? null : (string)reader["addressPostalCode"];
            address.PrimaryPhone = reader["addressPhone1"] == DBNull.Value ? null : (string)reader["addressPhone1"];
            address.SecondaryPhone = reader["addressPhone2"] == DBNull.Value ? null : (string)reader["addressPhone2"];
            address.Fax = reader["addressFax"] == DBNull.Value ? null : (string)reader["addressFax"];
            address.Notes = reader["addressNote"] == DBNull.Value ? null : (string)reader["addressNote"];

            var provinceAbbr = reader["cityProvinceAbr"] == DBNull.Value ? null : (string)reader["cityProvinceAbr"];

            if (provinceAbbr == null)
                provinceAbbr = reader["provinceAbr"] == DBNull.Value ? null : (string)reader["provinceAbr"];

            if (provinceAbbr != null)
            {
                sql = String.Format("SELECT p.provinceName FROM tbl_Province p WHERE p.provinceAbr = '{0}'", provinceAbbr);

                using (var command = new OleDbCommand(sql, connection))
                {
                    using (var innerReader = command.ExecuteReader())
                    {
                        if (innerReader.Read())
                        {
                            if (innerReader[0] != DBNull.Value && ProvincesAndStates[(string)innerReader[0]] != null)
                                address.ProvinceStateID = ProvincesAndStates[(string)innerReader[0]].ID;
                        }
                    }
                }
            }


            sql = String.Format("SELECT addressType from join_tbl_Address_AddressType WHERE addressId = {0}", address.ArchiveID);

            using (var command = new OleDbCommand(sql, connection))
            {
                using (var innerReader = command.ExecuteReader())
                {
                    if (innerReader.Read())
                    {
                        if (innerReader[0] != DBNull.Value && AddressTypes[(string)innerReader[0]] != null)
                            address.AddressTypeID = AddressTypes[(string)innerReader[0]].ID;
                    }
                }
            }

            try
            {
                NewAddresses.Add(address.ArchiveID, address);
            }
            catch { }
            
            return address;
        }

        private Contact CreateContactFromRow(OleDbConnection connection, OleDbDataReader reader)
        {
            var contact = new Contact();

            contact.ArchiveID = (int)reader["contactId"];
            contact.FirstName = reader["contactFirstName"] == DBNull.Value ? null : (string)reader["contactFirstName"];
            contact.LastName = reader["contactLastName"] == DBNull.Value ? null : (string)reader["contactLastName"];
            contact.PrimaryPhone = reader["contactPrimaryPhone"] == DBNull.Value ? null : (string)reader["contactPrimaryPhone"];
            contact.SecondaryPhone = reader["contactSecondaryPhone"] == DBNull.Value ? null : (string)reader["contactSecondaryPhone"];
            contact.Fax = reader["contactFax"] == DBNull.Value ? null : (string)reader["contactFax"];
            contact.Email = reader["contactEmail"] == DBNull.Value ? null : (string)reader["contactEmail"];
            contact.Notes = reader["contactNote"] == DBNull.Value ? null : (string)reader["contactNote"];

            try
            {
                contact.Address = reader["addressId"] == DBNull.Value ? null : NewAddresses[(int?)reader["addressId"]];
            }
            catch { }

            try
            {
                contact.TypeID = reader["contactType"] == DBNull.Value ? (long?)null : ContactTypes[(string)reader["contactType"]].ID;
            }
            catch { }

            var pext = reader["contactPrimaryPhoneExt"] == DBNull.Value ? null : (string)reader["contactPrimaryPhoneExt"];
            if (pext != null)
                contact.PrimaryPhone += " ext. " + pext;

            var sext = reader["contactSecondaryPhoneExt"] == DBNull.Value ? null : (string)reader["contactSecondaryPhoneExt"];
            if (sext != null)
                contact.SecondaryPhone += " ext. " + sext;

            var fext = reader["contactFaxExt"] == DBNull.Value ? null : (string)reader["contactFaxExt"];
            if (fext != null)
                contact.Fax += " ext. " + fext;            

            return contact;
        }

        private void InsertCompanies(List<Company> companies)
        {
            var context = new SingerDispatchDataContext(ConfigurationManager.ConnectionStrings["ConnectionParameters"].ConnectionString);

            context.Companies.InsertAllOnSubmit(companies);
            context.SubmitChanges();
        }

    }
}
