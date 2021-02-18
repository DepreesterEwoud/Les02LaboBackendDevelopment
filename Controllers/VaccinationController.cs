using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using CsvHelper;

using CsvHelper.Configuration;

using System.Globalization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Les02LaboBackendDevelopment.Configuration;
using Les02LaboBackendDevelopment.Models;
using Les02LaboBackendDevelopment.DTO;
using AutoMapper;

namespace Les02LaboBackendDevelopment.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class VaccinationController : ControllerBase
    {
        private CSVSettings _settings;

        private static List<VaccinType> _vaccinTypes;
        private static List<VaccinationLocation> _vaccinlocations;
        private static List<VaccinationRegistration> _registrations;

        private IMapper _mapper;
        public VaccinationController(IOptions<CSVSettings> settings, IMapper mapper){
            _mapper = mapper;
            _settings = settings.Value;
            if(_vaccinlocations == null){
                _vaccinlocations = ReadCSVLocation();
            }
            if(_vaccinTypes == null){
                _vaccinTypes = ReadCSVVaccin();
            }
            if(_registrations == null){
                _registrations = ReadCSVRegistrations();
            }
        }

        private void SaveRegistrations(){
            var config = new CsvConfiguration(CultureInfo.InvariantCulture){
                HasHeaderRecord = false,Delimiter = ";"
            };
            using(var writer = new StreamWriter(_settings.CSVRegistrations)){
                using(var csv = new CsvWriter(writer,config)){
                    csv.WriteRecords(_registrations);
                }
            }
        }

        private List<VaccinationLocation> ReadCSVLocation(){
            var config = new CsvConfiguration(CultureInfo.InvariantCulture){
                HasHeaderRecord = false,Delimiter = ";"
            };

            using(var reader = new StreamReader(_settings.CSVLocations)){
                using(var csv = new CsvReader(reader, config)){
                    var records = csv.GetRecords<VaccinationLocation>();
                    return records.ToList<VaccinationLocation>();
                }
            }
        }
        private List<VaccinType> ReadCSVVaccin(){
            var config = new CsvConfiguration(CultureInfo.InvariantCulture){
                HasHeaderRecord = false,Delimiter = ";"
            };

            using(var reader = new StreamReader(_settings.CSVVaccins)){
                using(var csv = new CsvReader(reader, config)){
                    var records = csv.GetRecords<VaccinType>();
                    return records.ToList<VaccinType>();
                }
            }
        }

        private List<VaccinationRegistration> ReadCSVRegistrations(){
            var config = new CsvConfiguration(CultureInfo.InvariantCulture){
                HasHeaderRecord = false,Delimiter = ";"
            };

            using(var reader = new StreamReader(_settings.CSVRegistrations)){
                using(var csv = new CsvReader(reader, config)){
                    var records = csv.GetRecords<VaccinationRegistration>();
                    return records.ToList<VaccinationRegistration>();
                }
            }
        }
            /* if(_registrations == null){
                _registrations = new List<VaccinationRegistration>();
                
            }
            if(_vaccinTypes == null){
                _vaccinTypes = new List<VaccinType>();
                _vaccinTypes.Add(new VaccinType(){
                    VaccinTypeId = Guid.NewGuid(),
                    Name = "Moderna"
                });
            }
            if(_vaccinlocations == null){
                _vaccinlocations = new List<VaccinationLocation>();
                _vaccinlocations.Add(new VaccinationLocation(){
                    VaccinationLocationId = Guid.NewGuid(),
                    Name = "Kortrijk Expo"
                });
            } */
        

        [Route("registration")]
        [HttpPost]
        public ActionResult<VaccinationRegistration> AddRegistration(VaccinationRegistration newRegistration){
            if(newRegistration == null)
                return new BadRequestResult();
            
            if(_vaccinlocations.Where(vt => vt.VaccinationLocationId == newRegistration.VaccinationLocationId).Count() == 0){
                return new BadRequestResult();
            }
            newRegistration.VaccinationRegistrationId = Guid.NewGuid();
            _registrations.Add(newRegistration);
            SaveRegistrations();
            return newRegistration;
        }

        [Route("registrations")]
        [HttpGet]
        public ActionResult<List<VaccinationRegistration>> GetRegistrations(string date = ""){
            if(string.IsNullOrEmpty(date)){
                return new OkObjectResult(_registrations);
            }
            else{
                return _registrations.Where(r => r.VaccinationDate == date).ToList<VaccinationRegistration>();
            }
        }

        
        [HttpGet]
        [Route("registrations")]
        [MapToApiVersion("2.0")]
        public ActionResult<List<VaccinationRegistrationDTO>> GetRegistrationsSmall(){
            return _mapper.Map<List<VaccinationRegistrationDTO>>(_registrations);
        }

        [Route("vaccins")]
        [HttpGet]
        public ActionResult<List<VaccinType>> GetVaccins(){
            return new OkObjectResult(_vaccinTypes);
        }

        [Route("locations")]
        [HttpGet]
        public ActionResult<List<VaccinationLocation>> GetLocations(){
            return new OkObjectResult(_vaccinlocations);
        }
    }
}
