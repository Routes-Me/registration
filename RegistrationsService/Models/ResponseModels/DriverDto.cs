namespace RegistrationsService.Models.ResponseModels
{
    //public class DriverDto
    //{
    //    public string email { get; set; }
    //    public string avatarUrl { get; set; }
    //    public string name { get; set; }
    //    public string invitationToken { get; set; }
    //    public virtual phone phone { get; set; }
    //}
    //public class phone
    //{
    //    public string number { get; set; }
    //    public string verificationToken { get; set; }
    //}

    public class DriverDto
    { 
        public string User_Id { get; set; }
        public string Institution_Id { get; set; }
        public string avatarUrl { get; set; }
    }

}
