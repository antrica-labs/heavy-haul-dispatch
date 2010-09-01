using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using SingerDispatch.Utils;

namespace SingerDispatch.Importer
{
    public class Program
    {   
        private Dictionary<string, CompanyPriorityLevel> PriorityLevels { get; set; }
        private Dictionary<bool?, CustomerType> CompanyTypes { get; set; }
        private Dictionary<string, ProvincesAndState> ProvincesAndStates { get; set; }
        private Dictionary<string, AddressType> AddressTypes { get; set; }
        private Dictionary<string, ContactType> ContactTypes { get; set; }
        private Dictionary<string, Rate> Rates { get; set; }
        private Dictionary<int?, Address> NewAddresses { get; set; }

        static void Main(string[] args)
        {
            var prog = new Program();            
            prog.Run();
        }

        public void Run()
        {
            Console.WriteLine("WARNING: THIS OPERATION WILL DESTROY ANY DATA CURRENTLY IN THE DATABASE!");
            Console.Write("Are you sure you wish to continue? [Y/N]: ");
            
            var key = Console.ReadKey(false);

            Console.WriteLine();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Y)
            {   
                Console.WriteLine("Opperation cancelled - Press any key to exit.");
                Console.ReadKey(true);

                return;
            }

            try
            {
                Console.WriteLine("Initializing database...");
                CleanDatabase();

                Console.WriteLine("Setting up import lookups...");
                SetupReferences();

                ImportOldData();

                Console.WriteLine("Import complete!");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("ERROR - Unable to complete database setup: " + e.Message);                
            }

            Console.WriteLine();

            Console.WriteLine("Hit any key to continue");
            Console.ReadKey(true);
        }

