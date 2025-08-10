using AutoMapper;
using BankingApp.Application.DTOs;
using BankingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Mappings
{
    public class MappinConfigs : Profile
    {
        public MappinConfigs()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Transaction, TransactionData>().ReverseMap();

        }
    }
}
