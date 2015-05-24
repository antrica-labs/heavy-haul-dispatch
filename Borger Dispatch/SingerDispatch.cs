using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace SingerDispatch
{
    partial class SingerDispatchDataContext
    {
        public SingerDispatchDataContext()
            : base(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionParameters"].ConnectionString)
        {
        }

        public void RevertChanges()
        {
            var cs = GetChangeSet();

            foreach (var item in cs.Inserts)
            {
                var t = item.GetType();
                GetTable(t).DeleteOnSubmit(item);
            }

            foreach (var item in cs.Deletes)
            {
                var t = item.GetType();
                GetTable(t).InsertOnSubmit(item);
            }

            foreach (var item in cs.Updates)
            {
                var t = item.GetType();
                foreach (var mm in GetTable(t).GetModifiedMembers(item))
                {
                    var pi = mm.Member as PropertyInfo;
                    if (pi != null)
                    {
                        pi.SetValue(item, mm.OriginalValue, null);
                    }

                    var fi = mm.Member as FieldInfo;
                    if (fi != null)
                    {
                        fi.SetValue(item, mm.OriginalValue);
                    }
                }
            }
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

        public override string ToString()
        {
            return Name;
        }
    }

    partial class ContactMethod
    {
        public override string ToString()
        {
            return Name;
        }
    }

    partial class Employee
    {
        partial void OnCreated()
        {
            Archived = Archived ?? false;
            IsAvailable = IsAvailable ?? true;
            IsSupervisor = IsSupervisor ?? false;
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
            return Prefix.Trim().Length > 0 ? string.Format("{0} - {1}", Prefix, Name) : string.Format("{0}", Name);
        }
    }

    partial class Equipment
    {
        partial void OnCreated()
        {
            Archived = Archived ?? false;
            IsDispatchable = IsDispatchable ?? true;
            HasWinch = HasWinch ?? false;
            IsProrated = IsProrated ?? false;
            IsOnlyForScheuerle = IsOnlyForScheuerle ?? false;
            IsOnlyForPushing = IsOnlyForPushing ?? false;
        }
    }

    partial class Commodity
    {
        public string NameAndUnit
        {
            get
            {
                var identifier = Unit;

                if (!string.IsNullOrEmpty(identifier) && !string.IsNullOrEmpty(Name))
                    identifier += " - " + Name;
                else
                    identifier += Name;

                return identifier;
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

    partial class QuoteNumber
    {
        partial void OnCreated()
        {
            CreateDate = DateTime.Now;
        }
    }

    /*
    -- alter table Quote add Number nvarchar(4000);
    -- update Quote set Number = NumberOrig;

    --CREATE TABLE QuoteNumber(
    --	[Number] [nvarchar](255) NOT NULL PRIMARY KEY,
    --	[CreateDate] [datetime] NOT NULL
    --);

    --insert into quotenumber (number, createdate) select distinct number, CURRENT_TIMESTAMP from quote;  
    */
    partial class Quote
    {
        partial void OnCreated()
        {
            Price = Price ?? 0.00m;
            IsItemizedBilling = IsItemizedBilling ?? false;
        }

        public string FriendlyNumber
        {
            get
            {
                return string.Format("Q{0}", Number);
            }
        }

        public string FriendlyName
        {
            get
            {
                return string.Format("Q{0} rev. {1}", Number, Revision);
            }
        }

        public string NumberAndRev
        {
            get
            {
                return String.Format("Q{0}-{1}", Number, Revision);
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

            foreach (var inclusion in QuoteInclusions)
            {
                copy.QuoteInclusions.Add(inclusion.Duplicate());
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

        public static string PrintCommodityList(Quote quote)
        {
            var list = quote.QuoteCommodities;
            var output = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                output.Append(list[i].NameAndUnit);

                if ((i + 1) != list.Count)
                    output.Append(", ");
            }

            return output.ToString();
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

            copy.Owner = Owner;
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

            jc.Owner = Owner;
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
                var identifier = Unit;

                if (!string.IsNullOrEmpty(identifier) && !string.IsNullOrEmpty(Name))
                    identifier += " - " + Name;
                else
                    identifier += Name;

                return identifier;
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

    partial class QuoteInclusion
    {
        public QuoteInclusion Duplicate()
        {
            var copy = new QuoteInclusion();

            copy.Inclusion = Inclusion;

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
    }

    partial class JobNumber
    {
        partial void OnCreated()
        {
            CreateDate = DateTime.Now;
        }
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

        public override string ToString()
        {
            string output;

            if (!string.IsNullOrWhiteSpace(Name))
                output = string.Format("{0} - {1}", Number, Name);
            else
                output = string.Format("{0}", Number);

            return output;
        }

        public static string PrintCommodityList(Job job)
        {
            var list = job.JobCommodities;
            var output = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                output.Append(list[i].NameAndUnit);

                if ((i + 1) != list.Count)
                    output.Append(", ");
            }

            return output.ToString();
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
                var identifier = Unit;

                if (!string.IsNullOrEmpty(identifier) && !string.IsNullOrEmpty(Name))
                    identifier += " - " + Name;
                else
                    identifier += Name;

                return identifier;
            }
        }

        public JobCommodity Duplicate()
        {
            var copy = new JobCommodity();

            copy.Owner = Owner;
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
                    UpdateLastKnownLocation();
                    break;
                case "UnloadAddress":
                    UpdateLastKnownAddress();
                    break;
                case "UnloadRoute":
                    UpdateLastKnownRoute();
                    break;
                case "UnloadInstructions":
                    UpdateLastKnownInstructions();
                    break;
                case "JobCommodity":
                    UpdateJobCommodity();
                    break;
            }
        }

        public void UpdateLastKnownLocation()
        {
            if (JobCommodity != null)
            {
                JobCommodity.DepartureSiteName = UnloadLocation;
                JobCommodity.ArrivalSiteName = null;

                if (JobCommodity.OriginalCommodity != null)
                    JobCommodity.OriginalCommodity.LastLocation = UnloadLocation;
            }
        }

        public void UpdateLastKnownAddress()
        {
            if (JobCommodity != null)
            {
                JobCommodity.DepartureAddress = UnloadAddress;
                JobCommodity.ArrivalAddress = null;

                if (JobCommodity.OriginalCommodity != null)
                    JobCommodity.OriginalCommodity.LastAddress = UnloadAddress;
            }
        }

        public void UpdateLastKnownRoute()
        {
            if (JobCommodity != null && JobCommodity.OriginalCommodity != null)
                JobCommodity.OriginalCommodity.LastRoute = UnloadRoute;
        }

        public void UpdateLastKnownInstructions()
        {
            if (JobCommodity != null && JobCommodity.OriginalCommodity != null)
                JobCommodity.OriginalCommodity.LastLoadInstructions = UnloadInstructions;

        }

        public void UpdateJobCommodity()
        {
            if (JobCommodity != null)
            {
                LoadLocation = JobCommodity.DepartureSiteName;
                LoadAddress = JobCommodity.DepartureAddress;
                UnloadLocation = JobCommodity.ArrivalSiteName;
                UnloadAddress = JobCommodity.ArrivalAddress;

                if (JobCommodity.OriginalCommodity == null) return;

                LoadRoute = JobCommodity.OriginalCommodity.LastRoute;
                LoadInstructions = JobCommodity.OriginalCommodity.LastLoadInstructions;
            }

            if (Load != null)
                Load.RecaculateDimensionsAndWeight();
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
        partial void OnCreated()
        {
            IsBilled = IsBilled ?? false;
        }

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

            return cp;
        }
    }

    partial class Dispatch
    {
        private Rate _suggesteRate;

        public Rate SuggestedRate
        {
            get
            {
                return _suggesteRate;
            }
            set
            {
                SendPropertyChanging();
                _suggesteRate = value;
                SendPropertyChanged("SuggestedRate");
            }
        }

        partial void OnCreated()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SuggestedRate")
                CalculateAdjustedRate();
        }

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

        private void CalculateAdjustedRate()
        {
            var enterprise = Load.Job != null && Load.Job.Company != null && Load.Job.Company.CustomerType != null && Load.Job.Company.CustomerType.IsEnterprise == true;

            if (SuggestedRate == null)
                AdjustedRate = null;
            else
            {
                try
                {
                    AdjustedRate = (from ra in Load.Job.Company.RateAdjustments where ra.Rate == SuggestedRate select ra).First().AdjustedRate;
                }
                catch
                {
                    AdjustedRate = (enterprise) ? SuggestedRate.HourlyEnterprise : SuggestedRate.HourlySpecialized;
                }
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
            else if (e.PropertyName == "Equipment" || e.PropertyName == "TrailerCombination")
                RecaculateDimensionsAndWeight();
            else if (e.PropertyName == "Rate")
                CalculateAdjustedRate();
            //else if (e.PropertyName == "Status")
            //    StatusChanged();
        }

        public void RecaculateDimensionsAndWeight()
        {
            LoadedWidth = 0.0;
            LoadedLength = 0.0;
            LoadedHeight = 0.0;
            CalculatedWeight = 0.0;

            if (Equipment != null)
                CalculatedWeight += Equipment.Tare ?? 0.0;

            var widest = 0.0;
            var longest = 0.0;
            var highest = 0.0;

            foreach (var commodity in LoadedCommodities)
            {
                var length = commodity.JobCommodity.Length ?? 0.0;
                var height = commodity.JobCommodity.Height ?? 0.0;
                var width = commodity.JobCommodity.Width ?? 0.0;

                if (length > longest)
                    longest = length;

                if (height > highest)
                    highest = height;

                if (width > widest)
                    widest = width;

                CalculatedWeight += commodity.JobCommodity.Weight ?? 0.0;
            }

            LoadedHeight += highest;

            if (TrailerCombination != null)
            {
                LoadedWidth += TrailerCombination.Width ?? 0.0;
                LoadedLength += TrailerCombination.Length ?? 0.0;
                LoadedHeight += TrailerCombination.Height ?? 0.0;
                CalculatedWeight += TrailerCombination.Tare ?? 0.0;
            }

            if (LoadedHeight < SingerConfigs.MinLoadHeight)
                LoadedHeight = SingerConfigs.MinLoadHeight;

            if (widest > LoadedWidth)
                LoadedWidth = widest;

            if (longest > LoadedLength)
                LoadedLength = longest;
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

        private void CalculateAdjustedRate()
        {
            var enterprise = Job != null && Job.Company != null && Job.Company.CustomerType != null && Job.Company.CustomerType.IsEnterprise == true;

            if (Rate == null)
                AdjustedRate = null;
            else
            {
                try
                {
                    AdjustedRate = (from ra in Job.Company.RateAdjustments where ra.Rate == Rate select ra).First().AdjustedRate;
                }
                catch
                {
                    AdjustedRate = (enterprise) ? Rate.HourlyEnterprise : Rate.HourlySpecialized;
                }
            }
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

        public override string ToString()
        {
            var commodities = LoadedCommodities.ToList();
            var output = new StringBuilder();

            if (commodities.Count == 0)
                return "[Empty]";

            for (var i = 0; i < commodities.Count; i++)
            {
                if (commodities[i].JobCommodity == null)
                    continue;

                output.Append(commodities[i].JobCommodity.NameAndUnit);

                if ((i + 1) != commodities.Count)
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

        public string NumberAndRev
        {
            get
            {
                return String.Format("{0}-{1}", Number, Revision);
            }
        }

        public IEnumerable<Load> IncludedLoads { get; set; }

        partial void OnCreated()
        {
            TaxRate = TaxRate ?? SingerConfigs.GST;
            InvoiceDate = InvoiceDate ?? DateTime.Now;
        }

        public void UpdateTotalCost()
        {
            TotalCost = 0.00m;

            foreach (var item in InvoiceLineItems)
            {
                var hours = item.Hours ?? 1;

                if (item.Rate != null)
                    TotalCost += (item.Rate * (decimal)hours);

                foreach (var extra in item.Extras)
                {
                    hours = extra.Hours ?? 1;

                    if (extra.Rate != null)
                        TotalCost += (extra.Rate * (decimal)hours);
                }
            }
        }

        public void UpdateTotalHours()
        {
            TotalHours = 0.0;

            foreach (var item in InvoiceLineItems)
            {
                if (item.Hours != null)
                    TotalHours += item.Hours;

                foreach (var extra in item.Extras)
                {
                    if (extra.Hours != null)
                        TotalHours += extra.Hours;
                }
            }

            UpdateTotalCost();
        }

        public Invoice Duplicate()
        {
            var copy = new Invoice();

            copy.Job = Job;
            copy.Company = Company;
            copy.Number = Number;
            copy.Comment = Comment;
            copy.Contact = Contact;
            copy.TaxRate = TaxRate;
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

        internal void AddPermitAcquisition(Load load, decimal totalCost)
        {
            var description = "Permit acquisition fee";
            var line = new InvoiceLineItem() { Description = description };

            if (totalCost < 150m)
                line.Rate = 15m;
            else
                line.Rate = totalCost * 0.1m;

            InvoiceLineItems.Add(line);
        }

        internal void AddLoadReferences(Load load)
        {
            foreach (var reference in load.ReferenceNumbers)
            {
                var item = new InvoiceReferenceNumber() { Field = reference.Field, Value = reference.Value };

                ReferenceNumbers.Add(item);
            }
        }

        internal void AddLoadedCommodities(Load load)
        {
            string loading, unloading;
            var line = new InvoiceLineItem();

            line.Description = string.Format(SingerConfigs.DefaultDispatchDescription, load.ToString());
            line.Rate = load.AdjustedRate;

            var fromDiffers = false;    
            var toDiffers = false;

            foreach (var commodity in load.LoadedCommodities)
            {                
                // Ugly, but I can't think of a better way to trim these strings and null out the empty ones
                commodity.LoadLocation = (commodity.LoadLocation ?? "").Trim();
                commodity.LoadAddress = (commodity.LoadAddress ?? "").Trim();
                commodity.UnloadLocation = (commodity.UnloadLocation ?? "").Trim();
                commodity.UnloadAddress = (commodity.UnloadAddress ?? "").Trim();

                if (commodity.LoadLocation.Length > 0 && commodity.LoadAddress.Length > 0)
                    loading = string.Format("{0} - {1}", commodity.LoadLocation, commodity.LoadAddress);
                else
                    loading = string.Format("{0} {1}", commodity.LoadLocation, commodity.LoadAddress).Trim();

                if (commodity.UnloadLocation.Length > 0 && commodity.UnloadAddress.Length > 0)
                    unloading = string.Format("{0} - {1}", commodity.UnloadLocation, commodity.UnloadAddress);
                else
                    unloading = string.Format("{0} {1}", commodity.UnloadLocation, commodity.UnloadAddress).Trim();
                
                line.ItemDate = line.ItemDate ?? commodity.LoadDate;
                line.Departure = line.Departure ?? loading;
                line.Destination = line.Destination ?? unloading;


                if (fromDiffers == false && line.Departure != loading)
                    fromDiffers = true;

                if (toDiffers == false && line.Destination != unloading)
                    toDiffers = true;
            }

            if (fromDiffers)
                line.Departure = "Various";

            if (toDiffers)
                line.Destination = "Various";

            InvoiceLineItems.Add(line);
        }

        internal void AddDispatches(Load load)
        {
            foreach (var dispatch in load.Dispatches)
            {
                var line = new InvoiceLineItem();

                if (dispatch.Equipment != null && dispatch.EquipmentType != null)
                    line.Description = string.Format("Dispatch #{0}: {1} - {2}", dispatch.Number, dispatch.EquipmentType.Name, dispatch.Equipment.UnitNumber);
                else
                    line.Description = string.Format("Dispatch #{0}", dispatch.Number);

                line.ItemDate = dispatch.MeetingDate;
                line.Rate = dispatch.AdjustedRate;
                line.Departure = dispatch.DepartingLocation;

                InvoiceLineItems.Add(line);
            }
        }

        internal decimal AddPermits(Load load)
        {
            var permitTotal = 0.00m;

            foreach (var permit in load.Permits)
            {
                var line = new InvoiceLineItem();

                var company = (permit.IssuingCompany != null) ? permit.IssuingCompany.Name : "";
                var type = (permit.PermitType != null) ? permit.PermitType.Name : "";
                var loadName = (permit.Load != null) ? permit.Load.Name : "N/A";

                line.Description = string.Format("{0} - {1}", company, type);
                line.Rate = permit.Cost;

                InvoiceLineItems.Add(line);

                permitTotal += permit.Cost ?? 0.0m;
            }

            return permitTotal;
        }

        internal void AddLoads(IEnumerable<Load> loads, Status status)
        {
            IncludedLoads = loads;

            foreach (var load in IncludedLoads)
            {
                AddLoadReferences(load);
                AddLoadedCommodities(load);
                AddDispatches(load);
                
                if (load.Permits.Count() > 0)
                    AddPermitAcquisition(load, AddPermits(load));

                if (status != null)
                    load.Status = status;                
            }
        }

        internal void AddThirdPartyServices(IEnumerable<ThirdPartyService> services)
        {
            foreach (var service in services)
            {
                var line = new InvoiceLineItem();

                var type = (service.ServiceType != null) ? service.ServiceType.Name : "UNKNOWN SERVICE";
                var company = (service.Company != null) ? service.Company.Name : "UNKNOWN COMPANY";
                var commodities = (service.Load != null) ? service.Load.ToString() : "various items";
                var loadName = (service.Load != null) ? service.Load.Name : "N/A";

                line.Description = string.Format("Supply {0} by {1} to transport {2}", type, company, commodities);

                if (!string.IsNullOrWhiteSpace(service.Reference))
                    line.Description += string.Format(" [Reference: {0}]", service.Reference);

                service.IsBilled = true;

                var fromDiffers = false;
                var toDiffers = false;

                foreach (var commodity in service.Load.LoadedCommodities)
                {
                    var loading = commodity.LoadLocation + " - " + commodity.LoadAddress;
                    var unloading = commodity.UnloadLocation + " - " + commodity.UnloadAddress;

                    line.ItemDate = line.ItemDate ?? commodity.LoadDate;
                    line.Departure = line.Departure ?? loading;
                    line.Destination = line.Destination ?? unloading;


                    if (fromDiffers == false && line.Departure != loading.Trim())
                        fromDiffers = true;

                    if (toDiffers == false && line.Destination != unloading.Trim())
                        toDiffers = true;
                }

                if (fromDiffers)
                    line.Departure = "Various";

                if (toDiffers)
                    line.Destination = "Various";

                InvoiceLineItems.Add(line);
            }
        }

        internal void AddStorageItems(IEnumerable<StorageItem> storage)
        {
            foreach (var item in storage)
            {
                var line = new InvoiceLineItem();

                var commodity = (item.JobCommodity != null) ? item.JobCommodity.NameAndUnit : "UNKNOWN";

                line.Description = string.Format("Supply secure storge for {0} from [date] to [date]", commodity);
                line.Rate = item.BillingRate;

                if (item.BillingInterval != null)
                    line.Description += string.Format(" (Billed {0})", item.BillingInterval.Name.ToLower());

                InvoiceLineItems.Add(line);
            }
        }
    }

    partial class InvoiceLineItem
    {
        partial void OnCreated()
        {
            TaxExempt = TaxExempt ?? false;

            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Rate":
                    UpdateTotalCost();
                    break;
                case "Hours":
                    UpdateTotalHours();
                    break;
            }
        }

        public void UpdateTotalCost()
        {
            if (Invoice != null)
                Invoice.UpdateTotalCost();
        }

        public void UpdateTotalHours()
        {
            if (Invoice != null)
                Invoice.UpdateTotalHours();
        }

        public InvoiceLineItem Duplicate()
        {
            var copy = new InvoiceLineItem();

            copy.Description = Description;
            copy.ItemDate = ItemDate;
            copy.Departure = Departure;
            copy.Destination = Destination;
            copy.Hours = Hours;
            copy.Rate = Rate;

            return copy;
        }
    }

    partial class InvoiceExtra
    {
        partial void OnCreated()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SomePropertyChanged);
        }

        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Rate":
                    UpdateTotalCost();
                    break;
                case "Hours":
                    UpdateTotalHours();
                    break;
            }
        }

        public void UpdateTotalCost()
        {
            if (LineItem != null)
                LineItem.UpdateTotalCost();
        }

        public void UpdateTotalHours()
        {
            if (LineItem != null)
                LineItem.UpdateTotalHours();
        }

        public InvoiceExtra Duplicate()
        {
            var copy = new InvoiceExtra();

            copy.Description = Description;
            copy.Hours = Hours;
            copy.Rate = Rate;

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
