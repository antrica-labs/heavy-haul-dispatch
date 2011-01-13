using System;
using System.Text;

namespace SingerDispatch
{
    partial class SingerDispatchDataContext
    {
        public SingerDispatchDataContext() : base(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionParameters"].ConnectionString)
        {
        }
    }

    partial class Company
    {
        public int? ArchiveID { get; set; }

        partial void OnCreated()
        {
            IsVisible = IsVisible ?? true;
            EquifaxComplete = EquifaxComplete ?? false;
        }

        public string Alias
        {
            get
            {
                return OperatingAs ?? Name;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void Notify(string property)
        {
            SendPropertyChanging();
            SendPropertyChanged(property);
        }
    }

    partial class Address
    {
        public int? ArchiveID { get; set; }

        public override string ToString()
        {
            var address = Line1;

            if (!string.IsNullOrWhiteSpace(City))
                address += ", " + City;

            if (ProvincesAndState != null)
                address += ", " + ProvincesAndState.Abbreviation;

            return address;
        }
    }

    partial class Contact
    {
        public int? ArchiveID { get; set; }

        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    partial class Employee
    {
        partial void OnCreated()
        {
            Archived = Archived ?? false;
        }

        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    partial class EquipmentType
    {
        public override string ToString()
        {
            return string.Format("{0} - {1}", Prefix, Name);
        }
    }

    partial class Equipment
    {
        partial void OnCreated()
        {
            Archived = Archived ?? false;
        }
    }

    partial class Commodity
    {
        public string NameAndUnit
        {
            get
            {
                var name = Name;

                if (!string.IsNullOrEmpty(Unit))
                    name += " - " + Unit;
                
                return name;
            }
        }

        partial void OnCreated()
        {
            DimensionsEstimated = DimensionsEstimated ?? true;
        }

        public Commodity Duplicate()
        {
            var copy = new Commodity();
            
            copy.Name = Name;
            copy.Value = Value;
            copy.Serial = Serial;
            copy.Unit = Unit;
            copy.Length = Length;
            copy.Width = Width;
            copy.Height = Height;
            copy.Weight = Weight;
            copy.DimensionsEstimated = DimensionsEstimated;
            copy.LastLocation = LastLocation;
            copy.LastAddress = LastAddress;
            copy.LastLoadInstructions = LastLoadInstructions;
            copy.LastRoute = LastRoute;

            return copy;
        }
    }

    partial class Quote
    {
        partial void OnCreated()
        {
            Price = Price ?? 0.00m;
            IsItemizedBilling = IsItemizedBilling ?? false;
        }

        public string FriendlyName
        {
            get
            {
                return string.Format("{0} rev. {1}", Number, Revision);
            }
        }

        public string NumberAndRev
        {
            get
            {
                return String.Format("{0}-{1}", Number, Revision);
            }
        }

        public Quote Duplicate()
        {
            var copy = new Quote();
                        
            copy.Company = Company;
            copy.Number = Number;
            copy.CareOfCompany = CareOfCompany;
            copy.BillingAddress = BillingAddress;
            copy.Contact = Contact;
            copy.Description = Description;
            copy.StartDate = StartDate;
            copy.EndDate = EndDate;
            copy.CreationDate = CreationDate;
            copy.ExpirationDate = ExpirationDate;
            copy.Employee = Employee;
            copy.Price = Price;

            foreach (var commodity in QuoteCommodities)
            {
                copy.QuoteCommodities.Add(commodity.Duplicate());
            }

            foreach (var supplement in QuoteSupplements)
            {
                copy.QuoteSupplements.Add(supplement.Duplicate());
            }

            foreach (var item in QuoteStorageItems)
            {
                copy.QuoteStorageItems.Add(item.Duplicate());
            }

            foreach (var item in QuoteConditions)
            {
                copy.QuoteConditions.Add(item.Duplicate());
            }

            foreach (var item in QuoteNotes)
            {
                copy.QuoteNotes.Add(item.Duplicate());
            }

            return copy;
        }

        public Job ToJob()
        {
            var job = new Job();

            // Fill the job properties with as many of the applicable quote properties as possible
            job.Quote = this;
            job.Company = Company;
            job.CareOfCompany = CareOfCompany;
            job.Contact = Contact;
            job.Notes = Description;

            foreach (var commodity in QuoteCommodities)
            {
                job.JobCommodities.Add(commodity.ToJobCommodity());
            }

            return job;
        }
    }

    partial class QuoteCommodity
    {
        private Boolean _InBulkChange;

        partial void OnCreated()
        {
            _InBulkChange = false;
            OrderIndex = 1;
            DimensionsEstimated = DimensionsEstimated ?? true;

            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OriginalCommodity == null) return;

            switch (e.PropertyName)
            {
                case "OriginalCommodity":
                    _InBulkChange = true;
                    OriginalCommodityChanged();
                    _InBulkChange = false;
                    break;
                default:
                    BasicPropertyChanged();
                    break;
            }
        }

        private void OriginalCommodityChanged()
        {
            if (OriginalCommodity == null) return;

            Name = OriginalCommodity.Name;
            Value = OriginalCommodity.Value;
            Serial = OriginalCommodity.Serial;
            Unit = OriginalCommodity.Unit;
            Length = OriginalCommodity.Length;
            Width = OriginalCommodity.Width;
            Height = OriginalCommodity.Height;
            Weight = OriginalCommodity.Weight;
            DimensionsEstimated = OriginalCommodity.DimensionsEstimated;
            Notes = OriginalCommodity.Notes;
            DepartureAddress = OriginalCommodity.LastAddress;
            DepartureSiteName = OriginalCommodity.LastLocation;
        }

        private void BasicPropertyChanged()
        {
            if (_InBulkChange || OriginalCommodity == null) return;

            OriginalCommodity.Name = Name;
            OriginalCommodity.Value = Value;
            OriginalCommodity.Serial = Serial;
            OriginalCommodity.Unit = Unit;
            OriginalCommodity.Length = Length;
            OriginalCommodity.Width = Width;
            OriginalCommodity.Height = Height;
            OriginalCommodity.Weight = Weight;
            OriginalCommodity.DimensionsEstimated = DimensionsEstimated;
            OriginalCommodity.LastLocation = DepartureSiteName;
            OriginalCommodity.LastAddress = DepartureAddress;
        }

        public QuoteCommodity Duplicate()
        {
            var copy = new QuoteCommodity();

            copy.OriginalCommodity = OriginalCommodity;
            copy.DepartureSiteName = DepartureSiteName;
            copy.DepartureAddress = DepartureAddress;
            copy.ArrivalSiteName = ArrivalSiteName;
            copy.ArrivalAddress = ArrivalAddress;
            copy.Name = Name;
            copy.Value = Value;
            copy.Serial = Serial;
            copy.Unit = Unit;
            copy.Length = Length;
            copy.Width = Width;
            copy.Height = Height;
            copy.Weight = Weight;
            copy.DimensionsEstimated = DimensionsEstimated;
            copy.Cost = Cost;
            copy.Notes = Notes;

            return copy;
        }

        public Commodity ToRecordedCommodity()
        {
            var commodity = new Commodity();

            commodity.Company = Owner;
            commodity.Name = Name;
            commodity.Unit = Unit;
            commodity.Value = Value;
            commodity.Serial = Serial;
            commodity.Notes = Notes;
            commodity.Length = Length;
            commodity.Width = Width;
            commodity.Height = Height;
            commodity.Weight = Weight;
            commodity.DimensionsEstimated = DimensionsEstimated;
            commodity.LastLocation = DepartureSiteName;
            commodity.LastAddress = DepartureAddress;

            return commodity;
        }

        public JobCommodity ToJobCommodity()
        {
            var jc = new JobCommodity();

            jc.OrderIndex = OrderIndex;
            jc.OriginalCommodity = OriginalCommodity;
            jc.Name = Name;
            jc.Value = Value;
            jc.Serial = Serial;
            jc.Unit = Unit;
            jc.DepartureSiteName = DepartureSiteName;
            jc.DepartureAddress = DepartureAddress;
            jc.ArrivalSiteName = ArrivalSiteName;
            jc.ArrivalAddress = ArrivalAddress;
            jc.Length = Length;
            jc.Width = Width;
            jc.Height = Height;
            jc.Weight = Weight;
            jc.DimensionsEstimated = DimensionsEstimated;
            jc.Cost = Cost;
            jc.Notes = Notes;            

            return jc;
        }

        public string NameAndUnit
        {
            get
            {
                var name = Name;

                if (!string.IsNullOrEmpty(Unit))
                    name += " - " + Unit;

                return name;
            }
        }
    }

    partial class Condition
    {
        partial void OnCreated()        
        {
            Archived = Archived ?? false;
            AutoInclude = AutoInclude ?? false;
        }
    }

    partial class Inclusion
    {
        partial void OnCreated()
        {
            Archived = Archived ?? false;
        }
    }

    partial class QuoteCondition
    {
        public QuoteCondition Duplicate()
        {
            var copy = new QuoteCondition();

            copy.OriginalCondition = OriginalCondition;
            copy.Line = Line;

            return copy;
        }
    }

    partial class QuoteSupplement
    {
        public QuoteSupplement Duplicate()
        {
            var copy = new QuoteSupplement();

            copy.Name = Name;
            copy.Details = Details;
            copy.BillingType = BillingType;
            copy.Quantity = Quantity;
            copy.Cost = Cost;

            return copy;
        }
    }

    partial class QuoteNote
    {
        public QuoteNote Duplicate()
        {           
            return new QuoteNote { Note = this.Note };
        }
    }

    partial class QuoteStorageItem
    {
        public QuoteStorageItem Duplicate()
        {
            var copy = new QuoteStorageItem();
            
            copy.Commodity = Commodity;
            copy.Price = Price;
            copy.Notes = Notes;

            return copy;
        }
    }

    partial class Rate
    {
        partial void OnCreated()
        {
            Archived = Archived ?? false;
        }

        public decimal? Hourly { get; set; }
        public decimal? Adjusted { get; set; }
    }

    partial class Job
    {
        partial void OnCreated()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
                StatusChanged();
        }

        private void StatusChanged()
        {
            if (Status == null || Status.Name != "Completed") return;
            
            foreach (var item in Loads)
            {
                item.Status = Status;
            }
        }

        public Job Duplicate()
        {
            var cp = new Job();

            cp.Name = Name;
            cp.CareOfCompany = CareOfCompany;
            cp.Company = Company;
            cp.Notes = Notes;            
            cp.Quote = Quote;            
            cp.Status = Status;
            
            foreach (var item in JobCommodities)
                cp.JobCommodities.Add(item.Duplicate());

            foreach (var item in Loads)
                cp.Loads.Add(item.Duplicate());

            foreach (var item in ReferenceNumbers)
                cp.ReferenceNumbers.Add(item.Duplicate());

            return cp;
        }
    }

    partial class JobReferenceNumber
    {
        public JobReferenceNumber Duplicate()
        {
            return new JobReferenceNumber { Field = Field, Value = Value };
        }
    }

    partial class JobCommodity
    {
        private Boolean _InBulkChange;

        partial void OnCreated()
        {
            _InBulkChange = false;
            DimensionsEstimated = DimensionsEstimated ?? true;
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OriginalCommodity == null) return;

            switch (e.PropertyName)
            {
                case "OriginalCommodity":
                    _InBulkChange = true;
                    OriginalCommodityChanged();
                    _InBulkChange = false;
                    break;
                default:
                    BasicPropertyChanged();
                    break;
            }
        }

        private void OriginalCommodityChanged()
        {
            if (OriginalCommodity == null) return;

            Name = OriginalCommodity.Name;
            Value = OriginalCommodity.Value;
            Serial = OriginalCommodity.Serial;
            Unit = OriginalCommodity.Unit;
            Length = OriginalCommodity.Length;
            Width = OriginalCommodity.Width;
            Height = OriginalCommodity.Height;
            Weight = OriginalCommodity.Weight;
            DimensionsEstimated = OriginalCommodity.DimensionsEstimated;
            Notes = OriginalCommodity.Notes;
            DepartureAddress = OriginalCommodity.LastAddress;
            DepartureSiteName = OriginalCommodity.LastLocation;
        }

        private void BasicPropertyChanged()
        {
            if (_InBulkChange || OriginalCommodity == null) return;

            OriginalCommodity.Name = Name;
            OriginalCommodity.Value = Value;
            OriginalCommodity.Serial = Serial;
            OriginalCommodity.Unit = Unit;
            OriginalCommodity.Length = Length;
            OriginalCommodity.Width = Width;
            OriginalCommodity.Height = Height;
            OriginalCommodity.Weight = Weight;
            OriginalCommodity.DimensionsEstimated = DimensionsEstimated;
            OriginalCommodity.LastAddress = DepartureAddress;
            OriginalCommodity.LastLocation = DepartureSiteName;
        }

        public Commodity ToRecordedCommodity()
        {
            var commodity = new Commodity();

            commodity.Company = Owner;
            commodity.Name = Name;
            commodity.Unit = Unit;
            commodity.Value = Value;
            commodity.Serial = Serial;
            commodity.Notes = Notes;
            commodity.Length = Length;
            commodity.Width = Width;
            commodity.Height = Height;
            commodity.Weight = Weight;
            commodity.DimensionsEstimated = DimensionsEstimated;
            commodity.LastLocation = DepartureSiteName;
            commodity.LastAddress = DepartureAddress;

            return commodity;
        }

        public string NameAndUnit
        {
            get
            {
                var name = Name;

                if (!string.IsNullOrEmpty(Unit))
                    name += " - " + Unit;

                return name;
            }
        }

        public JobCommodity Duplicate()
        {
            var copy = new JobCommodity();
                        
            copy.OriginalCommodity = OriginalCommodity;
            copy.Name = Name;
            copy.Value = Value;
            copy.Serial = Serial;
            copy.Unit = Unit;
            copy.DepartureSiteName = DepartureSiteName;
            copy.DepartureAddress = DepartureAddress;
            copy.ArrivalSiteName = ArrivalSiteName;
            copy.ArrivalAddress = ArrivalAddress;
            copy.Length = Length;
            copy.Width = Width;
            copy.Height = Height;
            copy.Weight = Weight;
            copy.DimensionsEstimated = DimensionsEstimated;
            copy.Cost = Cost;            
            copy.Notes = Notes;

            return copy;
        }
    }

