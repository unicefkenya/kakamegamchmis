using AutoMapper;

namespace MCHMIS.Models.Automapper
{
    public class AutomapperProfile:Profile
    {

public AutomapperProfile()
        {
            CreateMap<ApplicationUser, ApplicationUser>().ReverseMap();
        }    
}
}