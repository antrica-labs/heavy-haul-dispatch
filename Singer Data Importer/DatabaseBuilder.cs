using System;
using System.Collections.Generic;
using System.Globalization;
using SingerDispatch.Utils;

namespace SingerDispatch.Importer
{
    public class DatabaseBuilder
    {
        public SingerDispatchDataContext Database { get; set; }

        public DatabaseBuilder(SingerDispatchDataContext context)
        {
            Database = context;
        }

        public void CreateNewDatabase()
        {
            const string dateFormat = "M/d/yyyy";
            var provider = CultureInfo.InvariantCulture;

            if (Database.DatabaseExists())
            {
                throw new Exception("Database already exists!");
            }
                       
            Database.CreateDatabase();
            

            // Populate customer types
            Database.CustomerType.InsertOnSubmit(new CustomerType { Name = "Singer Specialized", IsEnterprise = false });
            Database.CustomerType.InsertOnSubmit(new CustomerType { Name = "M.E. Singer Enterprise", IsEnterprise = true });

            // Populate contact methods
            Database.ContactMethods.InsertOnSubmit(new ContactMethod { Name = "Email" });
            Database.ContactMethods.InsertOnSubmit(new ContactMethod { Name = "Phone" });
            Database.ContactMethods.InsertOnSubmit(new ContactMethod { Name = "Fax" });
                        

            // Populate address types
            var addresstypes = new List<AddressType>();

            addresstypes.Add(new AddressType { Name = "Billing Address" });
            addresstypes.Add(new AddressType { Name = "Head Office" });
            addresstypes.Add(new AddressType { Name = "Local Office" });
            addresstypes.Add(new AddressType { Name = "Site Office" });

            Database.AddressTypes.InsertAllOnSubmit(addresstypes);

            
            // Populate priority levels
            var levels = new List<CompanyPriorityLevel>();

            levels.Add(new CompanyPriorityLevel { Name = "Elite", Level = 1 });
            levels.Add(new CompanyPriorityLevel { Name = "Prestige", Level = 2 });
            levels.Add(new CompanyPriorityLevel { Name = "Premier", Level = 3 });
            levels.Add(new CompanyPriorityLevel { Name = "Regular", Level = 4 });
            levels.Add(new CompanyPriorityLevel { Name = "Cash on Delivery", Level = 5 });
            levels.Add(new CompanyPriorityLevel { Name = "Do Not Haul", Level = 6 });
            levels.Add(new CompanyPriorityLevel { Name = "Arrears", Level = 7 });
            levels.Add(new CompanyPriorityLevel { Name = "Not Approved", Level = 8 });

            Database.CompanyPriorityLevels.InsertAllOnSubmit(levels);


            // Populate seasons
            var seasons = new List<Season>();

            seasons.Add(new Season { Name = "%  Weight Restriction", SortOrder = 0 });
            seasons.Add(new Season { Name = "Fall Weight Restriction",SortOrder = 4  });
            seasons.Add(new Season { Name = "Post Weight Restriction", SortOrder = 2 });
            seasons.Add(new Season { Name = "Spring Weight Restriction", SortOrder = 1 });
            seasons.Add(new Season { Name = "Summer Weight Restriction", SortOrder = 3 });
            seasons.Add(new Season { Name = "Winter Weight Restriction", SortOrder = 5 });

            Database.Seasons.InsertAllOnSubmit(seasons);


            // Populate billing types
            var billingtypes = new List<BillingType>();

            billingtypes.Add(new BillingType { Name = "Per Item" });
            billingtypes.Add(new BillingType { Name = "Per Hour" });
            billingtypes.Add(new BillingType { Name = "Per Month" });
            billingtypes.Add(new BillingType { Name = "Cost Included" });

            Database.BillingTypes.InsertAllOnSubmit(billingtypes);


            // Populate billing intervals
            var intervals = new List<BillingInterval>();

            intervals.Add(new BillingInterval { Name = "Per day" });
            intervals.Add(new BillingInterval { Name = "Per week" });
            intervals.Add(new BillingInterval { Name = "Per month" });
            intervals.Add(new BillingInterval { Name = "Per year" });

            Database.BillingIntervals.InsertAllOnSubmit(intervals);

            // Insert the rate types
            var servicerate = new RateType { Name = "Service" };
            var trailerrate = new RateType { Name = "Trailer" };
            var tractorrate = new RateType { Name = "Tractor" };

            Database.RateTypes.InsertOnSubmit(servicerate);
            Database.RateTypes.InsertOnSubmit(trailerrate);
            Database.RateTypes.InsertOnSubmit(tractorrate);

            // Populate rates
            var rates = new List<Rate>();

            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "10Line", HourlyEnterprise = 700.00m });            
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "10LineDbl" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "12Line", HourlyEnterprise = 800.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "12Line - 6Line + 6Line" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "12LineDbl" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "12Wheel", HourlyEnterprise = 170.00m, HourlySpecialized = 140.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "14Line", HourlyEnterprise = 1000.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "14Line - 6Line + 8Line" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "14LineDbl" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "16Line - 8Line + 8Line" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "16Wheel", HourlyEnterprise = 225.00m, HourlySpecialized = 160.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "18Line", HourlyEnterprise = 1300.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "18Line - 10Line + 8Line" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "20Line", HourlyEnterprise = 1500.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "20Line - 10Line + 10Line" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "20Wheel", HourlyEnterprise = 245.00m, HourlySpecialized = 190.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "22Line - 10Line + 12Line" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "24Line - 12Line + 12Line" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "24Wheel", HourlyEnterprise = 245.00m, HourlySpecialized = 190.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "32Wheel", HourlyEnterprise = 270.00m, HourlySpecialized = 205.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "40Wheel", HourlyEnterprise = 300.00m, HourlySpecialized = 240.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "48Wheel", HourlyEnterprise = 335.00m, HourlySpecialized = 270.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "4Wheel" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "52Wheel", HourlyEnterprise = 360.00m, HourlySpecialized = 290.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "60Wheel", HourlyEnterprise = 360.00m, HourlySpecialized = 290.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "64Wheel", HourlyEnterprise = 375.00m, HourlySpecialized = 310.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "6Line", HourlyEnterprise = 600.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "6LineDbl", HourlyEnterprise = 540.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "76Wheel", HourlyEnterprise = 400.00m, HourlySpecialized = 370.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "7Axle", HourlyEnterprise = 265.00m, HourlySpecialized = 225.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "8Line", HourlyEnterprise = 650.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "8LineDbl" });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "8Wheel", HourlyEnterprise = 155.00m, HourlySpecialized = 125.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "96wheel", HourlyEnterprise = 750.00m, HourlySpecialized = 680.00m });
            rates.Add(new Rate { Archived = false, RateType = trailerrate, Name = "9Axle", HourlyEnterprise = 265.00m, HourlySpecialized = 225.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Bobcat", HourlyEnterprise = 95.00m, HourlySpecialized = 95.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Bull Moose - 0 Swamper", HourlyEnterprise = 190.00m, HourlySpecialized = 190.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Bull Moose - 1 Swamper", HourlyEnterprise = 225.00m, HourlySpecialized = 225.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Compressor Tarping", HourlyEnterprise = 700.00m, HourlySpecialized = 700.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Contractor" });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Cooler Tarping", HourlyEnterprise = 500.00m, HourlySpecialized = 500.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "In-Town Jack" });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Jack & Roll", HourlyEnterprise = 525.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Lease Operator Equipment" });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Lima Crane - 0 Swamper", HourlyEnterprise = 290.00m, HourlySpecialized = 290.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Lima Crane - 1 Swamper", HourlyEnterprise = 325.00m, HourlySpecialized = 325.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Other" });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Out-Town Jack" });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Pilot Car", HourlyEnterprise = 80.00m, HourlySpecialized = 80.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Pull Tractor", HourlyEnterprise = 225.00m, HourlySpecialized = 225.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Push Tractor", HourlyEnterprise = 225.00m, HourlySpecialized = 225.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Rigging - Bull Moose", HourlyEnterprise = 415.00m, HourlySpecialized = 415.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Rigging - Lima Crane", HourlyEnterprise = 525.00m, HourlySpecialized = 525.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Route Study", HourlyEnterprise = 95.00m, HourlySpecialized = 95.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Shop", HourlyEnterprise = 85.00m, HourlySpecialized = 85.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Storage" });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Subsistence", HourlyEnterprise = 175.00m, HourlySpecialized = 175.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Supervisor", HourlyEnterprise = 110.00m, HourlySpecialized = 110.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Swamper", HourlyEnterprise = 65.00m, HourlySpecialized = 75.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Tandem Picker", HourlyEnterprise = 245.00m, HourlySpecialized = 245.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Tandem Tractor", HourlyEnterprise = 125.00m, HourlySpecialized = 125.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Tandem Winch Tractor", HourlyEnterprise = 145.00m, HourlySpecialized = 145.00m });
            rates.Add(new Rate { Archived = false, RateType = servicerate, Name = "Trojan Loader", HourlyEnterprise = 170.00m, HourlySpecialized = 170.00m });

            Database.Rates.InsertAllOnSubmit(rates);


            // Job status types

            var jobstatustypes = new List<Status>();

            jobstatustypes.Add(new Status { Name = "Billed" });
            jobstatustypes.Add(new Status { Name = "Cancelled" });
            jobstatustypes.Add(new Status { Name = "Completed" });
            jobstatustypes.Add(new Status { Name = "Delayed" });
            jobstatustypes.Add(new Status { Name = "In Process" });
            jobstatustypes.Add(new Status { Name = "Pending" });
            jobstatustypes.Add(new Status { Name = "Storage" });

            Database.Statuses.InsertAllOnSubmit(jobstatustypes);


            // Fill employees table

            var employees = new List<Employee>();

            employees.Add(new Employee { FirstName = "Hired", LastName = "Driver", Phone = null, Mobile = null, IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = null, EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Ryan", LastName = "Millions", Phone = null, Mobile = PhoneNumberCleanup("(403)861-4538"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/1900", dateFormat, provider), EndDate = DateTime.ParseExact("1/1/2006", dateFormat, provider), Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Ralph", LastName = "Rathy", Phone = null, Mobile = PhoneNumberCleanup("(403)510-5472"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Shop Manager" });
            employees.Add(new Employee { FirstName = "Shane", LastName = "Byrne", Phone = null, Mobile = PhoneNumberCleanup("(403)369-5001"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Picker Operator" });
            employees.Add(new Employee { FirstName = "John", LastName = "Frank", Phone = PhoneNumberCleanup("(403)816-1646"), Mobile = PhoneNumberCleanup("(403)816-1646"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/1973", dateFormat, provider), EndDate = null, Responsibilities = "Winch Tractor" });
            employees.Add(new Employee { FirstName = "David", LastName = "Gilkes", Phone = PhoneNumberCleanup("(403)276-2704"), Mobile = PhoneNumberCleanup("(403)807-2074"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Shit disturber" });
            employees.Add(new Employee { FirstName = "Lance", LastName = "Neilson", Phone = PhoneNumberCleanup("(403)276-7785"), Mobile = PhoneNumberCleanup("(403)660-1466"), IsAvailable = true, IsSupervisor = true, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Field Supv/Trailer Op" });
            employees.Add(new Employee { FirstName = "Martin", LastName = "Singer", Phone = null, Mobile = PhoneNumberCleanup("(403)816-1642"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "General Manager" });
            employees.Add(new Employee { FirstName = "Keith", LastName = "Banner", Phone = PhoneNumberCleanup("(403)226-4672"), Mobile = PhoneNumberCleanup("(403)714-9321"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Darcy", LastName = "Haase", Phone = PhoneNumberCleanup("(403)285-6893"), Mobile = PhoneNumberCleanup("(403)860-8344"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Tractor(part time)" });
            employees.Add(new Employee { FirstName = "John", LastName = "Hall", Phone = PhoneNumberCleanup("(403)508-9966"), Mobile = PhoneNumberCleanup("(403)861-7568"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Winch Tractor" });
            employees.Add(new Employee { FirstName = "Jordy", LastName = "Cropley", Phone = PhoneNumberCleanup("(403)279-2280"), Mobile = PhoneNumberCleanup("(403)816-1645"), IsAvailable = true, IsSupervisor = true, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Jacking Supervisor" });
            employees.Add(new Employee { FirstName = "Wayne", LastName = "Sereda", Phone = null, Mobile = PhoneNumberCleanup("(403)816-1640"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("4/25/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Troy", LastName = "Duchscher", Phone = null, Mobile = PhoneNumberCleanup("(403)875-6979"), IsAvailable = true, IsSupervisor = true, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Jacking Supervisor" });
            employees.Add(new Employee { FirstName = "Dave", LastName = "Amyotte", Phone = null, Mobile = PhoneNumberCleanup("(403)605-5662"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Tractor(part time)" });
            employees.Add(new Employee { FirstName = "Allen", LastName = "Hart", Phone = PhoneNumberCleanup("(403)273-6899"), Mobile = PhoneNumberCleanup("(403)660-6161"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Tractor" });
            employees.Add(new Employee { FirstName = "Brian", LastName = "Jensen", Phone = PhoneNumberCleanup("(403)938-6414"), Mobile = PhoneNumberCleanup("(403)850-2429"), IsAvailable = true, IsSupervisor = true, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Field Supervisor" });
            employees.Add(new Employee { FirstName = "Greg", LastName = "Miller", Phone = PhoneNumberCleanup("(403)220-9849"), Mobile = PhoneNumberCleanup("(403)899-5374"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("7/1/2007", dateFormat, provider), Responsibilities = "Winch Tractor" });
            employees.Add(new Employee { FirstName = "Buck", LastName = "McCaw", Phone = null, Mobile = PhoneNumberCleanup("(403)899-0040"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Trailer Operator" });
            employees.Add(new Employee { FirstName = "Ryan", LastName = "Millions", Phone = null, Mobile = PhoneNumberCleanup("(403)861-4538"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("7/7/2006", dateFormat, provider), Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Randy", LastName = "Unser", Phone = PhoneNumberCleanup("(403)279-2280"), Mobile = PhoneNumberCleanup("(403)809-9097"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Winch Tractor" });
            employees.Add(new Employee { FirstName = "Don", LastName = "Lesniak", Phone = PhoneNumberCleanup("(403)253-4338"), Mobile = PhoneNumberCleanup("(403)990-4338"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Shannon", LastName = "Hunt", WindowsUserName = "shunt", Phone = PhoneNumberCleanup("(403)335-3739"), Mobile = PhoneNumberCleanup("(403)816-2148"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Dispatch Supervisor" });
            employees.Add(new Employee { FirstName = "Jordan", LastName = "Black", Phone = PhoneNumberCleanup("(403)207-7117"), Mobile = PhoneNumberCleanup("(403)852-6549"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Shop" });
            employees.Add(new Employee { FirstName = "Wayne", LastName = "Fairbrother", Phone = null, Mobile = PhoneNumberCleanup("(403)816-7243"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Winch Tractor" });
            employees.Add(new Employee { FirstName = "Jason", LastName = "Ip", Phone = PhoneNumberCleanup("(403)891-1288"), Mobile = PhoneNumberCleanup("(403)891-1288"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Richard", LastName = "Hoesing", Phone = null, Mobile = PhoneNumberCleanup("(403)934-1657"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("9/21/2007", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Tino", LastName = "Sarro", WindowsUserName = "vsarro", Phone = null, Mobile = PhoneNumberCleanup("(403)809-6648"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("4/13/2008", dateFormat, provider), Responsibilities = "Dispatch" });
            employees.Add(new Employee { FirstName = "Wyatt", LastName = "Singer", Phone = null, Mobile = PhoneNumberCleanup("(403)816-1640"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Justin", LastName = "LeBlanc", Phone = PhoneNumberCleanup("(403)280-2875"), Mobile = PhoneNumberCleanup("(403)807-0567"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("5/1/2006", dateFormat, provider), Responsibilities = "Push Tractor" });
            employees.Add(new Employee { FirstName = "Dan", LastName = "Klassen", Phone = PhoneNumberCleanup("(403)278-0499"), Mobile = PhoneNumberCleanup("(403)650-3886"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = null, Responsibilities = "Operations Manager" });
            employees.Add(new Employee { FirstName = "Steven", LastName = "Yan", Phone = null, Mobile = null, IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("12/25/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Kelly", LastName = "Carlson", Phone = PhoneNumberCleanup("(403)946-0154"), Mobile = PhoneNumberCleanup("(403)512-8031"), IsAvailable = true, IsSupervisor = true, IsSingerStaff = true, StartDate = DateTime.ParseExact("4/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("3/28/2007", dateFormat, provider), Responsibilities = "Field Supervisor" });
            employees.Add(new Employee { FirstName = "Darcy", LastName = "Nelson", Phone = null, Mobile = PhoneNumberCleanup("(403)921-0230"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("2/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Shawn", LastName = "Harvey", Phone = PhoneNumberCleanup("(403)995-2263"), Mobile = PhoneNumberCleanup("(403)703-7735"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("2/9/2007", dateFormat, provider), Responsibilities = "Trailer Operator" });
            employees.Add(new Employee { FirstName = "Greg", LastName = "Sandor", Phone = null, Mobile = PhoneNumberCleanup("(403)804-3570"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2004", dateFormat, provider), EndDate = DateTime.ParseExact("3/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Marcello", LastName = "Sarro", Phone = PhoneNumberCleanup("(403)703-7107"), Mobile = PhoneNumberCleanup("(403)703-7107"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("2/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Chris", LastName = "Beutler", Phone = PhoneNumberCleanup("(403)248-6674"), Mobile = PhoneNumberCleanup("(403)852-9726"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Push/Winch Tractor" });
            employees.Add(new Employee { FirstName = "Koal", LastName = "Graham", Phone = PhoneNumberCleanup("(403)207-8546"), Mobile = null, IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("5/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Darcy", LastName = "Hanlan", Phone = PhoneNumberCleanup("(780)417-2583"), Mobile = PhoneNumberCleanup("(780)203-1234"), IsAvailable = true, IsSupervisor = true, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("10/11/2006", dateFormat, provider), Responsibilities = "Trailer Operator / Pilot Driver" });
            employees.Add(new Employee { FirstName = "Howard", LastName = "Calpas", Phone = PhoneNumberCleanup("(403)901-1813"), Mobile = PhoneNumberCleanup("(403)801-8708"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("6/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("1/1/2006", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "TC", LastName = "Wells", Phone = PhoneNumberCleanup("(403)590-3591"), Mobile = PhoneNumberCleanup("(403)540-8449"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("2/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("10/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Robert", LastName = "Johnston", Phone = PhoneNumberCleanup("(403)590-9190"), Mobile = PhoneNumberCleanup("(403)519-7120"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("3/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Doug", LastName = "Wawia", Phone = PhoneNumberCleanup("(403)697-2139"), Mobile = PhoneNumberCleanup("(403)483-7028"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Tractor" });
            employees.Add(new Employee { FirstName = "Grant", LastName = "Hamel", Phone = PhoneNumberCleanup("(403)212-8022"), Mobile = PhoneNumberCleanup("(403)650-9241"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("7/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("5/5/2008", dateFormat, provider), Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Chad", LastName = "Congdon", Phone = PhoneNumberCleanup("(403)254-4779"), Mobile = PhoneNumberCleanup("(403)863-7209"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("6/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Tractor" });
            employees.Add(new Employee { FirstName = "Kelly", LastName = "Hodgins", Phone = PhoneNumberCleanup("(403)471-0304"), Mobile = PhoneNumberCleanup("(403)471-0304"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("2/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Victor", LastName = "Holtorf", Phone = PhoneNumberCleanup("(403)533-2199"), Mobile = PhoneNumberCleanup("(403)361-0124"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("4/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("5/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Brad", LastName = "Garner", Phone = PhoneNumberCleanup("(403)278-8231"), Mobile = PhoneNumberCleanup("(403)829-1670"), IsAvailable = true, IsSupervisor = true, IsSingerStaff = true, StartDate = DateTime.ParseExact("9/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Picker Operator" });
            employees.Add(new Employee { FirstName = "Cole", LastName = "Goodwin", Phone = null, Mobile = null, IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("1/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("9/1/2005", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Todd", LastName = "Chmelyk", Phone = PhoneNumberCleanup("(780)980-0814"), Mobile = PhoneNumberCleanup("(403)808-7964"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("10/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Tractor" });
            employees.Add(new Employee { FirstName = "Dustin", LastName = "Fraser", Phone = PhoneNumberCleanup("(403)355-5808"), Mobile = PhoneNumberCleanup("(403)809-5808"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/1/2005", dateFormat, provider), EndDate = DateTime.ParseExact("9/15/2006", dateFormat, provider), Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Ernie", LastName = "Loughran", Phone = PhoneNumberCleanup("(403)275-8571"), Mobile = PhoneNumberCleanup("(403)471-5269"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("2/1/2006", dateFormat, provider), EndDate = null, Responsibilities = "Winch Tractor" });
            employees.Add(new Employee { FirstName = "Mark", LastName = "Pasay", Phone = PhoneNumberCleanup("(403)286-1495"), Mobile = PhoneNumberCleanup("(403)875-1771"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2006", dateFormat, provider), EndDate = DateTime.ParseExact("8/18/2006", dateFormat, provider), Responsibilities = "Winch Tractor" });
            employees.Add(new Employee { FirstName = "Mike", LastName = "Collins", Phone = PhoneNumberCleanup("(403)252-1003"), Mobile = PhoneNumberCleanup("(403)863-6960"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("5/1/2006", dateFormat, provider), EndDate = null, Responsibilities = "Tractor" });
            employees.Add(new Employee { FirstName = "Frank", LastName = "Campeau", Phone = null, Mobile = null, IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/11/2006", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Kyle", LastName = "Paavola", Phone = null, Mobile = null, IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/10/2006", dateFormat, provider), EndDate = DateTime.ParseExact("9/2/2006", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Joe", LastName = "McKee", Phone = null, Mobile = PhoneNumberCleanup("(403)826-9706"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/17/2006", dateFormat, provider), EndDate = DateTime.ParseExact("10/13/2006", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Shawn", LastName = "Townsend", Phone = null, Mobile = PhoneNumberCleanup("(403)801-4636"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/14/2006", dateFormat, provider), EndDate = DateTime.ParseExact("11/2/2006", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Rebecca", LastName = "Singer", Phone = null, Mobile = PhoneNumberCleanup("(403)988-0742"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/1/2006", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Kevin", LastName = "Prychun", Phone = PhoneNumberCleanup("(403)381-3366"), Mobile = PhoneNumberCleanup("(403)315-1570"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("9/5/2006", dateFormat, provider), EndDate = DateTime.ParseExact("9/7/2006", dateFormat, provider), Responsibilities = "Door stop" });
            employees.Add(new Employee { FirstName = "Paul", LastName = "McGuire", Phone = null, Mobile = PhoneNumberCleanup("(403)808-4055"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("9/7/2006", dateFormat, provider), EndDate = DateTime.ParseExact("10/6/2006", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Luke", LastName = "Kozie", Phone = PhoneNumberCleanup("(403)935-4892"), Mobile = PhoneNumberCleanup("(403)862-7900"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("9/1/2006", dateFormat, provider), EndDate = DateTime.ParseExact("2/15/2007", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Kevin", LastName = "Skory", Phone = PhoneNumberCleanup("(403)652-7942"), Mobile = PhoneNumberCleanup("(403)652-0337"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = null, EndDate = DateTime.ParseExact("10/20/2006", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Allan", LastName = "Mitchell", Phone = PhoneNumberCleanup("(403)441-9270"), Mobile = PhoneNumberCleanup("(403)978-9270"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("11/14/2006", dateFormat, provider), EndDate = DateTime.ParseExact("12/2/2007", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "JP", LastName = "Corkey", Phone = null, Mobile = PhoneNumberCleanup("(403)366-0543"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("12/4/2006", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Ian", LastName = "Parker", Phone = PhoneNumberCleanup("(403)569-7650"), Mobile = PhoneNumberCleanup("(902)790-2957"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("7/3/2007", dateFormat, provider), EndDate = DateTime.ParseExact("11/2/2007", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Caz", LastName = "Wojnowski", Phone = null, Mobile = PhoneNumberCleanup("(403)463-8987"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("7/30/2007", dateFormat, provider), EndDate = DateTime.ParseExact("10/5/2008", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Andrew", LastName = "Nickel", Phone = null, Mobile = PhoneNumberCleanup("(403)463-8589"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/6/2007", dateFormat, provider), EndDate = DateTime.ParseExact("8/13/2007", dateFormat, provider), Responsibilities = "Pilot" });
            employees.Add(new Employee { FirstName = "Scott", LastName = "Crawford", Phone = PhoneNumberCleanup("(403)285-2696"), Mobile = PhoneNumberCleanup("(403)714-7710"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/9/2007", dateFormat, provider), EndDate = DateTime.ParseExact("10/10/2007", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Phil", LastName = "Brindle", Phone = null, Mobile = PhoneNumberCleanup("(403)660-8911"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/27/2007", dateFormat, provider), EndDate = DateTime.ParseExact("9/27/2008", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Marc", LastName = "Collin", Phone = PhoneNumberCleanup("(403)475-6928"), Mobile = PhoneNumberCleanup("(403)617-5505"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/27/2007", dateFormat, provider), EndDate = DateTime.ParseExact("11/2/2007", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Curtis", LastName = "Macmillan", Phone = PhoneNumberCleanup("(403)938-7763"), Mobile = PhoneNumberCleanup("(403)852-2638"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("10/29/2007", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Cody", LastName = "LaFrance", Phone = PhoneNumberCleanup("(403)697-8820"), Mobile = PhoneNumberCleanup("(403)796-3636"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("2/6/2008", dateFormat, provider), EndDate = null, Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Ory", LastName = "Rehm", Phone = PhoneNumberCleanup("(403)590-4531"), Mobile = PhoneNumberCleanup("(867)872-0833"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("2/7/2008", dateFormat, provider), EndDate = DateTime.ParseExact("12/1/2008", dateFormat, provider), Responsibilities = "Pilot Driver" });
            employees.Add(new Employee { FirstName = "Jeff", LastName = "Dobbs", Phone = null, Mobile = null, IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("2/16/2008", dateFormat, provider), EndDate = DateTime.ParseExact("4/6/2008", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Stan", LastName = "Fremstad", Phone = null, Mobile = PhoneNumberCleanup("(208)420-3262"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("5/22/2008", dateFormat, provider), EndDate = DateTime.ParseExact("9/26/2008", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Bill", LastName = "Both", Phone = null, Mobile = PhoneNumberCleanup("(403)542-9759"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("6/16/2008", dateFormat, provider), EndDate = DateTime.ParseExact("8/15/2008", dateFormat, provider), Responsibilities = "Driver" });
            employees.Add(new Employee { FirstName = "Dennis", LastName = "Hurly", Phone = null, Mobile = PhoneNumberCleanup("(403)305-8359"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/22/2008", dateFormat, provider), EndDate = DateTime.ParseExact("9/4/2008", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Jim", LastName = "Olliver", Phone = PhoneNumberCleanup("(403)295-1128"), Mobile = PhoneNumberCleanup("(403)481-4144"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/29/2008", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Pat", LastName = "Johnson", Phone = null, Mobile = PhoneNumberCleanup("(780)796-9228"), IsAvailable = false, IsSupervisor = false, IsSingerStaff = false, StartDate = DateTime.ParseExact("9/15/2008", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Kent", LastName = "Johnson", Phone = null, Mobile = PhoneNumberCleanup("(403)305-6897"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("9/15/2008", dateFormat, provider), EndDate = DateTime.ParseExact("9/26/2008", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Curt", LastName = "Fehr", Phone = null, Mobile = PhoneNumberCleanup("(403)700-2067"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("9/26/2008", dateFormat, provider), EndDate = DateTime.ParseExact("10/5/2008", dateFormat, provider), Responsibilities = null });
            employees.Add(new Employee { FirstName = "Jason", LastName = "Porte", Phone = null, Mobile = PhoneNumberCleanup("(403)836-7455"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("9/29/2008", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Ryan", LastName = "Moore", Phone = PhoneNumberCleanup("(403)601-6135"), Mobile = PhoneNumberCleanup("(403)305-0273"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("11/3/2008", dateFormat, provider), EndDate = null, Responsibilities = "Pilot" });
            employees.Add(new Employee { FirstName = "Corey", LastName = "Cuthill", Phone = PhoneNumberCleanup("(403)797-3018"), Mobile = PhoneNumberCleanup("(403)999-5859"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("11/18/2008", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Cody", LastName = "Hanna", Phone = PhoneNumberCleanup("(403)285-7318"), Mobile = PhoneNumberCleanup("(403)465-6702"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("1/26/2009", dateFormat, provider), EndDate = null, Responsibilities = null });
            employees.Add(new Employee { FirstName = "Julien", LastName = "Barré", Phone = null, Mobile = PhoneNumberCleanup("(403)921-8294"), IsAvailable = true, IsSupervisor = false, IsSingerStaff = true, StartDate = DateTime.ParseExact("8/1/2005", dateFormat, provider), EndDate = null, Responsibilities = "Tractor" });
            employees.Add(new Employee { FirstName = "Simon", LastName = "Twogood", WindowsUserName = "stwogood" });

            Database.Employees.InsertAllOnSubmit(employees);

            
            // Add Load and Unload methods
            var methods = new List<LoadUnloadMethod>();

            methods.Add(new LoadUnloadMethod { Name = "Bed Truck" });
            methods.Add(new LoadUnloadMethod { Name = "Bobcat" });
            methods.Add(new LoadUnloadMethod { Name = "Bull Moose" });
            methods.Add(new LoadUnloadMethod { Name = "Cradle Trailer" });
            methods.Add(new LoadUnloadMethod { Name = "Crane" });
            methods.Add(new LoadUnloadMethod { Name = "Cranes" });
            methods.Add(new LoadUnloadMethod { Name = "Drive off" });
            methods.Add(new LoadUnloadMethod { Name = "Drive on" });
            methods.Add(new LoadUnloadMethod { Name = "Forklift" });
            methods.Add(new LoadUnloadMethod { Name = "Jack & Roll" });
            methods.Add(new LoadUnloadMethod { Name = "Overhead Crane" });
            methods.Add(new LoadUnloadMethod { Name = "Picker" });
            methods.Add(new LoadUnloadMethod { Name = "Pickers" });
            methods.Add(new LoadUnloadMethod { Name = "RT Crane" });
            methods.Add(new LoadUnloadMethod { Name = "Scheuerle Trailer" });
            methods.Add(new LoadUnloadMethod { Name = "Tower Crane" });
            methods.Add(new LoadUnloadMethod { Name = "Winch" });

            Database.LoadUnloadMethods.InsertAllOnSubmit(methods);
                        

            // Add a note that was included in all previous quotes as a default condition...
            Database.Conditions.InsertOnSubmit(new Condition { Line = "Cargo Insruance coverage subject to declared values supplied by shipper/owner.", AutoInclude = true });

            Database.SubmitChanges();
        }

        public void PopulateDefaults()
        {
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "SingerName", Value = "Singer Specialized Ltd." });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "EnterpriseName", Value = "M.E. Singer Enterprise" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "GSTRate", Value = "0.05m" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "FuelTaxRate", Value = "0.00m" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "SingerAddress-StreetAddress", Value = "235132 84th St. SE" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "SingerAddress-City", Value = "Calgary, AB T1X 0K1" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "SingerAddress-Phone", Value = "(403) 569-8605" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "EnterpriseAddress-StreetAddress", Value = "235132 84th St. SE" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "EnterpriseAddress-City", Value = "Calgary, AB T1X 0K1" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "EnterpriseAddress-Phone", Value = "(403) 569-8605" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Quote-DefaultSignoff", Value = "We appreciate the opportunity to supply a quotation for your project.  Should you have any questions, concerns, or  comments, please feel free to contact me at your convenience." });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "PDF-ExecutablePath", Value = "PDF\\wkhtmltopdf.exe" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "PDF-Arguments", Value = @"--print-media-type --page-size Letter ""%HTML_FILE%"" ""%PDF_FILE%""" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Documents-SingerHeaderImg", Value = @"Images\DocumentHeader.png" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Documents-MEHeaderImg", Value = @"Images\DocumentHeader.png" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "GenericRemoveItemConfirmation", Value = "Are you sure you want to remove this item?" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "BillOfLading-MainLegal", Value = "COMBINATION SHORT FORM OF STRAIGHT BILL OF LADING - EXPRESS SHIPPING CONTRACT ADOPTED BY RAIL FREIGHT AND EXPRESS CARRIER'S SUBJECT TO THE JURISDICTION OF THE CANADIAN TRANSPORT COMMISION. ISSUED AT SHIPPER'S REQUEST.<br><br>RECIEVED AT THE POINT OF ORIGIN ON THE DATE SPECIFIED, FROM THE CONSIGNOR MENTIONED HEREIN, THE PROPERTY HEREIN DESCRIBED, IN APPARENT GOOD ORDER, EXCEPT AS NOTED (CONTENTS OF PACKAGES AND CONDITIONS OF CONTENTS ARE UNKNOWN) MARKED, CONSIGNED AND DESTINED AS INDICATED BELOW, WHICH THE CARRIER AGREES TO CARRY AND TO DELIVER TO THE CONSIGNEE AT THE SAID DESTINATION, IF ON ITS OWN AUTHORIZED ROUTE OR OTHERWISE TO CAUSE TO BE CARRIED BY ANOTHER CARRIER ON THE ROUTE TO SAID DESTINATION SUBJECT TO THE RATES AND CLASSIFICATION IN EFFECT ON THE DATE OF SHIPMENT. IT IS MUTUALLY AGREED, AS TO EACH CARRIER OF ALL OR ANY OF THE GOODS OVERALL OR ANY PORTION OF THE ROUTE TO DESTINATION; AND AS TO EACH PARTY OF ANY TIME INTERESTED IN ALL OR ANY OF THE GOODS, THAT EVERY SERVICE TO BE PERFORMED HEREUNDER SHALL BE SUBJECT TO ALL THE CONDITIONS NOTE PROHIBITED BY LAW, WHETHER PRINTED OR WRITTEN, INCLUDING CONDITIONS ON BACK HEREOF, WHICH ARE HEREBY AGREED BY THE CONSIGNOR ACCEPTED FOR HIMSELF AND HIS ASSIGNS." });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "BillOfLading-ShippersWeight", Value = "<p>The shipper declares that the weight of this shipment does not exceed the maximum freight weight shown above; and agree to indemnify the carrier and to pay as part of the transportation charges in accordance with the carrier's filed tariffs, and fine or penalty incurred by the carrier by reason of a violation of any provision of the Transport Safety Act, arising from any error or misstatement, intensional or otherwise, in this weight declaration.</p>" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "BillOfLading-NoticeOfClaim", Value = "<p>A) No carrier is liable for loss, damage or delay to any goods carried under the Bill of Lading unless notice thereof setting out particulars of the origin, detination and date of shipment of the goods and the estimated amount claimed in respect of such loss, damage or delay is given in writing to the originating carrier or the delievering carrier within seven (7) days after teh delievery of the goods, or, in the case of failure to make delivery, within nine (9) months from the date of shipment. B) The final statement of the claim must be filed within nine (9) months from the date of shipment together with a copy of the paid freight bill.</p>" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "BillOfLading-MaxLiability", Value = "Maximum liability of $2.00 per lb/$4.41 per kg unless declared valuation states otherwise" });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Dispatch-EnterpriseBoLFileCopies", Value = 1.ToString() });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Dispatch-EnterpriseBoLDriverCopies", Value = 1.ToString() });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Dispatch-SingerBoLFileCopies", Value = 1.ToString() });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Dispatch-SingerBoLDriverCopies", Value = 3.ToString() });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Dispatch-EnterprisePrintFileCopy", Value = false.ToString() });
            Database.Configurations.InsertOnSubmit(new Configuration { Name = "Dispatch-SingerPrintFileCopy", Value = true.ToString() });

            Database.SubmitChanges();
        }

        private static string PhoneNumberCleanup(string number)
        {
            return string.Format(new LafiPhoneFormatProvider(), "{0:de}", number);
        }
    }
}