    partial class LoadedCommodity
    {
        partial void OnCreated()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {            
            switch (e.PropertyName)
            {
                case "UnloadLocation":
                    UpdateLastKnownLocation(UnloadLocation);
                    break;
                case "UnloadAddress":
                    UpdateLastKnownAddress(UnloadAddress);
                    break;
                case "UnloadRoute":
                    UpdateLastKnownRoute(UnloadRoute);
                    break;
                case "UnloadInstructions":
                    UpdateLastKnownInstructions(UnloadInstructions);
                    break;
            }  
        }

        public void UpdateLastKnownLocation(string location)
        {
            if (JobCommodity != null)
            {
                JobCommodity.DepartureSiteName = location;
                JobCommodity.ArrivalSiteName = null;

                if (JobCommodity.OriginalCommodity != null)
                    JobCommodity.OriginalCommodity.LastLocation = location;
            }
        }

        public void UpdateLastKnownAddress(string address)
        {
            if (JobCommodity != null)
            {
                JobCommodity.DepartureAddress = address;
                JobCommodity.ArrivalAddress = null;

                if (JobCommodity.OriginalCommodity != null)
                    JobCommodity.OriginalCommodity.LastAddress = address;
            }
        }

        public void UpdateLastKnownRoute(string route)
        {
            if (JobCommodity != null && JobCommodity.OriginalCommodity != null)
                JobCommodity.OriginalCommodity.LastRoute = route;
        }

