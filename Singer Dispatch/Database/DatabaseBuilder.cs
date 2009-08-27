using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Database
{
    class DatabaseBuilder
    {
        private SingerDispatchDataContext database;

        public DatabaseBuilder(SingerDispatchDataContext context)
        {
            database = context;
        }

        public void CreateNewDatabase()
        {
            if (database.DatabaseExists())
            {
                throw new Exception("Database already exists!");
            }

            database.CreateDatabase();

            // Populate countries and provinces
            Country canada = new Country() { Name = "Canada" };
            Country usa = new Country() { Name = "USA" };

            database.Countries.InsertOnSubmit(canada);
            database.Countries.InsertOnSubmit(usa);

            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Ontario", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Quebec", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "British Columbia", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Alberta", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Manitoba", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Saskatchewan", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Nova Scotia", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "New Brunswick", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Newfoundland and Labrador", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Prince Edward Island", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Northwest Territories", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Yukon", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Nunavut", Country = canada });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Delaware", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Pennsylvania", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "New Jersey", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Georgia", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Connecticut", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Massachusetts", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Maryland", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "South Carolina", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "New Hampshire", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Virginia", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "New York", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "North Carolina", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Rhode Island", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Vermont", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Kentucky", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Tennessee", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Ohio", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Louisiana", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Indiana", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Mississippi", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Illinois", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Alabama", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Maine", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Missouri", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Arkansas", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Michigan", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Florida", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Texas", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Iowa", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Wisconsin", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "California", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Minnesota", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Oregon", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Kansas", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "West Virginia", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Nevada", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Nebraska", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Colorado", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "North Dakota", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "South Dakota", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Montana", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Washington", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Idaho", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Wyoming", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Utah", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Oklahoma", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "New Mexico", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Arizona", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Alaska", Country = usa });
            database.ProvincesAndStates.InsertOnSubmit(new ProvincesAndState() { Name = "Hawaii", Country = usa });

            // Populate service types
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Bed Truck" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Bridge" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Cats" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Cranemats / Rigmats" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Cranes" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Equipment Rental" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Hired Trucking" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Hot Shot Service" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Jack and Roll / Rigging" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Light Swinging" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Loading/Unloading" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Pilot Cars" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Police Escorts" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Provide & Place no Parking signage" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Railcar Tie Down" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Railway Crossing Police" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Rental Trailer" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Repairs" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Welders" });
            database.ServiceTypes.InsertOnSubmit(new ServiceType() { Name = "Wirelift" });

            // Populate address types
            database.AddressTypes.InsertOnSubmit(new AddressType() { Name = "Head Office" });
            database.AddressTypes.InsertOnSubmit(new AddressType() { Name = "Local Office" });
            database.AddressTypes.InsertOnSubmit(new AddressType() { Name = "Site Office" });

            // Populate priority levels            
            database.CompanyPriorityLevels.InsertOnSubmit(new CompanyPriorityLevel() { Name = "1. Elite" });
            database.CompanyPriorityLevels.InsertOnSubmit(new CompanyPriorityLevel() { Name = "2. Prestige" });
            database.CompanyPriorityLevels.InsertOnSubmit(new CompanyPriorityLevel() { Name = "3. Premier" });
            database.CompanyPriorityLevels.InsertOnSubmit(new CompanyPriorityLevel() { Name = "4. Regular" });
            database.CompanyPriorityLevels.InsertOnSubmit(new CompanyPriorityLevel() { Name = "5. Cash on Delivery" });

            // Poputlate billing types            
            database.BillingTypes.InsertOnSubmit(new BillingType() { Name = "Per Item" });
            database.BillingTypes.InsertOnSubmit(new BillingType() { Name = "Per Hour" });
            database.BillingTypes.InsertOnSubmit(new BillingType() { Name = "Cost Included" });

            // Populate rates
            database.Rates.InsertOnSubmit(new Rate() { Name = "10Line", IsSpecialized = 1, Hourly = 700.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "10Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "10LineDbl", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "10LineDbl", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12Line", IsSpecialized = 1, Hourly = 800.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12Line - 6Line + 6Line", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12Line - 6Line + 6Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12LineDbl", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12LineDbl", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12Wheel", IsSpecialized = 1, Hourly = 170.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "12Wheel", IsSpecialized = 0, Hourly = 140.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "14Line", IsSpecialized = 1, Hourly = 1000.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "14Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "14Line - 6Line + 8Line", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "14Line - 6Line + 8Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "14LineDbl", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "14LineDbl", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "16Line - 8Line + 8Line", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "16Line - 8Line + 8Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "16Wheel", IsSpecialized = 1, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "16Wheel", IsSpecialized = 0, Hourly = 160.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "18Line", IsSpecialized = 1, Hourly = 1300.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "18Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "18Line - 10Line + 8Line", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "18Line - 10Line + 8Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "20Line", IsSpecialized = 1, Hourly = 1500.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "20Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "20Line - 10Line + 10Line", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "20Line - 10Line + 10Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "20Wheel", IsSpecialized = 1, Hourly = 245.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "20Wheel", IsSpecialized = 0, Hourly = 190.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "22Line - 10Line + 12Line", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "22Line - 10Line + 12Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "24Line - 12Line + 12Line", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "24Line - 12Line + 12Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "24Wheel", IsSpecialized = 1, Hourly = 245.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "24Wheel", IsSpecialized = 0, Hourly = 190.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "32Wheel", IsSpecialized = 1, Hourly = 270.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "32Wheel", IsSpecialized = 0, Hourly = 205.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "40Wheel", IsSpecialized = 1, Hourly = 300.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "40Wheel", IsSpecialized = 0, Hourly = 240.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "48Wheel", IsSpecialized = 1, Hourly = 335.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "48Wheel", IsSpecialized = 0, Hourly = 270.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "4Wheel", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "4Wheel", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "52Wheel", IsSpecialized = 1, Hourly = 360.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "52Wheel", IsSpecialized = 0, Hourly = 290.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "60Wheel", IsSpecialized = 1, Hourly = 360.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "60Wheel", IsSpecialized = 0, Hourly = 290.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "64Wheel", IsSpecialized = 1, Hourly = 375.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "64Wheel", IsSpecialized = 0, Hourly = 310.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "6Line", IsSpecialized = 1, Hourly = 600.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "6Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "6LineDbl", IsSpecialized = 1, Hourly = 540.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "6LineDbl", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "76Wheel", IsSpecialized = 1, Hourly = 400.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "76Wheel", IsSpecialized = 0, Hourly = 370.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "7Axle", IsSpecialized = 1, Hourly = 265.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "7Axle", IsSpecialized = 0, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "8Line", IsSpecialized = 1, Hourly = 650.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "8Line", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "8LineDbl", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "8LineDbl", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "8Wheel", IsSpecialized = 1, Hourly = 155.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "8Wheel", IsSpecialized = 0, Hourly = 125.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "96wheel", IsSpecialized = 1, Hourly = 750.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "96wheel", IsSpecialized = 0, Hourly = 680.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "9Axle", IsSpecialized = 1, Hourly = 265.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "9Axle", IsSpecialized = 0, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Bobcat", IsSpecialized = 1, Hourly = 95.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Bobcat", IsSpecialized = 0, Hourly = 95.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Bull Moose - 0 Swamper", IsSpecialized = 1, Hourly = 190.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Bull Moose - 0 Swamper", IsSpecialized = 0, Hourly = 190.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Bull Moose - 1 Swamper", IsSpecialized = 1, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Bull Moose - 1 Swamper", IsSpecialized = 0, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Compressor Tarping", IsSpecialized = 1, Hourly = 700.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Compressor Tarping", IsSpecialized = 0, Hourly = 700.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Contractor", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Contractor", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Cooler Tarping", IsSpecialized = 1, Hourly = 500.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Cooler Tarping", IsSpecialized = 0, Hourly = 500.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "In-Town Jack", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "In-Town Jack", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Jack & Roll", IsSpecialized = 1, Hourly = 525.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Jack & Roll", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Lease Operator Equipment", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Lease Operator Equipment", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Lima Crane - 0 Swamper", IsSpecialized = 1, Hourly = 290.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Lima Crane - 0 Swamper", IsSpecialized = 0, Hourly = 290.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Lima Crane - 1 Swamper", IsSpecialized = 1, Hourly = 325.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Lima Crane - 1 Swamper", IsSpecialized = 0, Hourly = 325.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Other", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Other", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Out-Town Jack", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Out-Town Jack", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Pilot Car", IsSpecialized = 1, Hourly = 80.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Pilot Car", IsSpecialized = 0, Hourly = 80.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Pull Tractor", IsSpecialized = 1, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Pull Tractor", IsSpecialized = 0, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Push Tractor", IsSpecialized = 1, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "PushTractor", IsSpecialized = 0, Hourly = 225.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Rigging - Bull Moose", IsSpecialized = 1, Hourly = 415.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Rigging - Bull Moose", IsSpecialized = 0, Hourly = 415.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Rigging - Lima Crane", IsSpecialized = 1, Hourly = 525.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Rigging - Lima Crane", IsSpecialized = 0, Hourly = 525.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Route Study", IsSpecialized = 1, Hourly = 95.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Route Study", IsSpecialized = 0, Hourly = 95.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Shop", IsSpecialized = 1, Hourly = 85.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Shop", IsSpecialized = 0, Hourly = 85.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Storage", IsSpecialized = 1 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Storage", IsSpecialized = 0 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Subsistence", IsSpecialized = 1, Hourly = 175.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Subsistence", IsSpecialized = 0, Hourly = 175.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Supervisor", IsSpecialized = 1, Hourly = 110.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Supervisor", IsSpecialized = 0, Hourly = 110.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Swamper", IsSpecialized = 1, Hourly = 65.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Swamper", IsSpecialized = 0, Hourly = 75.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Tandem Picker", IsSpecialized = 1, Hourly = 245.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Tandem Picker", IsSpecialized = 0, Hourly = 245.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Tandem Tractor", IsSpecialized = 1, Hourly = 125.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Tandem Tractor", IsSpecialized = 0, Hourly = 125.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Tandem Winch Tractor", IsSpecialized = 1, Hourly = 145.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Tandem Winch Tractor", IsSpecialized = 0, Hourly = 145.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Trojan Loader", IsSpecialized = 1, Hourly = 170.00 });
            database.Rates.InsertOnSubmit(new Rate() { Name = "Trojan Loader", IsSpecialized = 0, Hourly = 170.00 });

            database.SubmitChanges();
        }
    }
}
