using System;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCHMIS.Interfaces

{
    public interface IEntity
    {
        [HiddenInput]
        object Id { get; set; }

        [Required]
        string CreatedById { get; set; }

        [Required]
        DateTime DateCreated { get; set; }

        [Required]
        ApplicationUser CreatedBy { get; set; }

        string ModifiedById { get; set; }

        DateTime? ModifiedDate { get; set; }

        ApplicationUser ModifiedBy { get; set; }
    }

    public interface IEntity<T> : IEntity
    {
        new T Id { get; set; }
    }
}