        public void UpdateLastKnownInstructions(string instructions)
        {
            if (JobCommodity != null && JobCommodity.OriginalCommodity != null)
                JobCommodity.OriginalCommodity.LastLoadInstructions = instructions;

        }

        public LoadedCommodity Duplicate()
        {
            var copy = new LoadedCommodity();
                        
            copy.LoadLocation = LoadLocation;
            copy.LoadAddress = LoadAddress;
            copy.LoadingProvince = LoadingProvince;
            copy.LoadSiteCompany = LoadSiteCompany;
            copy.LoadSiteContact = LoadSiteContact;
            copy.LoadingCompany = LoadingCompany;
            copy.LoadingContact = LoadingContact;
            copy.LoadMethod = LoadMethod;
            copy.LoadDate = LoadDate;
            copy.LoadTime = LoadTime;
            copy.LoadInstructions = LoadInstructions;
            copy.LoadRoute = LoadRoute;
            copy.UnloadLocation = UnloadLocation;
            copy.UnloadAddress = UnloadAddress;
            copy.UnloadingProvince = UnloadingProvince;
            copy.UnloadSiteCompany = UnloadSiteCompany;
            copy.UnloadSiteContact = UnloadSiteContact;            
            copy.UnloadingCompany = UnloadingCompany;
            copy.UnloadingContact = UnloadingContact;
            copy.UnloadMethod = UnloadMethod;
            copy.UnloadDate = UnloadDate;
            copy.UnloadTime = UnloadTime;
            copy.UnloadInstructions = UnloadInstructions;
            copy.UnloadRoute = UnloadRoute;
            copy.ShipperCompany = ShipperCompany;
            copy.ShipperAddress = ShipperAddress;
            copy.ConsigneeCompany = ConsigneeCompany;
            copy.ConsigneeAddress = ConsigneeAddress;
            copy.BoLComments = BoLComments;
            copy.BoLDangerousGoodsInfo = BoLDangerousGoodsInfo;

            return copy;
        }
    }

