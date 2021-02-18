using System;
using AutoMapper;
using Les02LaboBackendDevelopment.Models;
namespace Les02LaboBackendDevelopment.DTO
{
    public class AutoMapping : Profile
    {
        public AutoMapping(){
            CreateMap<VaccinationRegistration, VaccinationRegistrationDTO>();
        }
    }
}
