using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.Dtos.Wallet;
using api.Models;

namespace api.Mappers
{
    public static class WalletMappers //extension methods, declare them as static
    {
        public static WalletDto ToWalletDto(this Wallet walletModel)
        {
            return new WalletDto
            {
                Id = walletModel.Id,
                UserId = walletModel.UserId,
                Amount = walletModel.Amount,
                CreatedOn = walletModel.CreatedOn
            };
        }

        public static Wallet ToWalletFromCreateDto(this CreateWalletRequestDto walletDto)
        {
            return new Wallet
            {
                Amount = walletDto.Amount
            };
        }

        public static WalletResponseDto ToWalletResponseDto(this Wallet walletModel)
    {
        return new WalletResponseDto
        {
            Id = walletModel.Id,
            Amount = walletModel.Amount,
            UserId = walletModel.UserId
        };
    }
    }
}