    partial class ThirdPartyService
    {
        public ThirdPartyService Duplicate()
        {
            var copy = new ThirdPartyService();

            copy.Company = Company;
            copy.ServiceType = ServiceType;
            copy.Contact = Contact;
            copy.ServiceDate = ServiceDate;
            copy.Location = Location;
            copy.Reference = Reference;
            copy.Notes = Notes;

            return copy;
        }
    }

    partial class Permit
    {
        public Permit Duplicate()
        {
            var cp = new Permit();
                        
            cp.IssuingCompany = IssuingCompany;
            cp.PermitType = PermitType;            
            cp.Conditions = Conditions;
            cp.Cost = Cost;

            return cp;
        }
    }

    partial class Dispatch
    {
        public Dispatch Duplicate()
        {
            var cp = new Dispatch();

            cp.DepartingUnits = DepartingUnits;
            cp.MeetingDate = MeetingDate;
            cp.MeetingTime = MeetingTime;
            cp.DepartingLocation = DepartingLocation;
            cp.Description = Description;
            cp.Schedule = Schedule;            
            cp.Notes = Notes;
            
            return cp;
        }
        
        public string Name
        {
            get
            {
                string dispatchNumber;

                if (Load == null)
                    dispatchNumber = string.Format("{0}-{1:D2}", Load.Job.Number, Number);
                else
                    dispatchNumber = string.Format("{0}-{1:D2}-{2:D2}", Load.Job.Number, Load.Number, Number);

                return dispatchNumber;
            }
        }
    }

