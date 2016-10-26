namespace AssessmentManager
{
    public class UsernamePasswordPair
    {
        private string username = "";
        private string password = "";

        public UsernamePasswordPair(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string Username
        {
            get
            {
                return username;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
        }
    }
}
