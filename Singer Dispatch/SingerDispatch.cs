namespace SingerDispatch
{
    partial class User
    {
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    partial class Contact
    {
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