    partial class Load
    {        
        partial void OnCreated()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {            
            if (e.PropertyName.StartsWith("EWeight"))
                RecalculateEGross();
            else if (e.PropertyName.StartsWith("SWeight"))
                RecalculateSGross();
            //else if (e.PropertyName == "Status")
            //    StatusChanged();
        }

        private void RecalculateEGross()
        {
            var weight = 0.0;

            weight = 0;
            weight += EWeightSteer ?? 0.0;
            weight += EWeightDrive ?? 0.0;
            weight += EWeightGroup1 ?? 0.0; 
            weight += EWeightGroup2 ?? 0.0;
            weight += EWeightGroup3 ?? 0.0;
            weight += EWeightGroup4 ?? 0.0;
            weight += EWeightGroup5 ?? 0.0;
            weight += EWeightGroup6 ?? 0.0;
            weight += EWeightGroup7 ?? 0.0;
            weight += EWeightGroup8 ?? 0.0;
            weight += EWeightGroup9 ?? 0.0;
            weight += EWeightGroup10 ?? 0.0;

            EGrossWeight = weight;
        }

        private void RecalculateSGross()
        {
            var weight = 0.0;

            weight = 0;
            weight += SWeightSteer ?? 0.0;
            weight += SWeightDrive ?? 0.0;
            weight += SWeightGroup1 ?? 0.0;
            weight += SWeightGroup2 ?? 0.0;
            weight += SWeightGroup3 ?? 0.0;
            weight += SWeightGroup4 ?? 0.0;
            weight += SWeightGroup5 ?? 0.0;
            weight += SWeightGroup6 ?? 0.0;
            weight += SWeightGroup7 ?? 0.0;
            weight += SWeightGroup8 ?? 0.0;
            weight += SWeightGroup9 ?? 0.0;
            weight += SWeightGroup10 ?? 0.0;

            SGrossWeight = weight;
        }

        private void StatusChanged()
        {
            if (Status == null || Status.Name != "Completed") return;

            foreach (var item in LoadedCommodities)
            {
                if (item.JobCommodity != null)
                {
                    item.JobCommodity.DepartureSiteName = item.UnloadLocation;
                    item.JobCommodity.DepartureAddress = item.UnloadAddress;
                    item.JobCommodity.ArrivalSiteName = null;
                    item.JobCommodity.ArrivalAddress = null;

                    if (item.JobCommodity.OriginalCommodity != null)
                    {
                        item.JobCommodity.OriginalCommodity.LastLocation = item.UnloadLocation;
                        item.JobCommodity.OriginalCommodity.LastAddress = item.UnloadAddress;
                        item.JobCommodity.OriginalCommodity.LastRoute = item.UnloadRoute;
                        item.JobCommodity.OriginalCommodity.LastLoadInstructions = item.UnloadInstructions;
                    }
                }
            }
        }