        private static void CleanDatabase()
        {
            var linq = new SingerDispatchDataContext(ConfigurationManager.ConnectionStrings["NewDBConnectionParameters"].ConnectionString);

            if (linq.DatabaseExists())
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NewDBConnectionParameters"].ConnectionString))
                {
                    var sql = String.Format("DROP DATABASE [{0}]", connection.Database);
                    
                    connection.Open();
                    connection.ChangeDatabase("master");

                    SqlConnection.ClearPool(connection);

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            var builder = new DatabaseBuilder(linq);
            builder.CreateNewDatabase();
            builder.PopulateDefaults();
        }

        private void SetupReferences()
        {
            var linq = new SingerDispatchDataContext(ConfigurationManager.ConnectionStrings["NewDBConnectionParameters"].ConnectionString);

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


            CompanyTypes = new Dictionary<bool?, CustomerType>();
            foreach (var item in (from ct in linq.CustomerType select ct).ToList())
            {
                if (item.IsEnterprise != null && !CompanyTypes.ContainsKey(item.IsEnterprise)) 
                    CompanyTypes.Add(item.IsEnterprise, item);
            }
            

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

            Rates = new Dictionary<string, Rate>();

            foreach (var item in linq.Rates)
            {
                Rates.Add(item.Name, item);
            }
        }

        private void ImportOldData()
        {
            NewAddresses = new Dictionary<int?, Address>();

            List<Country> countries;
            List<ServiceType> serviceTypes;
            List<Company> companies;
            List<Inclusion> inclusions;
            List<Condition> conditions;
            List<ContactType> contactTypes;
            List<ExtraEquipmentType> extraEquipmentTypes;
            List<PermitType> permitTypes;
            
            
            var datasource = ConfigurationManager.ConnectionStrings["OldDBConnectionParameters"].ConnectionString;
            var provider = ConfigurationManager.ConnectionStrings["OldDBConnectionParameters"].ProviderName;
            var connectionString = String.Format("Provider={0};{1}", provider, datasource);

            using (var connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Importing support data...");

                countries = ImportCountriesAndProvinces(connection);
                serviceTypes = ImportServiceTypes(connection);
                contactTypes = ImportContactTypes(connection);
                inclusions = ImportInclusions(connection);
                conditions = ImportConditions(connection);
                extraEquipmentTypes = ImportExtraEquipmentTypes(connection);
                permitTypes = ImportPermitTypes(connection);

                Console.Write("Importing companies");
                companies = ImportCompanies(connection);                
            }

            Console.WriteLine();
            Console.WriteLine("Saving imported data to database...");

            var context = new SingerDispatchDataContext(ConfigurationManager.ConnectionStrings["NewDBConnectionParameters"].ConnectionString);

            context.Countries.InsertAllOnSubmit(countries);
            context.ServiceTypes.InsertAllOnSubmit(serviceTypes);
            context.ContactTypes.InsertAllOnSubmit(contactTypes);
            context.Inclusions.InsertAllOnSubmit(inclusions);
            context.Conditions.InsertAllOnSubmit(conditions);
            context.ExtraEquipmentTypes.InsertAllOnSubmit(extraEquipmentTypes);
            context.PermitTypes.InsertAllOnSubmit(permitTypes);
            context.Companies.InsertAllOnSubmit(companies);

            context.SubmitChanges();
        }

        private List<Country> ImportCountriesAndProvinces(OleDbConnection connection)
        {
            var countries = new Dictionary<string, Country>();

            countries.Add("Canada", new Country { Name = "Canada" });
            countries.Add("USA", new Country { Name = "USA" });

            const string select = "SELECT * FROM tbl_Province";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var country = countries[reader["countryName"] == DBNull.Value ? "" : (string)reader["countryName"]];

                        if (country == null) continue;

                        var prov = new ProvincesAndState();

                        prov.Name = reader["provinceName"] == DBNull.Value ? null : (string)reader["provinceName"];
                        prov.Abbreviation = reader["provinceAbr"] == DBNull.Value ? null : (string)reader["provinceAbr"];

                        country.ProvincesAndStates.Add(prov);
                    }
                }
            }

            return countries.Values.ToList();
        }

        private List<ServiceType> ImportServiceTypes(OleDbConnection connection)
        {
            var list = new List<ServiceType>();

            const string select = "SELECT * FROM tbl_ServiceType";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["serviceType"] == DBNull.Value ? null : (string)reader["serviceType"];

                        if (name == null) continue;

                        list.Add(new ServiceType { Name = name });
                    }
                }
            }

            return list;
        }

        private List<TrailerCombination> ImportTrailerCombinations(OleDbConnection connection)
        {
            var list = new List<TrailerCombination>();
            
            const string select = "SELECT * FROM join_tbl_TrailerCombination_WheelType";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var combo = new TrailerCombination();
                        
                        // rate - trailerUsedType, combination - trailerCombinationType, tare - trailerCombinationTare, height - trailerCombinationHeight, width - trailerCombinationWidth, length - trailerCombinationLengthWithTractor (feet)

                        combo.Rate = Rates[""];
                    }
                }
            }

            return list;
        }

        private List<ContactType> ImportContactTypes(OleDbConnection connection)
        {
            var list = new List<ContactType>();

            const string select = "SELECT * FROM tbl_ContactType";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["contactType"] == DBNull.Value ? null : (string) reader["contactType"];

                        if (name == null) continue;

                        var type = new ContactType { Name = name };    

                        list.Add(type);
                        ContactTypes[name.ToUpper()] = type;
                    }
                }
            }

            return list;
        }

        private static List<Inclusion> ImportInclusions(OleDbConnection connection)
        {
            var inclusions = new List<Inclusion>();

            const string select = "SELECT * FROM tbl_OptionType";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var line = reader["optionType"] == DBNull.Value ? null : (string)reader["optionType"];

                        inclusions.Add(new Inclusion { Line = line });
                    }
                }
            }
            
            return inclusions;
        }

        private static List<Condition> ImportConditions(OleDbConnection connection)
        {
            var conditions = new List<Condition>();

            const string select = "SELECT * FROM tbl_QuoteConditionType";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var line = reader["quoteConditionType"] == DBNull.Value ? null : (string)reader["quoteConditionType"];

                        conditions.Add(new Condition { Line = line });
                    }
                }
            }

            return conditions;
        }

        private List<ExtraEquipmentType> ImportExtraEquipmentTypes(OleDbConnection connection)
        {
            var types = new List<ExtraEquipmentType>();

            const string select = "SELECT * FROM tbl_DispatchExtraItemType";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["dispatchExtraItemType"] == DBNull.Value ? null : (string)reader["dispatchExtraItemType"];

                        types.Add(new ExtraEquipmentType { Name = name });
                    }
                }
            }

            return types;
        }

        private List<PermitType> ImportPermitTypes(OleDbConnection connection)
        {
            var types = new List<PermitType>();

            const string select = "SELECT * FROM tbl_PermitType";
            using (var command = new OleDbCommand(select, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["permitType"] == DBNull.Value ? null : (string)reader["permitType"];

                        types.Add(new PermitType { Name = name });
                    }
                }
            }

            return types;
        }

        private List<Company> ImportCompanies(OleDbConnection connection)
        {
            var companies = new List<Company>();

            const string select = "SELECT c.*, n.companyNoteNotes FROM tbl_Company AS c LEFT JOIN tbl_CompanyNotes AS n ON c.companyId = n.companyId";
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

            return companies;
        }

        private Company CreateCompanyFromRow(OleDbConnection connection, IDataRecord reader)
        {
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

            try
            {
                company.PriorityLevelID = PriorityLevels[priorityLevel].ID;
                company.CustomerTypeID = CompanyTypes[!isSingerCustomer].ID;
            }
            catch
            {
            }
            
            

            // Add all addresses for this company
            var addressSql = String.Format("SELECT a.*, c.cityName, c.provinceAbr AS cityProvinceAbr FROM tbl_Address a LEFT JOIN tbl_City c ON a.cityId = c.cityId WHERE a.companyId = {0} AND a.addressIsValid = 1", company.ArchiveID);
            using (var command = new OleDbCommand(addressSql, connection))
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
            //sql = String.Format("SELECT c.*, t.contactType FROM tbl_Contact c LEFT JOIN join_tbl_Contact_ContactType t ON c.contactId = t.contactId WHERE companyId = {0}", company.ArchiveID);
            var contactSql = String.Format("SELECT * from tbl_Contact WHERE companyId = {0}", company.ArchiveID);
            using (var command = new OleDbCommand(contactSql, connection))
            {
                using (var innerReader = command.ExecuteReader())
                {
                    while (innerReader.Read())
                    {
                        var contact = CreateContactFromRow(innerReader);

                        if (contact != null)
                        {
                            var typesSql = String.Format("SELECT contactType FROM join_tbl_Contact_ContactType where contactId = {0}", contact.ArchiveID);
                            using (var command2 = new OleDbCommand(typesSql, connection))
                            {
                                using (var innerReader2 = command2.ExecuteReader())
                                {
                                    while (innerReader2.Read())
                                    {
                                        var name = innerReader2["contactType"] == DBNull.Value ? null : (string) innerReader2["contactType"];

                                        try
                                        {
                                            if (name == null || ContactTypes[name.ToUpper()] == null) continue;

                                            var role = new ContactRoles();

                                            role.Contact = contact;
                                            role.ContactType = ContactTypes[name.ToUpper()];

                                            contact.ContactRoles.Add(role);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }

                            try
                            {
                                // Figure out what address to put them into...
                                if (contact.Address == null)
                                    contact.Address = company.Addresses.First();
                                

                                contact.Address.Contacts.Add(contact);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }


            // Add all recorded commodities for this company
            var commoditySql = String.Format("SELECT * FROM tbl_Commodity WHERE companyId = {0}", company.ArchiveID);
            using (var command = new OleDbCommand(commoditySql, connection))
            {
                using (var innerReader = command.ExecuteReader())
                {
                    while (innerReader.Read())
                    {
                        var commodity = CreateCommodityFromRow(innerReader);

                        if (commodity != null)
                            company.Commodities.Add(commodity);
                    }
                }
            }

            return company;
        }

        private Address CreateAddressFromRow(OleDbConnection connection, IDataRecord reader)
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
            address.PrimaryPhone = reader["addressPhone1"] == DBNull.Value ? null : PhoneNumberCleanup((string)reader["addressPhone1"]);
            address.SecondaryPhone = reader["addressPhone2"] == DBNull.Value ? null : PhoneNumberCleanup((string)reader["addressPhone2"]);
            address.Fax = reader["addressFax"] == DBNull.Value ? null : PhoneNumberCleanup((string)reader["addressFax"]);
            address.Notes = reader["addressNote"] == DBNull.Value ? null : (string)reader["addressNote"];

            var provinceAbbr = (reader["cityProvinceAbr"] == DBNull.Value ? null : (string)reader["cityProvinceAbr"]) ??
                               (reader["provinceAbr"] == DBNull.Value ? null : (string)reader["provinceAbr"]);

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
            catch
            {
            }

            return address;
        }

        private Contact CreateContactFromRow(IDataRecord reader)
        {
            var contact = new Contact();

            contact.ArchiveID = (int)reader["contactId"];
            contact.FirstName = reader["contactFirstName"] == DBNull.Value ? null : (string)reader["contactFirstName"];
            contact.LastName = reader["contactLastName"] == DBNull.Value ? null : (string)reader["contactLastName"];
            contact.PrimaryPhone = reader["contactPrimaryPhone"] == DBNull.Value ? null : PhoneNumberCleanup((string)reader["contactPrimaryPhone"]);
            contact.SecondaryPhone = reader["contactSecondaryPhone"] == DBNull.Value ? null : PhoneNumberCleanup((string)reader["contactSecondaryPhone"]);
            contact.Fax = reader["contactFax"] == DBNull.Value ? null : PhoneNumberCleanup((string)reader["contactFax"]);
            contact.Email = reader["contactEmail"] == DBNull.Value ? null : (string)reader["contactEmail"];
            contact.Notes = reader["contactNote"] == DBNull.Value ? null : (string)reader["contactNote"];

            try
            {
                contact.Address = reader["addressId"] == DBNull.Value ? null : NewAddresses[(int?)reader["addressId"]];
            }
            catch
            {
            }
            
            var pext = reader["contactPrimaryPhoneExt"] == DBNull.Value ? null : (string)reader["contactPrimaryPhoneExt"];
            if (pext != null)
                contact.PrimaryPhone += " ext " + pext;

            var sext = reader["contactSecondaryPhoneExt"] == DBNull.Value ? null : (string)reader["contactSecondaryPhoneExt"];
            if (sext != null)
                contact.SecondaryPhone += " ext " + sext;

            var fext = reader["contactFaxExt"] == DBNull.Value ? null : (string)reader["contactFaxExt"];
            if (fext != null)
                contact.Fax += " ext " + fext;            

            return contact;
        }

        private static Commodity CreateCommodityFromRow(IDataRecord reader)
        {
            if (reader["commodityName"] == DBNull.Value)
                return null;
                         
            var commodity = new Commodity();

            commodity.Name = (string)reader["commodityName"];
            commodity.Value = reader["commodityValue"] == DBNull.Value ? null : (decimal?)reader["commodityValue"];
            commodity.Serial = reader["commoditySerialNum"] == DBNull.Value ? null : (string)reader["commoditySerialNum"];
            commodity.Unit = reader["commodityUnitNum"] == DBNull.Value ? null : (string)reader["commodityUnitNum"];
            commodity.Owner = reader["commodityOwner"] == DBNull.Value ? null : (string)reader["commodityOwner"];
            commodity.LastLocation = reader["commodityLastKnownLocation"] == DBNull.Value ? null : (string)reader["commodityLastKnownLocation"];
            commodity.LastAddress = (reader["commodityLastKnownSiteName"] == DBNull.Value ? null : (string)reader["commodityLastKnownSiteName"]) ??
                                    (reader["commodityLastKnownLSD"] == DBNull.Value ? null : (string)reader["commodityLastKnownLSD"]);

            var length = reader["commodityActualLength"] == DBNull.Value ? null : (double?)reader["commodityActualLength"];
            var width = reader["commodityActualWidth"] == DBNull.Value ? null : (double?)reader["commodityActualWidth"];
            var height = reader["commodityActualHeight"] == DBNull.Value ? null : (double?)reader["commodityActualHeight"];
            var weight = reader["commodityActualWeight"] == DBNull.Value ? null : (double?)reader["commodityActualWeight"];

            var elength = reader["commodityEstimatedLength"] == DBNull.Value ? null : (double?)reader["commodityEstimatedLength"];
            var ewidth = reader["commodityEstimatedWidth"] == DBNull.Value ? null : (double?)reader["commodityEstimatedWidth"];
            var eheight = reader["commodityEstimatedHeight"] == DBNull.Value ? null : (double?)reader["commodityEstimatedHeight"];
            var eweight = reader["commodityEstimatedWeight"] == DBNull.Value ? null : (double?)reader["commodityEstimatedWeight"];

            commodity.Length = length ?? elength;
            commodity.Width = width ?? ewidth;
            commodity.Height = height ?? eheight;
            commodity.Weight = weight ?? eweight;

            commodity.SizeEstimated = (length == null) || (width == null) || (height == null);
            commodity.WeightEstimated = (weight == null);
            
            return commodity;
        }

        private static string PhoneNumberCleanup(string number)
        {
            return string.Format(new LafiPhoneFormatProvider(), "{0:de}", number);
        }
    }
}