        public void Notify(string property)
        {
            SendPropertyChanging();
            SendPropertyChanged(property);
        }

        public Load Duplicate()
        {
            var cp = new Load();

            cp.Status = Status;
            cp.Schedule = Schedule;            
            cp.Season = Season;
            cp.Info = Info;
            cp.StartDate = StartDate;
            cp.EndDate = EndDate;
            cp.Ban = Ban;
            cp.Notes = Notes;

            foreach (var item in LoadedCommodities)
                cp.LoadedCommodities.Add(item.Duplicate());

            foreach (var item in Permits)
                cp.Permits.Add(item.Duplicate());

            foreach (var item in ThirdPartyServices)
                cp.ThirdPartyServices.Add(item.Duplicate());

            foreach (var item in ReferenceNumbers)
                cp.ReferenceNumbers.Add(item.Duplicate());

            return cp;
        }

        public string Name
        {
            get
            {
                var name = String.Format("{0:D2}", Number);
                
                return name;
            }
        }

        public static string PrintCommodityList(Load load)
        {
            var list = load.LoadedCommodities;
            var output = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                output.Append(list[i].JobCommodity.NameAndUnit);

                if ((i + 1) != list.Count)
                    output.Append(", ");
            }

            return output.ToString();
        }
    }

    partial class LoadReferenceNumber
    {
        public LoadReferenceNumber Duplicate()
        {
            var cp = new LoadReferenceNumber();

            cp.Field = Field;
            cp.Value = Value;

            return cp;
        }
    }

    partial class Condition
    {
        public override string ToString()
        {
            return Line;
        }
    }

    partial class Invoice
    {
        private double? _totalHours;
        private decimal? _totalCost;
        
        public double? TotalHours
        {
            get
            {
                return _totalHours;
            }
            set
            {
                SendPropertyChanging();
                _totalHours = value;
                SendPropertyChanged("TotalHours");
            }
        }

        public decimal? TotalCost
        {
            get
            {
                return _totalCost;
            }
            set
            {
                SendPropertyChanging();
                _totalCost = value;
                SendPropertyChanged("TotalCost");
            }
        }

        partial void OnCreated()
        {
            GSTExempt = GSTExempt ?? false;
        }

        public Invoice Duplicate()
        {
            var copy = new Invoice();

            copy.Number = Number;
            copy.Comment = Comment;
            copy.Contact = Contact;
            copy.HourlyRate = HourlyRate;
            copy.Hours = Hours;
            copy.GSTExempt = GSTExempt;
            copy.InvoiceDate = InvoiceDate;

            foreach (var item in InvoiceLineItems)
            {
                var line = item.Duplicate();

                foreach (var extra in item.Extras)
                    line.Extras.Add(extra.Duplicate());

                copy.InvoiceLineItems.Add(line);
            }
            
            return copy;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Number, Revision);
        }
    }
    partial class InvoiceLineItem
    {
        public InvoiceLineItem Duplicate()
        {
            var copy = new InvoiceLineItem();

            copy.Description = Description;
            copy.StartDate = StartDate;
            copy.EndDate = EndDate;
            copy.Departure = Departure;
            copy.Destination = Destination;
            copy.Hours = Hours;
            copy.Cost = Cost;

            return copy;
        }        
    }

    partial class InvoiceExtra
    {
        public InvoiceExtra Duplicate()
        {
            var copy = new InvoiceExtra();

            copy.Description = Description;            
            copy.Hours = Hours;
            copy.Cost = Cost;

            return copy;
        }
    }

    partial class OutOfProvinceTravel
    {
        public OutOfProvinceTravel Duplicate()
        {
            return new OutOfProvinceTravel { ProvinceOrState = ProvinceOrState, Distance = Distance };
        }
    }

    partial class StorageItem
    {
        partial void OnCreated()
        {            
            ContractSigned = ContractSigned ?? false;
        } 
    }

    partial class TrailerCombination
    {
        partial void OnCreated()
        {
            Expandable = Expandable ?? false;
            Archived = Archived ?? false;
        }
    }
}
